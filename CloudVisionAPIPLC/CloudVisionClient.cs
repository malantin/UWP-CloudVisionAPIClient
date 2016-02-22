using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CloudVisionAPI
{
    public class CloudVisionAPIClient
    {
        //Google API Key
        private string APIKey = "AIzaSyB26mx-IjKgiEgnto_Ze_1M9CSrFjKg-Ro";

        public CloudVisionAPIClient(string APIKey)
        {
            if(APIKey.Length > 1)
            this.APIKey = APIKey;
        }

        /// <summary>
        /// Annotage an image with the Cloud Vision API
        /// </summary>
        /// <param name="file">The image file to annotate</param>
        /// <param name="type">The type of annotation requested</param>
        /// <param name="maxResults">The maximum number of results</param>
        /// <returns></returns>
        public async Task<string> AnnotateImage(StorageFile file, FeatureType type, short maxResults)
        {
            var imageByteArray = await getByteArrayAsync(file);
            var imageBase64 = getBase64(imageByteArray);
            var payload = buildJsonFile(imageBase64, type, maxResults);
            return await sendToVisionAPI(payload);
        }

        private async Task<string> sendToVisionAPI(string json)
        {
            using (var client = new HttpClient())
            {                
                client.BaseAddress = new Uri($"https://vision.googleapis.com/v1/images:annotate?key={APIKey}");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsync("", new StringContent(json, Encoding.UTF8, "application/json"));
                //if (response.IsSuccessStatusCode)
                //{
                    return await response.Content.ReadAsStringAsync();
                //}
            }
            //return "";
        }

        private string buildJsonFile(string base64Image, FeatureType type, short maxResults)
        {
            JObject json = new JObject();

            JArray requestsArray = new JArray();

            JObject requestObject = new JObject();
            
            JObject image = new JObject();

            image.Add(new JProperty("content", base64Image));

            JProperty imageProperty = new JProperty("image", image);

            JArray featuresArray = new JArray();

            JObject featuresObject = new JObject();

            featuresObject.Add(new JProperty("type", type));

            featuresObject.Add(new JProperty("maxResults", maxResults));

            JProperty featuresProperty = new JProperty("features", featuresArray);

            featuresArray.Add(featuresObject);

            requestObject.Add(imageProperty);
            requestObject.Add(featuresProperty);

            requestsArray.Add(requestObject);

            JProperty requestsProperty = new JProperty("requests", requestsArray);

            json.Add(requestsProperty);

            return json.ToString();
        }

        private async Task<byte[]> getByteArrayAsync(StorageFile file)
        {
            using (var inputStream = await file.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();

                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                return byteArray;
            }

        }

        private string getBase64(byte[] imageBytes)
        {
                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
        }
    }
}
