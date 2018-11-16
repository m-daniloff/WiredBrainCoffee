using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace WiredBrainCoffee.Storage
{
	public class CoffeeVideoStorage : ICoffeeVideoStorage
	{
		private readonly string _containerNameVideos = "videos";
		private readonly string _connectionString;

		public CoffeeVideoStorage(string connectionString)
		{
			this._connectionString = connectionString;
		}

		public async Task<CloudBlockBlob> UploadVideoAsync(byte[] videoByteArray, string blobName)
		{
			var cloudBlobContainer = await GetCoffeeVideosContainerAsync();

			var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
			cloudBlockBlob.Properties.ContentType = "video/mp4";
			await cloudBlockBlob.UploadFromByteArrayAsync(videoByteArray, 0, videoByteArray.Length);

			return cloudBlockBlob;
		}

		
		public async Task<bool> CheckIfBlobExistsAsync(string blobName)
		{
			var cloudBlobContainer = await GetCoffeeVideosContainerAsync();

			var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
			return await cloudBlockBlob.ExistsAsync();
		}

	    public async Task<IEnumerable<CloudBlockBlob>> ListVideoBlobsAsync(string prefix = null)
	    {
            var cloudBlockBlobs = new List<CloudBlockBlob>();
	        var cloudBlobContainer = await GetCoffeeVideosContainerAsync();

	        var blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(null);
            cloudBlockBlobs.AddRange(blobResultSegment.Results.OfType<CloudBlockBlob>());

	        return cloudBlockBlobs;
	    }
		private async Task<CloudBlobContainer> GetCoffeeVideosContainerAsync()
		{
			var cloudStorageAccount = CloudStorageAccount.Parse(_connectionString);
			var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
			var cloudBlobContainer = cloudBlobClient.GetContainerReference(_containerNameVideos);
			await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);
			return cloudBlobContainer;
		}

	}
}
