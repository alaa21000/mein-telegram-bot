using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TelegramBot
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            string token = "8358888054:AAGuWYUbxmKwLvxgJBJolFKp8U2pfQnZhaE";
            long chatId = 5712437248;

            string url = $"https://api.telegram.org/bot{token}/sendMessage";

            var message = new
            {
                chat_id = chatId,
                text = "🚀 HTTP Bot LEBENDIG! Kein NuGet-Chaos mehr! 😎"
            };

            var json = JsonConvert.SerializeObject(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Response: " + responseString);
            Console.ReadLine();
        }
    }
}
