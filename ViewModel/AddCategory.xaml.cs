using ShoppingListProject.Models;
using System.Collections.ObjectModel;

namespace ShoppingListProject.Views;
public partial class AddCategory : ContentView
{
    ObservableCollection<Category> Categories { get; set; }

    public AddCategory(ObservableCollection<Category> categories)
    {
        InitializeComponent();
        Categories = categories;
    }

    // metoda obslugujaca klikniecie przycisku "dodaj kategorie"
    private async void OnAddCategoryClicked(object sender, EventArgs e)
    {
        // sprawdzenie, czy nazwa kategorii jest pusta
        if (CategoryName.Text == null)
        {
            await Application.Current.MainPage.DisplayAlert("blad", "nie mozna wykonaæ tej akcji.", "ok");
        }
        else
        {
            // dodanie nowej kategorii do listy
            Categories.Add(new Category(CategoryName.Text));
            // powrot do glownego menu
            OnGoToMenuClicked(sender, e);
        }
    }

    // metoda do przejscia do glownego menu
    private void OnGoToMenuClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new MainPage(Categories));
    }
}
