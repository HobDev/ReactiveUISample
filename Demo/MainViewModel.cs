using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Realms;

namespace Demo
{
  public class MainViewModel : ReactiveObject
  {
    private readonly SourceCache<Company, string> _Companies;
    private readonly ReadOnlyObservableCollection<Company> _sortedCompanies;

    private readonly Realm _realm;

    public MainViewModel()
    {
      _realm = Realm.GetInstance();
      var allCompanies = _realm.All<Company>();

      allCompanies.SubscribeForNotifications((sender, changes, error) =>
      {
        _Companies.AddOrUpdate(allCompanies);
      });

      _Companies = new SourceCache<Company, string>(company => company.Id);
      _Companies.AddOrUpdate(allCompanies);

      var refreshObs = this.WhenAnyValue(x => x.Query).Throttle(TimeSpan.FromMilliseconds(500));
      var dataConnection = _Companies.Connect();

      dataConnection
       .AutoRefreshOnObservable(_ => refreshObs)
       .ObserveOn(RxApp.MainThreadScheduler)
       .Filter(m => Query == null || m.Name.IndexOf(Query, StringComparison.CurrentCultureIgnoreCase) >= 0)
       .Bind(out _sortedCompanies)
       .Subscribe();

      AddCompanyCommand = ReactiveCommand.Create(AddCompany);
      DeleteCompanyCommand = ReactiveCommand.Create<Company>(DeleteCompany);
    }

    public ReadOnlyObservableCollection<Company> Companies => _sortedCompanies;

    [Reactive]
    public string Query { get; set; }

    [Reactive]
    public string NewCompany { get; set; }

    public ReactiveCommand<Unit, Unit> AddCompanyCommand { get; set; }

    public ReactiveCommand<Company, Unit> DeleteCompanyCommand { get; set; }

    private void AddCompany()
    {
      if (!string.IsNullOrWhiteSpace(NewCompany))
      {
        _realm.Write(() => { _realm.Add(new Company { Name = NewCompany }); });
        NewCompany = string.Empty;
      }
    }

    private void DeleteCompany(Company company)
    {
      // remove the item from the collection because the update loop only adds/updates items
      _Companies.Remove(company);
      _realm.Write(() => { _realm.Remove(company); });
    }
  }
}
