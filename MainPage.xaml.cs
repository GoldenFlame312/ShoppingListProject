using ShoppingListProject.Models;
using ShoppingListProject.Views;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ShoppingListProject
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Category> Categories { get; set; }
        private const string ShoppingListFileName = "shoppingList.json";

        public MainPage()
        {
            InitializeComponent();
            LoadDataAsync();
            InitializeCategories();
        }

        public MainPage(ObservableCollection<Category> categories)
        {
            InitializeComponent();
            Categories = categories;
        }

        // inicjalizuje kategorie, jesli nie zostaly zaladowane z pliku
        private void InitializeCategories()
        {
            if (Categories == null || Categories.Count == 0)
            {
                Categories = new ObservableCollection<Category>
                {
                    new Category("Nabial"),
                    new Category("Owoce"),
                    new Category("Warzywa"),
                    new Category("AGD"),
                    new Category("RTV"),
                };
            }
        }

        // obsluguje klikniecie przycisku "lista produktow"
        private void onProductsListClicked(object sender, EventArgs e)
        {
            NavigateTo(new ProductsList(Categories));
        }

        // obsluguje klikniecie przycisku "lista kategorii"
        private void OnCategoryListClicked(object sender, EventArgs e)
        {
            NavigateTo(new CategoryList(Categories));
        }

        // obsluguje klikniecie przycisku "lista zakupow"
        private void OnShoppingListClicked(object sender, EventArgs e)
        {
            NavigateTo(new ShoppingList(Categories));
        }

        // obsluguje klikniecie przycisku "dodaj produkt"
        private void OnAddProductClicked(object sender, EventArgs e)
        {
            NavigateTo(new AddProduct(Categories));
        }

        // obsluguje klikniecie przycisku "dodaj kategorie"
        private void OnAddCategoryClicked(object sender, EventArgs e)
        {
            NavigateTo(new AddCategory(Categories));
        }

        // pomocnicza metoda do nawigacji
        private void NavigateTo(ContentView contentView)
        {
            Content = contentView;
        }

        // obsluguje klikniecie przycisku "zapisz liste"
        private async void OnSaveListClicked(object sender, EventArgs e)
        {
            await SaveDataAsync();
        }

        // zapisuje dane do pliku
        private async Task SaveDataAsync()
        {
            try
            {
                var shoppingListPath = Path.Combine(FileSystem.AppDataDirectory, ShoppingListFileName);
                var shoppingListData = new ShoppingListData { Categories = Categories };
                await SaveToJson(shoppingListPath, shoppingListData);
            }
            catch (Exception ex)
            {
                await ShowErrorAlert("Blad", $"Wystapil problem podczas zapisywania danych: {ex.Message}");
            }
        }

        // pomocnicza metoda do zapisywania danych do pliku JSON
        private async Task SaveToJson<T>(string filePath, T data)
        {
            var json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(filePath, json);
            await DisplayAlert("Sukces", $"Dane zostaly zapisane w pliku:\n {filePath}", "OK");
        }

        // klasa do przechowywania danych listy zakupow
        public class ShoppingListData
        {
            public ObservableCollection<Category> Categories { get; set; }
        }

        // laduje dane z pliku
        private async void LoadDataAsync()
        {
            try
            {
                var shoppingListPath = Path.Combine(FileSystem.AppDataDirectory, ShoppingListFileName);

                if (File.Exists(shoppingListPath))
                {
                    var shoppingListJson = await File.ReadAllTextAsync(shoppingListPath);
                    var shoppingListData = JsonSerializer.Deserialize<ShoppingListData>(shoppingListJson);

                    Categories = shoppingListData?.Categories ?? new ObservableCollection<Category>();
                }
                else
                {
                    Categories = new ObservableCollection<Category>();
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAlert("Blad", $"Wystapil problem podczas wczytywania danych: {ex.Message}");
                Categories = new ObservableCollection<Category>();
            }
        }

        // pomocnicza metoda do wyswietlania komunikatu o bledzie
        private async Task ShowErrorAlert(string title, string message)
        {
            await DisplayAlert(title, message, "OK");
        }

        // obsluguje klikniecie przycisku "eksportuj liste"
        private async void OnExportListClicked(object sender, EventArgs e)
        {
            try
            {
                var exportPath = GetExportPath();
                var shoppingListData = new ShoppingListData { Categories = Categories };
                var shoppingListJson = JsonSerializer.Serialize(shoppingListData);
                await File.WriteAllTextAsync(exportPath, shoppingListJson);

                await DisplayAlert("Sukces", $"Plik zostal zapisany w folderze: {exportPath}", "OK");
            }
            catch (Exception ex)
            {
                await ShowErrorAlert("Blad", $"Wystapil problem podczas eksportowania danych: {ex.Message}");
            }
        }

        // pomocnicza metoda do okreslenia sciezki eksportu
        private string GetExportPath()
        {
            string exportPath = string.Empty;

            if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                exportPath = Path.Combine(FileSystem.AppDataDirectory, ShoppingListFileName);
            }
            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                var downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                exportPath = Path.Combine(downloadsFolder, "Downloads", ShoppingListFileName);
            }
            else
            {
                exportPath = Path.Combine(FileSystem.AppDataDirectory, ShoppingListFileName);
            }

            return exportPath;
        }

        // obsluguje klikniecie przycisku "importuj liste"
        private async void OnImportListClicked(object sender, EventArgs e)
        {
            try
            {
                var fileResult = await FilePicker.PickAsync(new PickOptions { PickerTitle = "Wybierz plik do zaimportowania" });

                if (fileResult != null && Path.GetExtension(fileResult.FileName).ToLower() == ".json")
                {
                    var json = await File.ReadAllTextAsync(fileResult.FullPath);
                    var shoppingListData = JsonSerializer.Deserialize<ShoppingListData>(json);

                    if (shoppingListData != null)
                    {
                        Categories = shoppingListData.Categories ?? new ObservableCollection<Category>();
                        await DisplayAlert("Sukces", $"Plik {fileResult.FileName} zostal zaimportowany.", "OK");
                    }
                    else
                    {
                        await ShowErrorAlert("Blad", "Blad deserializacji pliku.");
                    }
                }
                else
                {
                    await ShowErrorAlert("Blad", "Prosze wybrac plik JSON.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAlert("Blad", $"Wystapil problem podczas importowania danych: {ex.Message}");
            }
        }
    }
}
