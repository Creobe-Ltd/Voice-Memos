using Creobe.VoiceMemos.Core.Extensions;
using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Models.Legacy;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data.Legacy.Repositories
{
    public class TagRepository : Repository<Tag>
    {
        public TagRepository(DataContext context) :
            base(context) { }

        public IQueryable<Tag> TagsByName(string tag)
        {
            return dbContext.GetTable<Tag>()
                .Where(t => t.Name.Contains(tag));
        }

        public override void Add(Tag entity)
        {
            base.Add(entity);

            _all.Sort(t => t.Name);
        }

        public override async Task LoadCollectionsAsync()
        {
            await Task.Run(() =>
            {
                _all = new ObservableCollection<Tag>(dbContext.GetTable<Tag>()
                    .OrderBy(t => t.Name));
            });
        }
    }
}
