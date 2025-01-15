using System.ComponentModel;

namespace ShoppingListProject.Models
{
    public class Product : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        private float quantity;
        private string unit;      
        private bool isOptional;
        private bool isBought;

        public bool IsPropertyChangedRegistered { get; set; } = false;

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public float Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        public string Unit
        {
            get => unit;
            set
            {
                if (unit != value)
                {
                    unit = value;
                    OnPropertyChanged(nameof(Unit));
                }
            }
        }

        

        public bool IsOptional
        {
            get => isOptional;
            set
            {
                if (isOptional != value)
                {
                    isOptional = value;
                    OnPropertyChanged(nameof(IsOptional));
                }
            }
        }

        public bool IsBought
        {
            get => isBought;
            set
            {
                if (isBought != value)
                {
                    isBought = value;
                    OnPropertyChanged(nameof(IsBought));
                }
            }
        }

        public Product(string name, float quantity, string unit, bool isOptional)
        {
            Name = name;
            Quantity = quantity;
            Unit = unit;
            IsOptional = isOptional;
            
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
