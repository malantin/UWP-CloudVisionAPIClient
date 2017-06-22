# UWP / .Net Google Cloud Vision API Client
A small client that enables you to use the Google Cloud Vision API Beta from Universal Windows Platform (UWP) applications using an API key for authentication.

You can use the class library with your Universal Windows Applications (UWP) (Windows 10) apps. It should also work with .Net Core. Please refer to the [Google Cloud Vision API Website](https://cloud.google.com/vision/) and [API Documentation](https://cloud.google.com/vision/docs/) for more information on the Google Cloud Vision API.

You will need to sign up for the Google Cloud Services first, enable the Cloud Vision API and create an API Key.

Working with the API client is just a few lines of code. These are the relevant parts of the client:

```c#
        public string APIKey

        public CloudVisionAPIClient(string APIKey)

        public CloudVisionAPIClient()

        /// <summary>
        /// Annotage an image with the Cloud Vision API
        /// </summary>
        /// <param name="file">The image file to annotate</param>
        /// <param name="type">The type of annotation requested</param>
        /// <param name="maxResults">The maximum number of results</param>
        /// <returns></returns>
        public async Task<string> AnnotateImage(Stream file, FeatureType type, short maxResults)
```

You can use the client like this:

```c#
private CloudVisionAPIClient APIClient = new CloudVisionAPIClient();

APIClient.AnnotateImage(imageStream, requestedFeatureType, 5);
```

The solution contains two projects. The client as a portable class library and a demo app to test the client and API.

![Screenshot Landmark Detection](https://github.com/n01d/CloudVisionAPIClient/raw/master/Images/screen_1.png)

![Screenshot Landmark Detection](https://github.com/n01d/CloudVisionAPIClient/raw/master/Images/screen_2.png)

‼ Watch out. This an unstable project and should be used with caution 😉