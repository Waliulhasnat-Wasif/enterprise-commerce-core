using EnterpriseCommerceCore.App.Step04_Association;

namespace EnterpriseCommerceCore.App.Step05_Composition
{
    // The ONLY view of an order line that outsiders ever see: read-only.
    // The real, mutable OrderLine class is hidden inside PurchaseOrder (see below),
    // so outside code can neither create lines nor change them. This is Abstraction
    // (Step 02) used to enforce Composition ownership.
    public interface IOrderLine
    {
        Product Product { get; }
        int Quantity { get; }
        decimal UnitPrice { get; }
        decimal LineTotal { get; }
    }
}