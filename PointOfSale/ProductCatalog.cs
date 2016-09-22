using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSale.Repository;

namespace PointOfSale
{
    /// <summary>
    /// Simple aggregate of <see cref="Product"></see> instances in the store.
    /// </summary>
    public class ProductCatalog
    {
        #region Members
        /// <summary>
        /// Gets/sets the <see cref="Product"></see> instances in the store.
        /// </summary>
        public List<Product> Products = new List<Product>();
        #endregion

        #region Public Methods
        /// <summary>
        /// Removes all <see cref="Product"></see> instances that share the same ID except the first instance.
        /// </summary>
        public void RemoveDuplicates()
        {
            Products = Products?.GroupBy(p => p.Id).Select(g => g.First()).ToList();
        }

        /// <summary>
        /// Returns the matching <see cref="Product"></see> by ID, if it exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product FindProduct(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            return Products?.SingleOrDefault(p => p.Id.Equals(id?.Trim()));
        }

        public void Validate()
        {
            var count = Products.Count(p => p.Price <= 0);
            if (count > 0)
                throw new InvalidProductValueException(
                    $"There are {count} product(s) in the product catalog that have a price that is negative or zero.", null);
        }
        #endregion
    }
}
