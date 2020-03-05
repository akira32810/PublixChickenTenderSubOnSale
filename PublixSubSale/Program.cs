using System;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Html5;
using OpenQA.Selenium.Remote;

using static PublixSubSale.JsonModel;

namespace PublixSubSale
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            //string tenderSub = getHtmlJsoNStringForTender();
            string tenderSub = await WebRequest.RequestPublixBread();
            Console.WriteLine(tenderSub);

            // Console.Read();
        }



        public static string getHtmlJsoNStringForTenderWithSelenium()
        {
            string MsgChickenTenderSub = string.Empty;
            try
            {

                //copy the MyChromeDataSelenium folder to this C:\temp.
                string profile = "C:\\Temp\\MyChromeDataSelenium";
                Console.WriteLine("profile location: " + profile);

                ChromeOptions options = new ChromeOptions();


                options.AddArguments("--enable-geolocation");
                options.AddArguments("--user-data-dir=" + profile + "");
                RemoteWebDriver driver = new ChromeDriver(options);

                RemoteLocationContext location = new RemoteLocationContext(driver);

                //set geolocation here - latitude and longtitude of your location
                location.PhysicalLocation = new Location(28.538336, -81.379234, 1.12);

                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                //set URL here
                driver.Url = "https://www.publix.com/pd/publix-chicken-tender-sub/BMO-DSB-100011";


                var pageContents = driver.PageSource;


                //get jsonString
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(pageContents);
                var script = doc.DocumentNode.Descendants().Where(x => x.HasClass("savings-and-promotion")).FirstOrDefault();

                if (script != null)
                {
                    var savingText = script.ChildNodes.Where(x => x.HasClass("savings")).FirstOrDefault().InnerText;
                    if (!string.IsNullOrEmpty(script.InnerHtml) && !string.IsNullOrEmpty(savingText))
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

                driver.Quit();
                driver.Dispose();

                //File.WriteAllText("C:\\temp\\htmltext.txt", chickenSandwichData);

                // js.ExecuteScript("console.log(" + location.PhysicalLocation.Latitude + ")");

                return MsgChickenTenderSub;

            }
            catch (Exception e)
            {
                MsgChickenTenderSub = e.Message;
                //email or text error;
                return MsgChickenTenderSub;
            }

        }




    }

}
