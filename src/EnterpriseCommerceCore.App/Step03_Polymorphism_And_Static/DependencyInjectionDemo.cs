using System;
using System.Collections.Generic;
using EnterpriseCommerceCore.App.Step02_Abstraction;

namespace EnterpriseCommerceCore.App.Step03_Polymorphism_And_Static
{
    // ============================================================
    // THE #1 CONFUSION BEGINNERS HAVE — clear this up before anything else:
    //
    // • Dependency Inversion Principle (DIP) — the "D" in SOLID (Phase 0). A DESIGN RULE:
    //   high-level modules should depend on abstractions, not concrete low-level modules.
    //
    // • Inversion of Control (IoC) — a broader GOAL: the responsibility for CREATING and
    //   WIRING objects is taken away from the class that uses them, and handed to
    //   something else (a container, a factory, or even just the calling code).
    //
    // • Dependency Injection (DI) — one specific TECHNIQUE for achieving IoC: a class's
    //   dependencies are handed ("injected") to it from outside, instead of the class
    //   creating them itself with 'new'.
    //
    // Memorize this sentence: "DIP is the RULE, IoC is the GOAL, DI is the MECHANISM."
    // ============================================================


    // ---------- 1. CONSTRUCTOR INJECTION (the most common, and the default choice) ----------
    public class CheckoutCart
    {
        // Depends on the ABSTRACT class, never a concrete one — this line IS the DIP rule
        // being applied. The DI technique below is what makes that rule enforceable.
        private readonly PaymentMethod _paymentMethod;

        // The dependency is MANDATORY and REQUIRED at the moment of creation — you literally
        // cannot construct a CheckoutCart without supplying one. Use constructor injection
        // whenever a dependency is non-optional.
        public CheckoutCart(PaymentMethod paymentMethod)
        {
            _paymentMethod = paymentMethod ?? throw new ArgumentNullException(nameof(paymentMethod));
        }

        public void Checkout()
        {
            Console.WriteLine("  [CheckoutCart] Initiating checkout...");
            Console.WriteLine("  " + _paymentMethod.ProcessPayment());
        }
    }


    // ---------- 2. PROPERTY (SETTER) INJECTION — for OPTIONAL dependencies ----------
    public class OrderNotifier
    {
        // Notice this is a settable PROPERTY, not a constructor parameter. Use this style
        // when a dependency is genuinely OPTIONAL, or when there's a sensible default
        // behavior without it — forcing it into the constructor would make every caller
        // supply something they might not have.
        public ILoggable? Logger { get; set; }

        public void NotifyOrderPlaced(string orderId)
        {
            Console.WriteLine($"  Order {orderId} placed.");
            // '?.' — only logs if someone actually injected a Logger; otherwise skipped safely.
            Logger?.LogAction($"Order {orderId} notification sent");
        }
    }


    // ---------- 3. METHOD INJECTION — dependency needed for ONE call, not the whole object ----------
    public class OrderReportExporter
    {
        // The dependency is passed directly into the method that needs it, for that single
        // call only — never stored as a field. Use this when DIFFERENT calls to the same
        // method might legitimately need DIFFERENT implementations of the same interface.
        public string ExportSummary(PaymentMethod payment, IPaymentValidator validatorToUse)
        {
            bool valid = validatorToUse.Validate(payment.Amount);
            return $"  Export -> {payment}, validated={valid} using {validatorToUse.GetType().Name}";
        }
    }


    // ---------- 4. WHY DI ACTUALLY MATTERS: a fake dependency, with zero mocking framework ----------
    public class FakePaymentForTesting : PaymentMethod
    {
        // This flag is the whole point — a test can check "did the code TRY to charge
        // something" without a single real gateway, network call, or dollar ever moving.
        public bool WasExecuted { get; private set; }

        public FakePaymentForTesting(decimal amount) : base(amount) { }

        public override decimal TransactionFee => 0m;

        protected override bool ExecuteTransaction()
        {
            WasExecuted = true; // no real card, no real PayPal call — completely safe to run 1000x
            return true;
        }
    }


    // ---------- 5. A "POOR MAN'S DI CONTAINER" — what ASP.NET Core's real DI container automates ----------
    public class SimplePaymentRegistry
    {
        private readonly Dictionary<string, Func<decimal, PaymentMethod>> _factories = new();

        // REGISTRATION: control over "how do I build a CreditCardPayment" is centralized
        // HERE, once — instead of every part of the app calling 'new CreditCardPayment(...)'
        // directly. This centralization IS Inversion of Control in miniature.
        public void Register(string key, Func<decimal, PaymentMethod> factory)
        {
            _factories[key] = factory;
        }

        // RESOLUTION: calling code asks for "a payment method for this key" without knowing
        // or caring which concrete class comes back. In Phase 1 (ASP.NET Core), this exact
        // idea reappears as builder.Services.AddScoped<IX, Y>() + constructor parameters —
        // this class is the hand-rolled version of that same mechanism.
        public PaymentMethod Resolve(string key, decimal amount)
        {
            if (!_factories.TryGetValue(key, out var factory))
                throw new InvalidOperationException($"No payment method registered for key '{key}'");
            return factory(amount);
        }
    }
}