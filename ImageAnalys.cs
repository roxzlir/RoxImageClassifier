using Microsoft.Extensions.Configuration;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace RoxImageClassifier
{
    public class ImageAnalys
    {
        private static ComputerVisionClient cvClient;

        public ImageAnalys()
        {
            // Bygger konfigurationen från appsettings.json
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            string cogSvcEndpoint = configuration["CognitiveServicesEndpoint"];
            string cogSvcKey = configuration["CognitiveServiceKey"];

            // Autentiserar Azure AI Vision-klienten
            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(cogSvcKey);
            cvClient = new ComputerVisionClient(credentials)
            {
                Endpoint = cogSvcEndpoint
            };
        }

        public async Task AnalyzeImageAsync(string imageFilePath)
        {
            if (!File.Exists(imageFilePath))
            {
                Console.WriteLine($"File not found: {imageFilePath}");
                return;
            }

            // Öppna bilden som en stream
            using (Stream imageStream = File.OpenRead(imageFilePath))
            {
                // Utför bildanalysen
                var visualFeatures = new List<VisualFeatureTypes?>()
                {
                    VisualFeatureTypes.Categories,
                    VisualFeatureTypes.Description,
                    VisualFeatureTypes.Faces,
                    VisualFeatureTypes.ImageType,
                    VisualFeatureTypes.Tags,
                    VisualFeatureTypes.Color,
                    VisualFeatureTypes.Adult,
                    VisualFeatureTypes.Brands
                };

                ImageAnalysis analysis = await cvClient.AnalyzeImageInStreamAsync(imageStream, visualFeatures);

                // Visa resultaten av analysen
                DisplayAnalysisResults(analysis);   
            }
            
        }

        private void DisplayAnalysisResults(ImageAnalysis analysis)
        {
            // Visar bildbeskrivning
            Console.WriteLine("Description:");
            foreach (var caption in analysis.Description.Captions)
            {
                Console.WriteLine($" - {caption.Text} with confidence {caption.Confidence}");
            }

            // Visar tags
            Console.WriteLine("Tags:");
            foreach (var tag in analysis.Tags)
            {
                Console.WriteLine($" - {tag.Name} with confidence {tag.Confidence}");
            }

            // Visar övriga egenskaper
            Console.WriteLine("Other details:");
            Console.WriteLine($" - Image format: {analysis.ImageType.ClipArtType}");
            Console.WriteLine($" - Adult content: {analysis.Adult.IsAdultContent}");
            Console.WriteLine($" - Racy content: {analysis.Adult.IsRacyContent}");
            Console.WriteLine($" - Dominant colors: {string.Join(", ", analysis.Color.DominantColors)}");
        }

        public async Task GetThumbnail(string imageFile)
        {
            Console.WriteLine("Generating thumbnail, please choose your measurments in pixels");
            Console.Write("Width: ");
            int width = GetInputNumber();
            Console.Write("Hight: ");
            int height = GetInputNumber();

            // Generera en miniatyrbild
            try
            {
                
                using (var imageData = File.OpenRead(imageFile))
                {
                    // Hämta miniatyrdata
                    var thumbnailStream = await cvClient.GenerateThumbnailInStreamAsync(width, height, imageData, true);

                    string storagePath = @"C:\Users\emilc\source\repos\RoxImageClassifier";
                    // Spara miniatyrbilden
                    string thumbnailFileName = $@"{storagePath}\thumbnail_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    using (Stream thumbnailFile = File.Create(thumbnailFileName))
                    {
                        await thumbnailStream.CopyToAsync(thumbnailFile);  // Använd await här också för asynkron kopiering
                    }

                    Console.WriteLine($"Thumbnail saved as: {thumbnailFileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade: {ex.Message}");
            }

        }

        private int GetInputNumber() //Simple but effective method for retrieving a user input as a double
        {
            int userInput;
            while (true)
            {
                string? inputNumber = Console.ReadLine();

                if (int.TryParse(inputNumber, out userInput))
                {
                    break;
                }

                Console.WriteLine("Try a valid input number!");
            }

            return userInput;
        }
    }
}
