
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
              Placeholder="Search Names"
            }.Bind(SearchBar.TextProperty, nameof(viewModel.Query)));

            CollectionView collection=new CollectionView();
            collection.ItemTemplate = new DataTemplate(()=>
            {
                SwipeView swipeView= new SwipeView();
                SwipeItem deleteSwipeItem = new SwipeItem()
                {
                    Text = "Delete",
                    BackgroundColor = Colors.Red,
                };
                deleteSwipeItem.Bind(MenuItem.CommandProperty,nameof(viewModel.DeletePersonCommand), source: viewModel);
                deleteSwipeItem.Bind(MenuItem.CommandParameterProperty, ".");
                Grid layout=new Grid
                {
                    Padding=10,
                  BackgroundColor=Colors.AliceBlue,
                    Children=
                    {
                       new Label{FontSize=15, TextColor=Colors.Black}.Bind(Label.TextProperty, nameof(Person.Name)),
                    }
                   
                }.Margins(0,0,0,5);

                swipeView.RightItems.Add(deleteSwipeItem);
                swipeView.Content = layout;
                return swipeView;
            });

            collection.SelectionMode= SelectionMode.None;
            collection.ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem;
            collection.ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                ItemSpacing=5,
            };
            collection.Bind(ItemsView.ItemsSourceProperty,nameof(viewModel.Names));
            collection.EmptyView = new Label { Text = "Empty! Add new Names" };


            Content = new VerticalStackLayout
            {
               
                Children =
                    {
                        collection,
                        new Border
            {
                    StrokeThickness=1,
                    Stroke= new SolidColorBrush(Colors.Black),
                    BackgroundColor=Colors.Transparent,
                    Padding=new Thickness(8,4),
                    HorizontalOptions=LayoutOptions.Fill,
                    StrokeShape=new RoundRectangle { CornerRadius=new CornerRadius(10)},
                    Content= new Entry
                    {
                        Placeholder="New Name",
                    }.Bind(Entry.TextProperty, nameof(viewModel.NewPerson))
            }.Margins(0,20,0,0),
                 new Button{Text="Add", WidthRequest=100, HorizontalOptions=LayoutOptions.Center}.BindCommand(nameof(viewModel.AddPersonCommand))
                    }
            }.Margins(15, 10, 15, 15);

            BindingContext = viewModel;
        }

      
    }
}