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

        private Visibility logInVisibility = Visibility.Hidden;
        public Visibility LogInVisibility
        {
            get { return logInVisibility; }
            set
            {
                logInVisibility = value;
                OnPropertyChanged("LogInVisibility");
            }
        }


        private Visibility logOutVisibility = Visibility.Visible;
        public Visibility LogOutVisibility
        {
            get { return logOutVisibility; }
            set
            {
                logOutVisibility = value;
                OnPropertyChanged("LogOutVisibility");
            }
        }

        private double cpizzaCost = 0;
        public double CustomPizzaCost
        {
            get { return cpizzaCost; }
            set
            {
                cpizzaCost = value;
                OnPropertyChanged("CustomPizzaCost");
            }
        }

        private int selectedCount = 1;
        public int SelectedCount
        {
            get { return selectedCount; }
            set
            {
                selectedCount = value;
                OnPropertyChanged("SelectedCount");
            }
        }

        private double orderCost = 0;
        public double OrderCost
        {
            get { return orderCost; }
            set
            {
                orderCost = value;
                OnPropertyChanged("OrderCost");
            }
        }

        private ObservableCollection<Pizza> availablePizza;
        public ObservableCollection<Pizza> ShownPizza { get; set; }
        public ObservableCollection<Pizza> AllPizza { get; set; }

        private ObservableCollection<Ingredient> availableIngredients;
        private List<int> ingrCountBeforeCustom;
        public ObservableCollection<Ingredient> ShownIngredients { get; set; }
        public ObservableCollection<Ingredient> Ingredients { get; set; }

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

        private Pizza changingPizza;
        public Pizza ChangingPizza
        {
            get { return changingPizza; }
            set
            {
                changingPizza = value;
                OnPropertyChanged("ChangingPizza");
                CustomPizzaCost = changingPizza.Price;
                FillComposition(changingPizza.Id);
                if(deletedCompositionStrings.Count != 0)
                    deletedCompositionStrings.Clear();
                CustomPizzaCost = changingPizza.Price;
            }
        }

        private Ingredient selectedIngredient;
        public Ingredient SelectedIngredient
        {
            get { return selectedIngredient; }
            set
            {
                selectedIngredient = value;
                OnPropertyChanged("SelectedIngredient");
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

        private List<PizzaCompositionString> allCompositionStrings;
        private List<PizzaCompositionString> deletedCompositionStrings;

        private PizzaCompositionString selectedCompositionItem;
        public PizzaCompositionString SelectedCompositionItem
        {
            get { return selectedCompositionItem; }
            set
            {
                selectedCompositionItem = value;
                OnPropertyChanged("SelectedCompositionItem");
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

            availableIngredients = dbo.GetAvailableIngredients();
            ShownIngredients = new ObservableCollection<Ingredient>(availableIngredients.ToList());

            availablePizza = dbo.GetAvailablePizza();
            ShownPizza = new ObservableCollection<Pizza>(availablePizza.ToList());

            Types = dbo.GetIngredientTypes();
            allCompositionStrings = dbo.GetAllCompositionString();
            deletedCompositionStrings = new List<PizzaCompositionString>();

            AllPizza = dbo.GetAllPizza();
            Ingredients = dbo.GetAllIngredients();

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
                        try
                        {
                            AddInOrder(selectedPizza, selectedCount);
                            SelectedCount = 1;
                        }
                        catch (System.NullReferenceException)
                        {
                            ds.ShowMessage("Пицца не была выбрана.");
                        }
                        catch
                        {
                            ds.ShowMessage("При добавлении пиццы произошла ошибка. Повторите попытку.");
                        }
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
                        try
                        {
                            if (selectedIngredient.CountStock >= selectedCount)
                            {
                                double dop = AddIngredientInComposition(selectedIngredient, selectedCount);

                                CustomPizzaCost = cpizzaCost + dop;
                                int ind = availableIngredients.IndexOf(selectedIngredient);
                                availableIngredients[ind].CountStock -= selectedCount;
                                if (availableIngredients[ind].CountStock < 1)
                                {
                                    ind = ShownIngredients.IndexOf(selectedIngredient);
                                    ShownIngredients.RemoveAt(ind);
                                }
                                SelectedCount = 1;
                                OnPropertyChanged("ShownIngredients");
                            }
                            else
                                ds.ShowMessage("Ингредиент не был добавлен в пиццу: указанного количества ингрединта нет на складе.");
                        }
                        catch (System.NullReferenceException)
                        {
                            ds.ShowMessage("Ингредиент не был выбран.");
                        }
                        catch
                        {
                            ds.ShowMessage("При добавлении ингредиента произошла ошибка. Повторите попытку.");
                        }
                    }));
            
            }
        }

        private double AddIngredientInComposition(Ingredient i, int count)
        {
            Composition.Add(new PizzaCompositionString()
            {
                Ingredient = i,
                Ingredient_FK = i.Id,
                Count = count
            });
            return i.Price * count;
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
                            ingrCountBeforeCustom.Clear();
                            foreach (Ingredient i in availableIngredients)
                                ingrCountBeforeCustom.Add(i.CountStock);
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
                            for (int i = 0; i < availableIngredients.Count; i++)
                            {
                                availableIngredients[i].CountStock = ingrCountBeforeCustom[i];
                                if (availableIngredients[i].CountStock > 0)
                                    availableIngredients[i].InStock = true;
                            }
                            ShownIngredients = new ObservableCollection<Ingredient>(availableIngredients.Select(i => i).Where(i => i.InStock).ToList());
                            OnPropertyChanged("ShownIngredients");
                            CustomVisibility = Visibility.Hidden;
                            MenuVisibility = Visibility.Visible;
                            SelectedPizza = null;
                            SelectedIngredient = null;
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
                        ingrCountBeforeCustom = new List<int>();
                        foreach (Ingredient i in availableIngredients)
                            ingrCountBeforeCustom.Add(i.CountStock);
                        SelectedPizza = null;
                        SelectedIngredient = null;
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

                                    foreach (Ingredient i in availableIngredients)
                                        dbo.UpdateIngredient(i);
                                    availableIngredients = dbo.GetAvailableIngredients();
                                    foreach (Pizza p in availablePizza)
                                        dbo.UpdatePizza(p);
                                    availablePizza = dbo.GetAvailablePizza();

                                    ShownIngredients = availableIngredients;
                                    OnPropertyChanged("ShownIngredients");
                                    ShownPizza = availablePizza;
                                    OnPropertyChanged("ShownPizza");
                                    SelectedPizza = null;
                                    SelectedIngredient = null;
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

        private RelayCommand exportReport;
        public RelayCommand ExportReport
        {
            get
            {
                return exportReport ??
                    (exportReport = new RelayCommand(obj =>
                    {
                        // soon...
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
            ShownPizza = new ObservableCollection<Pizza>(availablePizza.Select(i => i).Where(i => i.InStock == true).ToList());
            OnPropertyChanged("ShownPizza");
            ShownIngredients = new ObservableCollection<Ingredient>(availableIngredients.Select(i => i).Where(i => i.InStock).ToList());
            OnPropertyChanged("ShownIngredients");
            OrderCost = orderCost + pizza.Price * count;
        }
        private void UpdateIngredientsCount(int pizzaId, int count)
        {
            List<int> ingredientIDs = dbo.GetIngredientsIdByPizzaId(pizzaId);
            for (int i = 0; i < availableIngredients.Count; i++)
                if (ingredientIDs.Contains(availableIngredients[i].Id))
                {
                    availableIngredients[i].CountStock -= count;
                    if (availableIngredients[i].CountStock < 1)
                    {
                        availableIngredients[i].InStock = false;
                        ChangePizzaStock(availableIngredients[i].Id);
                    }
                }
        }
        private void ChangePizzaStock(int ingrId)
        {
            List<int> pizzaIDs = dbo.GetPizzasIdByIngredientId(ingrId);
            for (int i = 0; i < availablePizza.Count; i++)
                if(pizzaIDs.Contains(availablePizza[i].Id))
                    availablePizza[i].InStock = false;
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
        private bool ValidatePassword(string password)
        {
           return !string.IsNullOrEmpty(password);
        }

        private RelayCommand createPizza;
        public RelayCommand CreatePizza
        {
            get
            {
                return createPizza ??
                    (createPizza = new RelayCommand(obj =>
                    {
                        Pizza p = new Pizza()
                        {
                            Name = "Пустая пицца",
                            Price = 0,
                            Description = "Описание отсутсвует",
                            InStock = false,
                            IsCustom = false
                        };
                        dbo.AddPizza(p);
                        AllPizza.Add(p);
                    }));
            }
        }

        private RelayCommand updateSelectedPizza;
        public RelayCommand UpdateSelectedPizza
        {
            get
            {
                return updateSelectedPizza ??
                    (updateSelectedPizza = new RelayCommand(obj =>
                    {
                        try
                        {
                            selectedPizza.InStock = GetPizzaStock(selectedPizza.Id);
                            dbo.UpdatePizza(selectedPizza);
                        }
                        catch
                        {
                            ds.ShowMessage("Произошла ошибка при редактировании выбранного элемента. Попробуйте снова.");
                        }
                    }));
            }
        }
                
        private RelayCommand createIngr;
        public RelayCommand CreateIngr
        {
            get
            {
                return createIngr ??
                    (createIngr = new RelayCommand(obj =>
                    {
                        Ingredient i = new Ingredient()
                        {
                            Name = "Новый ингредиент",
                            Price = 0,
                            InStock = false,
                            CountStock = 0,
                            Type_FK = 1
                        };
                        dbo.AddIngredient(i);
                        Ingredients.Add(i);
                    }));
            }
        }

        private RelayCommand updateSelectedIngr;
        public RelayCommand UpdateSelectedIngr
        {
            get
            {
                return updateSelectedIngr ??
                    (updateSelectedIngr = new RelayCommand(obj =>
                    {
                        try
                        {
                            if (SelectedType != null)
                            {
                                selectedIngredient.Type_FK = selectedType.Id;
                            }
                            dbo.UpdateIngredient(selectedIngredient);
                            if (selectedIngredient.InStock)
                            {
                                List<int> pizzaIDs = dbo.GetPizzasIdByIngredientId(selectedIngredient.Id);
                                for (int i = 0; i < AllPizza.Count; i++)
                                {
                                    int id = AllPizza[i].Id;
                                    if (pizzaIDs.Contains(id))
                                    {
                                        bool val = GetPizzaStock(id);
                                        if ( val != AllPizza[i].InStock)
                                        {
                                            AllPizza[i].InStock = val;
                                            dbo.UpdatePizza(AllPizza[i]);
                                            
                                        }
                                    }
                                }
                            }
                            OnPropertyChanged("Ingredients");
                            OnPropertyChanged("AllPizza");
                        }
                        catch
                        {
                            ds.ShowMessage("Произошла ошибка при редактировании выбранного элемента. Попробуйте снова.");
                        }
                    }));
            }
        }

        private bool GetPizzaStock(int pizzaId)
        {
            List<int> IngrIDs = dbo.GetIngredientsIdByPizzaId(pizzaId);
            foreach (Ingredient i in Ingredients)
                if (IngrIDs.Contains(i.Id) && !i.InStock)
                    return false;
            return true;
        }
        private void FillComposition(int pizzaId)
        {
            var r = allCompositionStrings.Where(i => i.Pizza_FK == pizzaId).Select(i => i).ToList();
            Composition = new ObservableCollection<PizzaCompositionString>(r);
            OnPropertyChanged("Composition");
        }

        private RelayCommand deleteIngredientComposition;
        public RelayCommand DeleteIngredientComposition
        {
            get
            {
                return deleteIngredientComposition ??
                    (deleteIngredientComposition = new RelayCommand(obj =>
                    {
                        try
                        {
                            Composition = new ObservableCollection<PizzaCompositionString>(Composition
                            .Where(i => i.Id != selectedCompositionItem.Id).ToList());
                            deletedCompositionStrings.Add(selectedCompositionItem);
                            CustomPizzaCost = cpizzaCost - selectedCompositionItem.Ingredient.Price * selectedCompositionItem.Count;
                            OnPropertyChanged("Composition");
                        }
                        catch
                        {
                            ds.ShowMessage("Произошла ошибка при удалении выбранного элемента. Попробуйте снова.");
                        }
                    }));
            }
        }

        private RelayCommand updateComposition;
        public RelayCommand UpdateComposition
        {
            get
            {
                return updateComposition ??
                    (updateComposition = new RelayCommand(obj =>
                    {
                        try
                        {
                            foreach (PizzaCompositionString cs in Composition)
                            {
                                if (allCompositionStrings.Contains(cs))
                                    dbo.UpdateCompositionString(cs);
                                else
                                {
                                    cs.Pizza_FK = changingPizza.Id;
                                    dbo.AddCompositionString(cs);
                                }
                            }
                            foreach (PizzaCompositionString cs in deletedCompositionStrings)
                            {
                                try
                                {
                                    dbo.DeleteCompositionString(cs);
                                }
                                catch
                                {
                                    ds.ShowMessage("Произошла ошибка при удалении выбранного элемента. Попробуйте снова.");
                                }
                            }
                            deletedCompositionStrings.Clear();
                            changingPizza.Price = cpizzaCost;
                            changingPizza.InStock = GetPizzaStock(changingPizza.Id);
                            dbo.UpdatePizza(changingPizza);
                            allCompositionStrings = dbo.GetAllCompositionString();
                        }
                        catch
                        {
                            ds.ShowMessage("Произошла ошибка при изменении состава пиццы. Попробуйте снова.");
                        }
                    }));
            }
        }

        private RelayCommand addIngrInComposition;
        public RelayCommand AddIngrInComposition
        {
            get
            {
                return addIngrInComposition ??
                    (addIngrInComposition = new RelayCommand(obj =>
                    {
                        try
                        {
                            double dop = AddIngredientInComposition(selectedIngredient, selectedCount);
                            CustomPizzaCost = cpizzaCost + dop;
                            SelectedCount = 1;
                            OnPropertyChanged("Composition");
                        }
                        catch (System.NullReferenceException)
                        {
                            ds.ShowMessage("Ингредиент не был выбран.");
                        }
                        catch
                        {
                            ds.ShowMessage("При добавлении ингредиента произошла ошибка. Повторите попытку.");
                        }
                    }));
            }
        }

        private RelayCommand logIn;
        public RelayCommand LogIn
        {
            get
            {
                return logIn ??
                    (logIn = new RelayCommand(obj =>
                    {
                        if ((Composition.Count == 0 || ds.AcceptionDialog()) && ds.LogIn())
                        {
                            if (ValidatePassword(ds.Password))
                            {
                                LogOutVisibility = Visibility.Hidden;
                                LogInVisibility = Visibility.Visible;
                                // init collections
                                // soon ...
                            }
                            else ds.ShowMessage("Указанный пароль неверен. Повторите попытку.");
                        }
                    }));
            }
        }

        private RelayCommand logOut;
        public RelayCommand LogOut
        {
            get
            {
                return logOut ??
                    (logOut = new RelayCommand(obj =>
                    {
                        LogInVisibility = Visibility.Hidden;
                        LogOutVisibility = Visibility.Visible;
                        // init collections
                        // soon ...
                    }));
            }
        }
    }
}
