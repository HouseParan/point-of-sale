using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.Pricing
{
    public class BestForStorePricingStrategy : CompositePricingStrategy
    {
        /// <summary>
        /// Gets the highest price from all available strategies.
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public override decimal GetTotal(Sale sale)
        {
            return PricingStrategies.Select(strategy => strategy.GetTotal(sale)).Concat(new[] { decimal.MinValue }).Max();
        }
    }
}
