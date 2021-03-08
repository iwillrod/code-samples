using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeSamples
{
    public interface IGamesService
    {
        Task<IEnumerable<Game>> FindAll();

        Task<IEnumerable<Game>> GetGamesByCity(string cityName);

        Task<Game> FindOne(int gameId);

        Task<bool> CreateGame(Game entity, CancellationToken ct = default);

        Task<Game> UpdateGame(int gameId, Game updateCommand, CancellationToken ct = default);

        Task<Game> PatchUpdateGame(Game updatedGame, CancellationToken ct = default);

        Task<bool> Contains(int gameId, CancellationToken ct = default);

        Task<bool> Delete(int gameId, CancellationToken ct = default);
    }

    /// <summary>
    /// All of the business logic for games will go here
    /// </summary>
    public class GamesService : ServiceInt<Game>, IGamesService
    {
        private readonly Func<IGamesRepository> repoFactory;
        private IGamesRepository gameRepo => this.repoFactory();

        public GamesService(Func<IGamesRepository> repoFactory)
        {
            this.repoFactory = repoFactory;
        }

        protected override Game InitializeNew(Game entity)
        {
            return Game.InitializeNew(entity);
        }

        /// <summary>
        /// Find all of the games
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Game>> FindAll()
        {
            var games = await this.gameRepo.FindAll();
            if (games.Any())
            {
                return games;
            }

            return new List<Game>();
        }

        /// <summary>
        /// Return all of the games by city
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Game>> GetGamesByCity(string cityName)
        {
            return await this.gameRepo.GetGamesByCity(cityName);
        }

        /// <summary>
        /// Find one game by Id
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public async Task<Game> FindOne(int gameId)
        {
            return await this.gameRepo.FindOne(gameId);
        }

        /// <summary>
        /// Create a game 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> CreateGame(Game entity, CancellationToken ct)
        {
            return await base.Create(this.gameRepo, entity, ct);
        }

        /// <summary>
        /// Updates a game's city
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="updateCommand"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<Game> UpdateGame(int gameId, Game updateCommand, CancellationToken ct = default)
        {
            var game = await this.gameRepo.FindOne(gameId);
            if (game == null)
            {
                //We should probably add more logging here
                return null;
            }

            return await base.Update(
                this.gameRepo,
                g => g.UpdateGame(updateCommand),
                gameId,
                game,
                ct);
        }

        /// <summary>
        /// This update game will hit the repo directly
        /// </summary>
        /// <param name="updatedGame"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<Game> PatchUpdateGame(Game updatedGame, CancellationToken ct = default)
        {
            return await base.PatchUpdate(
                this.gameRepo,
                updatedGame.Id,
                updatedGame,
                ct);
        }

        /// <summary>
        /// Returns true if game exists
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> Contains(int gameId, CancellationToken ct = default)
        {
            return await this.gameRepo.Contains(gameId, ct);
        }

        /// <summary>
        /// Delete the game 
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> Delete(int gameId, CancellationToken ct = default)
        {
            var game = await this.gameRepo.FindOne(gameId);
            if (game == null)
            {
                //We should log or return another type of information
                return false;
            }

            return await this.gameRepo.Delete(game, ct);
        }
    }
}
