using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace PublixSubSale
{
    class TwiSend
    {
        public static void SendTwilio(string twiMessage)
        {
            string token = ConfigurationManager.AppSettings["twiAuthToken"];
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
