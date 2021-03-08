
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace CodeSamples
{
    public interface IGamesRepository : IRepo<Game, int>
    {
        Task<IEnumerable<Game>> GetGamesByCity(string cityName);
    }

    public class GamesRepository : PostgreSqlServerIntRepo<Game>, IGamesRepository
    {
        public GamesRepository(GameDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public override IQueryable<Game> GetAggregateSet(DbContext dbContext)
        {
            return this.dbContext.Set<Game>();
        }

        public override async Task<Game> FindOne(int id, CancellationToken cancellationToken = default)
        {
            var dbSet = this.GetAggregateSet(this.dbContext);
            return await dbSet.FirstOrDefaultAsync(g => g.Id == id);
        }

        /// <summary>
        /// Get the number of games that match the cityName
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Game>> GetGamesByCity(string cityName)
        {
            var dbSet = this.GetAggregateSet(this.dbContext);
            return await dbSet.Where(g => g.City == cityName).ToListAsync();
        }
    }
}
