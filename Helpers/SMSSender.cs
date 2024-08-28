using System.Net;

namespace PensionSystem.Helpers
{
    public class SMSSender
    {
        /// <summary>
        /// Api Key	Your API Key (Required) sender Your company Name/Brand Name like "xyz" Displayed of Recipient Mobile(Required mobile Recipient Mobile Number(Required) message
        /// Message For Recipient(Required)type Type = unicode for unicode SMS(OPTIONAL)
        ///date dd-mm-YYYY Example: 25-02-2014 (OPTIONAL)
        ///time    hh:mm:ss Example: 21:30:55 (OPTIONAL)
        ///format  Type is xml and json.Not Case sensitive.If set, the result will be returned according to the type specified. (OPTIONAL)
        /// </summary>
        /// <param name="toNumber"></param>
        /// <param name="MessageText"></param>
        /// <param name="MyUsername"></param>
        /// <param name="MyPassword"></param>
        /// <returns></returns>
        public static async Task<string> SendSMS(string toNumber, string MessageText, string MyUsername = "923100919980", string MyPassword = "2153")
        {
            string MyApiKey = "923100919980-6f5fa244-f027-42e1-8c9f-1f81647a90b4"; //Your API Key At Sendpk.com
            string URI = "http://Sendpk.com" +
                "/api/sms.php?" +
                "api_key=" + MyApiKey +
                "&sender=Ahmad Ali" +
                "&mobile=" + toNumber +
                "&message=" + Uri.UnescapeDataString(MessageText); // Visual Studio 10-15
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(URI);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse httpWebResponse)
                {
                    switch (httpWebResponse.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            return "404:URL not found :" + URI;

                        case HttpStatusCode.BadRequest:
                            return "400:Bad Request";

                        default:
                            return httpWebResponse.StatusCode.ToString();
                    }
                }
            }
            return string.Empty;
        }
    }
}