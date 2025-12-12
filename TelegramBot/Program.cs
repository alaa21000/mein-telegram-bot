using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TelegramBot
{
    class Program
    {
        // DEINE PRIVATEN STATS
        static int totalUsers = 0;
        static int totalPasswords = 0;
        static int totalGames = 0;
        static Dictionary<long, int> userPasswords = new Dictionary<long, int>();

        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            HttpClient client = new HttpClient();
            string token = "8358888054:AAGuWYUbxmKwLvxgJBJolFKp8U2pfQnZhaE";
            long offset = 0;

            Console.WriteLine("🔐 PASSWORD + GAME BOT running! 16 chars only");
            Console.WriteLine("📊 STATS: Users | Passwords | Games");

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

                                if (update["message"] == null)
                                    continue;

                                long chatId = (long)update["message"]["chat"]["id"];
                                string text = update["message"]["text"] != null
                                    ? update["message"]["text"].Value<string>()
                                    : "";

                                // DEINE PRIVATEN STATS - NEUER USER?
                                if (!userPasswords.ContainsKey(chatId))
                                {
                                    userPasswords[chatId] = 0;
                                    totalUsers++;
                                    Console.WriteLine($"🆕 USER #{totalUsers}: {chatId}");
                                }

                                Console.WriteLine($"Chat {chatId}: {text}");

                                string answer = GetAnswer(text);
                                string sendUrl = $"https://api.telegram.org/bot{token}/sendMessage";

                                // 2 MESSAGES für Passwörter!
                                if (answer.Length > 10 && answer.Length < 25) // Passwort erkannt
                                {
                                    userPasswords[chatId]++;
                                    totalPasswords++;
                                    Console.WriteLine($"🔐 STATS: {totalUsers} users | {totalPasswords} pw | User{chatId}: {userPasswords[chatId]}");

                                    // 1. NUR PASSWORT
                                    string jsonMsg1 = "{\"chat_id\":" + chatId + ",\"text\":\"" + answer.Replace("\"", "\\\"") + "\"}";
                                    var content1 = new StringContent(jsonMsg1, Encoding.UTF8, "application/json");
                                    await client.PostAsync(sendUrl, content1);

                                    // 2. MENU
                                    await Task.Delay(500);
                                    string menuMsg = "📋 /menu → Main Menu";
                                    string jsonMsg2 = "{\"chat_id\":" + chatId + ",\"text\":\"" + menuMsg.Replace("\"", "\\\"") + "\"}";
                                    var content2 = new StringContent(jsonMsg2, Encoding.UTF8, "application/json");
                                    await client.PostAsync(sendUrl, content2);
                                }
                                else
                                {
                                    // Normale Nachricht
                                    string jsonMsg = "{\"chat_id\":" + chatId + ",\"text\":\"" + answer.Replace("\"", "\\\"") + "\"}";
                                    var content = new StringContent(jsonMsg, Encoding.UTF8, "application/json");
                                    await client.PostAsync(sendUrl, content);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                await Task.Delay(1500);
            }
        }

        static string GetAnswer(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "❓ Empty. /menu";

            string lower = message.Trim().ToLower();

            // MAIN MENU
            if (lower == "/menu" || lower == "/help" || lower == "/start")
            {
                return "🤖 MAIN MENU:\n\n" +
                       "🔐 PASSWORDS:\n" +
                       "• /password     → 16 chars\n\n" +
                       "🎮 GAMES:\n" +
                       "• /game         → Rock Paper Scissors";
            }

            // Password: IMMER 16 Zeichen!
            if (lower == "/password" || lower.StartsWith("/password 16"))
            {
                return GeneratePassword(16);
            }
            else if (lower.StartsWith("/password "))
            {
                return "⚠️ Only /password (16 chars)\n\n/menu";
            }

            // Rock Paper Scissors
            else if (lower == "/game" || lower == "/play" || lower == "/rockpaperscissors")
            {
                return StartGame();
            }
            else if (lower == "/rock" || lower == "/paper" || lower == "/scissors")
            {
                totalGames++;
                Console.WriteLine($"🎮 STATS: {totalGames} games total");
                return PlayGame(lower);
            }
            else
            {
                return "❓ Unknown.\n\n/menu";
            }
        }

        static string GeneratePassword(int length)
        {
            Random rand = new Random();
            string safeFirst = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // KEIN # zuerst!
            string allChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@$%^&*()_+-=[]{}|;:,.<>?";

            string password = "";

            // 1. ERSTES Zeichen: NIE #!
            password += safeFirst[rand.Next(safeFirst.Length)];

            // REST: Alle Zeichen OK
            for (int i = 1; i < length; i++)
            {
                password += allChars[rand.Next(allChars.Length)];
            }

            return password; // 16 chars, NIE mit #!
        }

        static string StartGame()
        {
            return "🎮 ROCK PAPER SCISSORS!\n\n" +
                   "Choose:\n• /rock\n• /paper\n• /scissors\n\n/menu";
        }

        static string PlayGame(string myChoice)
        {
            string playerChoice = myChoice.TrimStart('/').ToLower();
            string[] botChoices = { "rock", "paper", "scissors" };
            Random rand = new Random();
            string bot = botChoices[rand.Next(3)];

            string result = "";
            if (playerChoice == bot)
                result = "🤝 TIE!";
            else if ((playerChoice == "rock" && bot == "scissors") ||
                     (playerChoice == "paper" && bot == "rock") ||
                     (playerChoice == "scissors" && bot == "paper"))
                result = "🎉 YOU WIN!";
            else
                result = "😎 I WIN!";

            return $"🎮 YOU: {playerChoice.ToUpper()} vs ME: {bot.ToUpper()}\n\n" +
                   result + "\n\n/game | /menu";
        }
    }
}
