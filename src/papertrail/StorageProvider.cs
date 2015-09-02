using System;
using System.IO;
using System.Text;
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

        public async Task<Uri> UploadAsync(string containerName, string blobName, Stream stream)
        {
            if (String.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName));
            }

            if (String.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName));
            }

            var container = BlobClient().GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            var blockBlob = container.GetBlockBlobReference(blobName);
            await blockBlob.UploadFromStreamAsync(stream);

            return blockBlob.Uri;
        }

        public async Task<string> GenerateSasUri(string containerName, string blobName)
        {
            if (String.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName));
            }

            if (String.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(nameof(blobName));
            }

            var container = BlobClient().GetContainerReference(containerName);

            if (!await container.ExistsAsync())
            {
                throw new FileNotFoundException("Container does not exist", containerName);
            }

            var blob = container.GetBlockBlobReference(blobName);
            if (!await blob.ExistsAsync())
            {
                throw new FileNotFoundException("Blob does not exist", blobName);
            }

            var sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessBlobPermissions.Read,
            };

            return blob.Uri + blob.GetSharedAccessSignature(sasConstraints);
        }
    }
}
