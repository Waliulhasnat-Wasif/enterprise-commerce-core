namespace EnterpriseCommerceCore.App.Step02_Abstraction
{
    // NOT every payment method needs this — that's the point of an interface.
    // Only classes that genuinely support refunds will implement it.
    public interface IRefundable
    {
        bool Refund(decimal amount);
    }
}