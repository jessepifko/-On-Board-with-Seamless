using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SeamlessLaunchpad
{
    public class Utilities
    {
        public const string BaseApi = "https://api.airtable.com";

        public static readonly HttpClient postClient;

        static Utilities()
        {
            postClient = new HttpClient();
        }
        /// <summary>
        /// Builds a set of key value pairs from a list of strings
        ///
        /// String size must be divisible by 2
        /// </summary>
        /// <param name="pairs">A list in the format of "[key]", "[value]" repeating</param>
        /// <returns>A list that can be passed to GetApiResponse, or null if an odd number of arguments is passed</returns>
        public static List<KeyValuePair<string, string>> BuildApiArguments(params string[] pairs)
        {
            if (pairs.Length % 2 == 1)
            {
                return null;
            }

            var ret = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < pairs.Length; i+=2)
            {
                ret.Add(new KeyValuePair<string, string>(pairs[i], pairs[i+1]));
            }

            return ret;
        }
        
        // GetApiResponse<Startup>("v0/appFo187B73tuYhyg", "Master List", Utilities.BaseApi, "api_key", ApiKey, "Name", "Startup 1"); 
        // Equates TO: https://api.airtable.com/v0/appFo187B73tuYhyg/Master%20List?api_key={ApiKey}&Name=Startup%201
        /// <summary>
        /// Calls an api and populates a model
        /// </summary>
        /// <param name="controller">The controller (in the API) to call data from</param>
        /// <param name="action">The endpoint to call to</param>
        /// <param name="baseUrl">The api location (everything before the the / after .com (ex. https://api.airtable.com)</param>
        /// <param name="options">A list of options to send to the endpoint</param>
        /// <typeparam name="T">The model to deserialize into</typeparam>
        /// <returns>A populated model with the data that the api returned</returns>
        public static async Task<List<T>> GetApiResponse<T>(string controller, string action, string baseUrl,
            params KeyValuePair<string, string>[] options) where T : new()
        {
            string url = $"{baseUrl}/" +
                         $"{controller}/" +
                         $"{Uri.EscapeDataString(action)}";

            bool first = true;
            foreach (KeyValuePair<string, string> argument in options)
            {
                url += first ? "?" : "&";
                url += $"{argument.Key}={Uri.EscapeDataString(argument.Value)}";
                first = false;
            }

            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse) request.GetResponse();
            }
            catch (WebException)
            {
                return null;
            }

            Stream s = response.GetResponseStream();
            if (s == null)
            {
                return null;
            }
            
            StreamReader rd = new StreamReader(s);

            string output = await rd.ReadToEndAsync();
            List<T> ret;
            try
            {
                ret = JsonConvert.DeserializeObject<List<T>>(output);
            }
            catch (JsonSerializationException) // most likely we only got one object back
            {
                ret = new List<T> {JsonConvert.DeserializeObject<T>(output)};
            }
            
            return ret;
        }

        public static async Task<List<T>> PostApiResponse<T>(string controller, string action, string baseUrl,
            params KeyValuePair<string, string>[] options) where T : new()
        {
            string url = $"{baseUrl}/" +
                         $"{controller}/" +
                         $"{Uri.EscapeDataString(action)}";

            var postVariables = new FormUrlEncodedContent(options);

            HttpResponseMessage response = await postClient.PostAsync(url, postVariables);

            string responseString = await response.Content.ReadAsStringAsync();

            List<T> ret;
            try
            {
                ret = JsonConvert.DeserializeObject<List<T>>(responseString);
            }
            catch (JsonSerializationException) // most likely we only got one object back
            {
                ret = new List<T> { JsonConvert.DeserializeObject<T>(responseString) };
            }

            return ret;
        }

        /// <summary>
        /// Calls an api and populates a model
        /// </summary>
        /// <param name="controller">The controller (in the API) to call data from</param>
        /// <param name="action">The endpoint to call to</param>
        /// <param name="baseUrl">The api location (everything before the the / after .com (ex. https://api.airtable.com)</param>
        /// <param name="options">A list of options to send to the endpoint (?api_key=key == "api_key", "key")</param>
        /// <typeparam name="T">The model to deserialize into</typeparam>
        /// <returns>A populated model with the data that the api returned</returns>
        public static async Task<List<T>> PostApiResponse<T>(string controller, string action, string baseUrl, params string[] options)
            where T : new()
        {
            var keyValuePairs = Utilities.BuildApiArguments(options);
            if (keyValuePairs == null && options.Length > 0)
            {
                return new List<T>();
            }

            return await PostApiResponse<T>(controller, action, baseUrl,
                (keyValuePairs ?? new List<KeyValuePair<string, string>>()).ToArray());
        }

        // GetApiResponse<Startup>("v0/appFo187B73tuYhyg", "Master List", Utilities.BaseApi, "api_key", ApiKey, "Name", "Startup 1"); 
        // Equates TO: https://api.airtable.com/v0/appFo187B73tuYhyg/Master%20List?api_key={ApiKey}&Name=Startup%201
        /// <summary>
        /// Calls an api and populates a model
        /// </summary>
        /// <param name="controller">The controller (in the API) to call data from</param>
        /// <param name="action">The endpoint to call to</param>
        /// <param name="baseUrl">The api location (everything before the the / after .com (ex. https://api.airtable.com)</param>
        /// <param name="options">A list of options to send to the endpoint (?api_key=key == "api_key", "key")</param>
        /// <typeparam name="T">The model to deserialize into</typeparam>
        /// <returns>A populated model with the data that the api returned</returns>
        public static async Task<List<T>> GetApiResponse<T>(string controller, string action, string baseUrl, params string[] options)
            where T : new()
        {
            var keyValuePairs = Utilities.BuildApiArguments(options);
            if (keyValuePairs == null && options.Length > 0)
            {
                return new List<T>();
            }

            return await GetApiResponse<T>(controller, action, baseUrl, 
                (keyValuePairs ?? new List<KeyValuePair<string, string>>()).ToArray());
        }
        
        // GetApiResponse<Startup>("v0/appFo187B73tuYhyg", "Master List", Utilities.BaseApi, "api_key", ApiKey, "Name", "Startup 1"); 
        // Equates TO: https://api.airtable.com/v0/appFo187B73tuYhyg/Master%20List?api_key={ApiKey}&Name=Startup%201
        /// <summary>
        /// Calls an api and populates a model
        /// </summary>
        /// <param name="controller">The controller (in the API) to call data from</param>
        /// <param name="action">The endpoint to call to</param>
        /// <param name="baseUrl">The api location (everything before the the / after .com (ex. https://api.airtable.com)</param>
        /// <typeparam name="T">The model to deserialize into</typeparam>
        /// <returns>A populated model with the data that the api returned</returns>
        public static async Task<List<T>> GetApiResponse<T>(string controller, string action, string baseUrl)
            where T : new()
        {
            return await GetApiResponse<T>(controller, action, baseUrl,
                (new List<KeyValuePair<string, string>>()).ToArray());
        }
    }
}