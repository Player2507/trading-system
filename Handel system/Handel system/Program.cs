using System;
// using System är NÖDVÄNDIGT för att kunna använda Console
// "using" betyder "importera" - vi importerar Systems funktionalitet
// System innehåller grundläggande klasser som Console, String, Int32, osv.

// Samma namespace som alla andra filer - detta är viktigt!
// Om detta namespace är annorlunda kommer klassen inte att kunna hitta
// User, Item, TradeRequest eller TradingSystem klasserna.
namespace TradingSystem
{

    // PROGRAM-KLASSEN - Programmets startpunkt


    class Program
    {
        static void Main(string[] args)
        {
            // Skapar vår handelsplattform - detta laddar även sparad data automatiskt
            TradingPlatform system = new TradingPlatform();

            // Huvudprogrammets loop - fortsätter köra tills användaren avslutar
            // VARFÖR WHILE(TRUE)? Vi vill att programmet ska fortsätta köra för evigt tills användaren väljer att avsluta
            // WHILE-LOOP: Upprepar kod så länge villkoret är sant
            bool running = true;
            while (running)
            {
                // Kontrollerar om användaren är inloggad för att visa rätt meny
                // IF-SATS: Kör kod bara om villkoret är sant
                if (!system.IsLoggedIn())
                {
                    running = ShowLoginMenu(system);
                }
                else
                {
                    // ELSE: Körs när if-villkoret är falskt
                    ShowMainMenu(system);
                }
            }

            Console.WriteLine("Tack för att du använde Trading System!");
        }

        // Visar meny för användare som INTE är inloggade
        // RETURNERAR: false om användaren vill avsluta, true annars
        // METOD: En återanvändbar kodblock som utför en specifik uppgift
        // STATIC: Metoden tillhör klassen, inte ett specifikt objekt
        // RETURN TYPE (bool): Metoden returnerar ett sant/falskt värde
        // PARAMETERS: system är input till metoden
        static bool ShowLoginMenu(TradingPlatform system)
        {
            Console.WriteLine("\n==== TRADING SYSTEM ====");
            Console.WriteLine("1. Registrera");
            Console.WriteLine("2. Logga in");
            Console.WriteLine("3. Avsluta");
            Console.Write("Välj ett alternativ: ");

            // ReadLine() läser användarinput från tangentbordet
            // VARIABEL: Lagrar data (här en sträng) för senare användning
            string choice = Console.ReadLine();

            // SWITCH-SATS: Jämför en variabel mot flera möjliga värden
            // VARFÖR SWITCH? Renare kod än flera if-else när man kollar en och samma variabel
            switch (choice)
            {
                case "1":
                    HandleRegister(system);
                    break; // BREAK: Avslutar switch-satsen
                case "2":
                    HandleLogin(system);
                    break;
                case "3":
                    return false; // RETURN: Avslutar metoden och skickar tillbaka ett värde
                default: // DEFAULT: Körs om inget case matchar
                    Console.WriteLine("Ogiltigt alternativ!");
                    break;
            }

            return true; // Fortsätt köra programmet
        }

        // Visar meny för inloggade användare
        static void ShowMainMenu(TradingPlatform system)
        {
            // STRING INTERPOLATION: $"{variabel}" sätter in variabelvärden i strängar
            Console.WriteLine($"\n====== Välkommen, {system.GetCurrentUsername()}! ======");
            Console.WriteLine("1. Ladda upp föremål");
            Console.WriteLine("2. Bläddra bland föremål");
            Console.WriteLine("3. Begär byte");
            Console.WriteLine("4. Visa bytesförfrågningar");
            Console.WriteLine("5. Acceptera byte");
            Console.WriteLine("6. Neka byte");
            Console.WriteLine("7. Visa genomförda byten");
            Console.WriteLine("8. Logga ut");
            Console.Write("Välj ett alternativ: ");

            string choice = Console.ReadLine();

            // Ytterligare en switch-sats för menyval
            switch (choice)
            {
                case "1":
                    HandleUploadItem(system);
                    break;
                case "2":
                    // METODANROP: Kör en metod från ett objekt
                    system.BrowseItems();
                    break;
                case "3":
                    HandleRequestTrade(system);
                    break;
                case "4":
                    system.ViewTradeRequests();
                    break;
                case "5":
                    HandleAcceptTrade(system);
                    break;
                case "6":
                    HandleDenyTrade(system);
                    break;
                case "7":
                    system.ViewCompletedTrades();
                    break;
                case "8":
                    system.Logout();
                    Console.WriteLine("Utloggad!");
                    break;
                default:
                    Console.WriteLine("Ogiltigt alternativ!");
                    break;
            }
        }

        //HANDLER-METODER 
        // Dessa metoder hanterar användarinput för olika åtgärder

        // VOID: Metoden returnerar inget värde
        static void HandleRegister(TradingPlatform system)
        {
            Console.Write("Ange användarnamn: ");
            string username = Console.ReadLine();

            Console.Write("Ange lösenord: ");
            string password = Console.ReadLine();

            // Anropar register-metoden - den returnerar true/false
            // BOOLEAN: En variabel som bara kan vara true eller false
            bool success = system.Register(username, password);

            // IF-sats för att kontrollera resultatet
            if (success)
            {
                Console.WriteLine("Registrering lyckades! Du kan nu logga in.");
            }
            else
            {
                Console.WriteLine("Användarnamnet är upptaget!");
            }
        }

        static void HandleLogin(TradingPlatform system)
        {
            Console.Write("Ange användarnamn: ");
            string username = Console.ReadLine();

            Console.Write("Ange lösenord: ");
            string password = Console.ReadLine();

            bool success = system.Login(username, password);

            if (success)
            {
                Console.WriteLine("Inloggning lyckades!");
            }
            else
            {
                Console.WriteLine("Ogiltigt användarnamn eller lösenord!");
            }
        }

        static void HandleUploadItem(TradingPlatform system)
        {
            Console.Write("Ange föremålets namn: ");
            string name = Console.ReadLine();

            Console.Write("Ange föremålets beskrivning: ");
            string description = Console.ReadLine();

            system.UploadItem(name, description);
        }

        static void HandleRequestTrade(TradingPlatform system)
        {
            Console.Write("Ange ID för föremål du vill byta mot: ");
            string input = Console.ReadLine();

            // TRY-CATCH: Felhantering!
            // VARFÖR? Användaren kan skriva "abc" istället för ett nummer, vilket orsakar en krasch
            // Try-catch förhindrar kraschen och hanterar felet på ett kontrollerat sätt
            try
            {
                // TYPE CONVERSION: Konverterar från string till int
                int itemId = int.Parse(input); // Konverterar sträng till heltal
                system.RequestTrade(itemId);
            }
            catch (Exception)
            {
                // EXCEPTION HANDLING: Denna kod körs om ett fel uppstår i try-blocket
                Console.WriteLine("Ogiltigt föremåls-ID! Ange ett nummer.");
            }
        }

        static void HandleAcceptTrade(TradingPlatform system)
        {
            Console.Write("Ange ID för bytesförfrågan att acceptera: ");
            string input = Console.ReadLine();

            try
            {
                int tradeId = int.Parse(input);
                system.AcceptTrade(tradeId);
            }
            catch (Exception)
            {
                Console.WriteLine("Ogiltigt byte-ID! Ange ett nummer.");
            }
        }

        static void HandleDenyTrade(TradingPlatform system)
        {
            Console.Write("Ange ID för bytesförfrågan att neka: ");
            string input = Console.ReadLine();

            try
            {
                int tradeId = int.Parse(input);
                system.DenyTrade(tradeId);
            }
            catch (Exception)
            {
                Console.WriteLine("Ogiltigt byte-ID! Ange ett nummer.");
            }
        }
    }
}