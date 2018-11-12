using Autofac;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using WiredBrainCoffee.AdminApp.Startup;
using WiredBrainCoffee.AdminApp.ViewModel;

namespace WiredBrainCoffee.AdminApp
{
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();

      ViewModel = App.Current.Container.Resolve<MainViewModel>();

      ApplicationView.PreferredLaunchViewSize = new Size(800, 620);
      ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
    }

    public MainViewModel ViewModel { get; }
  }
}
