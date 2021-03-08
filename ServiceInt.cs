using System;

namespace CodeSamples
{
    public abstract class ServiceInt<TSource> : ServiceBase<TSource, int>
        where TSource : AggregateInt
    {
        protected ServiceInt()
        {
        }
    }
}
