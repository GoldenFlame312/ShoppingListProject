using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ShoppingListProject.Models;

namespace ShoppingListProject.Views;

public partial class ProductsList : ContentView
{
    public ObservableCollection<Product> AllProducts { get; private set; }
    private ObservableCollection<Category> Categories { get; set; }
    private ObservableCollection<string> Shops { get; set; }

    public ProductsList(ObservableCollection<Category> categories)
    {
        InitializeComponent();
        Categories = categories;

        AllProducts = new ObservableCollection<Product>();

        // dodanie nasluchiwaczy zmian dla kategorii i produktow
        foreach (var category in Categories)
        {
            category.PropertyChanged += Category_PropertyChanged;
            foreach (var product in category.Products)
            {
                product.PropertyChanged += Product_PropertyChanged;
            }
        }

        // aktualizacja listy produktow
        UpdateProductsList();
        BindingContext = this;
    }

    // obsluguje zmiany w kategoriach
    private void Category_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Category.Products))
        {
            UpdateProductsList();
        }
    }

    // obsluguje zmiany w produktach
    private void Product_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Product.IsBought) || e.PropertyName == nameof(Product.Quantity))
        {
            UpdateProductsList();
        }
    }

    // aktualizuje liste produktow po kazdej zmianie
    private void UpdateProductsList()
    {
        var sortedProducts = Categories
            .SelectMany(category => category.Products)
            .OrderBy(product => product.IsBought)
            .ThenBy(product => product.Name)
            .ToList();

        // wykonanie aktualizacji na glownym watku UI
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AllProducts.Clear();
            foreach (var product in sortedProducts)
            {
                AllProducts.Add(product);
            }
        });
    }

    // obsluguje klikniecie przycisku usuniecia produktu
    private async void OnProductDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Product product)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Potwierdzenie",
                $"Czy na pewno chcesz usunac produkt \"{product.Name}\"?",
                "Tak",
                "Nie");

            if (confirm)
            {
                // usuwanie produktu z odpowiedniej kategorii
                var categoryToRemoveFrom = Categories.FirstOrDefault(category => category.Products.Contains(product));
                categoryToRemoveFrom?.Products.Remove(product);

                // aktualizacja listy produktow
                UpdateProductsList();
            }
        }
    }

    // obsluguje klikniecie przycisku zmniejszenia ilosci produktu
    private void OnDecreaseQuantityClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Product product)
        {
            if (product.Quantity > 1) // zmieniono warunek na >1, aby nie pozwalac na ujemne ilosci
            {
                product.Quantity -= 1;
            }
        }
    }

    // obsluguje klikniecie przycisku zwiekszenia ilosci produktu
    private void OnIncreaseQuantityClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Product product)
        {
            product.Quantity += 1;
        }
    }

    // obsluguje klikniecie przycisku przejscia do menu glownego
    private void OnGoToMenuClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new MainPage(Categories));
    }

    // obsluguje klikniecie przycisku oznaczenia produktu jako kupionego
    private void OnToggleProductBought(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Product product)
        {
            product.IsBought = !product.IsBought; // zmiana statusu zakupu
        }
    }
}
