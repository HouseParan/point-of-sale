using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSale.Repository;

namespace PointOfSale.Pricing
{
    public class BestForCustomerPricingStrategy : CompositePricingStrategy
    {
        /// <summary>
        /// Gets the lowest total price from all the available strategies.
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public override decimal GetTotal(Sale sale)
        {
            return PricingStrategies.Select(strategy => strategy.GetTotal(sale)).Concat(new[] {decimal.MaxValue}).Min();
        }
    }
}
