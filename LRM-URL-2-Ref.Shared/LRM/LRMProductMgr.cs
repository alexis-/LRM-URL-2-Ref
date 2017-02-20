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
    using System.Collections.Generic;

    using LRM_URL_2_Ref.LRM.Models;

    /// <summary>LRM Product Manager</summary>
    public class LRMProductMgr
    {
        #region Fields

        private static LRMProductMgr _instance;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LRMProductMgr"/> class.
        /// </summary>
        protected LRMProductMgr()
        {
            Products = new Dictionary<int, LRMProduct>();
        }

        #endregion



        #region Properties

        /// <summary>Gets the instance.</summary>
        public static LRMProductMgr Instance => _instance ?? (_instance = new LRMProductMgr());

        /// <summary>Gets or sets the products.</summary>
        private Dictionary<int, LRMProduct> Products { get; set; }

        #endregion



        #region Methods

        /// <summary>Gets the products.</summary>
        /// <returns></returns>
        public IEnumerable<LRMProduct> GetProducts()
        {
            return Products.Values;
        }

        /// <summary>Adds the or update product.</summary>
        /// <param name="product">The product.</param>
        /// <param name="shop">The shop.</param>
        public void AddOrUpdateProduct(LRMProduct product, LRMShop shop)
        {
            if (product == null || shop == null)
                return;

            // No such product, simply add this definition
            if (!Products.ContainsKey(product.Reference))
            {
                Products[product.Reference] = product;
            }

            // Update existing definition
            else
            {
                var localProduct = Products[product.Reference];
                
                foreach (var url in product.Urls)
                    localProduct.Urls.Add(url);

                foreach (var productProductVarProp in product.ProductVarProps)
                    localProduct.AddProductVarProp(
                        productProductVarProp.Key, productProductVarProp.Value);
            }
        }

        #endregion
    }
}