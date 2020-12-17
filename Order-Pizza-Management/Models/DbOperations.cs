using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Order_Pizza_Management.Models
{
    public class DbOperations
    {
        private PizzaModel db;
        public DbOperations()
        {
            db = new PizzaModel();
        }

        public ObservableCollection<Ingredient> GetAvailableIngredients()
        {
            var r =  db.Ingredient.Select(c => c)
                .Where(c => c.InStock && c.IsVisible).ToList();
            return new ObservableCollection<Ingredient>(r);
        }

        public ObservableCollection<Pizza> GetAvailablePizza()
        {
            var r = db.Pizza.Select(c => c)
                .Where(c => c.InStock == true && !c.IsCustom && c.IsVisible).ToList();
            return new ObservableCollection<Pizza>(r);
        }

        public ObservableCollection<IngredientType> GetIngredientTypes()
        {
            var r = db.IngredientType.Select(c => c).ToList();
            return new ObservableCollection<IngredientType>(r);
        }

        public int AddPizza(Pizza p)
        {
            db.Pizza.Add(p);
            db.SaveChanges();
            return p.Id;
        }

        public int AddCompositionString(PizzaCompositionString cs)
        {
            db.PizzaCompositionString.Add(cs);
            db.SaveChanges();
            return cs.Id;
        }

        public List<int> GetIngredientsIdByPizzaId(int pizzaId)
        {
            return db.PizzaCompositionString
                .Where(c => c.Pizza_FK == pizzaId)
                .Select(c => c.Ingredient_FK)
                .ToList();
        }

        public List<int> GetPizzasIdByIngredientId(int ingrId)
        {
            return db.PizzaCompositionString
                .Where(c => c.Ingredient_FK == ingrId)
                .Select(c => c.Pizza_FK)
                .ToList();
        }

        public int AddOrder(Order o)
        {
            db.Order.Add(o);
            db.SaveChanges();
            return o.Id;
        }

        public int AddOrderString(OrderString os)
        {
            db.OrderString.Add(os);
            db.SaveChanges();
            return os.Id;
        }

        public void UpdateIngredient(Ingredient ingr)
        {
            Ingredient i = db.Ingredient.Where(c => c.Id == ingr.Id).FirstOrDefault();
            if (i != null)
            {
                i.Name = ingr.Name;
                i.Price = ingr.Price;
                i.CountStock = ingr.CountStock;
                if (i.CountStock > 0) i.InStock = true;
                else i.InStock = false;
                i.Type_FK = ingr.Type_FK;
                i.IsVisible = ingr.IsVisible;
                db.SaveChanges();
            }
        }

        public void UpdatePizza(Pizza pizza)
        {
            Pizza p = db.Pizza.Where(c => c.Id == pizza.Id).FirstOrDefault();
            if (p != null)
            {
                p.Name = pizza.Name;
                p.Price = pizza.Price;
                p.InStock = pizza.InStock;
                p.Description = pizza.Description;
                p.IsCustom = pizza.IsCustom;
                p.IsVisible = pizza.IsVisible;
                db.SaveChanges();
            }
        }

        public ObservableCollection<Ingredient> GetAllIngredients()
        {
            var r = db.Ingredient.Select(c => c).ToList();
            return new ObservableCollection<Ingredient>(r);
        }

        public ObservableCollection<Pizza> GetAllPizza()
        {
            var r = db.Pizza.Select(c => c).ToList();
            return new ObservableCollection<Pizza>(r);
        }

        public int AddIngredient(Ingredient i)
        {
            db.Ingredient.Add(i);
            db.SaveChanges();
            return i.Id;
        }

        public List<PizzaCompositionString> GetAllCompositionString()
        {
            var r = db.PizzaCompositionString.Select(c => c).ToList();
            return r;
        }

        public void UpdateCompositionString(PizzaCompositionString cs)
        {
            PizzaCompositionString c = db.PizzaCompositionString.Where(i => i.Id == cs.Id).FirstOrDefault();
            if (c != null)
            {
                c.Ingredient_FK = cs.Ingredient_FK;
                c.Pizza_FK = cs.Pizza_FK;
                c.Count = cs.Count;
                db.SaveChanges();
            }
        }

        public void DeleteCompositionString(PizzaCompositionString cs)
        {
            PizzaCompositionString c = db.PizzaCompositionString.Where(i => i.Id == cs.Id).FirstOrDefault();
            db.PizzaCompositionString.Remove(c);
            db.SaveChanges();
        }

        public List<Order> GetOrdersByPeriod(DateTime period)
        {
            return db.Order
                .Where(i => i.CreatedAt.Month == period.Month &&
                i.CreatedAt.Year == period.Year).ToList();
        }

        public string GetOrderCompositionStringByOrderId(int orderId)
        {
            string s = "";
            var res = db.OrderString
                .Where(i => i.Order_FK == orderId)
                .Select(i => new { i.Pizza.Name, i.Count })
                .ToList();
            foreach (var item in res)
                s = $"{s} {item.Name} {item.Count}.";
            return s;
        }
    }
}
