using Creobe.VoiceMemos.Data.Legacy;
using Creobe.VoiceMemos.Data.Legacy.Repositories;
using Creobe.VoiceMemos.Models;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data.Legacy
{
    public class VoiceMemosUow : UnitOfWorkBase<VoiceMemosContext>
    {
        private FavoriteRepository favoriteRepository;

        public FavoriteRepository FavoriteRepository
        {
            get 
            {
                if (favoriteRepository == null)
                    favoriteRepository = new FavoriteRepository(DbContext);

                return favoriteRepository; 
            }
        }
        
        private MemoRepository memoRepository;

        public MemoRepository MemoRepository
        {
            get
            {
                if (memoRepository == null)
                    memoRepository = new MemoRepository(DbContext);

                return memoRepository;
            }
        }

        private MemoTagRepository memoTagRepository;

        public MemoTagRepository MemoTagRepository
        {
            get
            {
                if (memoTagRepository == null)
                    memoTagRepository = new MemoTagRepository(DbContext);

                return memoTagRepository;
            }
        }

        private TagRepository tagRepository;

        public TagRepository TagRepository
        {
            get
            {
                if (tagRepository == null)
                    tagRepository = new TagRepository(DbContext);

                return tagRepository;
            }
        }

        public VoiceMemosUow(string fileOrConnection) :
            base(fileOrConnection) { }

        public VoiceMemosUow(string fileOrConnection, System.Data.Linq.Mapping.MappingSource mapping) :
            base(fileOrConnection, mapping) { }

        public override async Task LoadCollectionsAsync()
        {
            await MemoRepository.LoadCollectionsAsync();
            await TagRepository.LoadCollectionsAsync();
            await MemoTagRepository.LoadCollectionsAsync();
            await FavoriteRepository.LoadCollectionsAsync();
        }
    }
}
