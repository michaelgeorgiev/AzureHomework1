using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;

namespace BlobStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("text-files");

            container.CreateIfNotExists();
            // Set permissions
            BlobContainerPermissions permissions = new BlobContainerPermissions()
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };

            container.SetPermissions(permissions);
            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("UploadedSampleText.txt");
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(@"..\..\SampleText.txt"))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            Console.WriteLine(text);

            blockBlob.Delete();
            Thread.Sleep(5000);
            // 01 - Connect to your azure storage account

            // 02 - Create a container called "text-files"

            // 03 - Set the container permissions to BlobContainerPublicAccessType.Blob

            // 04 - Upload SampleText.txt to a block block called "UploadedSampleText.txt"

            // 05 - Download "UploadedSampleText.txt" from the storage account and prints its contents using Console.WriteLine()

            // 05 - Delete UploadedSampleText.txt from the storage
        }
    }
}