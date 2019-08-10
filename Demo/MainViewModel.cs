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
using DynamicData.Aggregation;
using System.Collections.Generic;

namespace Demo
{

    public class MainViewModel : ReactiveObject
    {
        //readonly ObservableAsPropertyHelper<IEnumerable<Company>> companies;
        //public IEnumerable<Company> BindList => this.companies.Value;
        public IEnumerable<Company> Companies { get; set; }

        [Reactive]
        public string Query { get; set; }

        [Reactive]
        public string NewCompany { get; set; }

        public ReactiveCommand<Unit, IEnumerable<Company>> AddCompanyCommand { get; set; }
        public ReactiveCommand<Unit, IEnumerable<Company>> SearchCommand { get; set; }

        Realm _realm;

        public MainViewModel()
        {

            _realm = Realm.GetInstance();

            Companies = _realm.All<Company>();


            AddCompanyCommand = ReactiveCommand.Create<Unit, IEnumerable<Company>>(_ => AddButtonClicked());
            SearchCommand = ReactiveCommand.Create<Unit, IEnumerable<Company>>(_ => SortCollection());


            SearchCommand.ToProperty(this, x => x.Companies);
            AddCompanyCommand.ToProperty(this, x => x.Companies);

            this.WhenAnyValue(x => x.Query).Throttle(TimeSpan.FromSeconds(1)).Select(_ => Unit.Default).InvokeCommand(this, x => x.SearchCommand);


        }

        IEnumerable<Company> AddButtonClicked()
        {
            if (!string.IsNullOrWhiteSpace(NewCompany))
            {
                _realm.Write(() =>
                {
                    _realm.Add(new Company { Name = NewCompany });
                });
                NewCompany = string.Empty;

            }
            Companies = Companies.Where(x => x.Name != string.Empty);
            return Companies;
        }


        IEnumerable<Company> SortCollection()
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                Companies = Companies.Where(x => x.Name != string.Empty);
                return Companies;

            }

            Companies = Companies.Where(x => x.Name.IndexOf(Query, StringComparison.InvariantCultureIgnoreCase) >= 0);

            return Companies;

        }



    }
}

