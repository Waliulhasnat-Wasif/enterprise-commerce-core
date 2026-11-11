namespace EnterpriseCommerceCore.App.Step03_Polymorphism_And_Static
{
    public class BaseIdentity
    {
        // STATIC METHOD: resolved at COMPILE TIME based on the DECLARED type of the
        // reference you call it through. 
        // STATIC METHOD: Bound at COMPILE TIME to the CLASS itself, NOT an instance.
        // In C#, you MUST call it using the class name (e.g., ClassName.MethodName()); 
        // calling it through an instance reference is a compile-time error.
        // Because it belongs to the type layout and cannot be marked virtual, 
        // static methods CANNOT participate in runtime polymorphism.
        public static string Identify() => "Static: BaseIdentity (compile-time binding)";

        // INSTANCE + VIRTUAL: resolved at RUNTIME based on the ACTUAL object type,
        // regardless of what type the reference variable is declared as.
        public virtual string IdentifyInstance() => "Instance: BaseIdentity (runtime binding)";
    }

    public class DerivedIdentity : BaseIdentity
    {
        // 'new' here (not 'override') — static members can ONLY ever be hidden, never
        // overridden, because there's no object at runtime to dispatch on.
        public static new string Identify() => "Static: DerivedIdentity (still compile-time!)";

        // TRUE override — this is what real polymorphism looks like.
        public override string IdentifyInstance() => "Instance: DerivedIdentity (runtime override wins)";
    }
}