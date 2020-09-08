using Creobe.SoundCloud;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Helpers
{
    public class SoundCloudHelper
    {
        private static Regex _specialRegEx = new Regex(@"[^0-9a-zA-Z\.]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static SoundCloudAuthClient _authClient = new SoundCloudAuthClient("fa3c66c1c32cb0baba9c719b88fd7c2b");
        private static SoundCloudSession _session;

        public static async Task<SoundCloudSessionStatus> GetSessionStatusAsync()
        {
            try
            {
                var result = await _authClient.InitializeAsync();

                if (result.Status == SoundCloudSessionStatus.Connected)
                    _session = result.Session;

                return result.Status;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public static async Task<SoundCloudSessionStatus> LoginAsync()
        {
            try
            {
                var result = await _authClient.LoginAsync("non-expiring");

                if (result.Status == SoundCloudSessionStatus.Connected)
                    _session = result.Session;

                return result.Status;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public static void Logout()
        {
            _authClient.LogOut();
        }

        public static Task<string> UploadFileAsync(string title, string fileName, string contentType)
        {
            return UploadFileAsync(title, fileName, contentType, CancellationToken.None, null);
        }

        public static async Task<string> UploadFileAsync(string title, string fileName, string contentType, CancellationToken ct, IProgress<SoundCloudUploadProgressChangedEventArgs> progress)
        {
            if (_session == null)
                throw new InvalidOperationException("Session is null");

            string uploadFileName = _specialRegEx.Replace(title, "-");

            SoundCloudApiClient client = new SoundCloudApiClient(_session);

            using (var fileStream = await StorageHelper.OpenFileForReadAsync(fileName))
            {
                var result = await client.UploadAsync(title, uploadFileName, contentType, fileStream, ct, progress);
                var dict = (IDictionary<string, object>)result;

                return (string)dict["permalink_url"];
            }
        }
    }
}
