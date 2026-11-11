using System;
using System.Collections.Generic;

namespace EnterpriseCommerceCore.App.Step04_Association
{
    // AGGREGATION: Category "has" Products, but it does NOT create or own them.
    // Products are created elsewhere, can sit in SEVERAL categories at once,
    // and keep existing when a Category is removed.
    public class Category
    {
        public string Name { get; }

        private readonly List<Product> _products = new();
        public IReadOnlyList<Product> Products => _products;

        public Category(string name)
        {
            Name = name;
        }

        public void Add(Product product)
        {
            if (product is null) throw new ArgumentNullException(nameof(product));
            if (!_products.Contains(product)) _products.Add(product);
        }

        // Removing a product from the category does NOT destroy the product itself.
        public bool Remove(Product product) => _products.Remove(product);

        public override string ToString() => $"Category[{Name}, {_products.Count} products]";
    }
}