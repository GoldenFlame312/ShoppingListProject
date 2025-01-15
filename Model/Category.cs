using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ShoppingListProject.Models;

public class Category : INotifyPropertyChanged
{
    private ObservableCollection<Product> products;
    private bool isExpanded;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Name { get; set; }

    public ObservableCollection<Product> Products
    {
        get => products;
        set
        {
            if (products != value)
            {
                products = value;
                OnPropertyChanged(nameof(Products));
            }
        }
    }
    public bool IsExpanded
    {
        get => isExpanded;
        set
        {
            if (isExpanded != value)
            {
                isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }
    }
    public Category() { }
    public Category(string name)
    {
        Name = name;
        Products = new ObservableCollection<Product>();
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
