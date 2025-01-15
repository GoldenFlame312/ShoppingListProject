using ShoppingListProject.Models;
using System.Collections.ObjectModel;

namespace ShoppingListProject.Views;

public partial class CategoryList : ContentView
{
    // observablecollection dla kategorii
    ObservableCollection<Category> Categories { get; set; }

    public CategoryList(ObservableCollection<Category> categories)
    {
        InitializeComponent();
        Categories = categories;

        // resetowanie rozwiniêcia kategorii
        ResetListExpanding();

        BindingContext = this;
        categoriesList.ItemsSource = Categories;
    }

    // obsluguje klikniecie przycisku "usun kategorie"
    private async void OnCategoryDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Category category)
        {
            bool confirm = await ShowDeleteConfirmationDialog(category);

            if (confirm)
            {
                RemoveCategory(category);
            }
        }
    }

    // metoda pomocnicza do pokazania potwierdzenia przed usunieciem kategorii
    private async Task<bool> ShowDeleteConfirmationDialog(Category category)
    {
        return await Application.Current.MainPage.DisplayAlert(
            "potwierdzenie",
            $"Czy na pewno chcesz usunac kategorie \"{category.Name}\" i wszystkie jej produkty?",
            "tak",
            "nie");
    }

    // metoda pomocnicza do usuwania kategorii
    private void RemoveCategory(Category category)
    {
        Categories.Remove(category);
    }

    // resetowanie rozwiniêcia kategorii
    private void ResetListExpanding()
    {
        foreach (var category in Categories)
        {
            category.IsExpanded = false;
        }
    }

    // obsluguje klikniecie przycisku "rozwin/zwin kategorie"
    private void OnCategoryExpandClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Category category)
        {
            ToggleCategoryExpansion(category);
        }
    }

    // metoda pomocnicza do prze³aczania stanu rozwiniêcia kategorii
    private void ToggleCategoryExpansion(Category category)
    {
        category.IsExpanded = !category.IsExpanded;
    }

    // obsluguje klikniecie przycisku "powrot"
    private void OnGoToMenuClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new MainPage(Categories));
    }
}
