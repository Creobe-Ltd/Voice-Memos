using Creobe.VoiceMemos.Core.Extensions;
using Creobe.VoiceMemos.Data.Models;
using Lex.Db;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data.Repositories
{
    public class TagRepository : Repository<Tag>
    {
        public TagRepository(DbInstance dbInstance) :
            base(dbInstance) { }

        public IEnumerable<Tag> TagsByName(string tag)
        {
            return instance.Table<Tag>()
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
                _all = new ObservableCollection<Tag>(instance.Table<Tag>()
                    .OrderBy(t => t.Name));
            });
        }
    }
}
