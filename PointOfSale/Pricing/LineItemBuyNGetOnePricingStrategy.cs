using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.Pricing
{
    /// <summary>
    /// Applies 'Buy N Get One' type discounts to the <see cref="Sale"/>.
    /// </summary>
    public class LineItemBuyNGetOnePricingStrategy : ISalePricingStrategy
    {
        #region Members
        private readonly List<SalesLineItemDiscount> _discounts;
        #endregion

        #region Constructors
        public LineItemBuyNGetOnePricingStrategy(List<SalesLineItemDiscount> discounts)
        {
            _discounts = discounts;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Mutates the sale with 'Buy N Get One' type discounts and gets the total.
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public decimal GetTotal(Sale sale)
        {
            var discountedLineItems = new List<SalesLineItem>();
            var originalLineItems = new List<SalesLineItem>();

            foreach (var lineItem in sale.LineItems)
            {
                // Apply discounts directly to line item, if any.

                foreach (
                    var discount in
                    _discounts
                        .Where(d => d.IsValid() && d.DiscountAppliedOnNextProduct)
                        .Where(d => d.ProductId == lineItem.ProductId && lineItem.Quantity > d.ThresholdQuantity)
                        .OrderByDescending(d => d.ThresholdQuantity))
                {
                    // There may be eligible discount(s) to apply to current line item based on quantity.
                    // Apply the discounts with the largest quantities first as they generally have the better discounts.

                    while (lineItem.Quantity > discount.ThresholdQuantity)
                    {
                        // Create a separate line item that receives the discount.
                        var discountedLineItem =
                            new SalesLineItem(new Product(lineItem.ProductId, lineItem.ProductPrice), 1);
                        discountedLineItem.SetDiscount(discount.Discount);

                        // Save to temporary list.
                        discountedLineItems.Add(discountedLineItem);
                        
                        // Decrement original line item quantity by the threshold quantity plus the promotional item.
                        lineItem.AddQuantity(-1 * (discount.ThresholdQuantity + 1));
                        
                        // Create a new line item with the original threshold quantity to a temporary list.
                        // Do not want to count these quantities again.
                        var originalLineItem = new SalesLineItem(new Product(lineItem.ProductId, lineItem.ProductPrice),
                            discount.ThresholdQuantity);
                        originalLineItems.Add(originalLineItem);

                        // TODO: Add a limit, i.e. only get the discount a certain number of times.
                    }
                }
            }

            sale.RemoveZeroQuantityLineItems();

            foreach (var item in originalLineItems)
                sale.MakeLineItem(new Product(item.ProductId, item.ProductPrice), item.Quantity);

            foreach (var item in discountedLineItems)
                sale.AddLineItem(item); // Add these directly to the sale.

            var total = sale.LineItems.Sum(i => i.GetSubtotal());
            return total;
        }
        #endregion
    }
}
