using System.Data.Linq;

namespace Creobe.VoiceMemos.Core.Data
{
    public interface IMigration
    {
        int Version { get; }

        void Apply(DataContext dbContext);
    }
}
