using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Order_Pizza_Management.Models;

namespace Order_Pizza_Management.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public int selectedCount { get; set; }

        public ObservableCollection<Pizza> allPizza { get; set; }
        public List<Pizza> shownPizza { get; set; }
        private ObservableCollection<Ingredient> allIngredients;
        public List<Ingredient> shownIngredients { get; set; }
        public ObservableCollection<OrderString> orderStrings { get; set; }
        public ObservableCollection<PizzaCompositionString> composition { get; set; }
        public ObservableCollection<IngredientType> Types { get; set; }

        private Pizza selectedPizza;
        private Ingredient selectedIngridient;
        private IngredientType selectedType;

        public Pizza SelectedPizza
        {
            get { return selectedPizza; }
            set
            {
                selectedPizza = value;
                OnPropertyChanged("SelectedPizza");
            }
        }
        public Ingredient SelectedIngridient
        {
            get { return selectedIngridient; }
            set
            {
                selectedIngridient = value;
                OnPropertyChanged("SelectedIngridient");
            }
        }
        public IngredientType SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                OnPropertyChanged("SelectedType");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private DbOperations dbo;
        public MainWindowViewModel()
        {
            dbo = new DbOperations();
            selectedCount = 1;

            allIngredients = dbo.GetAvailableIngredients();
            shownIngredients = allIngredients.ToList();
            allPizza = dbo.GetAvailablePizza();
            shownPizza = allPizza.ToList();
            Types = dbo.GetIngredientTypes();
            selectedType = Types.Last();

            composition = new ObservableCollection<PizzaCompositionString>();
            orderStrings = new ObservableCollection<OrderString>();
        }

        private RelayCommand addPizzaInOrder;
        public RelayCommand AddPizzaInOrder
        {
            get
            {
                return addPizzaInOrder ??
                    (addPizzaInOrder = new RelayCommand(obj =>
                    {
                        AddInOrder(selectedPizza, selectedCount);
                    }));
            }
        }

        private RelayCommand addIngredientInPizza;
        public RelayCommand AddIngredientInPizza
        {
            get
            {
                return addIngredientInPizza ??
                    (addIngredientInPizza = new RelayCommand(obj =>
                    {
                        if (selectedIngridient.CountStock >= selectedCount)
                        {
                            composition.Add(new PizzaCompositionString()
                            {
                                Ingredient = selectedIngridient,
                                Ingredient_FK = selectedIngridient.Id,
                                Count = selectedCount
                            });
                            selectedCount = 1;
                        }
                        // else показать сообщение
                    }));
            }

        }

        private RelayCommand addCustomPizza;
        public RelayCommand AddCustomPizza
        {
            get
            {
                return addCustomPizza ??
                    (addCustomPizza = new RelayCommand(obj =>
                    {
                        if (composition.Count != 0)
                        {
                            double price = 0;
                            foreach (PizzaCompositionString cs in composition)
                                price += cs.Ingredient.Price * cs.Count;

                            Pizza customPizza = new Pizza()
                            {
                                InStock = true,
                                IsCustom = true,
                                Price = price,
                            };

                            int id = dbo.AddPizza(customPizza);

                            foreach (PizzaCompositionString cs in composition)
                            {
                                cs.Pizza_FK = id;
                                dbo.AddCompositionString(cs);
                            }
                            composition.Clear();
                            //AddInOrder(customPizza, 1);
                        }
                    }));
            }
        }

        private void AddInOrder(Pizza pizza, int count)
        {
            orderStrings.Add(new OrderString()
            {
                Pizza = pizza,
                Pizza_FK = pizza.Id,
                Count = count,
            });

            UpdateIngredientCount(count);
            shownPizza = allPizza.Select(i => i).Where(i => i.InStock == true).ToList();
            shownIngredients = allIngredients.Select(i => i).Where(i => i.InStock).ToList();
        }

        private void UpdateIngredientCount(int count)
        {
            foreach (Ingredient i in allIngredients)
            {
                i.CountStock -= count;
                if (i.CountStock < 1)
                {
                    i.InStock = false;
                    ChangePizzaStock(i.Id);
                }
            }
        }

        private void ChangePizzaStock(int ingrId)
        {
            List<Pizza> pizzaList = allPizza
                .Select(i => i)
                .Where(i => i.PizzaCompositionString.Select(c => c)
                            .Where(c => c.Ingredient_FK == ingrId) != null)
                .ToList();
            
            foreach (Pizza pizza in pizzaList)
                pizza.InStock = false;
        }
    }
}
