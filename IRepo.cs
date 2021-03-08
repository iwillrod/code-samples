using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeSamples
{
    public interface IRepo<TSource, TDbId>
        where TSource : class
    {
        Task<bool> Create(TSource item, CancellationToken ct = default);

        Task<bool> Delete(TSource item, CancellationToken ct = default);

        Task<bool> Update(TSource item, CancellationToken ct = default);

        Task<TSource> FindOne(TDbId id, CancellationToken cancellationToken = default);

        Task<IEnumerable<TSource>> FindAll();

        Task<bool> Contains(TDbId id, CancellationToken ct = default);
    }
}
