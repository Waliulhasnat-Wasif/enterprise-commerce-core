using System;
using System.Collections.Generic;

namespace EnterpriseCommerceCore.App.Step04_Association
{
    public class Product
    {
        public string Name { get; }
        public decimal Price { get; }

        // Private list so outside code cannot add/remove directly and break consistency.
        private readonly List<Supplier> _suppliers = new();

        // IReadOnlyList = callers can READ but not modify. Encapsulation of the relationship.
        public IReadOnlyList<Supplier> Suppliers => _suppliers;

        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        // BIDIRECTIONAL SYNC: adding a supplier here ALSO updates the supplier's side.
        // The Contains() check is the guard that stops infinite back-and-forth recursion.
        public void AddSupplier(Supplier supplier)
        {
            if (supplier is null) throw new ArgumentNullException(nameof(supplier));
            if (_suppliers.Contains(supplier)) return; // already linked: stop here

            _suppliers.Add(supplier);
            supplier.AddProduct(this); // keep the other side in sync
        }

        public override string ToString() => $"Product[{Name}]";
    }
}