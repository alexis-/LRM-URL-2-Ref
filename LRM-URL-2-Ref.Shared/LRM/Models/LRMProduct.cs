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

namespace LRM_URL_2_Ref.LRM.Models
{
    using System.Collections.Generic;

    /// <summary>LRM Product data</summary>
    public class LRMProduct
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="LRMProduct" /> class.</summary>
        /// <param name="reference">The reference.</param>
        /// <param name="url">The URL.</param>
        public LRMProduct(int reference, string url)
        {
            Reference = reference;
            Urls = new SortedSet<string>(new[] { url });

            ProductVarProps = new Dictionary<LRMShop, LRMProductVarProp>();
        }

        #endregion



        #region Properties

        /// <summary>Gets or sets the reference.</summary>
        public int Reference { get; set; }

        /// <summary>
        /// Gets or sets the URLs.
        /// </summary>
        public SortedSet<string> Urls { get; set; }

        /// <summary>Gets or sets the product definitions.</summary>
        public Dictionary<LRMShop, LRMProductVarProp> ProductVarProps { get; set; }

        #endregion



        #region Methods

        /// <summary>Adds the product variable property.</summary>
        /// <param name="shop">The shop.</param>
        /// <param name="productVarProp">The product variable property.</param>
        public void AddProductVarProp(LRMShop shop, LRMProductVarProp productVarProp)
        {
            ProductVarProps[shop] = productVarProp;
        }

        #endregion
    }
}