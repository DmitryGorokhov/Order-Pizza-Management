namespace Order_Pizza_Management.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderString")]
    public partial class OrderString
    {
        public int Id { get; set; }

        public int Count { get; set; }

        public int Pizza_FK { get; set; }

        public int Order_FK { get; set; }

        public virtual Order Order { get; set; }

        public virtual Pizza Pizza { get; set; }
    }
}
