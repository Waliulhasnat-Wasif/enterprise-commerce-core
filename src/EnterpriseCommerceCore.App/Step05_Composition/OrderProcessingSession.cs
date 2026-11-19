using System;

namespace EnterpriseCommerceCore.App.Step05_Composition
{
    // A resource that needs explicit cleanup (imagine a file or a database connection).
    public sealed class AuditTrail : IDisposable
    {
        public void Record(string message) => Console.WriteLine($"  [Audit] {message}");
        public void Dispose() => Console.WriteLine("  [Audit] trail closed.");
    }

    // COMPOSITION + RESOURCES: the session CREATES and OWNS an AuditTrail, so it is
    // also responsible for cleaning it up. Rule: whoever owns a disposable part must
    // dispose it. That is how "part dies with the whole" is realized for real resources.
    public sealed class OrderProcessingSession : IDisposable
    {
        private readonly AuditTrail _audit = new(); // created INSIDE: owned

        public void Process(PurchaseOrder order)
        {
            _audit.Record($"Processing {order}");
        }

        public void Dispose()
        {
            _audit.Dispose(); // the owner closes its part
            Console.WriteLine("  [Session] closed.");
        }
    }
}