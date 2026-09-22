using System;
using System.Collections.Generic;
using EnterpriseCommerceCore.App.Step02_Abstraction;

namespace EnterpriseCommerceCore.App.Step03_Polymorphism_And_Static
{
    public static class Step03Runner
    {
        public static void Run()
        {
            Console.WriteLine("\n========== STEP 03: POLYMORPHISM + STATIC + DEPENDENCY INJECTION ==========\n");

            Console.WriteLine("-- 1. Polymorphism via a mixed collection (Strategy-style) --");
            var payments = new List<PaymentMethod>
            {
                new CreditCardPayment(1200m, "4242"),
                new PaypalPayment(50m, "wasif@paypal.com"),
                new BankTransferPayment(50000m, "0123456789")
            };
            PaymentProcessorService.ProcessAll(payments);
            Console.WriteLine($"  Total processed so far: {PaymentProcessorService.TotalProcessed}\n");

            Console.WriteLine("-- 2. Polymorphism via interface-filtered collection --");
            var refundables = new List<IRefundable>();
            foreach (var p in payments)
            {
                if (p is IRefundable r) refundables.Add(r);
            }
            foreach (var refundable in refundables)
            {
                refundable.Refund(20m);
            }
            Console.WriteLine($"  {refundables.Count} of {payments.Count} payment methods supported refunds.\n");

            Console.WriteLine("-- 3. Static field shared across ALL instances --");
            var t1 = new TransactionCounter();
            var t2 = new TransactionCounter();
            var t3 = new TransactionCounter();
            Console.WriteLine($"  t1.Id = {t1.InstanceId}, t2.Id = {t2.InstanceId}, t3.Id = {t3.InstanceId}");
            Console.WriteLine($"  TransactionCounter.InstanceCount (shared) = {TransactionCounter.InstanceCount}\n");

            Console.WriteLine("-- 4. const vs static readonly --");
            Console.WriteLine("  'const' is baked in at compile time. 'static readonly' is computed at RUNTIME");
            Console.WriteLine("  (proven earlier — ServiceStartedAt came from DateTime.UtcNow inside a static ctor).\n");

            Console.WriteLine("-- 5. Static vs Instance: compile-time vs runtime binding (THE trap) --");
            BaseIdentity baseRef = new DerivedIdentity();
            Console.WriteLine("  " + BaseIdentity.Identify());
            Console.WriteLine("  " + DerivedIdentity.Identify());
            // Note: in C# (unlike some languages), calling a static method THROUGH an
            // instance reference (e.g. baseRef.Identify()) is a compile-time error — you
            // are forced to use the class name, which is itself a hint that static members
            // don't belong to any particular object.
            Console.WriteLine("  " + baseRef.IdentifyInstance());
            Console.WriteLine();

            Console.WriteLine("-- 6. Dependency Injection — the real-world payoff of Abstraction + Polymorphism --\n");

            Console.WriteLine("  6a. Constructor Injection (mandatory dependency)");
            var cart1 = new CheckoutCart(new CreditCardPayment(500m, "1234"));
            var cart2 = new CheckoutCart(new PaypalPayment(20m, "test@paypal.com"));
            cart1.Checkout();
            cart2.Checkout();
            Console.WriteLine("  ^ CheckoutCart never wrote 'new CreditCardPayment' itself — it was HANDED one.\n");

            Console.WriteLine("  6b. Property Injection (optional dependency)");
            var notifierWithLogging = new OrderNotifier { Logger = new CreditCardPayment(0m, "0000") as ILoggable };
            var notifierWithoutLogging = new OrderNotifier(); // Logger left null — no crash, no forced dependency
            notifierWithLogging.NotifyOrderPlaced("ORD-1001");
            notifierWithoutLogging.NotifyOrderPlaced("ORD-1002");
            Console.WriteLine();

            Console.WriteLine("  6c. Method Injection (dependency scoped to a single call)");
            var exporter = new OrderReportExporter();
            Console.WriteLine(exporter.ExportSummary(new BankTransferPayment(300m, "999888"), new BankTransferPayment(0m, "x")));
            Console.WriteLine();

            Console.WriteLine("  6d. Why this matters: testing WITHOUT touching a real gateway");
            var fake = new FakePaymentForTesting(100m);
            var testCart = new CheckoutCart(fake); // DI lets us swap in a FAKE with zero code changes to CheckoutCart
            testCart.Checkout();
            Console.WriteLine($"  Did the fake gateway get triggered? {fake.WasExecuted}");
            Console.WriteLine("  ^ This exact swap-in is what Moq will automate for you from Phase 3 onward.\n");

            Console.WriteLine("  6e. A hand-rolled 'DI container' — what ASP.NET Core automates in Phase 1");
            var registry = new SimplePaymentRegistry();
            registry.Register("card", amount => new CreditCardPayment(amount, "5555"));
            registry.Register("paypal", amount => new PaypalPayment(amount, "resolved@paypal.com"));

            var resolvedCard = registry.Resolve("card", 750m);
            Console.WriteLine("  " + resolvedCard.ProcessPayment());
            Console.WriteLine("  ^ Calling code asked for \"card\" and never wrote 'new CreditCardPayment' directly.\n");

            Console.WriteLine("========== END OF STEP 03 ==========\n");
        }
    }
}