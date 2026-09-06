using System;

namespace EnterpriseCommerceCore.App.Step02_Abstraction
{
    // ABSTRACT CLASS: partial implementation + contract, for CLOSELY RELATED types.
    // You CANNOT do "new PaymentMethod(...)" anywhere — try it, it won't compile.
    // That restriction is the whole point: PaymentMethod only makes sense through
    // a concrete child (CreditCard, Paypal, etc).
    public abstract class PaymentMethod : IPaymentValidator
    {
        public decimal Amount { get; protected set; }
        public Guid TransactionId { get; init; }

        // ABSTRACT PROPERTY: no body at all. Every derived class MUST supply its own
        // TransactionFee logic. Different from a virtual property, which CAN supply
        // a default that children are free to ignore.
        public abstract decimal TransactionFee { get; }

        protected PaymentMethod(decimal amount)
        {
            Amount = amount;
            TransactionId = Guid.NewGuid();
        }

        // ABSTRACT METHOD: no body. Forces every derived class to define HOW payment
        // actually happens — PaymentMethod itself has no opinion on that.
        protected abstract bool ExecuteTransaction();

        // Implements the interface contract with a concrete, shared default —
        // derived classes CAN override this (it's virtual) if they need custom rules.
        public virtual bool Validate(decimal amount)
        {
            return amount > 0 && amount <= 500000m;
        }

        // ============================================================
        // TEMPLATE METHOD PATTERN — the real-world reason abstraction exists.
        // ProcessPayment() defines the FIXED skeleton of "how a payment happens":
        // validate -> execute -> receipt. The ORDER never changes. Only the
        // abstract steps (ExecuteTransaction, TransactionFee) change per payment type.
        // This is a concrete, NON-abstract method — children don't override this part.
        // ============================================================
        public string ProcessPayment()
        {
            if (!Validate(Amount))
                return "❌ Payment rejected — failed validation.";

            bool success = ExecuteTransaction();
            if (!success)
                return "❌ Payment failed during execution.";

            return GetReceipt();
        }

        // Concrete helper — uses the abstract TransactionFee, proving abstract members
        // can still be consumed by ordinary methods on the base class.
        protected string GetReceipt()
        {
            decimal total = Amount + TransactionFee;
            return $"✅ Receipt [{TransactionId}] — Amount: {Amount:C}, Fee: {TransactionFee:C}, Total: {total:C}";
        }

        public override string ToString() => $"{GetType().Name}[{TransactionId}]";
    }
}