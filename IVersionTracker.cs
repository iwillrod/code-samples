using System;
namespace CodeSamples
{
    public interface IVersionTracker
    {
        int Version { get; set; }

        void MarkAsPersisted();

        bool HasChanges();
    }
}
