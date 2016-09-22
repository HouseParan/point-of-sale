using PointOfSale.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale
{
    /// <summary>
    /// Simple aggregate of promotion(s) that are defined by the marketing team.
    /// </summary>
    public class PromotionCatalog
    {
        #region Members
        /// <summary>
        /// Gets/sets the name of the overall, composite strategy to use when pricing a <see cref="Sale"/>. 
        /// </summary>
        public string OverallStrategy = "BestForCustomer";

        public List<SalesLineItemDiscount> SalesLineItemDiscounts = new List<SalesLineItemDiscount>();
        #endregion

        #region Public Methods
        public void RemoveInapplicablePromotions(DateTime now)
        {
            SalesLineItemDiscounts.RemoveAll(discount => !discount.IsApplicable(now));
        }

        public void RemoveInvalidPromotions()
        {
            SalesLineItemDiscounts.RemoveAll(discount => !discount.IsValid());
        }
        #endregion
    }
}
