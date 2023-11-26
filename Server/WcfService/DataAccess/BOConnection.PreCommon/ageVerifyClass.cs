using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LSOmni.DataAccess.BOConnection.PreCommon.AgeVerify
{
    public class AgeVerification
    {
        private string APIKey = "lCr0Q4OXQZtrf4hDXs10osYPfPqkkl0g";
        private string APISecret = "w8zMYJXeDOz4ky8Q";
        private string APISite = "https://api.agechecker.net/v1/create";
        public string IDNameFirst = "";
        public string IDNameLast = "";
        public string IDAddress = "";
        public string IDCity = "";
        public string IDState = "";
        public string IDZip = "";
        public string IDCountry = "";
        public int IDDOBDay = 0;
        public int IDDOBMonth = 0;
        public int IDDOBYear = 0;
        public int OptionsMinAge = 18;
        public string OptionsCustIP = "";
        public string OptionsUUID = "";
        public string ResultsUUID = "";
        public string ResultsStatus = "";
        public Boolean ResultsBlocked = false;
        public string ResultsErrorCode = "";
        public string ResultsErrorMsg = "";
        public string ResultsUploadType = "";

        public AgeVerification()
        {
        }

        public async Task<string> AgeVerifyCheckResultAsync()
        {
            if (OptionsUUID == string.Empty)
            {
                return "UUID is required";
            }
            HttpClient request = new HttpClient();
            var result = await request.GetAsync("https://api.agechecker.net/v1/status/" + OptionsUUID);
            string resultText = await result.Content.ReadAsStringAsync();
            AgeVerifyGetValues(resultText);
            return result.StatusCode.ToString() + " " + resultText;
        }

        public async Task<string> AgeVerifyAsync()
        {
            HttpClient request = new HttpClient();

            var obj1 = new
            {
                first_name = IDNameFirst,
                last_name = IDNameLast,
                address = IDAddress,
                city = IDCity,
                state = IDState,
                zip = IDZip,
                country = IDCountry,
                dob_day = IDDOBDay,
                dob_month = IDDOBMonth,
                dob_year = IDDOBYear
            };
            var obj2 = new
            {
                min_age = OptionsMinAge,
                customer_ip = OptionsCustIP
            };
            var obj3 = new
            {
                key = APIKey,
                secret = APISecret,
                data = obj1,
                options = obj2
            };
            JsonContent content = JsonContent.Create(obj3);

            HttpResponseMessage response = await request.PostAsync(APISite, content);
            Console.WriteLine();
            Console.WriteLine("Request payload:");
            Console.WriteLine();
            Console.WriteLine(obj3.ToString());
            Console.WriteLine();
            string jsonResponse = await response.Content.ReadAsStringAsync();
            AgeVerifyGetValues(jsonResponse);
            return jsonResponse;
        }

        public void AgeVerifyGetValues(string JsonResponseText)
        {
            if(JsonResponseText == string.Empty)
            {
                ResultsErrorMsg = "Json message must not be empty";
            }
            try
            {
                JsonNode AgeCheckerResponse = JsonNode.Parse(JsonResponseText);
                try
                {
                    ResultsBlocked = (Boolean)AgeCheckerResponse["blocked"];
                }
                catch
                {
                    ResultsBlocked = false;
                }
                try
                {
                    ResultsErrorCode = (string)AgeCheckerResponse["error"]["code"];
                }
                catch
                {
                    ResultsErrorCode = "";
                }
                try
                {
                    ResultsErrorMsg = (string)AgeCheckerResponse["error"]["message"];
                }
                catch
                {
                    ResultsErrorMsg = "";
                }
                try
                {
                    ResultsStatus = (string)AgeCheckerResponse["status"];
                }
                catch
                {
                    ResultsStatus = "";
                }
                try
                {
                    ResultsUploadType = (string)AgeCheckerResponse["upload_type"];
                }
                catch
                {
                    ResultsUploadType = "";
                }
                try
                {
                    ResultsUUID = (string)AgeCheckerResponse["uuid"];
                }
                catch
                {
                    ResultsUUID = "";
                }
            }
            catch (Exception e)
            {
                ResultsErrorMsg = "Unable to parse Json response: " + e.Message;
            }
            return;
        }
    }
}
