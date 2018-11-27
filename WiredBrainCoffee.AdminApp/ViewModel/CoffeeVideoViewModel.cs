using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using WiredBrainCoffee.AdminApp.Service;
using WiredBrainCoffee.Storage;

namespace WiredBrainCoffee.AdminApp.ViewModel
{
	public class CoffeeVideoViewModel : ViewModelBase
	{
		private CloudBlockBlob _cloudBlockBlob;

		private readonly IFilePickerDialogService _filePickerDialogService;
		private readonly IMessageDialogService _messageDialogService;
		private IMainViewModel _mainViewModel;
		private readonly ICoffeeVideoStorage _coffeeVideoStorage;
	    private string _title;
	    private string _description;

        public CoffeeVideoViewModel(CloudBlockBlob cloudBlockBlob,
		 ICoffeeVideoStorage coffeeVideoStorage,
		 IFilePickerDialogService filePickerDialogService,
		 IMessageDialogService messageDialogService,
		 IMainViewModel mainViewModel)
		{
			_cloudBlockBlob = cloudBlockBlob
			  ?? throw new ArgumentNullException(nameof(cloudBlockBlob));

			_filePickerDialogService = filePickerDialogService;
			_messageDialogService = messageDialogService;
			_mainViewModel = mainViewModel;
			_coffeeVideoStorage = coffeeVideoStorage;
		}

		public string BlobName => _cloudBlockBlob.Name;

		public string BlobUri => _cloudBlockBlob.Uri.ToString();

	    public string Title
	    {
	        get { return _title; }
	        set
	        {
	            if (_title != value)
	            {
	                _title = value;
	                OnPropertyChanged();
	                OnPropertyChanged(nameof(IsMetadataChanged));
	            }
	        }
	    }

	    public string Description
	    {
	        get { return _description; }
	        set
	        {
	            if (_description != value)
	            {
	                _description = value;
	                OnPropertyChanged();
	                OnPropertyChanged(nameof(IsMetadataChanged));
	            }
	        }
	    }

	    public bool IsMetadataChanged
	    {
	        get
	        {
	            var (title, description) = _coffeeVideoStorage.GetBlobMetadata(_cloudBlockBlob);
	            return !string.Equals(title, Title) || !string.Equals(description, Description);
	        }
	    }

        public async Task DownloadVideoToFileAsync()
		{
			try
			{
				var storageFile = await _filePickerDialogService.ShowMp4FileSaveDialogAsync(BlobName);
				if (storageFile != null)
				{
					_mainViewModel.StartLoading($"Downloading your video {BlobName}");
					using (var streamToWrite = await storageFile.OpenStreamForWriteAsync())
					{
						await _coffeeVideoStorage.DownloadVideoAsync(_cloudBlockBlob, streamToWrite);
					}
				}
			}
			catch (Exception ex)
			{
				await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
			}
			finally
			{
				_mainViewModel.StopLoading();
			}
		}

		public async Task DeleteVideoAsync()
		{
			try
			{
				var isOk = await _messageDialogService.ShowOkCancelDialogAsync($"Delete the video {_cloudBlockBlob.Name}?", "Question");
				if (isOk)
				{
					_mainViewModel.StartLoading($"Deleting your video {BlobName}");
					await _coffeeVideoStorage.DeleteVideoAsync(_cloudBlockBlob);
					_mainViewModel.RemoveCoffeeVideoViewModel(this);
					_mainViewModel.StopLoading();
					_mainViewModel = null;
				}
			}
			catch (Exception ex)
			{
				await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
			}
			finally
			{
				if (_mainViewModel != null)
				{
					_mainViewModel.StopLoading();
				}
			}
		}

	    public async Task UpdateMetadataAsync()
	    {
	        try
	        {
	            _mainViewModel.StartLoading($"Updating metadata");
	            await _coffeeVideoStorage.UpdateMetadataAsync(_cloudBlockBlob, Title, Description);
	            OnPropertyChanged(nameof(IsMetadataChanged));
	        }
	        catch (Exception ex)
	        {
	            await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
	        }
	        finally
	        {
	            _mainViewModel.StopLoading();
	        }
	    }

	    public async Task ReloadMetadataAsync()
	    {
	        try
	        {
	            _mainViewModel.StartLoading($"Reloading metadata");
	            await _coffeeVideoStorage.ReloadMetadataAsync(_cloudBlockBlob);
	            UpdateViewModelPropertiesFromMetadata();
	        }
	        catch (Exception ex)
	        {
	            await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
	        }
	        finally
	        {
	            _mainViewModel.StopLoading();
	        }
	    }

	    private void UpdateViewModelPropertiesFromMetadata()
	    {
	        var (title, description) = _coffeeVideoStorage.GetBlobMetadata(_cloudBlockBlob);
	        Title = title;
	        Description = description;
	    }
    }
}
