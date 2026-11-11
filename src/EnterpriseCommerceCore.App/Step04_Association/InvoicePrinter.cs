namespace EnterpriseCommerceCore.App.Step04_Association
{
    public class InvoicePrinter
    {
        // DEPENDENCY: Order appears only as a method PARAMETER. InvoicePrinter has no
        // field holding an Order, uses it for this one call, then forgets it.
        // No field = no lasting relationship = Dependency, not Association.
        public string Print(Order order)
        {
            var paymentInfo = order.Payment is null ? "payment pending" : order.Payment.ToString();
            return $"  INVOICE -> {order} | customer: {order.Customer.Name} | {paymentInfo}";
        }
    }
}