

using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using MongoDB.Bson;
using ReactiveUI;
using Realms;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace InplaceSearching
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SourceCache<Person, ObjectId> _Names;
        private  ReadOnlyObservableCollection<Person> _sortedNames;

        public ReadOnlyObservableCollection<Person> Names => _sortedNames;


        readonly Realm realm;

        [ObservableProperty]
        string query;


        [ObservableProperty]
        Person selectedPerson;

        [ObservableProperty]
        string newPerson;
        public MainViewModel()
        {
            realm = Realm.GetInstance();
            IQueryable<Person> allNames = realm.All<Person>();
            allNames.SubscribeForNotifications((sender, changes, error) =>
            {
                _Names.AddOrUpdate(allNames);
            });
            _Names = new SourceCache<Person, ObjectId>(company => company.Id);
            _Names.AddOrUpdate(allNames);

            IObservable<PropertyValue<MainViewModel, string>>? refreshObs = this.WhenPropertyChanged(t => t.Query).Throttle(TimeSpan.FromMicroseconds(500));

            IObservable<IChangeSet<Person, ObjectId>>? dataConnection = _Names.Connect();

          
                dataConnection
               .AutoRefreshOnObservable(_ => refreshObs)
               .ObserveOn(RxApp.MainThreadScheduler)
               .Filter(m => Query == null || m.Name.IndexOf(Query, StringComparison.CurrentCultureIgnoreCase) >= 0)
               .Bind(out _sortedNames)
               .Subscribe();
           
           
        }

        [RelayCommand]
        async Task AddPerson()
        {
            if(!string.IsNullOrWhiteSpace(NewPerson))
            {
                realm.Write(()=>realm.Add(new Person { Name= NewPerson }));
                NewPerson = "";
            }
        }


        [RelayCommand]  
        async Task DeletePerson(Person person)
        {
            // remove the item from the collection because the update loop only adds/updates items
            _Names.Remove(person);
            realm.Write(()=>realm.Remove(person));
        }

    
    }
}
