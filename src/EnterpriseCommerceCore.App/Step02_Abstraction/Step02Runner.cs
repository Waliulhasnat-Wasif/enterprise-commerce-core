using System;

namespace EnterpriseCommerceCore.App.Step02_Abstraction
{
    public static class Step02Runner
    {
        public static void Run()
        {
            Console.WriteLine("\n========== STEP 02: ABSTRACTION — FULL A TO Z ==========\n");

            // var x = new PaymentMethod(100m);
            // ^ Would NOT compile — you cannot instantiate an abstract class directly.
            // This line existing at all is the proof; keep it commented and read the error
            // in your head: "Cannot create an instance of the abstract type PaymentMethod."

            Console.WriteLine("-- 1. Template Method Pattern in action --");
            var card = new CreditCardPayment(2500m, "4242");
            var paypal = new PaypalPayment(3m, "wasif@paypal.com"); // deliberately below PayPal's $5 minimum
            var bank = new BankTransferPayment(10000m, "0123456789");

            Console.WriteLine(card.ProcessPayment());
            Console.WriteLine(paypal.ProcessPayment()); // will show validation FAILURE — proves override works
            Console.WriteLine(bank.ProcessPayment());
            Console.WriteLine();

            Console.WriteLine("-- 2. Abstract property forces different fee logic per type --");
            Console.WriteLine($"Card fee (2.5%)     : {card.TransactionFee:C}");
            Console.WriteLine($"PayPal fee (3.4%+30c): {paypal.TransactionFee:C}");
            Console.WriteLine($"Bank fee (flat)     : {bank.TransactionFee:C}\n");

            Console.WriteLine("-- 3. Interfaces are selective, unlike inheritance --");
            if (card is IRefundable cardRefund) cardRefund.Refund(500m);
            if (paypal is IRefundable paypalRefund) paypalRefund.Refund(3m);
            // bank does NOT implement IRefundable — this 'is' check will simply be false:
            Console.WriteLine($"Is BankTransferPayment refundable? {bank is IRefundable}\n");

            Console.WriteLine("-- 4. Default Interface Method vs Overridden --");
            ILoggable cardLogger = card;       // has its own override
            cardLogger.LogAction("Charged");
            ILoggable defaultLogger = new BankAccountLoggerStub();
            defaultLogger.LogAction("Transferred"); // uses ILoggable's default body — no override provided
            Console.WriteLine();

            Console.WriteLine("-- 5. Explicit Interface Implementation (interview trap) --");
            var publisher = new OrderEventPublisher();
            publisher.Notify("Plain public call");                 // the normal public method
            ((ICustomerNotifier)publisher).Notify("Your order shipped!");   // must cast to reach this one
            ((ISystemAuditLogger)publisher).Notify("Order Number 1234 shipped"); // different implementation, same method name
            Console.WriteLine("^ Same object, three different Notify() behaviors depending on the reference type.\n");

            Console.WriteLine("========== END OF STEP 02 ==========\n");
        }
    }

    // Minimal stub just to prove ILoggable's DEFAULT method works without any override.
    internal class BankAccountLoggerStub : ILoggable { }
}