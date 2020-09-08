using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Core.Legacy.Data
{
    public interface IUnitOfWork
    {
        Task LoadCollectionsAsync();
        void DeleteDatabase();
        void Save();
    }
}
