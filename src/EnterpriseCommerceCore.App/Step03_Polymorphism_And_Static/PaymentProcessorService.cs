using System;
using System.Collections.Generic;
using EnterpriseCommerceCore.App.Step02_Abstraction;

namespace EnterpriseCommerceCore.App.Step03_Polymorphism_And_Static
{
    // STATIC CLASS: cannot be instantiated ("new PaymentProcessorService()" won't compile),
    // cannot be inherited, cannot inherit anything, cannot implement an interface.
    // Every member inside MUST be static — there's no such thing as an "instance" of this class.
    public static class PaymentProcessorService
    {
        // STATIC FIELD: belongs to the CLASS itself, not to any object. One shared copy,
        // no matter how many times ProcessAll() is called.
        private static int _totalProcessed;

        // STATIC READONLY: value is set once (here, inside the static constructor) and then
        // locked — but unlike 'const', it CAN be computed at runtime (DateTime.UtcNow is not
        // known at compile time, so it could never be a const).
        private static readonly DateTime ServiceStartedAt;

        // CONST: must be a compile-time constant (numbers, strings, bool) — the compiler bakes
        // this value directly into every place it's used. It's implicitly static; you never
        // write "static" on a const, and you can never change it at runtime.
        private const decimal MaxSingleTransaction = 1_000_000m;

        // STATIC CONSTRUCTOR: runs AUTOMATICALLY, EXACTLY ONCE, the first time this class is
        // touched by any code — before the first field access or method call. You never call
        // it yourself, it takes no parameters, and you can't overload it.
        static PaymentProcessorService()
        {
            ServiceStartedAt = DateTime.UtcNow;
            Console.WriteLine($"  [Static Ctor fired ONCE] Service initialized at {ServiceStartedAt:T}");
        }

        // STATIC METHOD operating on a POLYMORPHIC collection — this is the bridge between
        // the two topics of this step. The method itself is static (called via class name,
        // no object needed), but the loop inside triggers real runtime polymorphism.
        public static void ProcessAll(List<PaymentMethod> payments)
        {
            foreach (var payment in payments)
            {
                if (payment.Amount > MaxSingleTransaction)
                {
                    Console.WriteLine("  Skipped — exceeds max transaction limit.");
                    continue;
                }

                // POLYMORPHISM: 'payment' is typed as the abstract PaymentMethod, but
                // ProcessPayment() internally calls ExecuteTransaction() and TransactionFee —
                // and at RUNTIME, C# picks whichever CreditCard/Paypal/BankTransfer version
                // actually applies. The static method doesn't know or care which one.
                Console.WriteLine(payment.ProcessPayment());
                _totalProcessed++;
            }

            // STATIC LOCAL FUNCTION (C# 7+): a function defined and used ONLY inside this
            // method, that explicitly cannot capture/modify variables from the outer method
            // (that's what 'static' means here — a compiler-enforced guarantee of no hidden
            // state capture). Good for small helpers you don't want polluting the class.
            static string FormatCount(int count) => count == 1 ? "1 payment" : $"{count} payments";

            Console.WriteLine($"  Batch complete — processed {FormatCount(payments.Count)} this run.");
        }

        // STATIC PROPERTY: read from anywhere via PaymentProcessorService.TotalProcessed,
        // no object required. Private setter equivalent here — only this class increments it.
        public static int TotalProcessed => _totalProcessed;
    }
}