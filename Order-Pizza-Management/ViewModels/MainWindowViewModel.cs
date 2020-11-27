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
        private double cpizzaCost;
        public double CustomPizzaCost
        {
            get { return cpizzaCost; }
            set
            {
                cpizzaCost = value;
                OnPropertyChanged("CustomPizzaCost");
            }
        }

        private int selectedCount;
        public int SelectedCount
        {
            get { return selectedCount; }
            set
            {
                selectedCount = value;
                OnPropertyChanged("SelectedCount");
            }
        }

        private double orderCost;
        public double OrderCost
        {
            get { return orderCost; }
            set
            {
                orderCost = value;
                OnPropertyChanged("OrderCost");
            }
        }

        private ObservableCollection<Pizza> allPizza { get; set; }
        public ObservableCollection<Pizza> shownPizza { get; set; }

        private ObservableCollection<Ingredient> allIngredients;
        public ObservableCollection<Ingredient> shownIngredients { get; set; }

        public ObservableCollection<OrderString> orderStrings { get; set; }
        public ObservableCollection<PizzaCompositionString> composition { get; set; }
        public ObservableCollection<IngredientType> Types { get; set; }

        private Pizza selectedPizza;
        public Pizza SelectedPizza
        {
            get { return selectedPizza; }
            set
            {
                selectedPizza = value;
                OnPropertyChanged("SelectedPizza");
            }
        }

        private Ingredient selectedIngridient;
        public Ingredient SelectedIngridient
        {
            get { return selectedIngridient; }
            set
            {
                selectedIngridient = value;
                OnPropertyChanged("SelectedIngridient");
            }
        }

        private IngredientType selectedType;
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
            orderCost = 0;
            cpizzaCost = 0;

            allIngredients = dbo.GetAvailableIngredients();
            shownIngredients = new ObservableCollection<Ingredient>(allIngredients.ToList());
            allPizza = dbo.GetAvailablePizza();
            shownPizza = new ObservableCollection<Pizza>(allPizza.ToList());
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
                        SelectedCount = 1;
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

                            CustomPizzaCost = cpizzaCost + selectedIngridient.Price * selectedCount;
                            SelectedCount = 1;
                            //int ind = allIngredients.IndexOf(selectedIngridient);
                            //allIngredients[ind].CountStock -= selectedCount;
                            //if(allIngredients[ind].CountStock < )
                            //shownIngredients
                        }
                        else
                        {
                            MessageBoxResult res = MessageBox.Show("Ингредиент не был добавлен в пиццу: указанного количества ингрединта нет на складе.","Внимание");    
                        }
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
                            Pizza customPizza = new Pizza()
                            {
                                Name = "Пользовательская",
                                InStock = true,
                                IsCustom = true,
                                Price = CustomPizzaCost,
                            };

                            int id = dbo.AddPizza(customPizza);

                            foreach (PizzaCompositionString cs in composition)
                            {
                                cs.Pizza_FK = id;
                                dbo.AddCompositionString(cs);
                            }
                            composition.Clear();
                            AddInOrder(customPizza, 1);
                        }
                    }));
            }
        }

        private void AddInOrder(Pizza pizza, int count)
        {
            OrderString os = new OrderString()
            {
                Pizza = pizza,
                Pizza_FK = pizza.Id,
                Count = count,
            };
            orderStrings.Add(os);

            UpdateIngredientsCount(pizza.Id, count);
            shownPizza = new ObservableCollection<Pizza>(allPizza.Select(i => i).Where(i => i.InStock == true).ToList());
            shownIngredients = new ObservableCollection<Ingredient>(allIngredients.Select(i => i).Where(i => i.InStock).ToList());
            OrderCost = orderCost + pizza.Price * count;
        }
        private void UpdateIngredientsCount(int pizzaId, int count)
        {
            List<int> ingredientIDs = dbo.GetIngredientsIdByPizzaId(pizzaId);
            for (int i = 0; i < allIngredients.Count; i++)
                if (ingredientIDs.Contains(allIngredients[i].Id))
                {
                    allIngredients[i].CountStock -= count;
                    if (allIngredients[i].CountStock < 1)
                    {
                        allIngredients[i].InStock = false;
                        ChangePizzaStock(allIngredients[i].Id);
                    }
                }
        }
        private void ChangePizzaStock(int ingrId)
        {
            List<int> pizzaIDs = dbo.GetPizzasIdByIngredientId(ingrId);
            for (int i = 0; i < allPizza.Count; i++)
                if(pizzaIDs.Contains(allPizza[i].Id))
                    allPizza[i].InStock = false;
        }

        private RelayCommand finishOrdering;
        public RelayCommand FinishOrdering
        {
            get
            {
                return finishOrdering ??
                    (finishOrdering = new RelayCommand(obj =>
                    {
                        // soon...
                    }));
            }
        }
    }
}
