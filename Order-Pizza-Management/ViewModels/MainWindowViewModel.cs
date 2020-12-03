using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using Order_Pizza_Management.Models;
using Order_Pizza_Management.Utils;

namespace Order_Pizza_Management.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private Visibility menuVisibility = Visibility.Visible;
        public Visibility MenuVisibility
        {
            get { return menuVisibility; }
            set
            {
                menuVisibility = value;
                OnPropertyChanged("MenuVisibility");
            }
        }

        private Visibility customVisibility = Visibility.Hidden;
        public Visibility CustomVisibility
        {
            get { return customVisibility; }
            set
            {
                customVisibility = value;
                OnPropertyChanged("CustomVisibility");
            }
        }

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

        private ObservableCollection<Pizza> allPizza;
        public ObservableCollection<Pizza> ShownPizza { get; set; }

        private ObservableCollection<Ingredient> allIngredients;
        private List<int> beforeCustom;
        public ObservableCollection<Ingredient> ShownIngredients { get; set; }

        public ObservableCollection<OrderString> OrderStrings { get; set; }
        public ObservableCollection<PizzaCompositionString> Composition { get; set; }
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
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private DbOperations dbo;
        private DialogService ds;

        public MainWindowViewModel()
        {
            dbo = new DbOperations();
            ds = new DialogService();
            selectedCount = 1;
            orderCost = 0;
            cpizzaCost = 0;

            allIngredients = dbo.GetAvailableIngredients();
            ShownIngredients = new ObservableCollection<Ingredient>(allIngredients.ToList());
            allPizza = dbo.GetAvailablePizza();
            ShownPizza = new ObservableCollection<Pizza>(allPizza.ToList());
            Types = dbo.GetIngredientTypes();
            selectedType = Types.Last();

            Composition = new ObservableCollection<PizzaCompositionString>();
            OrderStrings = new ObservableCollection<OrderString>();
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
                            Composition.Add(new PizzaCompositionString()
                            {
                                Ingredient = selectedIngridient,
                                Ingredient_FK = selectedIngridient.Id,
                                Count = selectedCount
                            });

                            CustomPizzaCost = cpizzaCost + selectedIngridient.Price * selectedCount;
                            int ind = allIngredients.IndexOf(selectedIngridient);
                            allIngredients[ind].CountStock -= selectedCount;
                            if (allIngredients[ind].CountStock < 1)
                            {
                                ind = ShownIngredients.IndexOf(selectedIngridient);
                                ShownIngredients.RemoveAt(ind);
                            }
                            SelectedCount = 1;
                            OnPropertyChanged("ShownIngredients");
                        }
                        else
                            ds.ShowMessage("Ингредиент не был добавлен в пиццу: указанного количества ингрединта нет на складе.");    
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
                        if (Composition.Count != 0)
                        {
                            Pizza customPizza = new Pizza()
                            {
                                Name = "Пользовательская",
                                InStock = true,
                                IsCustom = true,
                                Price = CustomPizzaCost,
                            };

                            int id = dbo.AddPizza(customPizza);

                            foreach (PizzaCompositionString cs in Composition)
                            {
                                cs.Pizza_FK = id;
                                dbo.AddCompositionString(cs);
                            }
                            Composition.Clear();
                            AddInOrder(customPizza, 1);
                            CustomPizzaCost = 0;
                            beforeCustom.Clear();
                            foreach (Ingredient i in allIngredients)
                                beforeCustom.Add(i.CountStock);
                        }
                        else
                            ds.ShowMessage("Пользовательская пицца пуста. Добавьте ингредиенты и попробуйте снова.");
                    }));
            }
        }

        private RelayCommand switchToMenu;
        public RelayCommand SwitchToMenu
        {
            get
            {
                return switchToMenu ??
                    (switchToMenu = new RelayCommand(obj =>
                    {
                        if (Composition.Count == 0 || ds.AcceptionDialog())
                        {
                            Composition.Clear();
                            CustomPizzaCost = 0;
                            for (int i = 0; i < allIngredients.Count; i++)
                            {
                                allIngredients[i].CountStock = beforeCustom[i];
                                if (allIngredients[i].CountStock > 0)
                                    allIngredients[i].InStock = true;
                            }
                            ShownIngredients = new ObservableCollection<Ingredient>(allIngredients.Select(i => i).Where(i => i.InStock).ToList());
                            OnPropertyChanged("ShownIngredients");
                            CustomVisibility = Visibility.Hidden;
                            MenuVisibility = Visibility.Visible;
                        }
                    }));
            }
        }

        private RelayCommand switchToCustom;
        public RelayCommand SwitchToCustom
        {
            get
            {
                return switchToCustom ??
                    (switchToCustom = new RelayCommand(obj =>
                    {
                        MenuVisibility = Visibility.Hidden;
                        CustomVisibility = Visibility.Visible;
                        beforeCustom = new List<int>();
                        foreach (Ingredient i in allIngredients)
                            beforeCustom.Add(i.CountStock);
                    }));
            }
        }

        private RelayCommand finishOrdering;
        public RelayCommand FinishOrdering
        {
            get
            {
                return finishOrdering ??
                    (finishOrdering = new RelayCommand(obj =>
                    {
                        if (OrderStrings.Count != 0)
                        {
                            if (ds.EnterOrderDataDialog())
                            {
                                if (Validate(ds.Address, ds.PhoneNubmber))
                                {
                                    Order order = new Order()
                                    {
                                        Address = ds.Address,
                                        PhoneNumber = ds.PhoneNubmber,
                                        Cost = OrderCost
                                    };
                                    int orderId = dbo.AddOrder(order);
                                    foreach (OrderString os in OrderStrings)
                                    {
                                        os.Order_FK = orderId;
                                        dbo.AddOrderString(os);
                                    }
                                    OrderStrings.Clear();
                                    OrderCost = 0;

                                    foreach (Ingredient i in allIngredients)
                                        dbo.UpdateIngredient(i);
                                    allIngredients = dbo.GetAvailableIngredients();
                                    foreach (Pizza p in allPizza)
                                        dbo.UpdatePizza(p);
                                    allPizza = dbo.GetAvailablePizza();

                                    ShownIngredients = allIngredients;
                                    OnPropertyChanged("ShownIngredients");
                                    ShownPizza = allPizza;
                                    OnPropertyChanged("ShownPizza");
                                }
                                else
                                    ds.ShowMessage("Введены неверные данные. Заказ не был сохранен. Попробуйте снова.");
                            }
                        }
                        else
                            ds.ShowMessage("Заказ пуст. Добавьте пиццу из меню или пользовательскую пиццу и попробуйте снова.");
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
            OrderStrings.Add(os);

            UpdateIngredientsCount(pizza.Id, count);
            ShownPizza = new ObservableCollection<Pizza>(allPizza.Select(i => i).Where(i => i.InStock == true).ToList());
            OnPropertyChanged("ShownPizza");
            ShownIngredients = new ObservableCollection<Ingredient>(allIngredients.Select(i => i).Where(i => i.InStock).ToList());
            OnPropertyChanged("ShownIngredients");
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
        private bool Validate(string address, string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(address))
                return false;
            var cleaned = Regex.Replace(phoneNumber, @"[^0-9]+", "");
            if (cleaned.Length == 11)
                return true;
            else
                return false;
        }
    }
}
