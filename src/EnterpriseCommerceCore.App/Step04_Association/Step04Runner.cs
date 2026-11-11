using System;
using System.Collections.Generic;
using EnterpriseCommerceCore.App.Step01_Inheritance;
using EnterpriseCommerceCore.App.Step02_Abstraction;

namespace EnterpriseCommerceCore.App.Step04_Association
{
    public static class Step04Runner
    {
        public static void Run()
        {
            Console.WriteLine("\n========== STEP 04: ASSOCIATION, A TO Z ==========\n");

            Console.WriteLine("-- 1. Many-to-One: two orders share ONE customer object --");
            var wasif = new Customer("Wasif", "wasif@example.com", 120);
            var order1 = new Order(wasif);
            var order2 = new Order(wasif);
            Console.WriteLine("  " + order1);
            Console.WriteLine("  " + order2);
            Console.WriteLine($"  Same Customer object? {ReferenceEquals(order1.Customer, order2.Customer)}");
            wasif.AddLoyaltyPoints(30, "order bonus");
            Console.WriteLine($"  Via order1: {order1.Customer.LoyaltyPoints} pts | via order2: {order2.Customer.LoyaltyPoints} pts");
            Console.WriteLine("  ^ One shared object: change it once, every order sees it.\n");

            Console.WriteLine("-- 2. Unidirectional: Order knows Customer, not the reverse --");
            // wasif.Orders;  // does NOT exist: Customer has no idea which orders point at it.
            Console.WriteLine("  order1.Customer works, but Customer has no Orders property.\n");

            Console.WriteLine("-- 3. Independent lifecycle --");
            var tempOrders = new List<Order> { new Order(wasif), new Order(wasif) };
            tempOrders.Clear(); // these orders are gone...
            Console.WriteLine($"  Orders discarded. Customer still alive: {wasif}\n");

            Console.WriteLine("-- 4. One-to-One (optional): Order and PaymentMethod --");
            var order = new Order(wasif);
            var printer = new InvoicePrinter();
            Console.WriteLine(printer.Print(order)); // payment pending

            var card = new CreditCardPayment(800m, "4242"); // created OUTSIDE, independent
            order.AttachPayment(card);
            Console.WriteLine(printer.Print(order));
            Console.WriteLine($"  Same payment object? {ReferenceEquals(order.Payment, card)}\n");

            Console.WriteLine("-- 5. Many-to-Many, Bidirectional (kept in sync) --");
            var laptop = new Product("Laptop", 1200m);
            var phone = new Product("Phone", 800m);
            var techCorp = new Supplier("TechCorp");
            var globalTrade = new Supplier("GlobalTrade");

            laptop.AddSupplier(techCorp);     // from the Product side
            laptop.AddSupplier(globalTrade);
            techCorp.AddProduct(phone);       // from the Supplier side

            Console.WriteLine($"  Laptop suppliers : {string.Join(", ", laptop.Suppliers)}");
            Console.WriteLine($"  Phone suppliers  : {string.Join(", ", phone.Suppliers)}");
            Console.WriteLine($"  TechCorp products: {string.Join(", ", techCorp.Products)}");
            Console.WriteLine("  ^ We never called phone.AddSupplier(...), yet it knows TechCorp. Sync worked.\n");

            Console.WriteLine("-- 6. Aggregation: parts outlive and are shared across wholes --");
            var electronics = new Category("Electronics");
            var featured = new Category("Featured");
            electronics.Add(laptop);
            electronics.Add(phone);
            featured.Add(laptop); // the SAME laptop in two categories (shared)
            Console.WriteLine($"  {electronics} | {featured}");

            featured.Remove(laptop);
            Console.WriteLine($"  Removed laptop from Featured. Still in Electronics? {electronics.Products.Contains(laptop)}");

            Category? tempCategory = new Category("Temp");
            tempCategory.Add(laptop);
            tempCategory = null; // the whole category is dropped...
            Console.WriteLine($"  Temp category dropped. Laptop still usable: {laptop} at {laptop.Price:C}\n");

            Console.WriteLine("-- 7. Dependency vs Association --");
            Console.WriteLine("  InvoicePrinter used an Order only inside Print(). No field = Dependency.");
            Console.WriteLine("  Order keeps a Customer in a property for its whole life = Association.\n");

            Console.WriteLine("========== END OF STEP 04 ==========\n");
        }
    }
}