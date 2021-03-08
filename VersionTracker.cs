using System;
using System.Runtime.Serialization;

namespace CodeSamples
{
    public class VersionTracker : IVersionTracker
    {
        [DataMember(Name = "createdUtcDate")]
        public DateTime CreatedUtcDate { get; set; }

        [DataMember(Name = "updatedUtcDate")]
        public DateTime UpdatedUtcDate { get; set; }

        [DataMember(Name = "version")]
        public int Version { get; set; }

        protected int initialVersion = -1;
        protected bool hasChanges;

        public bool HasChanges()
        {
            return this.hasChanges;
        }

        public void MarkAsPersisted()
        {
            this.initialVersion = this.Version;
            this.hasChanges = false;
        }

        public void MarkAsChanged()
        {
            if(this.initialVersion == -1)
            {
                this.initialVersion = this.Version;
            }

            if(this.Version <= 0)
            {
                this.CreatedUtcDate = DateTime.UtcNow;
            }

            if(this.initialVersion == this.Version)
            {
                this.Version++;
            }

            this.UpdatedUtcDate = DateTime.UtcNow;
            this.hasChanges = true;
        }
    }
}
