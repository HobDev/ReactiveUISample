
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;

namespace InplaceSearching
{
    public partial class MainPage : ContentPage
    {
       

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            Shell.SetTitleView(this, new SearchBar
            {
                Placeholder="Select Company"
            }.Bind(SearchBar.TextProperty, nameof(viewModel.Query)));

            CollectionView collection=new CollectionView();
            DataTemplate dataTemplate = new DataTemplate(()=>
            {
                VerticalStackLayout layout=new VerticalStackLayout
                {
                    new Label{}.Bind(Label.TextProperty, nameof(Company.Name)),
                    new BoxView{HorizontalOptions=LayoutOptions.Fill, HeightRequest=1, Color=Colors.BlueViolet}
                };

                return layout;
            });

            collection.ItemTemplate= dataTemplate;
            collection.ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem;
            collection.SelectionMode= SelectionMode.None;
            collection.ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                ItemSpacing=5,
            };
            collection.Bind(ItemsView.ItemsSourceProperty,nameof(viewModel.Companies));
            collection.Bind(SelectableItemsView.SelectedItemProperty,nameof(viewModel.SelectedCompany));

            collection.Footer = new VerticalStackLayout
            {
                 new Border
            {
                    StrokeThickness=0,
                    Padding=new Thickness(8,4),
                    HorizontalOptions=LayoutOptions.Fill,
                    StrokeShape=new RoundRectangle { CornerRadius=new CornerRadius(10)},
                    Content= new Entry
                    {
                        Placeholder="Company Name",
                    }.Bind(Entry.TextProperty, nameof(viewModel.NewCompany))
            },
                 new Button{Text="Add", HorizontalOptions=LayoutOptions.Center}.BindCommand(nameof(viewModel.AddCompanyCommand))
            };
                
           

            BindingContext = viewModel;
        }

      
    }
}