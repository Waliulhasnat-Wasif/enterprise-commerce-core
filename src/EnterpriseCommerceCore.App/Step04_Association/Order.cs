using System;
using EnterpriseCommerceCore.App.Step01_Inheritance;
using EnterpriseCommerceCore.App.Step02_Abstraction;

namespace EnterpriseCommerceCore.App.Step04_Association
{
    public class Order
    {
        public Guid OrderId { get; } = Guid.NewGuid();

        // ASSOCIATION (many-to-one, UNIDIRECTIONAL): many Orders can point to ONE Customer.
        // Order knows its Customer, but Customer has no idea which Orders point at it.
        // The Customer is created OUTSIDE and handed in: Order does not own it.
        public Customer Customer { get; }

        // ASSOCIATION (one-to-one, optional "0..1"): an order may not have a payment yet,
        // hence nullable. The PaymentMethod (Step 02) lives its own life independent of Order.
        public PaymentMethod? Payment { get; private set; }

        public Order(Customer customer)
        {
            // An Order without a Customer makes no sense, so we reject null immediately.
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        }

        public void AttachPayment(PaymentMethod payment)
        {
            Payment = payment ?? throw new ArgumentNullException(nameof(payment));
        }

        public override string ToString() => $"Order[{OrderId.ToString()[..8]}] for {Customer.Name}";
    }
}