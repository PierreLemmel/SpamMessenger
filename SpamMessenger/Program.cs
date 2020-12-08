using OpenQA.Selenium;
using OpenQA.Selenium.Opera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SpamMessenger
{
    public static class Program
    {
        private static readonly Random rng = new Random();

        public static string Target => Targets.Juliette;
        public static bool Stop => false;
        public static string Message => MessagePool[rng.Next(MessagePool.Count)];
        public static int MessageDelay => rng.Next(500, 1500);

        public static IReadOnlyList<string> MessagePool => MessagePools.Bidou;

        public static class Delay
        {
            public static Task Technic => Task.Delay(200);
            public static Task Message => Task.Delay(MessageDelay);

            public static async Task While(Func<bool> condition)
            {
                while (condition())
                {
                    await Technic;
                }
            }

            public static Task Until(Func<bool> condition) => While(() => !condition());
        }

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting driver");
            string operaDriverDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Config.DriverFolder, "Opera");
            using IWebDriver driver = new OperaDriver(operaDriverDirectory);
            Console.WriteLine("Driver started");

            Console.WriteLine("opening messenger");
            string targetUrl = Config.BaseUrl + Target;
            driver.Url = targetUrl;
            Console.WriteLine("Messenger opened");

            Console.WriteLine("Waiting for facebook authentication...");
            await Delay.Until(() => driver.Url == targetUrl);
            Console.WriteLine("Authenticated to facebook");

            while (!Stop)
            {
                string msg = Message;
                
                IWebElement textbox = driver.FindElement(By.CssSelector("div[aria-autocomplete=list] span"));
                textbox.SendKeys(msg);

                IWebElement button = driver.FindElement(By.CssSelector("a[aria-label=Envoyer][role=button]"));
                button.Click();

                Console.WriteLine($"Message sent: '{msg}'");

                await Delay.Message;
            }

            Console.ReadKey();
            Console.WriteLine("End of program");
        }


        public static class Config
        {
            public const string DriverFolder = "../../../../Drivers";
            public const string BaseUrl = "https://www.messenger.com/t/";
        } 
    }

    public static partial class Targets
    {
        // Implement targets in other file
    }

    public static partial class MessagePools
    {
        // Implement targets in other file
    }
}
