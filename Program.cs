using System;
using System.Threading.Tasks;
using RoxImageClassifier;

class Program
{
    static async Task Main(string[] args)
    {

        int menuChoice;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("- Welcome to Rox image analysis and face expression classification app -");
            Console.WriteLine(" here you can either get a complete image analys for any picture ");
            Console.WriteLine("  or get a face expression classification from any human face ");
            Console.WriteLine("");
            Console.WriteLine("(1) - Analys any image URL");
            Console.WriteLine("(2) - Face expression classification (either from image file or URL");
            Console.WriteLine("(8) - Exit to menu");
            menuChoice = GetInputNumber();
            switch (menuChoice)
            {
                case 1:
                    Console.Clear();
                    await UseImageAnalisys();
                    break;
                case 2:
                    Console.Clear();
                    await UseImageClassifications();
                    break;
                case 8:
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Please select option 1, 2 or 8.");
                    break;
            }
            if (menuChoice == 8) break;
        }


    }

    static async Task UseImageAnalisys()
    {
        // Skapar en instans av ImageAnalys-klassen
        ImageAnalys imageAnalys = new ImageAnalys();

        // Hämtar bildfilens sökväg och namn
        Console.WriteLine("Please enter the path to the image file:");
        string imageFile = Console.ReadLine();
        // Analyserar bilden
        await imageAnalys.AnalyzeImageAsync(imageFile);

        while (true)
        {
            Console.WriteLine("Would you like to create a thumbnail picture?");
            Console.Write("J / N: ");
            string choice = Console.ReadLine();
            if (choice.ToLower() == "n") break;
            await imageAnalys.GetThumbnail(imageFile);
        }
        
    }

    static async Task UseImageClassifications()
    {
        // Samma här, instans av ImageClasification-klassen
        ImageClassifications imageClassifications = new ImageClassifications();
        int userChoice;
        while (true)
        {
            Console.WriteLine("Would you like to scan a face selected from a file path or a URL path?");
            Console.WriteLine("(1) - Enter image file path");
            Console.WriteLine("(2) - Enter image URL");
            Console.WriteLine("(8) - Exit to menu");
            userChoice = GetInputNumber();
            switch (userChoice)
            {

                case 1:
                    Console.WriteLine("Please enter the path to the image file:");
                    string imageFile = Console.ReadLine();
                    // Hämtar bildfilens sökväg och namn och kör classidication analysen
                    await imageClassifications.ClassifyImageFileAsync(imageFile);
                        break;
                case 2:
                    Console.WriteLine("Please enter a image URL:");
                    string imageURL = Console.ReadLine();
                    // Hämtar bildens URL
                    await imageClassifications.ClassifyImageURLAsync(imageURL);
                    break;
                case 8:
                    break;
                default: Console.WriteLine("Only 1, 2 or 8 is valid inputs.");
                    break;
            }
            if (userChoice == 8) break;
        }
    }

    static int GetInputNumber() 
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
