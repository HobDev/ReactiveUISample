using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Reactive;
using System.Linq;
using System.Windows.Input;
using DynamicData;
using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;
using Realms;
using DynamicData.Binding;
using System.Collections.Generic;

namespace Demo
{

    public class MainViewModel : ReactiveObject
    {

        public IEnumerable<Company> Companies { get; set; }


        [Reactive]
        public string Query { get; set; }

        [Reactive]
        public string NewCompany { get; set; }

        public ReactiveCommand<Unit, Unit> ButtonClickedCommand { get; set; }
        public ReactiveCommand<Unit, Unit> Search { get; set; }

        Realm _realm;

        public MainViewModel()
        {

            _realm = Realm.GetInstance();
            Companies = _realm.All<Company>();

            //    _companies = new SourceList<Company>();
            //    _companies.AddRange(new[]
            //    {
            //    new Company{Name="EPF CORPORATION"},
            //    new Company{Name="ABC COMPANY PVT. LTD."},
            //    new Company{Name="UNIQUE COMPUTER SYSTEMS"},
            //    new Company{Name="MICROSOFT PRIVATE LIMITED"},
            //});

            // Delay to once every 500 milliseconds doing an update.
            // var refreshObs = this.WhenAnyValue(x => x.Query).Throttle(TimeSpan.FromMilliseconds(500));
            this.WhenAnyValue(x => x.Query).InvokeCommand(Search);
            //_companies.Connect()
            // .AutoRefreshOnObservable(_ => refreshObs)
            // .Filter(m => Query == null || m.Name.IndexOf(Query, StringComparison.CurrentCultureIgnoreCase) >= 0)
            //.Sort(SortExpressionComparer<Company>.Ascending(t => t.Name))
            // .ObserveOn(RxApp.MainThreadScheduler)
            //.Bind(out _sortedCompanies)
            //.Subscribe();

            ButtonClickedCommand = ReactiveCommand.CreateFromTask(async () => await AddButtonClicked());
            Search = ReactiveCommand.CreateFromTask(async () => await SortCollection());
        }



        async Task AddButtonClicked()
        {
            if (!string.IsNullOrWhiteSpace(NewCompany))
            {
                _realm.Write(() =>
                {
                    _realm.Add(new Company { Name = NewCompany });
                });
                NewCompany = string.Empty;
            }
        }

        async Task SortCollection()
        {

            Companies = _realm.All<Company>().OrderBy(m => m.Name).Where(company => company.Name.ToLower().Contains(Query.ToLower()));

        }
    }
}
