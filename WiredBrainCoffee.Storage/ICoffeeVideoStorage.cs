using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using System.IO;

namespace WiredBrainCoffee.Storage
{
    public interface ICoffeeVideoStorage
    {
		Task<CloudBlockBlob> UploadVideoAsync(byte[] videoByteArray, string blobname, string title, string description);
		Task<bool> CheckIfBlobExistsAsync(string blobName);

		Task<IEnumerable<CloudBlockBlob>> ListVideoBlobsAsync(string prefix = null);

		Task DownloadVideoAsync(CloudBlockBlob cloudBlockBlob, Stream targetStream);
		Task DeleteVideoAsync(CloudBlockBlob cloudBlockBlob);

        Task UpdateMetadataAsync(CloudBlockBlob cloudBlockBlob, string title, string description);
        Task ReloadMetadataAsync(CloudBlockBlob cloudBlockBlob);
        (string title, string description) GetBlobMetadata(CloudBlockBlob cloudBlockBlob);
    }
}