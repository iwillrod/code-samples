using System;
namespace CodeSamples
{
    /// <summary>
    /// Class to represent an aggregate without explicit key
    /// </summary>
    public abstract class Aggregate : VersionTracker
    {
    }

    /// <summary>
    /// Class to represent an aggregate with an explicit key
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Aggregate<TKey> : VersionTracker
    {
        public virtual TKey Id { get; set; }
    }
}
