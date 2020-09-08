using Creobe.VoiceMemos.Core.Extensions;
using Creobe.VoiceMemos.Data.Models;
using Lex.Db;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data.Repositories
{
    public class MemoRepository : Repository<Memo>
    {
        private ObservableCollection<Memo> _recent;
        private ObservableCollection<Memo> _allWithLocation;

        public MemoRepository(DbInstance dbInstance) :
            base(dbInstance) { }

        public ObservableCollection<Memo> Recent
        {
            get
            {
                return _recent;
            }
        }

        public int UnplayedCount
        {
            get
            {
                return instance.Table<Memo>()
                    .Where(m => !m.IsPlayed)
                    .Count();
            }
        }

        public ObservableCollection<Memo> AllWithLocation
        {
            get
            {
                return _allWithLocation;
            }
        }


        public IEnumerable<Memo> ByTag(int id)
        {
            return instance.Table<Memo>()
                .Where(m => m.TagsFK != null && m.TagsFK.Contains(id));
        }

        public IEnumerable<Memo> ByTagWithLocation(int id)
        {
            return ByTag(id)
                .Where(m => m.Longitude.HasValue && m.Latitude.HasValue);
        }

        public IEnumerable<Memo> Untagged()
        {
            return instance.Table<Memo>()
                .Where(m => m.Tags.Count < 1);
        }

        public override void Add(Memo entity)
        {
            base.Add(entity);

            _all.SortDescending(m => m.CreatedDate);

            _recent.AddToCollection(entity, m => m.CreatedDate, ListSortDirection.Descending);

            if (_recent.Count > 4)
                _recent.RemoveFromCollection(_recent.LastOrDefault());

            if (entity.Latitude.HasValue && entity.Longitude.HasValue)
                _allWithLocation.AddToCollection(entity);
        }

        public override void Update(Memo entity)
        {
            base.Update(entity);

            if (entity.Latitude.HasValue && entity.Longitude.HasValue)
                _allWithLocation.AddToCollection(entity);
            else
                _allWithLocation.Remove(entity);

        }

        public override void Delete(Memo entity)
        {
            base.Delete(entity);

            _recent.RemoveFromCollection(entity);

            if (_recent.Count < 4 && _recent.Count < _all.Count)
            {
                foreach (var item in _all)
                {
                    if (!_recent.Contains(item))
                    {
                        _recent.AddToCollection(item);
                        break;
                    }
                }
            }

            if (entity.Latitude.HasValue && entity.Longitude.HasValue)
                _allWithLocation.RemoveFromCollection(entity);
        }

        public override async Task LoadCollectionsAsync()
        {
            await Task.Run(async () =>
            {
                await Task.Run(() =>
                {
                    _recent = new ObservableCollection<Memo>(instance.Table<Memo>().LoadAll()
                        .OrderByDescending(m => m.CreatedDate)
                        .Take(4));
                });

                await Task.Run(() =>
                {
                    _all = new ObservableCollection<Memo>(instance.Table<Memo>().LoadAll()
                        .OrderByDescending(m => m.CreatedDate));
                });

                await Task.Run(() =>
                {
                    _allWithLocation = new ObservableCollection<Memo>(instance.Table<Memo>().LoadAll()
                        .Where(m => m.Longitude.HasValue && m.Latitude.HasValue));
                });
            });

        }
    }
}
