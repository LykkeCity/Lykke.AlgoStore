using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage;

namespace Lykke.AlgoStore.AzureRepositories.Utils
{
    public class StorageConnectionManager : IStorageConnectionManager
    {
        private readonly IReloadingManager<string> _reloadingDbManager;
        private CloudStorageAccount _storageAccount;

        public StorageConnectionManager(IReloadingManager<string> reloadingDbManager)
        {
            _reloadingDbManager = reloadingDbManager;
            Initialize().Wait();
        }

        private async Task Initialize()
        {
            string connection = await _reloadingDbManager.Reload();
            var storageAccount = CloudStorageAccount.Parse(connection);
            _storageAccount = storageAccount;
        }

        public async Task Refresh()
        {
            await Initialize();
        }

        public StorageConnectionData GetData(string key)
        {
            var result = new StorageConnectionData();
            result.DateHeader = GetDateHeader();
            result.VersionHeader = GetServerVersionHeader();
            result.Url = GetUrl(key);

            GetAuthenticationHeader(result);

            return result;
        }

        private string GetDateHeader()
        {
            const string dateHeaderFormat = "x-ms-date:{0}";

            return string.Format(dateHeaderFormat, DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture));
        }

        private string GetServerVersionHeader()
        {
            const string serverVersionHeaderFormat = "x-ms-version:{0}";

            return string.Format(serverVersionHeaderFormat, "2017-04-17");
        }

        private void GetAuthenticationHeader(StorageConnectionData data)
        {
            string headers = string.Format("{0}/n{1}", data.DateHeader, data.VersionHeader);
            string resource = string.Format("/{0}/{1}", _storageAccount.Credentials.AccountName, AlgoBlobRepository.BlobContainer);

            var messageSignature = String.Format("GET\n\n\n\n\n\n\n\n\n\n\n\n{0}\n{1}", headers, resource);

            byte[] signatureBytes = Encoding.UTF8.GetBytes(messageSignature);

            // Create the HMACSHA256 version of the storage key.
            var sha256 = new HMACSHA256(Convert.FromBase64String(_storageAccount.Credentials.ExportBase64EncodedKey()));

            // This is the actual header that will be added to the list of request headers.
            AuthenticationHeaderValue authHV = new AuthenticationHeaderValue("SharedKey", _storageAccount.Credentials.AccountName + ":" + Convert.ToBase64String(sha256.ComputeHash(signatureBytes)));

            data.AuthorizationHeader = authHV.ToString();
        }

        private string GetUrl(string key)
        {
            const string urlFormat = "{0}{1}/{2}";
            //https://myaccount.blob.core.windows.net/mycontainer/myblob

            return string.Format(
                urlFormat, _storageAccount.BlobEndpoint.AbsoluteUri,
                AlgoBlobRepository.BlobContainer,
                key);
        }
    }
}
