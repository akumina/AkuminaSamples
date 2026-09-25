using Akumina.Interchange.Core.Entities.TextAnalytics;
using Akumina.Interchange.Core.Interfaces;
using Akumina.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Akumina.Custom.MachineTranslatorService
{
    public class AwsTranslate : IMachineTranslatorService
    {
        private const string AccessKey = "<Access Key>";
        private const string SecretKey = "<Secret Key>";

        /// <summary>
        /// Analyze the text and extract the Key Phrases and Named Entities using TextAnalytics (Language) service
        /// </summary>
        /// <param name="text">Text needs to be analyzed</param>
        /// <param name="language">Language of the text</param>
        /// <returns>Returns the Text Analytics Output which will have Key Phrases and Named Entities</returns>
        public Task<TextAnalyticsOutput> AnalyzeTextAsync(string text, string language)
        {
            TraceEvents.Log.Debug(message: "Akumina.Custom.MachineTranslatorService.AwsTranslate.AnalyzeTextAsync");
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Detects what language the text is
        /// </summary>
        /// <param name="text">text to translate</param>
        /// <returns>language code of text; e.g. en-us</returns>
        public Task <string> DetectTextLanguage(string text)
        {
            TraceEvents.Log.Debug(message: "Akumina.Custom.MachineTranslatorService.AwsTranslate.DetectTextLanguage");
            string responseText = AwsTranslateText(text, "en", "es");
            JObject json = JObject.Parse(responseText);
            TraceEvents.Log.Debug(json, "Amazon Translate Response ");

            if (json.ToString().Contains("TranslatedText"))
            {
                //To access to the properties in "dot" notation use a dynamic object
                var obj = JsonConvert.DeserializeObject<ResponseModel>(responseText);
                var sourceLanguageCode = obj.SourceLanguageCode;
                TraceEvents.Log.Debug($"Detected language: {sourceLanguageCode}");
                return Task.FromResult(sourceLanguageCode);
            }
            else
            {
                TraceEvents.Log.Debug("TranslatedText not found in response.");
                throw new Exception(json.ToString());
            }
        }

        /// <summary>
        /// Translate text (self detects source language)
        /// </summary>
        /// <param name="text">text to translate</param>
        /// <param name="targetLanguageCode">language to translate to (e.g., es-es)</param>
        /// <returns>translated text</returns>
        public Task<string> TranslateAsync(string text, string targetLanguageCode)
        {
            TraceEvents.Log.Debug(message: "Akumina.Custom.MachineTranslatorService.AwsTranslate.TranslateAsync");
            TraceEvents.Log.Debug(message: $"Text: {text} - TargetLanguageCode: {targetLanguageCode}");
            string responseText = AwsTranslateText(text, "en", targetLanguageCode);
            JObject json = JObject.Parse(responseText);
            TraceEvents.Log.Debug(json, "Amazon Translate Response ");

            if (json.ToString().Contains("TranslatedText"))
            {
                //To access to the properties in "dot" notation use a dynamic object
                var obj = JsonConvert.DeserializeObject<ResponseModel>(responseText);
                var translatedText = obj.TranslatedText;
                TraceEvents.Log.Debug($"Detected Source language: {obj.SourceLanguageCode} - Detected Target language: { obj.TargetLanguageCode} - Translated text: {obj.TranslatedText}");
                return Task.FromResult(translatedText);
            }
            else
            {
                TraceEvents.Log.Debug("TranslatedText not found in response.");
                throw new Exception(json.ToString());
            }
        }

        /// <summary>
        /// Translate text
        /// </summary>
        /// <param name="text">text to translate</param>
        /// <param name="sourceLanguageCode">language of text (e.g., en-us)</param>
        /// <param name="targetLanguageCode">language to translate to (e.g., es-es)</param>
        /// <returns>translated text</returns>
        public Task<string> TranslateAsync(string text, string sourceLanguageCode, string targetLanguageCode)
        {
            TraceEvents.Log.Debug(message: "Akumina.Custom.MachineTranslatorService.AwsTranslate.TranslateAsync");
            TraceEvents.Log.Debug(message: $"Text: {text} - SourceLanguageCode: {targetLanguageCode} - TargetLanguageCode: {targetLanguageCode}");

            string responseText = AwsTranslateText(text, sourceLanguageCode, targetLanguageCode);
            JObject json = JObject.Parse(responseText);
            TraceEvents.Log.Debug(json, "Amazon Translate Response ");

            if (json.ToString().Contains("TranslatedText"))
            {
                //To access to the properties in "dot" notation use a dynamic object
                var obj = JsonConvert.DeserializeObject<ResponseModel>(responseText);
                var translatedText = obj.TranslatedText;
                TraceEvents.Log.Debug($"Detected Source language: {obj.SourceLanguageCode} - Detected Target language: {obj.TargetLanguageCode} - Translated text: {obj.TranslatedText}");
                return Task.FromResult(translatedText);
            }
            else
            {
                TraceEvents.Log.Debug("TranslatedText not found in response.");
                throw new Exception(json.ToString());
            }
        }

        /// <summary>
        /// Validate the Translator Key and returns whether the key is valid or not
        /// </summary>
        /// <param name="subscriptionKey">Subscription Key added for the Tenant</param>
        /// <returns>Returns the validated result (true/false)</returns>
        public bool ValidateTranslatorKey(string subscriptionKey)
        {
            TraceEvents.Log.Debug(message: "Akumina.Custom.MachineTranslatorService.AwsTranslate.ValidateTranslatorKey");
            TraceEvents.Log.Debug(message: $"SubscriptionKey: {subscriptionKey}");
            return true;
        }

        #region Private methods
        private static string AwsTranslateText(string text, string sourceLang, string targetLang)
        {
            var date = DateTime.UtcNow;

            const string algorithm = "AWS4-HMAC-SHA256";
            const string regionName = "us-east-1";
            const string serviceName = "translate";
            const string method = "POST";
            const string canonicalUri = "/";
            const string canonicalQueryString = "";
            const string x_amz_target_header = "AWSShineFrontendService_20170701.TranslateText";

            const string contentType = "application/x-amz-json-1.1";


            const string host = serviceName + "." + regionName + ".amazonaws.com";

            var obj = new
            {
                SourceLanguageCode = sourceLang,
                TargetLanguageCode = targetLang,
                Text = text
            };
            var requestPayload = new JavaScriptSerializer().Serialize(obj);

            var hashedRequestPayload = HexEncode(Hash(ToBytes(requestPayload)));

            var dateStamp = date.ToString("yyyyMMdd");
            var requestDate = date.ToString("yyyyMMddTHHmmss") + "Z";
            var credentialScope = string.Format("{0}/{1}/{2}/aws4_request", dateStamp, regionName, serviceName);

            var bytes = ToBytes(requestPayload);

            var headers = new SortedDictionary<string, string>
            {
                {"content-length", bytes.Length.ToString()},
                {"content-type", contentType},
                {"host", host},
                {"x-amz-date", requestDate},
                {"x-amz-target", x_amz_target_header}
            };

            string canonicalHeaders =
                string.Join("\n", headers.Select(x => x.Key.ToLowerInvariant() + ":" + x.Value.Trim())) + "\n";

            string signedHeaders =
            string.Join(";", headers.Select(x => x.Key.ToLowerInvariant()));

            // Task 1: Create a Canonical Request For Signature Version 4
            var canonicalRequest = method + '\n' + canonicalUri + '\n' + canonicalQueryString +
                                   '\n' + canonicalHeaders + '\n' + signedHeaders + '\n' + hashedRequestPayload;

            var hashedCanonicalRequest = HexEncode(Hash(ToBytes(canonicalRequest)));

            // Task 2: Create a String to Sign for Signature Version 4
            // StringToSign  = Algorithm + '\n' + RequestDate + '\n' + CredentialScope + '\n' + HashedCanonicalRequest

            var stringToSign = string.Format("{0}\n{1}\n{2}\n{3}", algorithm, requestDate, credentialScope,
                hashedCanonicalRequest);

            // Task 3: Calculate the AWS Signature Version 4

            // HMAC(HMAC(HMAC(HMAC("AWS4" + kSecret,"20130913"),"eu-west-1"),"tts"),"aws4_request")
            byte[] signingKey = GetSignatureKey(SecretKey, dateStamp, regionName, serviceName);

            // signature = HexEncode(HMAC(derived-signing-key, string-to-sign))
            var signature = HexEncode(HmacSha256(stringToSign, signingKey));

            // Task 4: Prepare a signed request
            // Authorization: algorithm Credential=access key ID/credential scope, SignedHeadaers=SignedHeaders, Signature=signature

            var authorization =
                string.Format("{0} Credential={1}/{2}/{3}/{4}/aws4_request, SignedHeaders={5}, Signature={6}",
                    algorithm, AccessKey, dateStamp, regionName, serviceName, signedHeaders, signature);

            // Send the request
            string endpoint = "https://" + host; // + canonicalUri ;

            var webRequest = WebRequest.Create(endpoint);

            webRequest.Method = method;
            webRequest.Timeout = 20000;
            webRequest.ContentType = contentType;
            webRequest.Headers.Add("X-Amz-Date", requestDate);
            webRequest.Headers.Add("Authorization", authorization);
            webRequest.Headers.Add("X-Amz-Target", x_amz_target_header);

            webRequest.ContentLength = bytes.Length;

            using (Stream newStream = webRequest.GetRequestStream())
            {
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Flush();
            }

            var response = (HttpWebResponse)webRequest.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                {
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        string res = streamReader.ReadToEnd();
                        return res;
                    }
                }
            }
            return null;
        }

        private static byte[] GetSignatureKey(String key, String dateStamp, String regionName, String serviceName)
        {
            byte[] kDate = HmacSha256(dateStamp, ToBytes("AWS4" + key));
            byte[] kRegion = HmacSha256(regionName, kDate);
            byte[] kService = HmacSha256(serviceName, kRegion);
            return HmacSha256("aws4_request", kService);
        }

        private static byte[] ToBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str.ToCharArray());
        }

        private static string HexEncode(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
        }

        private static byte[] Hash(byte[] bytes)
        {
            return SHA256.Create().ComputeHash(bytes);
        }

        private static byte[] HmacSha256(String data, byte[] key)
        {
            return new HMACSHA256(key).ComputeHash(ToBytes(data));
        }

        #endregion
    }

    /// <summary>
    /// Response Model for Amazon API Response conversion
    /// </summary>
    public class ResponseModel
    {
        public string SourceLanguageCode { get; set; }
        public string TargetLanguageCode { get; set; }
        public string TranslatedText { get; set; }
    }
}
