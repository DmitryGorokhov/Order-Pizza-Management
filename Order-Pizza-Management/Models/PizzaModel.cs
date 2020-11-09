namespace Order_Pizza_Management.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PizzaModel : DbContext
    {
        public PizzaModel()
            : base("name=PizzaModel")
        {
        }

        public virtual DbSet<Ingredient> Ingredient { get; set; }
        public virtual DbSet<IngredientType> IngredientType { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderString> OrderString { get; set; }
        public virtual DbSet<Pizza> Pizza { get; set; }
        public virtual DbSet<PizzaCompositionString> PizzaCompositionString { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ingredient>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Ingredient>()
                .HasMany(e => e.PizzaCompositionString)
                .WithRequired(e => e.Ingredient)
                .HasForeignKey(e => e.Ingredient_FK)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<IngredientType>()
                .Property(e => e.Value)
                .IsUnicode(false);

            modelBuilder.Entity<IngredientType>()
                .HasMany(e => e.Ingredient)
                .WithRequired(e => e.IngredientType)
                .HasForeignKey(e => e.Type_FK)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.PhoneNumber)
                .IsFixedLength();

            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderString)
                .WithRequired(e => e.Order)
                .HasForeignKey(e => e.Order_FK)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pizza>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Pizza>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Pizza>()
                .HasMany(e => e.OrderString)
                .WithRequired(e => e.Pizza)
                .HasForeignKey(e => e.Pizza_FK)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pizza>()
                .HasMany(e => e.PizzaCompositionString)
                .WithRequired(e => e.Pizza)
                .HasForeignKey(e => e.Pizza_FK)
                .WillCascadeOnDelete(false);
        }
    }
}
