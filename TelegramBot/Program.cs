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
            // HIER dein Bot-Code (while-Schleife, HttpClient, getUpdates, sendMessage ...)
        }

        static string GetAntwort(string nachricht)
        {
            // HIER deine Antwort-Logik (hallo, preis, /passwort ...)
            return "";
        }

        static string GenerierePasswort(int laenge)
        {
            Random rand = new Random();
            string zeichen = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";

            string passwort = "";
            for (int i = 0; i < laenge; i++)
            {
                int index = rand.Next(zeichen.Length);
                passwort += zeichen[index];
            }

            return $"Passwort ({laenge} Zeichen): {passwort}";
        }
    }
}
