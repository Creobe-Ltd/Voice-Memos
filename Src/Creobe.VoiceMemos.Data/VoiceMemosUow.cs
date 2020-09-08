using Creobe.VoiceMemos.Data.Legacy;
using Creobe.VoiceMemos.Data.Repositories;
using Creobe.VoiceMemos.Models;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data
{
    public class VoiceMemosUow : UnitOfWorkBase
    {
        private FavoriteRepository favoriteRepository;

        public FavoriteRepository FavoriteRepository
        {
            get 
            {
                if (favoriteRepository == null)
                    favoriteRepository = new FavoriteRepository(VoiceMemosDatabase.Instance);

                return favoriteRepository; 
            }
        }
        
        private MemoRepository memoRepository;

        public MemoRepository MemoRepository
        {
            get
            {
                if (memoRepository == null)
                    memoRepository = new MemoRepository(VoiceMemosDatabase.Instance);

                return memoRepository;
            }
        }

        //private MemoTagRepository memoTagRepository;

        //public MemoTagRepository MemoTagRepository
        //{
        //    get
        //    {
        //        if (memoTagRepository == null)
        //            memoTagRepository = new MemoTagRepository(DbConnection);

        //        return memoTagRepository;
        //    }
        //}

        private TagRepository tagRepository;

        public TagRepository TagRepository
        {
            get
            {
                if (tagRepository == null)
                    tagRepository = new TagRepository(VoiceMemosDatabase.Instance);

                return tagRepository;
            }
        }

        public override async Task LoadCollectionsAsync()
        {
            await MemoRepository.LoadCollectionsAsync();
            await TagRepository.LoadCollectionsAsync();
            //await MemoTagRepository.LoadCollectionsAsync();
            await FavoriteRepository.LoadCollectionsAsync();
        }
    }
}
