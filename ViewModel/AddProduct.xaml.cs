using ShoppingListProject.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace ShoppingListProject.Views
{
    public partial class AddProduct : ContentView
    {
        ObservableCollection<Category> Categories { get; set; }

        public AddProduct(ObservableCollection<Category> categories)
        {
            InitializeComponent();
            Categories = categories;

            // ustawienie zrodla danych dla comboboxa kategorii
            productCategory.ItemsSource = Categories.Select(c => c.Name).ToList();
        }

        // metoda obslugujaca klikniecie przycisku "dodaj produkt"
        private async void OnAddProductClicked(object sender, EventArgs e)
        {
            if (IsFormValid())
            {
                AddNewProduct();
                OnGoToMenuClicked(sender, e);
            }
            else
            {
                await DisplayValidationError();
            }
        }

        // metoda sprawdzajaca poprawnosc formularza
        private bool IsFormValid()
        {
            return !string.IsNullOrEmpty(productName.Text) &&
                   float.TryParse(productQuantity.Text, out var quantity) && quantity >= 0 &&
                   productUnit.SelectedItem != null &&
                   productCategory.SelectedItem != null;
        }

        // metoda dodajaca nowy produkt do wybranej kategorii
        private void AddNewProduct()
        {
            string name = productName.Text;
            float quantity = float.Parse(productQuantity.Text);
            string unit = productUnit.SelectedItem as string;
            string category = productCategory.SelectedItem as string;
            bool isOptional = productIsOptional.IsChecked;

            Product newProduct = new Product(name, quantity, unit, isOptional);

            // dodanie nowego produktu do odpowiedniej kategorii
            var categoryWithNewProduct = Categories.FirstOrDefault(c => c.Name == category);
            categoryWithNewProduct?.Products.Add(newProduct);
        }

        // metoda wyswietlajaca komunikat o bledzie, gdy formularz jest niepoprawny
        private async Task DisplayValidationError()
        {
            await Application.Current.MainPage.DisplayAlert("blad", "prosze uzupelnic wszystkie pola poprawnie.", "ok");
        }

        // metoda do przejscia do glownego menu
        private void OnGoToMenuClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new MainPage(Categories));
        }
    }
}
