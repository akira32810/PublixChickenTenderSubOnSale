using System;
using System.Configuration;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Html5;
using OpenQA.Selenium.Remote;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using static PublixSubSale.JsonModel;

namespace PublixSubSale
{
    class Program
    {
        static void Main(string[] args)
        {
            string tenderSub = getHtmlJsoNStringForTender();

            Console.WriteLine(tenderSub);

            //maybe email also?
           // Console.Read();
        }



        public static string getHtmlJsoNStringForTender()
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
                var script = doc.DocumentNode.Descendants().Where(x => x.Name == "script").Where(x => x.InnerText.Contains("window.pblxDataLayer")).FirstOrDefault().InnerText;

                var scriptMod = script.Replace("window.pblxDataLayer =", "");


                int index = scriptMod.IndexOf("var loadAttempts");
                if (index > 0)
                {
                    scriptMod = scriptMod.Substring(0, index);
                }
                var chickenSandwichData = scriptMod.Replace("};", "}");
                var jobject = JsonConvert.DeserializeObject<RootObject>(chickenSandwichData);



                driver.Quit();
                driver.Dispose();

                //File.WriteAllText("C:\\temp\\htmltext.txt", chickenSandwichData);

                if (jobject.products.price != null)
                {
                    if (jobject.products.price.Contains("6.09"))
                    {
                        MsgChickenTenderSub = "Chicken Tender Sub is not on sale :(";
                        SendTwilio(MsgChickenTenderSub);
                    }

                    else
                    {
                        MsgChickenTenderSub = "Chicken Tender Sub is on sale!!!";
                        SendTwilio(MsgChickenTenderSub);
                        //email or text error
                    }
                }

                else
                {
                    MsgChickenTenderSub = "Can't get data correctly, fix profile if possible.";
                    SendTwilio(MsgChickenTenderSub);
                    //email or text error
                }

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

        public static void SendTwilio(string twiMessage)
        {
             string token= ConfigurationManager.AppSettings["twiAuthToken"];
            // DANGER! This is insecure. See http://twil.io/secure
            const string accountSid = "AC91787b972210ed0da6a0d02c22652044";
            string authToken = token;

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: twiMessage,
                from: new Twilio.Types.PhoneNumber("+13212042204"),
                to: new Twilio.Types.PhoneNumber("+14079632120")
            );
        }


    }

}
