using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSale.Repository;

namespace PointOfSale.Pricing
{
    /// <summary>
    /// Abstract class for implementing multiple pricing strategies.
    /// </summary>
    public abstract class CompositePricingStrategy : ISalePricingStrategy
    {
        #region Members
        protected readonly List<ISalePricingStrategy> PricingStrategies = new List<ISalePricingStrategy>();
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the given pricing strategy to the overall composite strategy.
        /// </summary>
        /// <param name="strategy"></param>
        public void Add(ISalePricingStrategy strategy)
        {
            PricingStrategies.Add(strategy);
        }

        public abstract decimal GetTotal(Sale sale);
        #endregion
    }
}
