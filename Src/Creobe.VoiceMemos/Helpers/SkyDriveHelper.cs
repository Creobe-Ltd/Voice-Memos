using Microsoft.Live;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Helpers
{
    public class SkyDriveHelper
    {
        private static string[] _scopes = new string[] { "wl.basic", "wl.signin", "wl.offline_access", "wl.skydrive_update" };
        private static Regex _specialRegEx = new Regex(@"[^0-9a-zA-Z\.]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static LiveAuthClient _authClient = new LiveAuthClient("0000000044109B61");
        private static LiveConnectSession _session;

        public static async Task<LiveConnectSessionStatus> GetSessionStatusAsync()
        {
            try
            {
                var result = await _authClient.InitializeAsync(_scopes);

                if (result.Status == LiveConnectSessionStatus.Connected)
                    _session = result.Session;

                return result.Status;
            }
            catch (LiveAuthException ex)
            {
                throw new LiveAuthException(ex.ErrorCode, ex.Message, ex.InnerException);
            }
        }

        public static async Task<LiveConnectSessionStatus> LoginAsync()
        {
            try
            {
                var result = await _authClient.InitializeAsync(_scopes);

                if (result.Status != LiveConnectSessionStatus.Connected)
                    result = await _authClient.LoginAsync(_scopes);

                if (result.Status == LiveConnectSessionStatus.Connected)
                    _session = result.Session;

                return result.Status;
            }
            catch (LiveAuthException ex)
            {
                throw new LiveAuthException(ex.ErrorCode, ex.Message, ex.InnerException);
            }
        }

        public static void Logout()
        {
            try
            {
                _authClient.Logout();
            }
            catch (LiveConnectException ex)
            {
                throw new LiveConnectException(ex.ErrorCode, ex.Message, ex.InnerException);
            }
        }

        public static Task<string> UploadFileAsync(string sourceFileName, string destinationFileName)
        {
            return UploadFileAsync(sourceFileName, destinationFileName, CancellationToken.None, null);
        }

        public static async Task<string> UploadFileAsync(string sourceFileName, string destinationFileName, CancellationToken ct, IProgress<LiveOperationProgress> progress)
        {
            if (_session == null)
                throw new InvalidOperationException("Session is null");

            string uploadFileName = _specialRegEx.Replace(destinationFileName, "-");

            LiveConnectClient client = new LiveConnectClient(_session);

            using (var fileStream = await StorageHelper.OpenFileForReadAsync(sourceFileName))
            {
                var folderId = await GetOrCreateFolderAsync("Voice Memos", ct);
                var operationResult = await client.UploadAsync(folderId, uploadFileName, fileStream, OverwriteOption.Overwrite, ct, progress);

                dynamic result = operationResult.Result;
                return result.id;
            }
        }

        public static Task<string> GetOrCreateFolderAsync(string folder)
        {
            return GetOrCreateFolderAsync(folder, CancellationToken.None);
        }

        public static async Task<string> GetOrCreateFolderAsync(string folder, CancellationToken ct)
        {
            var folderId = await GetFolderAsync(folder, ct);

            if (string.IsNullOrWhiteSpace(folderId))
                folderId = await CreateFolderAsync(folder, ct);

            return folderId;
        }

        public static Task<string> GetFolderAsync(string folder)
        {
            return GetFolderAsync(folder, CancellationToken.None);
        }

        public static async Task<string> GetFolderAsync(string folder, CancellationToken ct)
        {
            if (_session == null)
                throw new InvalidOperationException("Session is null");

            LiveConnectClient client = new LiveConnectClient(_session);

            var operationResult = await client.GetAsync("me/skydrive/files?filter=folders", ct);
            var iEnum = operationResult.Result.Values.GetEnumerator();
            iEnum.MoveNext();
            var folders = iEnum.Current as IEnumerable;

            string folderId = null;

            foreach (dynamic f in folders)
            {
                if (f.name == folder)
                {
                    folderId = f.id;
                    break;
                }
            }

            return folderId;
        }

        public static Task<string> CreateFolderAsync(string folder)
        {
            return CreateFolderAsync(folder, CancellationToken.None);
        }

        public static async Task<string> CreateFolderAsync(string folder, CancellationToken ct)
        {
            if (_session == null)
                throw new InvalidOperationException("Session is null");

            LiveConnectClient client = new LiveConnectClient(_session);

            var folderData = new Dictionary<string, object>();
            folderData.Add("name", folder);

            var operationResult = await client.PostAsync("me/skydrive", folderData, ct);
            dynamic result = operationResult.Result;
            return result.id;
        }

        public static Task<string> GetLinkAsync(string fileId)
        {
            return GetLinkAsync(fileId, CancellationToken.None);
        }

        public static async Task<string> GetLinkAsync(string fileId, CancellationToken ct)
        {
            if (_session == null)
                throw new InvalidOperationException("Session is null");

            LiveConnectClient client = new LiveConnectClient(_session);

            var operationResult = await client.GetAsync(fileId + "/shared_read_link", ct);
            dynamic result = operationResult.Result;
            return result.link;
        }

        public static IEnumerable<LivePendingUpload> GetPendingUploads()
        {
            if (_session != null)
            {
                LiveConnectClient client = new LiveConnectClient(_session);

                return client.GetPendingBackgroundUploads();
            }


            return null;
        }
    }
}
