﻿using System;
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
        IEnumerable<Company> companies;
        public IEnumerable<Company> Companies
        {
            get => companies;
            set => this.RaiseAndSetIfChanged(ref this.companies, value);
        }


        string query;
        public string Query
        {
            get => query;
            set => this.RaiseAndSetIfChanged(ref query, value);
        }

        [Reactive]
        public string NewCompany { get; set; }

        public ReactiveCommand<Unit, Unit> AddCompanyCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SearchCommand { get; set; }

        Realm _realm;

        public MainViewModel()
        {

            _realm = Realm.GetInstance();
            Companies = _realm.All<Company>();

            AddCompanyCommand = ReactiveCommand.CreateFromTask(async () => await AddButtonClicked());
            SearchCommand = ReactiveCommand.CreateFromTask(async () => await SortCollection());



            this.WhenAnyValue(x => x.Query).Where(query => !String.IsNullOrWhiteSpace(query)).Throttle(TimeSpan.FromSeconds(1)).Select(_ => Unit.Default).InvokeCommand(this, x => x.SearchCommand);
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
