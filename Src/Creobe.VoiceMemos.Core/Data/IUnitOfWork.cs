using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Core.Data
{
    public interface IUnitOfWork
    {
        Task LoadCollectionsAsync();
        void DeleteDatabase();
        //void Save();
    }
}
