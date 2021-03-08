using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CodeSamples
{
    public abstract class SqlRepo<TSource, TKey> : IRepo<TSource, TKey>
        where TSource : Aggregate<TKey>
    {
        protected DbContext dbContext;

        public abstract IQueryable<TSource> GetAggregateSet(DbContext dbContext);

        public abstract Task<TSource> FindOne(TKey id, CancellationToken cancellationToken = default);

        public abstract Task<bool> Contains(TKey id, CancellationToken ct = default);

        /// <summary>
        /// Base class method to check in db if expression exists
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        protected async Task<bool> ContainsByExpression(Expression<Func<TSource, bool>> expression, CancellationToken ct = default)
        {
            return await this.dbContext.Set<TSource>().AnyAsync(expression, ct);
        }

        /// <summary>
        /// Create method for many entities
        /// </summary>
        /// <param name="items"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> Create(IEnumerable<TSource> items, CancellationToken ct = default)
        {
            return await this.Insert(this.dbContext, items, ct);
        }

        /// <summary>
        /// Create method for a single entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> Create(TSource item, CancellationToken ct = default)
        {
            return await this.Insert(this.dbContext, new[] { item }, ct);
        }

        private async Task<bool> Insert(DbContext passedDbContext, IEnumerable<TSource> items, CancellationToken ct)
        {
            var collection = passedDbContext.Set<TSource>();
            var itemsArray = items as TSource[] ?? items.ToArray();

            collection.AddRange(itemsArray);

            await passedDbContext.SaveChangesAsync(ct);

            foreach (var item in itemsArray)
            {
                item.MarkAsPersisted();
            }

            return true;
        }

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> Delete(TSource item, CancellationToken ct = default)
        {
            try
            {
                this.dbContext.Set<TSource>().Attach(item);
                this.dbContext.Entry(item).State = EntityState.Deleted;
                await this.dbContext.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex; //TODO: Need to deal with this exception
            }
        }

        /// <summary>
        /// Update an entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<bool> Update(TSource item, CancellationToken ct = default)
        {
            try
            {
                this.dbContext.Set<TSource>().Attach(item);
                var entry = this.dbContext.Entry(item);

                //Action<EntityEntry<TSource>> originalValues = o => o.OriginalValues["Version"] = item.Version - 1;
                //TODO: Need to revise this and see how I can do validation on the Version
                entry.State = EntityState.Modified;

                await this.dbContext.SaveChangesAsync(ct);
                return true;
            }
            catch (Exception ex)
            {
                throw ex; //TODO: Handle exception
            }
        }

        public async Task<IEnumerable<TSource>> FindAll()
        {
            var dbSet = this.GetAggregateSet(this.dbContext);
            return await dbSet.OrderBy(o => o.Id).ToListAsync();
        }

        public void Dispose()
        {
            this.dbContext.Dispose();
        }
    }
}
