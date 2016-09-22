using PointOfSale;
using PointOfSale.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace PointOfSaleRunner
{
    internal class Register
    {
        private static void Main(string[] args)
        {
            WriteLine(new string('-', 80));
            WriteLine("Welcome to GroceryCo Checkout");
            WriteLine(new string('-', 80));
            WriteLine();

            while (true)
            {
                Write("Filename (or Q to quit) > ");

                var input = ReadLine();

                if (string.IsNullOrEmpty(input))
                    continue;

                if (input.Trim().Equals("q", StringComparison.OrdinalIgnoreCase))
                    break;

                var productLines = ReadProductLines(input);
                if (productLines == null)
                    continue;
                
                var productCatalog = ReadProductCatalog();    
                if (productCatalog == null)
                    continue;

                ProcessSale(input, productLines, productCatalog);
            }
        }

        private static void ProcessSale(string filename, IEnumerable<string> productLines, ProductCatalog productCatalog)
        {
            Sale sale = null;
            try
            {
                sale = new Sale();
            }
            catch (PromotionCatalogFileException ex)
            {
                WriteLine("Error reading promotion catalog.");
                WriteLine($"{ex.Message}");
                WriteLine($"{ex.InnerException?.Message}");
                WriteLine("Please have a system administrator correct the error.");
                return;
            }

            WriteLine(new string('-', 80));
            WriteLine($"Sale from {filename}");
            WriteLine(new string('-', 80));

            foreach (
                var productLine in
                productLines.Where(line => !string.IsNullOrEmpty(line)).Select(line => line.Trim()))
            {
                var product = productCatalog.FindProduct(productLine);
                if (product == null)
                {
                    WriteLine($"Warning! '{productLine}' was not found in the product catalog, skipping.");
                    continue;
                }
                sale.MakeLineItem(product, quantity: 1);
            }

            var total = sale.GetTotal();

            foreach (var lineItem in sale.LineItems.OrderBy(i => i.ProductId))
                Write(lineItem.GetLineItemString());

            WriteLine(new string('-', 80));
            WriteLine("Your total is ".PadRight(30) + $"{total:c}");
            WriteLine(new string('-', 80));
        }

        private static IEnumerable<string> ReadProductLines(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            string[] productLines;
            try
            {
                productLines = System.IO.File.ReadAllLines(filename.Trim());
            }
            catch (Exception ex)
            {
                WriteLine($"Error reading input products from '{filename}'.");
                WriteLine($"{ex.Message}");
                WriteLine($"{ex.InnerException?.Message}");
                return null;
            }

            return productLines;
        }

        private static ProductCatalog ReadProductCatalog()
        {
            ProductCatalog catalog;
            try
            {
                WriteLine("Reading ProductCatalog.json...");
                catalog = new PointOfSaleFileReader().ReadProductCatalog("ProductCatalog.json");
                WriteLine($"Found {catalog.Products.Count} product(s) in catalog.");
                WriteLine();
            }
            catch (Exception ex)
            {
                WriteLine("Error reading product catalog.");
                WriteLine($"{ex.Message}");
                WriteLine($"{ex.InnerException?.Message}");
                WriteLine("Please have a system administrator correct the error.");
                return null;
            }

            return catalog;
        }
    }
}
