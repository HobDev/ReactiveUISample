using ReactiveUI.XamForms;

namespace Demo
{
  public partial class MainPage : ReactiveContentPage<MainViewModel>
  {
    public MainPage()
    {
      InitializeComponent();

      ViewModel = new MainViewModel();
    }
  }
}
