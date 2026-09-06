namespace EnterpriseCommerceCore.App.Step02_Abstraction
{
    // Implements NEITHER IRefundable nor ILoggable — proves interfaces are truly
    // optional add-ons, while the abstract class contract (TransactionFee,
    // ExecuteTransaction) is MANDATORY because it came from inheritance, not an interface.
    public class BankTransferPayment : PaymentMethod
    {
        public string BankAccountNumber { get; }

        public BankTransferPayment(decimal amount, string bankAccountNumber) : base(amount)
        {
            BankAccountNumber = bankAccountNumber;
        }

        public override decimal TransactionFee => 15m; // flat fee, no percentage

        protected override bool ExecuteTransaction()
        {
            Console.WriteLine($"  Initiating bank transfer from account {BankAccountNumber}...");
            return true;
        }
    }
}