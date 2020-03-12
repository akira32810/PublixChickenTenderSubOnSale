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
                var script = doc.DocumentNode.Descendants().Where(x => x.HasClass("savings-and-promotion")).FirstOrDefault();
                var savingText = script.ChildNodes.Where(x => x.HasClass("savings")).FirstOrDefault();
                if (script != null && savingText != null )
                {
               
                        if (!string.IsNullOrEmpty(script.InnerHtml) && !string.IsNullOrEmpty(savingText.InnerText))
                        {
                            MsgChickenTenderSub = "Chicken Tender Sub at Publix is on sale!!! " + savingText;
                            TwiSend.SendTwilio(MsgChickenTenderSub);
                        }
                    
                }

                else
                {
                    MsgChickenTenderSub = "Chicken Tender Sub at Publix is not on sale :(";
                    TwiSend.SendTwilio(MsgChickenTenderSub);
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
