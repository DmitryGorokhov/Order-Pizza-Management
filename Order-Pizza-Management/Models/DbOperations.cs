using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .Where(c => c.InStock == true).ToList();
            return new ObservableCollection<Ingredient>(r);
        }

        public ObservableCollection<Pizza> GetAvailablePizza()
        {
            var r = db.Pizza.Select(c => c)
                .Where(c => c.InStock == true && c.IsCustom == false).ToList();
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

    }
}
