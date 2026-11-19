using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseCommerceCore.App.Step01_Inheritance;
using EnterpriseCommerceCore.App.Step04_Association;

namespace EnterpriseCommerceCore.App.Step05_Composition
{
    public enum OrderStatus { Draft, Placed }

    public class PurchaseOrder
    {
        public Guid OrderId { get; } = Guid.NewGuid();

        // ASSOCIATION (from Step 04): Customer exists independently of this order.
        public Customer Customer { get; }

        // COMPOSITION with a value object: the order owns its shipping address.
        public Address? ShippingAddress { get; private set; }

        public OrderStatus Status { get; private set; } = OrderStatus.Draft;

        // COMPOSITION: the order OWNS its lines. The list is private, and the lines are
        // instances of a PRIVATE nested class, so nobody outside can create or touch one.
        private readonly List<OrderLine> _lines = new();

        // Outsiders get a read-only wrapper over the IOrderLine interface.
        // AsReadOnly() blocks even sneaky casts to IList from adding or removing items.
        public IReadOnlyList<IOrderLine> Lines => _lines.AsReadOnly();

        // Derived from the parts: never stored separately, so it can never go out of sync.
        public decimal Total => _lines.Sum(l => l.LineTotal);

        public PurchaseOrder(Customer customer)
        {
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        }

        // ---------- All changes to the parts go THROUGH the whole, which guards the rules ----------

        public void AddItem(Product product, int quantity)
        {
            EnsureDraft();
            if (product is null) throw new ArgumentNullException(nameof(product));
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");

            // Business rule: adding the same product again increases the existing line.
            var existing = _lines.FirstOrDefault(l => ReferenceEquals(l.Product, product));
            if (existing is not null)
            {
                existing.Quantity += quantity;
                return;
            }

            // The whole CREATES the part itself. Nobody hands a line to the order.
            _lines.Add(new OrderLine(product, quantity));
        }

        public bool RemoveItem(Product product)
        {
            EnsureDraft();
            return _lines.RemoveAll(l => ReferenceEquals(l.Product, product)) > 0;
        }

        public void SetShippingAddress(Address address)
        {
            EnsureDraft();
            ShippingAddress = address ?? throw new ArgumentNullException(nameof(address));
        }

        public void Place()
        {
            EnsureDraft();
            if (_lines.Count == 0)
                throw new InvalidOperationException("Cannot place an empty order.");
            if (ShippingAddress is null)
                throw new InvalidOperationException("Shipping address is required before placing.");

            Status = OrderStatus.Placed; // from now on, the order is locked
        }

        private void EnsureDraft()
        {
            if (Status != OrderStatus.Draft)
                throw new InvalidOperationException("Order is already placed and can no longer be modified.");
        }

        public override string ToString() =>
            $"PurchaseOrder[{OrderId.ToString()[..8]}] {Status}, {_lines.Count} line(s), total {Total:N2}";

        // ============================================================
        // PRIVATE NESTED CLASS: invisible outside PurchaseOrder.
        // The enclosing class CAN create it (it is in scope), nobody else can even name it.
        // Its setter is therefore only reachable by PurchaseOrder: true ownership.
        // ============================================================
        private sealed class OrderLine : IOrderLine
        {
            public Product Product { get; }
            public int Quantity { get; set; }

            // SNAPSHOT of the price at order time. If the catalog price changes tomorrow,
            // yesterday's order must NOT change. Real systems always do this.
            public decimal UnitPrice { get; }

            public decimal LineTotal => UnitPrice * Quantity;

            public OrderLine(Product product, int quantity)
            {
                Product = product;
                Quantity = quantity;
                UnitPrice = product.Price;
            }
        }
    }
}