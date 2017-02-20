// 
// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

// ReSharper disable StyleCop.SA1101
// ReSharper disable PossibleMultipleEnumeration
namespace LRM_URL_2_Ref
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using LRM_URL_2_Ref.LRM;
    using LRM_URL_2_Ref.LRM.Models;

    /// <summary>
    /// Main class
    /// </summary>
    class Program
    {
        #region Methods

        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Missing argument.");
                return;
            }


            // Variables
            string inFileName = args[0];
            string outFilename = args[1];

            FileStream inFileStream;
            FileStream outFileStream;

            StreamReader streamReader;
            StreamWriter streamWriter;

            List<LRMShop> shops = new List<LRMShop>();
            List<string> urls = new List<string>();
            string inLine;


            // List requested shops
            for (int i = 2; i < args.Length; i++)
            {
                int shopId;

                if (int.TryParse(args[i], out shopId))
                    shops.Add(LRMConst.Shops[shopId]);
            }

            // Or default to Ivry
            if (args.Length == 2)
            {
                shops.Add(LRMConst.Shops[142]);
            }


            // Get URLs
            try
            {
                inFileStream = File.OpenRead(inFileName);
                streamReader = new StreamReader(inFileStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while opening input file : " + ex);
                throw;
            }

            while ((inLine = streamReader.ReadLine()) != null)
            {
                urls.Add(inLine);
            }


            // Prepare out file
            try
            {
                File.Delete(outFilename);

                outFileStream = File.OpenWrite(outFilename);
                streamWriter = new StreamWriter(outFileStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while opening output file : " + ex);
                throw;
            }


            // Crawl
            try
            {
                LRMCrawler crawler = new LRMCrawler(urls, shops);

                crawler.CrawlAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while crawling : " + ex);
                throw;
            }


            // Output result & cleanup
            try
            {
                // Write header
                const string CsvHeaderFixed = "Url;Reference;";
                string csvHeader = CsvHeaderFixed
                                   + string.Join(",", shops.Select(CreateShopHeader));
                streamWriter.WriteLine(csvHeader);

                // Write data
                foreach (var product in LRMProductMgr.Instance.GetProducts())
                {
                    string buff = string.Join("\\n", product.Urls) + ";" + product.Reference + ";";
                    buff += CreateProductVarPropLine(shops, product.ProductVarProps);
                    
                    streamWriter.WriteLine(buff);
                }

                streamWriter.Close();
                streamReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while outpouting result : " + ex);
                throw;
            }
        }

        /// <summary>
        /// Creates the product variable property line.
        /// </summary>
        /// <param name="shops">The shops.</param>
        /// <param name="productVarProps">The product variable props.</param>
        /// <returns>CSV data line</returns>
        private static string CreateProductVarPropLine(
            List<LRMShop> shops,
            Dictionary<LRMShop, LRMProductVarProp> productVarProps)
        {
            return string.Join(
                ";",
                shops.Select(
                    s => productVarProps[s].Price + ";" + productVarProps[s].Stock));
        }

        /// <summary>
        /// Creates the shop header.
        /// </summary>
        /// <param name="shopId">The shop identifier.</param>
        /// <returns>Csv Header</returns>
        private static string CreateShopHeader(LRMShop shop)
        {
            string shopName = shop.Name;

            return shopName + " Price;" + shopName + " Stock";
        }

        #endregion
    }
}