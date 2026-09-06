namespace EnterpriseCommerceCore.App.Step02_Abstraction
{
    // Two UNRELATED interfaces that happen to both need a method called "Notify"
    // — a totally realistic naming collision when combining third-party contracts.
    public interface ICustomerNotifier
    {
        void Notify(string message);
    }

    public interface ISystemAuditLogger
    {
        void Notify(string message);
    }

    // EXPLICIT INTERFACE IMPLEMENTATION: when one class implements two interfaces
    // with an identical method signature, you can implement each SEPARATELY by
    // prefixing the method with the interface name and dropping the access modifier.
    public class OrderEventPublisher : ICustomerNotifier, ISystemAuditLogger
    {
        // Explicit implementation — NOT accessible via a plain OrderEventPublisher
        // reference. Only reachable when the variable is TYPED as ICustomerNotifier.
        void ICustomerNotifier.Notify(string message)
        {
            Console.WriteLine($"  [Customer SMS] {message}");
        }

        // Same method name, completely separate implementation — only reachable
        // through an ISystemAuditLogger-typed reference.
        void ISystemAuditLogger.Notify(string message)
        {
            Console.WriteLine($"  [Audit Log] {message}");
        }

        // A normal PUBLIC method is unaffected by the above — it's a third, independent path.
        public void Notify(string message)
        {
            Console.WriteLine($"  [Default public Notify] {message}");
        }
    }
}