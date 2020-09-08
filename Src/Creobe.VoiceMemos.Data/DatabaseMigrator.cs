using Creobe.VoiceMemos.Data;
using Creobe.VoiceMemos.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data
{
    public class DatabaseMigrator
    {
        public static async Task MigrateAsync()
        {
            VoiceMemos.Data.Legacy.VoiceMemosContext.Initialize("Data Source=isostore:/VoiceMemos.sdf");

            var dbContext = new VoiceMemos.Data.Legacy.VoiceMemosContext("Data Source=isostore:/VoiceMemos.sdf");

            VoiceMemosDatabase.Instance.Purge();

            await Task.Run(() =>
            {

                //foreach (var tag in dbContext.Tags)
                //{
                //    var newTag = new Tag
                //    {
                //        Id = tag.Id,
                //        CreatedDate = tag.CreatedDate,
                //        ModifiedDate = tag.ModifiedDate,
                //        Name = tag.Name
                //    };

                //    App.Uow.TagRepository.Add(newTag);
                //}

                var tags = dbContext.Tags.Select(t => new Tag
                {
                    Id = t.Id,
                    CreatedDate = t.CreatedDate,
                    ModifiedDate = t.ModifiedDate,
                    Name = t.Name
                }).ToList();

                VoiceMemosDatabase.Tags.Save(tags);
            });

            await Task.Run(() =>
            {
                var memos = dbContext.Memos.Select(m => new Memo
                    {
                        Id = m.Id,
                        AudioFile = m.AudioFile,
                        AudioFormat = m.AudioFormat,
                        BitRate = m.BitRate,
                        Channels = m.Channels,
                        CreatedDate = m.CreatedDate,
                        Description = m.Description,
                        Duration = m.Duration,
                        ImageFile = m.ImageFile,
                        IsPlayed = m.IsPlayed,
                        Latitude = m.Latitude,
                        Longitude = m.Longitude,
                        ModifiedDate = m.ModifiedDate,
                        SampleRate = m.SampleRate,
                        Title = m.Title,
                        TagsFK = m.Tags.Select(t => t.Tag.Id).ToArray()
                    }).ToList();

                //foreach (var tag in memo.Tags)
                //{
                //    newMemo.Tags.Add(App.Uow.TagRepository.Find(tag.Id));
                //}

                //App.Uow.MemoRepository.Add(newMemo);

                VoiceMemosDatabase.Memos.Save(memos);

            });

            await Task.Run(() =>
            {
                var favorites = dbContext.Favorites.Select(f => new Favorite
                    {
                        Id = f.Id,
                        CreatedDate = f.CreatedDate,
                        MemoFK = f.Memo.Id,
                        ModifiedDate = f.ModifiedDate,
                    }).ToList();

                VoiceMemosDatabase.Favorites.Save(favorites);
                
            });
        }

        //private static void MigrateMemo(Creobe.VoiceMemos.Models.Legacy.Memo memo)
        //{



        //}

        //private static void MigrateTag(Creobe.VoiceMemos.Models.Legacy.Tag tag)
        //{

        //}
    }
}
