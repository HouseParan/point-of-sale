using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSale.Repository;

namespace PointOfSale.Pricing
{
    /// <summary>
    /// Factory for constructing pricing strategies. 
    /// <para>Ordinarily, the singleton pattern is recommended in this instance, but it is really hard to unit test and 
    /// this scenario requires resources to be reconstructed every time anyway.</para>
    /// </summary>
    public class PricingStrategyFactory
    {
        #region Members
        private readonly IFileManager _fileManager;
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileManager"></param>
        public PricingStrategyFactory(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public PricingStrategyFactory() : this(new SystemFileManager())
        {
            
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Build and return the currently applicable sales pricing strategy.
        /// </summary>
        /// <returns></returns>
        public ISalePricingStrategy GetSalePricingStrategy()
        {
            // Always (re)read in all promotions.
            // TODO: make location configurable.
            var promotions = new PointOfSaleFileReader(_fileManager).ReadPromotionCatalog("PromotionCatalog.json");

            // Construct a composite pricing strategy.
            CompositePricingStrategy overallStrategy;

            // TODO: could use reflection in the future to construct class instances
            if (promotions.OverallStrategy.Equals("BestForCustomer"))
                overallStrategy = new BestForCustomerPricingStrategy();
            else if (promotions.OverallStrategy.Equals("BestForStore"))
                overallStrategy = new BestForStorePricingStrategy();
            else
                throw new InvalidOperationException(
                    $"'{promotions.OverallStrategy}' is an unknown pricing strategy, " +
                    "supported values are 'BestForCustomer' or 'BestForStore'.");
            
            // Must always add Line Item strategies first which mutates the sale with line item discounts.
            // However, in the future, the 'BestForStore' strategy may want to apply these after any transactional discounts instead.
            overallStrategy.Add(new LineItemBuyNGetOnePricingStrategy(promotions.SalesLineItemDiscounts));
            overallStrategy.Add(new LineItemQuantityPricingStrategy(promotions.SalesLineItemDiscounts));

            // Add other (probably transactional) strategies as required here.

            return overallStrategy;
        }

        #endregion
    }
}
