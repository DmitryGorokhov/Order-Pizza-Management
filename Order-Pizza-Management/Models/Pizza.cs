namespace Order_Pizza_Management.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Pizza")]
    public partial class Pizza
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pizza()
        {
            OrderString = new HashSet<OrderString>();
            PizzaCompositionString = new HashSet<PizzaCompositionString>();
        }

        public int Id { get; set; }

        [StringLength(70)]
        public string Name { get; set; }

        public double Price { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public bool? InStock { get; set; }

        public bool IsCustom { get; set; }

        public bool IsVisible { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderString> OrderString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PizzaCompositionString> PizzaCompositionString { get; set; }
    }
}
