namespace EnterpriseCommerceCore.App.Step02_Abstraction
{
    // Implements TWO interfaces (IRefundable, ILoggable) on top of the abstract base —
    // proof that a class can inherit ONE class but implement MULTIPLE interfaces.
    public class CreditCardPayment : PaymentMethod, IRefundable, ILoggable
    {
        public string CardLastFourDigits { get; }

        public CreditCardPayment(decimal amount, string cardLastFourDigits) : base(amount)
        {
            CardLastFourDigits = cardLastFourDigits;
        }

        public override decimal TransactionFee => Amount * 0.025m; // 2.5% card processing fee

        // SEALED OVERRIDE on an abstract member: allowed because PaymentMethod's
        // ExecuteTransaction was abstract (implicitly "overridable"). Sealing it here
        // means nothing that further inherits CreditCardPayment can change this logic.
        protected sealed override bool ExecuteTransaction()
        {
            Console.WriteLine($"  Charging card ending in {CardLastFourDigits}...");
            return true;
        }

        public bool Refund(decimal amount)
        {
            Console.WriteLine($"  Refunding {amount:C} to card ending in {CardLastFourDigits}");
            return true;
        }

        // Overriding the interface's DEFAULT method with custom behavior —
        // proves default interface methods are opt-in, not forced.
        public void LogAction(string action)
        {
            Console.WriteLine($"  [LOG - CreditCard override] {action} for card ...{CardLastFourDigits}");
        }
    }
}