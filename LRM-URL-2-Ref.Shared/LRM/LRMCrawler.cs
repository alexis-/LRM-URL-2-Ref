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

// ReSharper disable PossibleMultipleEnumeration

namespace LRM_URL_2_Ref.LRM
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using LRM_URL_2_Ref.LRM.Models;

    /// <summary>LRM Crawler class</summary>
    public class LRMCrawler
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="LRMCrawler" /> class.</summary>
        /// <param name="urls">The URLs.</param>
        /// <param name="shops">The shops.</param>
        public LRMCrawler(IEnumerable<string> urls, IEnumerable<LRMShop> shops)
        {
            RequestedShops = new List<LRMShop>(shops);
            RequestedUrls = new List<string>(urls);

            CookieContainer cookieContainer = new CookieContainer();

            HttpClientHandler handler = new HttpClientHandler
                { CookieContainer = cookieContainer, UseCookies = true };
            HttpClient = new HttpClient(handler);
        }

        #endregion



        #region Properties

        /// <summary>Gets or sets the HTTP client.</summary>
        private HttpClient HttpClient { get; set; }

        /// <summary>Gets or sets the requested shops.</summary>
        private List<LRMShop> RequestedShops { get; set; }

        /// <summary>Gets or sets the requested URLs.</summary>
        private List<string> RequestedUrls { get; set; }

        #endregion



        #region Methods

        /// <summary>Crawls the asynchronous.</summary>
        /// <returns>Wait-able task</returns>
        public async Task CrawlAsync()
        {
            foreach (var shop in RequestedShops)
            {
                var lrmProducts = await CrawlShopAsync(shop).ConfigureAwait(false);

                foreach (var product in lrmProducts)
                {
                    LRMProductMgr.Instance.AddOrUpdateProduct(product, shop);
                }
            }
        }

        /// <summary>Crawls the shop asynchronous.</summary>
        /// <param name="shop">The shop.</param>
        /// <returns>Wait-able task</returns>
        private async Task<IEnumerable<LRMProduct>> CrawlShopAsync(LRMShop shop)
        {
            await SelectShopAsync(shop).ConfigureAwait(false);

            IEnumerable<Task<LRMProduct>> tasks =
                RequestedUrls.Select(
                    u => LRMProductCrawler.GetProductAsync(HttpClient, u, shop));

            // ReSharper disable once CoVariantArrayConversion
            Task.WaitAll(tasks.ToArray());

            return tasks.Select(t => t.Result);
        }

        /// <summary>Selects the shop.</summary>
        /// <param name="shop">The shop.</param>
        /// <returns>Wait-able task</returns>
        public async Task SelectShopAsync(LRMShop shop)
        {
            //cookieContainer.Add(baseUrl, new Cookie("tracking", "\"{\\\"x1\\\":\\\"142_Ivry_sur_Seine\\\",\\\"x11\\\":\\\"21_Vitry_sur_Seine\\\",\\\"x6\\\":\\\"1\\\",\\\"x10\\\":\\\"1\\\",\\\"x9\\\":null}\""));
            //cookieContainer.Add(baseUrl, new Cookie("store", "store=142|dateContext=20170219"));
            await
                HttpClient.GetStringAsync(
                              "http://www.leroymerlin.fr/v3/contextualization/store/contextualize.do?storeId="
                              + shop.Id).ConfigureAwait(false);
        }

        #endregion
    }
}