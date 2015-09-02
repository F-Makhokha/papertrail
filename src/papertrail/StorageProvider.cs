using System;
using System.IO;
using Microsoft.Azure;
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

        public void Upload(string blobName, Stream stream)
        {
            var container = BlobClient().GetContainerReference("test");
            container.CreateIfNotExists();

            var blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.UploadFromStream(stream);
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(@"path\myfile"))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }
    }
}
