using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static PublixSubSale.JsonModel;

namespace PublixSubSale
{
    class WebRequest
    {
        public static async Task<string> RequestPublixBread()
        {
            string sourceData = string.Empty;
            string MsgChickenTenderSub = string.Empty;
            try
            {
              

                Uri uri = new Uri("https://www.publix.com/pd/publix-chicken-tender-sub/BMO-DSB-100011");

                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.CookieContainer = new CookieContainer();

                    handler.CookieContainer.Add(uri, new Cookie("Store", "%7B%22StoreName%22%3A%22The%20Paramount%20on%20Lake%20Eola%22%2C%22StoreNumber%22%3A1131%2C%22Option%22%3A%22ACFJNOR%22%2C%22ShortStoreName%22%3A%22Lake%20Eola%22%7D")); // Adding a Cookie
                    using (HttpClient client = new HttpClient(handler))
                    {
                        var content = await client.GetStringAsync(uri);
                        CookieCollection collection = handler.CookieContainer.GetCookies(uri); // Retrieving a Cookie
                        sourceData = content;
                    }

                }

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(sourceData);
                var script = doc.DocumentNode.Descendants().Where(x => x.Name == "script").Where(x => x.InnerText.Contains("window.pblxDataLayer")).FirstOrDefault().InnerText;

                var scriptMod = script.Replace("window.pblxDataLayer =", "");


                int index = scriptMod.IndexOf("var loadAttempts");
                if (index > 0)
                {
                    scriptMod = scriptMod.Substring(0, index);
                }
                var chickenSandwichData = scriptMod.Replace("};", "}");
                var jobject = JsonConvert.DeserializeObject<RootObject>(chickenSandwichData);


                if (jobject.products.price != null)
                {
                    if (jobject.products.price.Contains("6.09"))
                    {
                        MsgChickenTenderSub = "Chicken Tender Sub is not on sale :(";
                        TwiSend.SendTwilio(MsgChickenTenderSub);
                    }

                    else
                    {
                        MsgChickenTenderSub = "Chicken Tender Sub is on sale!!!";
                        TwiSend.SendTwilio(MsgChickenTenderSub);
                        //email or text error
                    }
                }

                else
                {
                    MsgChickenTenderSub = "Can't get data correctly, fix profile if possible.";
                    TwiSend.SendTwilio(MsgChickenTenderSub);
                    //email or text error
                }

                return MsgChickenTenderSub;
            }

            catch (Exception e)
            {
                MsgChickenTenderSub = e.Message;
                //email or text error;
                TwiSend.SendTwilio(MsgChickenTenderSub);
                return MsgChickenTenderSub;
            }

        }

  
    }
}
