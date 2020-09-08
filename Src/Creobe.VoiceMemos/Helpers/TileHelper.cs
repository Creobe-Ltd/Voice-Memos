using Creobe.VoiceMemos.Data.Models;
using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Resources;
using Creobe.VoiceMemos.TileTemplates;
using Microsoft.Phone.Shell;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Creobe.VoiceMemos.Helpers
{
    public class TileHelper
    {
        public static void PinRecord()
        {
            Uri tileUri = App.ViewLocator.View("Main", new { Action = "Record" });

            if (ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri) == null)
            {
                StandardTileData tileData = new StandardTileData();
                tileData.Title = AppResources.RecordTileTitle;
                tileData.BackgroundImage = new Uri("/Assets/Tiles/RecordTileMedium.png", UriKind.RelativeOrAbsolute);

                ShellTile.Create(tileUri, tileData);
            }
        }

        public static void UnPinRecord()
        {
            Uri tileUri = App.ViewLocator.View("Main", new { Action = "Record" });

            var tile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri);

            if (tile != null)
                tile.Delete();
        }

        public static bool IsRecordPinnned()
        {
            Uri tileUri = App.ViewLocator.View("Main", new { Action = "Record" });

            return ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri) != null;
        }

        public async static void PinMemo(Memo memo)
        {
            try
            {
                Uri tileUri = App.ViewLocator.View("Main", new { Action = "Memo", Id = memo.Id });

                if (ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri) == null)
                {
                    StandardTileData tileData = new StandardTileData();
                    tileData.BackgroundImage = await GetMemoTileBackground(memo);
                    tileData.BackBackgroundImage = await GetMemoTileBackBackground(memo);

                    ShellTile.Create(tileUri, tileData);
                }
            }
            catch
            {
                //do nothing

            }
        }

        public static void UnPinMemo(Memo memo)
        {
            Uri tileUri = App.ViewLocator.View("Main", new { Action = "Memo", Id = memo.Id });

            var tile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri);

            if (tile != null)
                tile.Delete();
        }

        public async static void UpdateMemo(Memo memo)
        {
            try
            {
                Uri tileUri = App.ViewLocator.View("Main", new { Action = "Memo", Id = memo.Id });

                var tile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri);

                if (tile != null)
                {
                    StandardTileData tileData = new StandardTileData();
                    tileData.BackgroundImage = await GetMemoTileBackground(memo);
                    tileData.BackBackgroundImage = await GetMemoTileBackBackground(memo);

                    tile.Update(tileData);
                }
            }
            catch
            {
                //do nothing
            }
        }

        public static bool IsMemoPinned(Memo memo)
        {
            Uri tileUri = App.ViewLocator.View("Main", new { Action = "Memo", Id = memo.Id });

            return ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri) != null;
        }

        public static void PinTag(Tag tag)
        {
            Uri tileUri = App.ViewLocator.View("Main", new { Action = "Tag", Id = tag.Id });

            if (ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri) == null)
            {
                StandardTileData tileData = new StandardTileData();
                tileData.Title = tag.Name;
                tileData.BackgroundImage = new Uri("/Assets/Tiles/TagTileMedium.png", UriKind.RelativeOrAbsolute);

                ShellTile.Create(tileUri, tileData);
            }
        }

        public static void UnPinTag(Tag tag)
        {
            Uri tileUri = App.ViewLocator.View("Main", new { Action = "Tag", Id = tag.Id });

            var tile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri);

            if (tile != null)
                tile.Delete();
        }

        public static bool IsTagPinnned(Tag tag)
        {
            Uri tileUri = App.ViewLocator.View("Main", new { Action = "Tag", Id = tag.Id });

            return ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri) != null;
        }

        public static void PinFavorites()
        {
            Uri tileUri = App.ViewLocator.View("Main", new { Action = "Favorites" });

            if (ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri) == null)
            {
                StandardTileData tileData = new StandardTileData();
                tileData.Title = AppResources.FavoritesTileTitle;
                tileData.BackgroundImage = new Uri("/Assets/Tiles/FavoritesTileMedium.png", UriKind.RelativeOrAbsolute);

                ShellTile.Create(tileUri, tileData);
            }
        }

        public static void UnPinFavorites()
        {
            Uri tileUri = App.ViewLocator.View("Main", new { Action = "Favorites" });

            var tile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri);

            if (tile != null)
                tile.Delete();
        }

        public static bool IsFavoritesPinnned()
        {
            Uri tileUri = App.ViewLocator.View("Main", new { Action = "Favorites" });

            return ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == tileUri) != null;
        }

        public static void SetPrimaryTileCount(int count)
        {
            var tile = ShellTile.ActiveTiles.FirstOrDefault();

            if (tile != null)
            {
                IconicTileData tileData = new IconicTileData();
                tileData.Count = count;

                tile.Update(tileData);
            }
        }

        public static void UnPinSecondaryTiles()
        {
            foreach (var tile in ShellTile.ActiveTiles.Where(t => t.NavigationUri != new Uri("/", UriKind.RelativeOrAbsolute)))
            {
                tile.Delete();
            }
        }

        public static void ClearPrimaryTileCount()
        {
            SetPrimaryTileCount(0);
        }

        private async static Task<Uri> GetMemoTileBackground(Memo memo)
        {
            if (!string.IsNullOrWhiteSpace(memo.ImageFile))
            {
                MemoMediumWithImage tileTemplate = new MemoMediumWithImage();

                var image = await StorageHelper.GetImageFromFileAsync(memo.ImageFile);
                image.DecodePixelHeight = 336;

                tileTemplate.BackgroundImage = image;

                WriteableBitmap wb = new WriteableBitmap(336, 336);
                wb.Render(tileTemplate, null);
                wb.Invalidate();

                await StorageHelper.SaveTileImageAsync(memo.Id + ".jpg", wb);

                return new Uri("isostore:/Shared/ShellContent/" + memo.Id + ".jpg", UriKind.RelativeOrAbsolute);

            }

            return new Uri("/Assets/Tiles/MemoNoImageTileMedium.png", UriKind.RelativeOrAbsolute);
        }

        private async static Task<Uri> GetMemoTileBackBackground(Memo memo)
        {

            MemoMediumBack tileTemplate = new MemoMediumBack();

            if (!string.IsNullOrWhiteSpace(memo.ImageFile))
            {
                var image = await StorageHelper.GetImageFromFileAsync(memo.ImageFile);
                image.DecodePixelHeight = 336;

                tileTemplate.BackgroundImage = image;
            }

            tileTemplate.Title = memo.Title;
            tileTemplate.CreatedDate = memo.CreatedDate.Value;
            tileTemplate.Duration = TimeSpan.FromSeconds(memo.Duration);
            tileTemplate.UpdateLayout();

            WriteableBitmap wb = new WriteableBitmap(336, 336);
            wb.Render(tileTemplate, null);
            wb.Invalidate();

            await StorageHelper.SaveTileImageAsync(memo.Id + "_back.jpg", wb);

            return new Uri("isostore:/Shared/ShellContent/" + memo.Id + "_back.jpg", UriKind.RelativeOrAbsolute);
        }
    }
}
