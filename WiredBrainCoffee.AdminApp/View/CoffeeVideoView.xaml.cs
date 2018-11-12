using Windows.UI.Xaml.Controls;
using WiredBrainCoffee.AdminApp.ViewModel;

namespace WiredBrainCoffee.AdminApp.View
{
  public sealed partial class CoffeeVideoView : UserControl
  {
    public CoffeeVideoView()
    {
      this.InitializeComponent();
    }

    private CoffeeVideoViewModel _viewModel;

    public CoffeeVideoViewModel ViewModel
    {
      get { return _viewModel; }
      set
      {
        _viewModel = value;
        this.Bindings.Update();
      }
    }
  }
}
