using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PaperTrail
{
    public class StorageProvider
    {
        private readonly string _connectionStr;
        private readonly CloudStorageAccount _storageAccount;

        private Func<CloudBlobClient> BlobClient => () => _storageAccount.CreateCloudBlobClient();

        public StorageProvider(string connectionStr)
        {
            _connectionStr = connectionStr;
            _storageAccount = CloudStorageAccount.Parse(_connectionStr);
        }

        public async Task<Uri> UploadAsync(string blobName, Stream stream)
        {
            if (String.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName));
            }

            var container = BlobClient().GetContainerReference("test");
            await container.CreateIfNotExistsAsync();

            var blockBlob = container.GetBlockBlobReference(blobName);
            await blockBlob.UploadFromStreamAsync(stream);

            return blockBlob.Uri;
        }
    }
}
