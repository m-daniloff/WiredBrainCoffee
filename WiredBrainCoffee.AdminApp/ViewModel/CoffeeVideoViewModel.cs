using Microsoft.WindowsAzure.Storage.Blob;

namespace WiredBrainCoffee.AdminApp.ViewModel
{
	public class CoffeeVideoViewModel : ViewModelBase
	{
		private CloudBlockBlob _cloudBlockBlob;

		public CoffeeVideoViewModel(CloudBlockBlob cloudBlockBlob)
		{
			_cloudBlockBlob = cloudBlockBlob;
		}

		public string BlobName => _cloudBlockBlob.Name;

		public string BlobUri => _cloudBlockBlob.Uri.ToString();
	}
}
