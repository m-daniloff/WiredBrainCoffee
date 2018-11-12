using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using WiredBrainCoffee.AdminApp.Service;
using WiredBrainCoffee.Storage;

namespace WiredBrainCoffee.AdminApp.ViewModel
{
  public interface IAddCoffeeVideoDialogViewModel
  {
    bool DialogResultIsOk { get; }
    byte[] BlobByteArray { get; }
    string BlobName { get; }
  }
  public class AddCoffeeVideoDialogViewModel : ViewModelBase, IAddCoffeeVideoDialogViewModel
  {
    private string _blobNameWithoutExtension;
    private ICoffeeVideoStorage _coffeeVideoStorage;
    private IFilePickerDialogService _filePickerDialogService;
    private readonly IMessageDialogService _messageDialogService;

    public AddCoffeeVideoDialogViewModel(ICoffeeVideoStorage coffeeVideoStorage,
      IFilePickerDialogService filePickerDialogService,
      IMessageDialogService messageDialogService)
    {
      _coffeeVideoStorage = coffeeVideoStorage;
      _filePickerDialogService = filePickerDialogService;
      _messageDialogService = messageDialogService;
    }

    public byte[] BlobByteArray { get; private set; }

    public string BlobNameWithoutExtension
    {
      get => _blobNameWithoutExtension;
      set
      {
        _blobNameWithoutExtension = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(IsPrimaryButtonEnabled));
      }
    }

    public bool IsPrimaryButtonEnabled => BlobByteArray != null && !string.IsNullOrWhiteSpace(BlobNameWithoutExtension);

    public string BlobName => BlobNameWithoutExtension + ".mp4";

    public bool DialogResultIsOk { get; set; }

    public async Task SelectVideoAsync()
    {
      var storageFile = await _filePickerDialogService.ShowMp4FileOpenDialogAsync();

      if (storageFile != null)
      {
        BlobNameWithoutExtension = Path.GetFileNameWithoutExtension(storageFile.Name);

        var randomAccessStream = await storageFile.OpenReadAsync();
        BlobByteArray = new byte[randomAccessStream.Size];
        using (var dataReader = new DataReader(randomAccessStream))
        {
          await dataReader.LoadAsync((uint)randomAccessStream.Size);
          dataReader.ReadBytes(BlobByteArray);
        }

        OnPropertyChanged(nameof(IsPrimaryButtonEnabled));
      }
    }

    public async Task PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
      try
      {
        // Cancel always to keep dialog open, as we have async code in this event.
        // args.Cancel doesn't work after an await statement. So here we always cancel
        // and if the dialog should be closed, we hide it manually by calling its Hide method
        args.Cancel = true;

        var blobExists = await _coffeeVideoStorage.CheckIfBlobExistsAsync(BlobName);

        if (blobExists)
        {
          await _messageDialogService.ShowInfoDialogAsync(
            $"A blob with the name \"{BlobName}\" exists already. " +
            $"Please select another name, thanks! :)", "Info");
        }
        else
        {
          sender.Hide();
          DialogResultIsOk = true;
        }
      }
      catch(Exception ex)
      {
        await _messageDialogService.ShowInfoDialogAsync(ex.Message, "Error");
      }
    }
  }
}
