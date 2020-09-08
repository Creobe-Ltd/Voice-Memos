using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Core.Data
{
    public interface IRepository<T>
    {
        ObservableCollection<T> All { get; }
        int Count { get; }

        T Find(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);
        Task LoadCollectionsAsync();
    }
}
