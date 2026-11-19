using System;
using EnterpriseCommerceCore.App.Step01_Inheritance;
using EnterpriseCommerceCore.App.Step04_Association;

namespace EnterpriseCommerceCore.App.Step05_Composition
{
    public static class Step05Runner
    {
        public static void Run()
        {
            Console.WriteLine("\n========== STEP 05: COMPOSITION, A TO Z ==========\n");

            var customer = new Customer("Wasif", "wasif@example.com", 120);
            var laptop = new Product("Laptop", 1200m);
            var phone = new Product("Phone", 800m);

            Console.WriteLine("-- 1. The whole builds its own parts --");
            var order = new PurchaseOrder(customer);
            order.AddItem(laptop, 1);
            order.AddItem(phone, 2);
            foreach (var line in order.Lines)
                Console.WriteLine($"  {line.Product.Name} x{line.Quantity} @ {line.UnitPrice:N2} = {line.LineTotal:N2}");
            Console.WriteLine($"  {order}\n");
            // new PurchaseOrder.OrderLine(...)  // does NOT compile: the class is private to PurchaseOrder

            Console.WriteLine("-- 2. Same product added again: the whole merges the line --");
            order.AddItem(phone, 1);
            Console.WriteLine($"  Lines now: {order.Lines.Count} (phone quantity became 3)");
            Console.WriteLine($"  {order}\n");

            Console.WriteLine("-- 3. Parts are protected from outside tampering --");
            try { ((System.Collections.IList)order.Lines).Add(new object()); }
            catch (NotSupportedException) { Console.WriteLine("  Sneaky cast + Add blocked: the collection is read-only."); }
            // order.Lines[0].Quantity = 99;  // does NOT compile: IOrderLine exposes no setter
            Console.WriteLine();

            Console.WriteLine("-- 4. Invariants guarded by the whole --");
            try { order.AddItem(phone, 0); }
            catch (ArgumentOutOfRangeException ex) { Console.WriteLine($"  Rejected: {ex.ParamName} must be positive."); }

            try { order.Place(); }
            catch (InvalidOperationException ex) { Console.WriteLine($"  Rejected: {ex.Message}"); }

            var address = new Address("12 Road 5", "Dhaka", "1212", "Bangladesh");
            order.SetShippingAddress(address);
            order.Place();
            Console.WriteLine($"  Placed OK -> {order}");

            try { order.AddItem(laptop, 1); }
            catch (InvalidOperationException ex) { Console.WriteLine($"  Rejected: {ex.Message}"); }
            Console.WriteLine();

            Console.WriteLine("-- 5. Value object equality (record) --");
            var a1 = new Address("12 Road 5", "Dhaka", "1212", "Bangladesh");
            var a2 = new Address("12 Road 5", "Dhaka", "1212", "Bangladesh");
            Console.WriteLine($"  a1 == a2 (same values)?        {a1 == a2}");
            Console.WriteLine($"  ReferenceEquals(a1, a2)?      {ReferenceEquals(a1, a2)}");
            var moved = a1 with { City = "Chattogram" }; // new copy, original untouched
            Console.WriteLine($"  'with' made a changed COPY: {moved.City}; original still {a1.City}\n");

            Console.WriteLine("-- 6. Lifecycle: what dies with the order, what survives --");
            Console.WriteLine("  If this order is discarded, its lines go with it (nobody else could hold them).");
            Console.WriteLine($"  Customer survives: {customer}");
            Console.WriteLine($"  Products survive : {laptop}, {phone}\n");

            Console.WriteLine("-- 7. Same picture, three relationship types --");
            Console.WriteLine("  PurchaseOrder -> Customer   : ASSOCIATION  (customer is independent)");
            Console.WriteLine("  Category      -> Products   : AGGREGATION  (Step 04, shared parts)");
            Console.WriteLine("  PurchaseOrder -> OrderLine  : COMPOSITION  (owned, dies with the order)\n");

            Console.WriteLine("-- 8. Composition over Inheritance --");
            var plan = new DeliveryPlan(new ExpressSpeed()).With(new GiftWrap()).With(new Insurance());
            Console.WriteLine($"  {plan.Describe()} -> fee {plan.TotalFee():N2}");
            var cheapPlan = new DeliveryPlan(new StandardSpeed());
            Console.WriteLine($"  {cheapPlan.Describe()} -> fee {cheapPlan.TotalFee():N2}");
            Console.WriteLine("  ^ Express+Gift+Insurance needed ZERO new classes. Inheritance would need one per combination.\n");

            Console.WriteLine("-- 9. Composition + IDisposable (owner cleans up its part) --");
            using (var session = new OrderProcessingSession())
            {
                session.Process(order);
            } // Dispose() runs here automatically
            Console.WriteLine("  ^ The session closed its AuditTrail for us the moment the using block ended.\n");

            Console.WriteLine("========== END OF STEP 05 ==========\n");
        }
    }
}