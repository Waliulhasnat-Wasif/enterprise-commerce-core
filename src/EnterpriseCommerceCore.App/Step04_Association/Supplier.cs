using System;
using System.Collections.Generic;

namespace EnterpriseCommerceCore.App.Step04_Association
{
    public class Supplier
    {
        public string Name { get; }

        private readonly List<Product> _products = new();
        public IReadOnlyList<Product> Products => _products;

        public Supplier(string name)
        {
            Name = name;
        }

        // Mirror image of Product.AddSupplier. Calling EITHER side keeps BOTH in sync.
        public void AddProduct(Product product)
        {
            if (product is null) throw new ArgumentNullException(nameof(product));
            if (_products.Contains(product)) return;

            _products.Add(product);
            product.AddSupplier(this);
        }

        public override string ToString() => $"Supplier[{Name}]";
    }
}