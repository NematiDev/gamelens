using GameLens.Models.Domain;

namespace GameLens.Services
{
    public interface IGameService
    {
        Task<List<Game>> SearchGamesAsync(string query, int page = 1, int pageSize = 20);
        Task<Game> GetGameByIdAsync(int gameId);
    }
}
