namespace EnterpriseCommerceCore.App.Step02_Abstraction
{
    // PURE INTERFACE: 100% contract, zero implementation, zero state.
    // Any class implementing this MUST provide its own Validate() logic.
    public interface IPaymentValidator
    {
        bool Validate(decimal amount);
    }
}