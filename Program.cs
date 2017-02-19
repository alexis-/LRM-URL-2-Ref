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
            if (args.Length != 2)
            {
                Console.WriteLine("Missing argument.");
                return;
            }

            string inFileName = args[0];
            string outFilename = args[1];

            FileStream inFileStream;
            FileStream outFileStream;

            StreamReader streamReader;
            StreamWriter streamWriter;

            List<string> urls = new List<string>();
            string inLine;

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

            while ((inLine = streamReader.ReadLine()) != null)
            {
                urls.Add(inLine);
            }

            IEnumerable<Task<string>> tasks = null;
            
            try
            {
                Uri baseUrl = new Uri("http://leroymerlin.fr");

                CookieContainer cookieContainer = new CookieContainer();
                cookieContainer.Add(baseUrl, new Cookie("tracking", "\"{\\\"x1\\\":\\\"142_Ivry_sur_Seine\\\",\\\"x11\\\":\\\"21_Vitry_sur_Seine\\\",\\\"x6\\\":\\\"1\\\",\\\"x10\\\":\\\"1\\\",\\\"x9\\\":null}\""));
                cookieContainer.Add(baseUrl, new Cookie("store", "store=142|dateContext=20170213"));

                HttpClientHandler handler = new HttpClientHandler
                    { CookieContainer = cookieContainer };
                HttpClient webClient = new HttpClient(handler);

                tasks = urls.Select(u => ProcessUrlAsync(u, webClient));

                // ReSharper disable once CoVariantArrayConversion
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while crawling : " + ex);
                throw;
            }

            try
            {
                // Write header
                const string CsvHeader = "Url,Reference,Price,Stock";
                streamWriter.WriteLine(CsvHeader);

                foreach (var task in tasks)
                {
                    streamWriter.WriteLine(task.Result);
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
        /// Processes the URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="client">The client.</param>
        /// <returns>CSV value</returns>
        private static async Task<string> ProcessUrlAsync(string url, HttpClient client)
        {
            if (!url.Contains("leroymerlin.fr"))
            {
                return url + "," + "," + ",";
            }

            var html = await client.GetStringAsync(url).ConfigureAwait(false);

            string ret = url + "," + ExtractInformationFromHtml(html);
            Console.WriteLine(ret);

            return ret;
        }

        /// <summary>
        /// Extracts the information from HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns>CSV value</returns>
        private static string ExtractInformationFromHtml(string html)
        {
            // product_id : '68715843',
            // product_unitprice_ati : '8.0',
            // <span>126 Unités</span>
            const string ProductIdRegex = "product_id : '([0-9]+)',";
            const string PriceRegex = "product_unitprice_ati : '([0-9\\.]+)',";
            const string InventoryCountRegex = "<span>([0-9]+) Unités</span>";

            Match productIdMatch = Regex.Match(html, ProductIdRegex);
            Match priceMatch = Regex.Match(html, PriceRegex);
            Match inventoryCountMatch = Regex.Match(html, InventoryCountRegex);

            return string.Join(
                ",",
                SafeGetMatch(productIdMatch),
                SafeGetMatch(priceMatch),
                SafeGetMatch(inventoryCountMatch));
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