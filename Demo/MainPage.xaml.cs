using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace Demo
{

    public partial class MainPage : ReactiveContentPage<MainViewModel>
    {


        public MainPage()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();


            this.Bind(ViewModel, vm => vm.Query, v => v.SearchHandler.Text);
            this.BindCommand(ViewModel, vm => vm.ButtonClickedCommand, v => v.AddButton);
            this.Bind(ViewModel, vm => vm.NewCompany, v => v.NewCompanyEntry.Text);
            this.OneWayBind(ViewModel, vm => vm.Companies, view => view.CompaniesListView.ItemsSource);


        }
    }
}
