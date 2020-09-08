using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Creobe.VoiceMemos.Helpers
{
    public class StorageHelper
    {
        public static StorageFolder Local
        {
            get
            {
                return ApplicationData.Current.LocalFolder;
            }
        }

        public static async Task<IRandomAccessStream> CreateFileForWriteAsync(string fileName)
        {
            return await CreateFileForWriteAsync(fileName, false);
        }

        public static async Task<IRandomAccessStream> CreateFileForWriteAsync(string fileName, bool overwrite)
        {
            var collisionOption = overwrite ? CreationCollisionOption.OpenIfExists : CreationCollisionOption.FailIfExists;
            var file = await Local.CreateFileAsync(fileName, collisionOption);
            var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
            return fileStream;
        }

        public static async Task<Stream> OpenFileForReadAsync(string fileName)
        {
            var stream = await Local.OpenStreamForReadAsync(fileName);
            return stream;
        }

        public static async Task<Stream> OpenFileForWriteAsync(string fileName)
        {
            var stream = await Local.OpenStreamForWriteAsync(fileName, CreationCollisionOption.OpenIfExists);
            return stream;
        }

        public static async Task CopyFileAsync(string sourceFileName, string destinationFolder, string destinationFileName)
        {
            var file = await Local.GetFileAsync(sourceFileName);
            var folder = await Local.CreateFolderAsync(destinationFolder, CreationCollisionOption.OpenIfExists);
            await file.CopyAsync(folder, destinationFileName, NameCollisionOption.ReplaceExisting);
        }

        public static async void SaveImageFile(string fileName, BitmapImage image)
        {
            var wb = new WriteableBitmap(image);
            wb.Invalidate();

            await SaveImageFileAsync(fileName, wb);
        }

        public static async Task SaveImageFileAsync(string fileName, WriteableBitmap image)
        {
            using (var fileStream = await (CreateFileForWriteAsync(fileName, true)))
            {
                using (var outStream = fileStream.AsStream())
                {
                    image.SaveJpeg(outStream, image.PixelWidth, image.PixelHeight, 0, 85);
                    outStream.Flush();
                    outStream.Close();
                }
            }
        }

        public static async Task SaveTileImageAsync(string fileName, WriteableBitmap image)
        {
            using (var fileStream = await (CreateFileForWriteAsync(Path.Combine("Shared\\ShellContent", fileName), true)))
            {
                using (var outStream = fileStream.AsStream())
                {
                    image.SaveJpeg(outStream, image.PixelWidth, image.PixelHeight, 0, 85);
                    outStream.Flush();
                    outStream.Close();
                }
            }
        }

        public static async Task<BitmapImage> GetImageFromFileAsync(string fileName)
        {
            var stream = await OpenFileForReadAsync(fileName);
            var image = new BitmapImage();
            image.SetSource(stream);
            return image;
        }

        public static async Task<WriteableBitmap> GetBitmapFromFileAsync(string fileName)
        {
            var bmp = await GetImageFromFileAsync(fileName);
            return new WriteableBitmap(bmp);
        }

        public static string GetFilePath(string fileName)
        {
            return Path.Combine(Local.Path, fileName);
        }

        public static bool FileExists(string fileName)
        {
            return File.Exists(GetFilePath(fileName));
        }

        public static async Task DeleteFileAsync(string fileName)
        {
            if (FileExists(fileName))
            {
                var file = await Local.GetFileAsync(fileName);
                await file.DeleteAsync();
            }
        }

        public static long GetAvailableFreeSpace()
        {
            using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return iso.AvailableFreeSpace;
            }
        }

        public static void DeleteIsolatedStorage()
        {
            using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                iso.Remove();
            }
        }
    }
}
