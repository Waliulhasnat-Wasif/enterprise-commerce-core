namespace EnterpriseCommerceCore.App.Step02_Abstraction
{
    // DEFAULT INTERFACE METHOD (C# 8+ feature):
    // Interfaces can now carry a BODY for a method. Any class implementing ILoggable
    // gets this behavior for FREE — but can still override it if it wants something custom.
    // This blurs the old "interface = zero implementation" rule, which is exactly why
    // interviewers ask about it — know this is a C# 8+ addition, not classic OOP theory.
    public interface ILoggable
    {
        void LogAction(string action)
        {
            Console.WriteLine($"  [LOG - default] Action performed: {action}");
        }
    }
}