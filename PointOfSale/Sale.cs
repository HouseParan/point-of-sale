using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSale.Pricing;
using PointOfSale.Repository;

namespace PointOfSale
{
    public class Sale 
    {
        #region Members
        /// <summary>
        /// Gets the list of items that the customer wishes to purchase.
        /// </summary>
        public List<SalesLineItem> LineItems { get; } = new List<SalesLineItem>();

        /// <summary>
        /// The pricing strategy to use to calculate the final total of the <see cref="Sale"/>.
        /// </summary>
        private readonly ISalePricingStrategy _pricingStrategy;
        #endregion

        #region Constructors
        public Sale() : this(new SystemFileManager())
        {
            
        }

        public Sale(IFileManager fileManager)
        {
            _pricingStrategy = new PricingStrategyFactory(fileManager).GetSalePricingStrategy();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the final total for the current <see cref="Sale"/>.
        /// </summary>
        /// <returns></returns>
        public decimal GetTotal()
        {
            return _pricingStrategy.GetTotal(this);
        }

        /// <summary>
        /// Adds the line item directly to the current <see cref="Sale"/>.
        /// </summary>
        /// <param name="lineItem"></param>
        public void AddLineItem(SalesLineItem lineItem)
        {
            if (lineItem != null)
                LineItems.Add(lineItem);
        }

        /// <summary>
        /// <para>Creates a new line item for the given product if one does not already exit and adds it to the current <see cref="Sale"/>.</para>
        /// <para>Otherwise, the existing line item's quantity is updated.</para>
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        public void MakeLineItem(Product product, int quantity)
        {
            if (product == null)
                return;

            var lineItem = new SalesLineItem(product, quantity);
            var existingLineItem = LineItems.SingleOrDefault(i => i.Equals(lineItem));

            if (existingLineItem == null)
                LineItems.Add(lineItem);
            else
                existingLineItem.AddQuantity(lineItem.Quantity);
        }

        /// <summary>
        /// Removes all line items from the <see cref="Sale"/> that have quantity equal to 0.
        /// </summary>
        public void RemoveZeroQuantityLineItems()
        {
            LineItems.RemoveAll(li => li.Quantity == 0);
        }
        #endregion
    }
}
