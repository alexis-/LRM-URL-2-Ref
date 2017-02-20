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

namespace LRM_URL_2_Ref.LRM
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using LRM_URL_2_Ref.LRM.Models;

    /// <summary>Crawl product page</summary>
    internal static class LRMProductCrawler
    {
        #region Methods

        /// <summary>
        /// Gets the product asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="url">The URL.</param>
        /// <param name="shop">The shop.</param>
        /// <returns>Wait-able task</returns>
        public static async Task<LRMProduct> GetProductAsync(
            HttpClient client, string url, LRMShop shop)
        {
            if (!url.Contains("leroymerlin.fr/v3/p/produits/"))
            {
                return null;
            }

            string html = await ProcessUrlAsync(url, client).ConfigureAwait(false);

            if (html == null)
            {
                return null;
            }

            int reference = GetReference(html);

            if (reference <= 0)
            {
                return null;
            }

            LRMProduct product = new LRMProduct(reference, url);
            product.AddProductVarProp(shop, new LRMProductVarProp(GetPrice(html), GetStock(html)));

            return product;
        }

        /// <summary>
        /// Processes the URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="client">The client.</param>
        /// <returns>CSV value</returns>
        private static async Task<string> ProcessUrlAsync(string url, HttpClient client)
        {
            if (!url.Contains("leroymerlin.fr"))
            {
                return null;
            }

            return await client.GetStringAsync(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the reference.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns>Reference id</returns>
        private static int GetReference(string html)
        {
            int reference;
            Match match = Regex.Match(html, LRMConst.ProductIdRegex);

            if (!int.TryParse(SafeGetMatch(match), out reference))
            {
                reference = -1;
            }

            return reference;
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns>Price tag</returns>
        private static double GetPrice(string html)
        {
            double price;
            Match match = Regex.Match(html, LRMConst.PriceRegex);

            if (!double.TryParse(SafeGetMatch(match), NumberStyles.Any, CultureInfo.InvariantCulture, out price))
            {
                price = -1;
            }

            return price;
        }

        /// <summary>
        /// Gets the stock.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns>Stock count</returns>
        private static int GetStock(string html)
        {
            int stock;
            Match match = Regex.Match(html, LRMConst.StockRegex);

            if (!int.TryParse(SafeGetMatch(match), out stock))
            {
                stock = -1;
            }

            return stock;
        }
        
        /// <summary>
        /// Safes the get match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>Extracted match</returns>
        private static string SafeGetMatch(Match match)
        {
            try
            {
                return match.Groups[1].Value;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion
    }
}