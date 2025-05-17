using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using GameLens.Models.Domain;
using GameLens.Data;
using GameLens.Models.DTOs;

namespace GameLens.Services
{
    public class GameService : IGameService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GameService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.rawg.io/api";
        private readonly string _imageStoragePath;

        public GameService(HttpClient httpClient, ApplicationDbContext dbContext, IConfiguration configuration, ILogger<GameService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiKey = _configuration["RawgApi:ApiKey"] ?? throw new InvalidOperationException("RAWG API key is not configured.");
            _imageStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "games");

            // Ensure image storage directory exists
            Directory.CreateDirectory(_imageStoragePath);
        }

        public async Task<List<Game>> SearchGamesAsync(string query, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Search query cannot be empty.", nameof(query));
            }

            try
            {
                // Check database first to reduce API calls
                var cachedGames = await _dbContext.Games
                    .Where(g => g.Name != null && g.Name.Contains(query))
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (cachedGames.Any())
                {
                    _logger.LogInformation("Returning {Count} cached games for query: {Query}", cachedGames.Count, query);
                    return cachedGames;
                }

                // Fetch from RAWG API
                var url = $"{_baseUrl}/games?key={_apiKey}&search={Uri.EscapeDataString(query)}&page={page}&page_size={pageSize}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var rawgResponse = JsonSerializer.Deserialize<RawgPaginatedResponseDto<RawgGameSearchResultDto>>(content);

                if (rawgResponse?.Results == null || !rawgResponse.Results.Any())
                {
                    _logger.LogWarning("No games found for query: {Query}", query);
                    return new List<Game>();
                }

                var games = new List<Game>();
                foreach (var rawgGame in rawgResponse.Results)
                {
                    // Fetch full details to get genres, platforms, developers, publishers
                    var game = await GetGameByIdAsync(rawgGame.Id);
                    games.Add(game);
                }

                return games;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch games from RAWG API for query: {Query}", query);
                throw new GameServiceException("Unable to fetch games from RAWG API.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while searching games for query: {Query}", query);
                throw new GameServiceException("An unexpected error occurred while searching games.", ex);
            }
        }

        public async Task<Game> GetGameByIdAsync(int gameId)
        {
            if (gameId <= 0)
            {
                throw new ArgumentException("Invalid RAWG game ID.", nameof(gameId));
            }

            try
            {
                // Check database first
                var cachedGame = await _dbContext.Games
                    .Include(g => g.Genres)
                    .Include(g => g.Platforms)
                    .Include(g => g.Developers)
                    .Include(g => g.Publishers)
                    .FirstOrDefaultAsync(g => g.Id == gameId);

                if (cachedGame != null)
                {
                    _logger.LogInformation("Returning cached game with ID: {GameId}", gameId);
                    return cachedGame;
                }

                // Fetch from RAWG API
                var url = $"{_baseUrl}/games/{gameId}?key={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var rawgGame = JsonSerializer.Deserialize<RawgGameDetailDto>(content);

                if (rawgGame == null)
                {
                    _logger.LogWarning("Game not found for ID: {GameId}", gameId);
                    throw new GameServiceException($"Game with ID {gameId} not found.");
                }

                return await SaveGameToDatabaseAsync(rawgGame);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch game from RAWG API for ID: {GameId}", gameId);
                throw new GameServiceException("Unable to fetch game from RAWG API.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching game for ID: {GameId}", gameId);
                throw new GameServiceException("An unexpected error occurred while fetching game.", ex);
            }
        }

        private async Task<Game> SaveGameToDatabaseAsync(RawgGameDetailDto rawgGame)
        {
            // Check if game already exists
            var existingGame = await _dbContext.Games
                .Include(g => g.Genres)
                .Include(g => g.Platforms)
                .Include(g => g.Developers)
                .Include(g => g.Publishers)
                .FirstOrDefaultAsync(g => g.Id == rawgGame.Id);

            if (existingGame != null)
            {
                return existingGame;
            }

            // Download and save background image
            string? backgroundImagePath = null;
            if (!string.IsNullOrEmpty(rawgGame.BackgroundImage))
            {
                try
                {
                    var imageUrl = rawgGame.BackgroundImage;
                    var imageExtension = Path.GetExtension(imageUrl.Split('?')[0]) ?? ".jpg";
                    var imageFileName = $"{rawgGame.Id}_{Guid.NewGuid()}{imageExtension}";
                    var imagePath = Path.Combine(_imageStoragePath, imageFileName);

                    using var imageResponse = await _httpClient.GetAsync(imageUrl);
                    imageResponse.EnsureSuccessStatusCode();

                    using var imageStream = await imageResponse.Content.ReadAsStreamAsync();
                    using var fileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write);
                    await imageStream.CopyToAsync(fileStream);

                    backgroundImagePath = $"/images/games/{imageFileName}";
                    _logger.LogInformation("Saved background image for game ID: {GameId} at {Path}", rawgGame.Id, backgroundImagePath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to save background image for game ID: {GameId}", rawgGame.Id);
                    // Continue without image
                }
            }

            // Map RAWG game to local Game model
            var game = new Game
            {
                Id = rawgGame.Id,
                Name = rawgGame.Name,
                Description = rawgGame.Description,
                Metacritic = rawgGame.Metacritic,
                Released = rawgGame.Released,
                LocalBackgroundImage = backgroundImagePath,
                Genres = rawgGame.Genres?.Select(g =>
                {
                    var existingGenre = _dbContext.Genres.Local.FirstOrDefault(x =>  x.Id == g.Id)
                                        ?? _dbContext.Genres.FirstOrDefault(x => x.Id == g.Id);

                    return existingGenre ?? new Genre { Id = g.Id, Name = g.Name };
                }).ToList<Genre>() ?? new List<Genre>(),
                Platforms = rawgGame.Platforms?.Select(p =>
                {
                    var platformId = p.Platform.Id;
                    var existingPlatform = _dbContext.Platforms.Local.FirstOrDefault(x => x.Id == platformId)
                                           ?? _dbContext.Platforms.FirstOrDefault(x => x.Id == platformId);

                    return existingPlatform ?? new Platform { Id = platformId, Name = p.Platform.Name };
                }).ToList() ?? new List<Platform>(),
                Developers = rawgGame.Developers?.Select(d =>
                {
                    var existingDeveloper = _dbContext.Developers.Local.FirstOrDefault(x => x.Id == d.Id)
                                        ?? _dbContext.Developers.FirstOrDefault(x => x.Id == d.Id);

                    return existingDeveloper ?? new Developer { Id = d.Id, Name = d.Name };
                }).ToList<Developer>() ?? new List<Developer>(),
                Publishers = rawgGame.Publishers?.Select(p =>
                {
                    var existingPublisher = _dbContext.Publishers.Local.FirstOrDefault(x => x.Id == p.Id)
                                        ?? _dbContext.Publishers.FirstOrDefault(x => x.Id == p.Id);

                    return existingPublisher ?? new Publisher { Id = p.Id, Name = p.Name };
                }).ToList<Publisher>() ?? new List<Publisher>()
            };

            _dbContext.Games.Add(game);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Saved game to database: {GameName} (ID: {GameId})", game.Name, game.Id);
            return game;
        }
    }

    public class GameServiceException : Exception
    {
        public GameServiceException(string message) : base(message) { }
        public GameServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}