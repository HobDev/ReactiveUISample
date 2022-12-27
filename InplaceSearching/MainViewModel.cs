

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;

namespace InplaceSearching
{
   public partial class MainViewModel:ObservableObject
    {
        readonly Realm realm;

        [ObservableProperty]
        string query;

      public  IEnumerable<Company> Companies { get; }

        [ObservableProperty]
        Company selectedCompany;

        [ObservableProperty]
        string newCompany;
        public MainViewModel()
        {
            realm = Realm.GetInstance();
        }

        [RelayCommand]
        async Task AddCompany()
        {
            if(!string.IsNullOrWhiteSpace(NewCompany))
            {
                realm.Write(()=>realm.Add(new Company { Name= NewCompany}));
            }
        }

        [RelayCommand]  
        async Task DeleteCompany()
        {
            //realm.Write(()=>realm.Remove());
        }

    
    }
}
