using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;


namespace RoxImageClassifier
{
    public class ImageClassifications
    {
        static CustomVisionPredictionClient prediction_client;
        static IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        static IConfigurationRoot configuration = builder.Build();
        static Guid project_id = Guid.Parse(configuration["ProjectID"]);
        static string model_name = configuration["ModelName"];
        public ImageClassifications()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            string prediction_endpoint = configuration["PredictionEndpoint"];
            string prediction_key = configuration["PredictionKey"];
            //Guid project_id = Guid.Parse(configuration["ProjectID"]);
            //string model_name = configuration["ModelName"];

            // Authenticate a client for the prediction API
            prediction_client = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(prediction_key))
            {
                Endpoint = prediction_endpoint
            };
        }

        public async Task ClassifyImageURLAsync(string imageURL)
        {
            var result = await prediction_client.ClassifyImageUrlAsync(project_id, model_name, new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models.ImageUrl(imageURL));

            foreach (var prediction in result.Predictions)
            {
                Console.WriteLine($"Tag - {prediction.TagName} with probability of: {prediction.Probability:P1}");
            }
        }

        public async Task ClassifyImageFileAsync(string imageFilePath)
        {
            MemoryStream image_data = new MemoryStream(File.ReadAllBytes(imageFilePath));
            var result = await prediction_client.ClassifyImageAsync(project_id, model_name, image_data);

            // Loop over each label prediction and print any with probability > 50%
            foreach (var prediction in result.Predictions)
            {
                Console.WriteLine($"Tag - {prediction.TagName} with probability of: {prediction.Probability:P1}");
            }
        }
    }
}
