using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            HttpClient client = new HttpClient();
            string token = "8358888054:AAGuWYUbxmKwLvxgJBJolFKp8U2pfQnZhaE";
            long offset = 0;

            Console.WriteLine("🔐 PASSWORT-BOT läuft! /passwort 12 testen!");

            while (true)
            {
                try
                {
                    string updatesUrl = $"https://api.telegram.org/bot{token}/getUpdates?offset={offset}";
                    string response = await client.GetStringAsync(updatesUrl);
                    var json = JObject.Parse(response);

                    if (json["ok"].Value<bool>())
                    {
                        var updates = json["result"] as JArray;
                        if (updates != null)
                        {
                            foreach (var update in updates)
                            {
                                offset = (long)update["update_id"] + 1;
                                long chatId = (long)update["message"]["chat"]["id"];
                                string text = update["message"]["text"].Value<string>() ?? "";

                                Console.WriteLine($"Rex: {text}");

                                string antwort = GetAntwort(text);

                                string sendUrl = $"https://api.telegram.org/bot{token}/sendMessage";
                                string jsonMsg = "{\"chat_id\":" + chatId + ",\"text\":\"" + antwort.Replace("\"", "\\\"") + "\"}";

                                var content = new StringContent(jsonMsg, Encoding.UTF8, "application/json");
                                await client.PostAsync(sendUrl, content);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler: " + ex.Message);
                }

                await Task.Delay(1500);
            }
        }

        static string GetAntwort(string nachricht)
        {
            string lower = nachricht.ToLower().Trim();

            if (lower.StartsWith("/passwort "))
            {
                string laengeStr = lower.Substring(10).Trim();
                int laenge = 12;

                if (int.TryParse(laengeStr, out int l) && l >= 6 && l <= 20)
                    laenge = l;

                return GenerierePasswort(laenge);
            }
            else if (lower == "/passwort")
                return GenerierePasswort(12);
            else if (lower == "/start" || lower == "hallo")
                return "🔐 PASSWORT-GENERATOR\n\n/passwort 12 - 12 Zeichen Passwort\n/passwort 16 - 16 Zeichen Passwort\n/passwort - Standard 12 Zeichen\n\n💡 Länge 6-20 möglich!";
            else if (lower == "/hilfe")
                return "🤖 PASSWORT-BOT:\n• /passwort [6-20] - Passwort generieren\n• /passwort - Standard 12 Zeichen\n• /start - Willkommen\n• /hilfe - Diese Hilfe";
            else
                return "❓ Unbekannter Befehl\n\n🔐 /passwort 12\n? /hilfe";
        }

        static string GenerierePasswort(int laenge)
        {
            Random rand = new Random();
            string zeichen = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+-=[]{}|;:,.<>?";

            string passwort = "";
            for (int i = 0; i < laenge; i++)
            {
                int index = rand.Next(zeichen.Length);
                passwort += zeichen[index];
            }

            return $"🔐 Passwort ({laenge} Zeichen):\n\n`{passwort}`\n\n✅ Kopiere es!\n\n/passwort [andere Länge]";
        }
    }
}
