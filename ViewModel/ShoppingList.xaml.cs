using ShoppingListProject.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace ShoppingListProject.Views
{
    public partial class ShoppingList : ContentView
    {
        ObservableCollection<Category> Categories { get; set; }
        ObservableCollection<Product> SortedProducts = new ObservableCollection<Product>();

        public ShoppingList(ObservableCollection<Category> categories)
        {
            InitializeComponent();
            Categories = categories;
            BindingContext = this;

            // Subskrybowanie zdarzen dla kategorii i produktow
            SubscribeToCategoryChanges();

            // Sortowanie produktow na starcie
            SortProducts();

            // Ustawienie listy produktow
            productsList.ItemsSource = SortedProducts;
        }

        // Metoda subskrybujaca zmiany w kategoriach i produktach
        private void SubscribeToCategoryChanges()
        {
            foreach (var category in Categories)
            {
                category.Products.CollectionChanged += OnProductsChanged;
                foreach (var product in category.Products)
                {
                    product.PropertyChanged += OnProductPropertyChanged;
                }
            }
        }

        // Metoda do sortowania produktow
        private void SortProducts()
        {
            SortedProducts.Clear();
            foreach (var category in Categories)
            {
                foreach (var product in category.Products)
                {
                    if (!product.IsBought)
                    {
                        SortedProducts.Add(product);
                    }
                }
            }
        }

        // Obsluga zmian w kolekcji produktow
        private void OnProductsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UnsubscribeFromOldProducts(e);
            SubscribeToNewProducts(e);

            // Sortowanie po kazdej zmianie
            SortProducts();
        }

        // Metoda do odsubskrybowania zmian w starych produktach
        private void UnsubscribeFromOldProducts(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Product product in e.OldItems)
                {
                    product.PropertyChanged -= OnProductPropertyChanged;
                }
            }
        }

        // Metoda do subskrybowania zmian w nowych produktach
        private void SubscribeToNewProducts(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Product product in e.NewItems)
                {
                    product.PropertyChanged += OnProductPropertyChanged;
                }
            }
        }

        // Obsluga zmian w wlasciwosciach produktow
        private void OnProductPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Product.IsBought) || e.PropertyName == nameof(Product.IsOptional))
            {
                SortProducts();
            }
        }

        // Przejscie do glownego menu
        private void OnGoToMenuClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new MainPage(Categories));
        }
    }
}
