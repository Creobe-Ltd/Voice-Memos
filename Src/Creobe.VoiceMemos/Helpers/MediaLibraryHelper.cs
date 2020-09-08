using Microsoft.Devices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System;

namespace Creobe.VoiceMemos.Helpers
{
    public class MediaLibraryHelper
    {
        public static bool SaveToMediaLibrary(string fileName, string name, int duration)
        {
            bool isSuccess = false;
            isSuccess = SaveToMediaLibrary(fileName, null, name, duration);
            return isSuccess;
        }

        public static bool SaveToMediaLibrary(string audioFileName, string imageFileName, string name, int duration)
        {
            bool isSuccess = false;
            isSuccess = SaveToMediaLibrary(audioFileName, imageFileName, name, "Voice Memos", "Voice Memos", duration);
            return isSuccess;
        }

        public static bool SaveToMediaLibrary(string audioFileName, string imageFileName, string name, string artistName, string albumName, int duration)
        {
            bool isSuccess = false;

            using (MediaLibrary mediaLibrary = new MediaLibrary())
            {
                try
                {
                    var metaData = new SongMetadata
                    {
                        ArtistName = artistName,
                        AlbumName = albumName,
                        Name = name,
                        Duration = TimeSpan.FromSeconds(duration)
                    };

                    if (!string.IsNullOrWhiteSpace(imageFileName))
                        metaData.AlbumArtUri = new Uri(imageFileName, UriKind.RelativeOrAbsolute);

                    var song = mediaLibrary.SaveSong(new Uri(audioFileName, UriKind.RelativeOrAbsolute), metaData, SaveSongOperation.CopyToLibrary);

                    isSuccess = true; ;
                }
                catch
                {
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        public static void AddToHistory(string name, string imageFileName, int id)
        {
            MediaHistoryItem historyItem = new MediaHistoryItem();
            historyItem.Title = name;
            historyItem.Source = string.Empty;
            historyItem.PlayerContext.Add("Action", "Memo");
            historyItem.PlayerContext.Add("Id", id.ToString());
            MediaHistory.Instance.WriteRecentPlay(historyItem);
        }
    }
}
