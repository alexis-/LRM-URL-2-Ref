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

    /// <summary>
    /// LRM Constant data
    /// </summary>
    public static class LRMConst
    {
        // Product data Regexes
        //   product_id : '68715843',
        //   product_unitprice_ati : '8.0',
        //   <span>126 Unités</span>

        /// <summary>
        /// The product identifier regex
        /// </summary>
        public const string ProductIdRegex = "product_id : '([0-9]+)',";

        /// <summary>
        /// The price regex
        /// </summary>
        public const string PriceRegex = "product_unitprice_ati : '([0-9\\.]+)',";

        /// <summary>
        /// The inventory count regex
        /// </summary>
        public const string StockRegex = "<span>([0-9]+) Unités</span>";

        /// <summary>
        /// The shops
        /// </summary>
        public static Dictionary<int, LRMShop> Shops = new Dictionary<int, LRMShop>
        {
            { 55, new LRMShop("paris_beaubourg", 55) },
            { 182, new LRMShop("paris_daumesnil", 182) },
            { 190, new LRMShop("paris_19_-_rosa_parks", 190) },
            { 142, new LRMShop("ivry_sur_seine", 142) },
            { 114, new LRMShop("saint_ouen", 114) },
            { 21, new LRMShop("vitry_sur_seine", 21) },
            { 115, new LRMShop("saint_denis__st_denis-la-plaine_", 115) },
            { 72, new LRMShop("gennevilliers", 72) },
            { 63, new LRMShop("rosny-sous-bois", 63) },
            { 161, new LRMShop("rueil_malmaison", 161) },
            { 22, new LRMShop("bonneuil__bonneuil-sur-marne_", 22) }
        };
    }
}