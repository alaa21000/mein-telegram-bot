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

            Console.WriteLine("🛒 SHOP BOT VOLL LÄUFT! Schreib: hallo/preis/menü");

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

                                Console.WriteLine($"Chat {chatId}: {text}");

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

            if (lower == "hallo" || lower == "/start")
                return "😊 Hi Rex! Willkommen im SHOP-BOT!\n\n🛒 /preis - Alle Preise\n🍽️ /menü - Shop-Menü\n❓ /hilfe - Hilfe";
            else if (lower == "preis" || lower == "/preis")
                return "🛒 PREISE:\n• Milch: 1.00€\n• Brot: 2.00€\n• Äpfel: 3.00€\n• Banane: 1.50€\n\n💳 /menü zum Kaufen!";
            else if (lower == "menü" || lower == "/menü")
                return "🍽️ SHOP-MENÜ:\n1️⃣ /preis - Preise\n2️⃣ /kaufen Milch - Milch kaufen\n3️⃣ /kaufen Brot - Brot kaufen\n4️⃣ /korb - Warenkorb\n❓ /hilfe";
            else if (lower == "hilfe" || lower == "/hilfe")
                return "🤖 SHOP-BOT:\n• /start - Neustart\n• /preis - Preise\n• /menü - Menü\n• /kaufen [Produkt]\n• /korb - Warenkorb";
            else if (lower.StartsWith("/kaufen "))
            {
                string produkt = lower.Substring(7);
                return $"✅ {produkt} in Warenkorb! (Demo)\n\n/korb - Warenkorb ansehen";
            }
            else if (lower == "/korb")
                return "🛍️ WARNKORB:\n• Milch x1 (1€)\n• Brot x1 (2€)\n\n💰 Gesamt: 3€\n\n/menü - Weiter einkaufen";
            else
                return "❓ Befehl nicht erkannt.\nSchreib /hilfe oder /menü";
        }
    }
}
