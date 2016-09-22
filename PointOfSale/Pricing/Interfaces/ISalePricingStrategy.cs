// ReSharper disable once CheckNamespace
namespace PointOfSale.Pricing
{
    public interface ISalePricingStrategy
    {
        /// <summary>
        /// Calculates the total amount that the customer must pay for the given sale.
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        decimal GetTotal(Sale sale);        
    }
}
