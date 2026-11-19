using System.Collections.Generic;
using System.Linq;

namespace EnterpriseCommerceCore.App.Step05_Composition
{
    // ---------- THE PROBLEM: inheritance explodes with every new option ----------
    // 2 speeds x 2 extras already needs 4 classes. Add "Insured" and it becomes 8.
    // N independent options need 2^N classes. Impossible to maintain.
    public class StandardDelivery { public virtual decimal Fee() => 60m; }
    public class ExpressDelivery : StandardDelivery { public override decimal Fee() => 150m; }
    public class GiftStandardDelivery : StandardDelivery { public override decimal Fee() => base.Fee() + 30m; }
    public class GiftExpressDelivery : ExpressDelivery { public override decimal Fee() => base.Fee() + 30m; }
    // ... and still no "Insured" version of any of them.


    // ---------- THE SOLUTION: small parts, assembled by composition ----------
    public interface IDeliverySpeed { string Name { get; } decimal Fee(); }
    public interface IDeliveryExtra { string Name { get; } decimal Fee(); }

    public class StandardSpeed : IDeliverySpeed { public string Name => "Standard"; public decimal Fee() => 60m; }
    public class ExpressSpeed : IDeliverySpeed { public string Name => "Express"; public decimal Fee() => 150m; }

    public class GiftWrap : IDeliveryExtra { public string Name => "Gift wrap"; public decimal Fee() => 30m; }
    public class Insurance : IDeliveryExtra { public string Name => "Insurance"; public decimal Fee() => 45m; }

    // The plan is COMPOSED of one speed plus any extras. New option = ONE new small class,
    // no explosion, no changes to DeliveryPlan (Open/Closed in action).
    public class DeliveryPlan
    {
        private readonly IDeliverySpeed _speed;
        private readonly List<IDeliveryExtra> _extras = new();

        public DeliveryPlan(IDeliverySpeed speed) => _speed = speed;

        public DeliveryPlan With(IDeliveryExtra extra)
        {
            _extras.Add(extra);
            return this; // allows chaining: plan.With(a).With(b)
        }

        public decimal TotalFee() => _speed.Fee() + _extras.Sum(e => e.Fee());

        public string Describe() =>
            _speed.Name + (_extras.Count > 0 ? " + " + string.Join(" + ", _extras.Select(e => e.Name)) : "");
    }
}