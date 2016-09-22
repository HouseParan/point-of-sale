PointOfSale Library
-------------------------------------------------------------------------------

Logic for a simple Point of Sale system.

-------------------------------------------------------------------------------
The choice of JSON files to store products and promotions was quick and dirty as it's difficult to exert a lot 
of control over the construction of the catalogs. So all properties are public and nothing stops other classes 
from messing around inside, this is far from ideal.

The main work is done in LineItemBuyNGetOnePricingStrategy.cs and LineItemQuantityPricingStrategy.cs which
mutate the Sale object to apply the line item discounts. Implementation works but I hazard to guess that 
there's a more elegant way to do this. However, it should be fairly easy to extend with transactional, customer 
discounts, etc.

 

