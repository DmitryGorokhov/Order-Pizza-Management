namespace Order_Pizza_Management.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PizzaCompositionString")]
    public partial class PizzaCompositionString
    {
        public int Id { get; set; }

        public int Count { get; set; }

        public int Ingredient_FK { get; set; }

        public int Pizza_FK { get; set; }

        public virtual Ingredient Ingredient { get; set; }

        public virtual Pizza Pizza { get; set; }
    }
}
