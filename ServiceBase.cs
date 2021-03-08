using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeSamples
{
    public abstract class ServiceBase<TSource, TKey>
        where TSource : Aggregate<TKey>
    {
        /// <summary>
        /// NOTE: We could also have passed in the repo through the
        /// concrete classes that implement this class
        /// Since we are passing in repo to the base class through
        /// the parameters, to promote the use of the same repo/dbContext
        /// </summary>
        protected ServiceBase()
        {
        }

        protected abstract TSource InitializeNew(TSource entity);

        /// <summary>
        /// TODO: Instead of returning a bool, we can return more states
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="entity"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        protected virtual async Task<bool> Create(IRepo<TSource, TKey> repo, TSource entity, CancellationToken ct = default)
        {
            try
            {
                entity = this.InitializeNew(entity);
                var isSuccess = await repo.Create(entity, ct);
                if (isSuccess)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex; //TODO: Deal with exceptions that can arise from this operation
            }
        }

        /// <summary>
        /// TODO: Instead of returning the updated TSource, return a complex object
        /// with info about the update
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="actionOnTheSource"></param>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        protected virtual async Task<TSource> Update(IRepo<TSource, TKey> repo, Action<TSource> actionOnTheSource, TKey id, TSource entity, CancellationToken ct = default)
        {
            try
            {
                actionOnTheSource(entity);
                if (!entity.HasChanges())
                {
                    return entity;
                }

                var isSuccess = await repo.Update(entity, ct);
                if (!isSuccess)
                {
                    return null;
                }

                entity.MarkAsPersisted();

                //TODO: We can have this in a loop to try to update again
                //Here we should use the ID in order to do a retry
                return entity;
            }
            catch (Exception ex)
            {
                throw ex; //TODO: Deal with exception
            }
        }

        protected virtual async Task<TSource> PatchUpdate(IRepo<TSource, TKey> repo, TKey id, TSource entity, CancellationToken ct = default)
        {
            try
            {
                entity.MarkAsChanged();
                var isSuccess = await repo.Update(entity, ct);
                if (!isSuccess)
                {
                    return null;
                }

                entity.MarkAsPersisted();
                return entity;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}