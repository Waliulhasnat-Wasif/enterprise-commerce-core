namespace EnterpriseCommerceCore.App.Step03_Polymorphism_And_Static
{
    public class BaseIdentity
    {
        // STATIC METHOD: resolved at COMPILE TIME based on the DECLARED type of the
        // reference you call it through. Static methods CANNOT be marked virtual,
        // and therefore CANNOT truly participate in polymorphism.
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