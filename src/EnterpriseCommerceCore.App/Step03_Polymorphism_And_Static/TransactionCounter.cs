namespace EnterpriseCommerceCore.App.Step03_Polymorphism_And_Static
{
    // A normal (non-static) class that USES a static field internally —
    // this is the classic beginner-clarifying example: "how many objects have I made so far?"
    public class TransactionCounter
    {
        // STATIC FIELD: exactly ONE copy exists, shared by every TransactionCounter object
        // ever created — NOT one copy per object.
        private static int _instanceCount = 0;

        // STATIC PROPERTY: also shared, read via TransactionCounter.InstanceCount
        public static int InstanceCount => _instanceCount;

        // INSTANCE PROPERTY: this one IS per-object — every object gets its own InstanceId.
        public int InstanceId { get; }

        public TransactionCounter()
        {
            _instanceCount++;          // shared counter goes up
            InstanceId = _instanceCount; // this object's own personal snapshot of that count
        }
    }
}