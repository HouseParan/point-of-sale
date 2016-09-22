using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale
{
    public class Product
    {
        #region Members
        private readonly string _id;

        [JsonProperty(Required = Required.Always)]
        public string Id => _id;

        private readonly decimal _price;

        [JsonProperty(Required = Required.Always)]
        public decimal Price => _price;

        #endregion

        #region Constructors
        public Product(string id, decimal price)
        {
            _id = id?.Trim();
            _price = price;
        }
        #endregion

        #region Public Methods
        public override bool Equals(object obj)
        {
            var product = obj as Product;
            return product != null && Id.Equals(product.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion
    }    

}
