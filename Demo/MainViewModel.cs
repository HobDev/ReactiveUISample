using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Reactive;
using DynamicData;
using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;
using Realms;
using System.Linq;

namespace Demo
{

    public class MainViewModel : ReactiveObject
    {
        private readonly SourceCache<Company, string> _Companies;
        private readonly ReadOnlyObservableCollection<Company> _sortedCompanies;
        public ReadOnlyObservableCollection<Company> Companies => _sortedCompanies;



        [Reactive]
        public string Query { get; set; }

        [Reactive]
        public string NewCompany { get; set; }

        public ReactiveCommand<Unit, Unit> AddCompanyCommand { get; set; }

        Realm _realm;

        public MainViewModel()
        {

            _realm = Realm.GetInstance();
            _realm.RealmChanged += (s, e) => { _Companies.AddOrUpdate(_realm.All<Company>()); };
            _Companies = new SourceCache<Company, string>(company => company.Id);
            _Companies.AddOrUpdate(_realm.All<Company>());

            var refreshObs = this.WhenAnyValue(x => x.Query).Throttle(TimeSpan.FromMilliseconds(500));
            var dataConnection = _Companies.Connect();

            dataConnection
                .AutoRefreshOnObservable(_ => refreshObs)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Filter(m => Query == null || m.Name.IndexOf(Query, StringComparison.CurrentCultureIgnoreCase) >= 0)
                .Bind(out _sortedCompanies)
                .Subscribe();

            AddCompanyCommand = ReactiveCommand.CreateFromTask(async () => await AddCompany());

        }

        async Task AddCompany()
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
    }
}

