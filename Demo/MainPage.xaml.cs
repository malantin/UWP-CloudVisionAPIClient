using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Media.Capture;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using CloudVision;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Create Cloud Vision API Client and set Key for API Access
        private CloudVisionAPIClient APIClient = new CloudVisionAPIClient();
        //private CloudVisionAPIClient APIClient = new CloudVisionAPIClient("Set API Key here");

        private StorageFile imageStorageFile;
        private Stream imageStream;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void pickImage()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                imageStorageFile = file;
                await openAndSetImageAsync(imageStorageFile);
                resultText.Text = "loading...";
                var requestedFeatureType = getSelectedType();
                var result = APIClient.AnnotateImage(imageStream, requestedFeatureType, 5);
                resultText.Text = await result;
            }
            else
            {
                return;
            }
        }

        private async void captureImage()
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.AllowCropping = false;

            StorageFile file = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (file == null)
            {
                // No photo captured
                return;
            }

            imageStorageFile = file;
            await openAndSetImageAsync(imageStorageFile);
            resultText.Text = "loading...";
            var requestedFeatureType = getSelectedType();
            var result = APIClient.AnnotateImage(imageStream, requestedFeatureType, 5);
            resultText.Text = await result;
        }

        private async Task openAndSetImageAsync(StorageFile file)
        {
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);

            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

            SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

            imageControl.Source = bitmapSource;

            imageStream = await file.OpenStreamForReadAsync();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            pickImage();
        }

        private async void TypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (resultText != null && imageStream != null)
            {
                resultText.Text = "loading...";
                var requestedFeatureType = getSelectedType();
                var result = APIClient.AnnotateImage(imageStream, requestedFeatureType, 5);
                resultText.Text = await result;
            }
        }

        private FeatureType getSelectedType()
        {
            if (TypeListBox != null && TypeListBox.SelectedItem != null && resultText != null)
            {
                var selectedItemText = ((ListBoxItem)TypeListBox.SelectedItem).Content.ToString();
                var requestedFeatureType = (FeatureType)Enum.Parse(typeof(FeatureType), selectedItemText);
                return requestedFeatureType;
            }
            else return FeatureType.LABEL_DETECTION;
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            captureImage();
        }

        private void APIKeyBox_LostFocus(object sender, RoutedEventArgs e)
        {
            APIClient.APIKey = APIKeyBox.Text;
        }
    }
}
