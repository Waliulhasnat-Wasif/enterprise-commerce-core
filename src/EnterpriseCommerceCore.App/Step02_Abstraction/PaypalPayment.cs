namespace EnterpriseCommerceCore.App.Step02_Abstraction
{
    // Implements only IRefundable — deliberately NOT ILoggable, to prove interface
    // adoption is selective per class, unlike inheritance which is all-or-nothing.
    public class PaypalPayment : PaymentMethod, IRefundable
    {
        public string PaypalEmail { get; }

        public PaypalPayment(decimal amount, string paypalEmail) : base(amount)
        {
            PaypalEmail = paypalEmail;
        }

        public override decimal TransactionFee => Amount * 0.034m + 0.30m; // Paypal-style fee

        protected override bool ExecuteTransaction()
        {
            Console.WriteLine($"  Redirecting to PayPal for {PaypalEmail}...");
            return true;
        }

        public bool Refund(decimal amount)
        {
            Console.WriteLine($"  Refunding {amount:C} via PayPal to {PaypalEmail}");
            return true;
        }

        // CUSTOM VALIDATION OVERRIDE: PayPal has its own rule — proves virtual members
        // in an abstract class can be selectively overridden per child.
        public override bool Validate(decimal amount)
        {
            bool baseValid = base.Validate(amount); // reuse parent's rule, then add to it
            return baseValid && amount >= 5m;        // PayPal requires a $5 minimum
        }
    }
}