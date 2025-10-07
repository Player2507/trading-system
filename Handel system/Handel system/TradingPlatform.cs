using System;
using System.Collections.Generic;
using System.IO;

namespace TradingSystem 
{

    // KLASS: TradingPlatform

    // Detta är HJÄRNAN i vårt system - den hanterar allt!

    public class TradingPlatform
    {

        Dictionary<string, User> users;
        Dictionary<int, Item> items;
        Dictionary<int, TradeRequest> tradeRequests;

        // Nuvarande inloggad användare (null om ingen är inloggad)
        User currentUser;

        // Räknare för att generera unika ID:n
        int nextItemId;
        int nextTradeId;

        // Filnamn för att spara data

        private string USERS_FILE = "users.txt";
        private string ITEMS_FILE = "items.txt";
        private string TRADES_FILE = "trades.txt";

        // KONSTRUKTOR: Sätter upp systemet
        public TradingPlatform()
        {
            users = new Dictionary<string, User>();
            items = new Dictionary<int, Item>();
            tradeRequests = new Dictionary<int, TradeRequest>();
            currentUser = null;
            nextItemId = 1;
            nextTradeId = 1;

            // Laddar automatiskt data när systemet startar
            LoadAllData();
        }

        // ANVÄNDARHANTERINGSMETODER 2

        // Registrera en ny användare
        // RETURNERAR: true om lyckat, false om användarnamnet är taget
        public bool Register(string username, string password)
        {
            if (users.ContainsKey(username))
            {
                return false; // Användarnamnet är taget!
            }
            User newUser = new User(username, password);
            users.Add(username, newUser);
            SaveUsers();
            return true;
        }

        // Logga in - kontrollera användarnamn och lösenord
        public bool Login(string username, string password)
        {
            if (users.TryGetValue(username, out var user) && user.Password == password)
            {
                currentUser = user;
                return true;
            }
            return false;
        }

        // Logga ut - rensa nuvarande användare
        public void Logout()
        {
            currentUser = null;
        }

        // Kontrollera om någon är inloggad
        public bool IsLoggedIn()
        {
            // Om currentUser är null, är ingen inloggad
            return currentUser != null;
        }

        // Hämta nuvarande användares användarnamn
        public string GetCurrentUsername()
        {
            // VARFÖR KOLLA EFTER NULL? Förhindrar krascher om ingen är inloggad
            if (currentUser == null)
            {
                return "Inte inloggad";
            }
            return currentUser.Username;
        }

        // OBJEKTHANTERINGSMETODER 

        // Ladda upp ett nytt objekt för byte
        public void UploadItem(string name, string description)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Du måste vara inloggad för att ladda upp objekt!");
                return;
            }
            Item newItem = new Item(nextItemId, name, description, currentUser.Username);
            nextItemId++;
            items.Add(newItem.Id, newItem);
            currentUser.AddItem(newItem);
            SaveItems();
            Console.WriteLine($"Objekt '{name}' uppladdat framgångsrikt!");
        }

        // Visa alla objekt från ANDRA användare (inte dina egna)
        public void BrowseItems()
        {
            if (currentUser == null)
            {
                Console.WriteLine("Du måste vara inloggad!");
                return;
            }

            Console.WriteLine("\n=== TILLGÄNGLIGA OBJEKT ===");
            bool foundItems = false;

            // Loopa igenom alla objekt med foreach
            foreach (var item in items.Values)
            {
                // Visa endast objekt från ANDRA användare med != jämförelse
                if (item.OwnerUsername != currentUser.Username)
                {
                    Console.WriteLine(item.ToString());
                    foundItems = true;
                }
            }

            if (!foundItems)
            {
                Console.WriteLine("Inga objekt tillgängliga för byte.");
            }
        }

        // BYTESFÖRFRÅGNINGSMETODER 

        // Begär att byta mot ett objekt
        public void RequestTrade(int itemId)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Du måste vara inloggad!");
                return;
            }
            if (!items.TryGetValue(itemId, out var item))
            {
                Console.WriteLine("Objektet hittades inte!");
                return;
            }
            if (item.OwnerUsername == currentUser.Username)
            {
                Console.WriteLine("Du kan inte byta mot ditt eget objekt!");
                return;
            }
            TradeRequest trade = new TradeRequest(nextTradeId, currentUser.Username, item.OwnerUsername, itemId);
            nextTradeId++;
            tradeRequests.Add(trade.Id, trade);
            SaveTrades();
            Console.WriteLine("Bytesförfrågan skickad!");
        }

        // Visa bytesförfrågningar där DU är ägaren
        public void ViewTradeRequests()
        {
            if (currentUser == null)
            {
                Console.WriteLine("Du måste vara inloggad!");
                return;
            }

            Console.WriteLine("\n=== DINA BYTESFÖRFRÅGNINGAR ===");
            bool foundRequests = false;

            foreach (var trade in tradeRequests.Values)
            {
                // Visa väntande förfrågningar där nuvarande användare är ägaren
                if (trade.OwnerUsername == currentUser.Username &&
                    trade.Status == TradeStatus.Pending)
                {
                    Console.WriteLine(trade.ToString());
                    foundRequests = true;
                }
            }

            if (!foundRequests)
            {
                Console.WriteLine("Inga väntande bytesförfrågningar.");
            }
        }

        // Acceptera en bytesförfrågan
        public void AcceptTrade(int tradeId)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Du måste vara inloggad!");
                return;
            }

            TradeRequest trade = FindTradeById(tradeId);

            if (trade == null)
            {
                Console.WriteLine("Bytesförfrågan hittades inte!");
                return;
            }

            // Verifiera att du äger denna bytesförfrågan
            if (trade.OwnerUsername != currentUser.Username)
            {
                Console.WriteLine("Detta är inte din bytesförfrågan!");
                return;
            }

            // Kan endast acceptera väntande byten
            if (trade.Status != TradeStatus.Pending)
            {
                Console.WriteLine("Detta byte har redan behandlats!");
                return;
            }

            // Ändra status till Accepterad
            trade.Status = TradeStatus.Accepted;

            // Överför objektets ägarskap
            TransferItem(trade.ItemId, trade.RequesterUsername);

            SaveTrades();
            SaveItems();

            Console.WriteLine("Byte accepterat!");
        }

        // Neka en bytesförfrågan
        public void DenyTrade(int tradeId)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Du måste vara inloggad!");
                return;
            }

            TradeRequest trade = FindTradeById(tradeId);

            if (trade == null)
            {
                Console.WriteLine("Bytesförfrågan hittades inte!");
                return;
            }

            if (trade.OwnerUsername != currentUser.Username)
            {
                Console.WriteLine("Detta är inte din bytesförfrågan!");
                return;
            }

            if (trade.Status != TradeStatus.Pending)
            {
                Console.WriteLine("Detta byte har redan behandlats!");
                return;
            }

            trade.Status = TradeStatus.Denied;
            SaveTrades();

            Console.WriteLine("Byte nekat!");
        }

        // Visa genomförda byten (accepterade eller nekade)
        public void ViewCompletedTrades()
        {
            if (currentUser == null)
            {
                Console.WriteLine("Du måste vara inloggad!");
                return;
            }

            Console.WriteLine("\n=== GENOMFÖRDA BYTEN ===");
            bool foundTrades = false;

            foreach (var trade in tradeRequests.Values)
            {
                // Visa byten som involverar nuvarande användare som INTE är väntande
                if ((trade.OwnerUsername == currentUser.Username ||
                     trade.RequesterUsername == currentUser.Username) &&
                    trade.Status != TradeStatus.Pending)
                {
                    Console.WriteLine(trade.ToString());
                    foundTrades = true;
                }
            }

            if (!foundTrades)
            {
                Console.WriteLine("Inga genomförda byten.");
            }
        }



        // Hitta objekt med ID
        Item FindItemById(int id)
        {
            items.TryGetValue(id, out var item);
            return item;
        }

        // Hitta byte med ID
        TradeRequest FindTradeById(int id)
        {
            tradeRequests.TryGetValue(id, out var trade);
            return trade;
        }

        // Hitta användare med användarnamn
        User FindUserByUsername(string username)
        {
            users.TryGetValue(username, out var user);
            return user;
        }

        // Överför objektets ägarskap
        void TransferItem(int itemId, string newOwnerUsername)
        {
            Item item = FindItemById(itemId);
            User oldOwner = FindUserByUsername(item.OwnerUsername);
            User newOwner = FindUserByUsername(newOwnerUsername);

            if (item == null || oldOwner == null || newOwner == null)
            {
                return;
            }

            // Ta bort från gammal ägare
            oldOwner.RemoveItem(item);

            // Ändra ägarskap
            item.OwnerUsername = newOwnerUsername;

            // Lägg till hos ny ägare
            newOwner.AddItem(item);
        }

        // FILSPARA/LADDNINGSMETODER 
        // Dessa använder TRY-CATCH för felhantering!
        // VARFÖR TRY-CATCH? Filer kan misslyckas (låsta, saknade, disk full)
        // Utan try-catch skulle programmet KRASCHA!

        void SaveUsers()
        {
            try
            {
                string[] lines = new string[users.Count];
                int i = 0;
                foreach (var user in users.Values)
                {
                    lines[i++] = user.ToFileString();
                }
                File.WriteAllLines(USERS_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid sparande av användare: {ex.Message}");
            }
        }

        void SaveItems()
        {
            try
            {
                string[] lines = new string[items.Count];
                int i = 0;
                foreach (var item in items.Values)
                {
                    lines[i++] = item.ToFileString();
                }
                File.WriteAllLines(ITEMS_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid sparande av objekt: {ex.Message}");
            }
        }

        void SaveTrades()
        {
            try
            {
                string[] lines = new string[tradeRequests.Count];
                int i = 0;
                foreach (var trade in tradeRequests.Values)
                {
                    lines[i++] = trade.ToFileString();
                }
                File.WriteAllLines(TRADES_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid sparande av byten: {ex.Message}");
            }
        }

        void LoadAllData()
        {
            LoadUsers();
            LoadItems();
            LoadTrades();
            UpdateIdCounters(); // Se till att ID:n inte överlappar
        }

        void LoadUsers()
        {
            try
            {
                if (!File.Exists(USERS_FILE))
                {
                    return;
                }
                string[] lines = File.ReadAllLines(USERS_FILE);
                foreach (var line in lines)
                {
                    if (line.Trim() != "")
                    {
                        User user = User.FromFileString(line);
                        users.Add(user.Username, user);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid laddning av användare: {ex.Message}");
            }
        }

        void LoadItems()
        {
            try
            {
                if (!File.Exists(ITEMS_FILE))
                {
                    return;
                }
                string[] lines = File.ReadAllLines(ITEMS_FILE);
                foreach (var line in lines)
                {
                    if (line.Trim() != "")
                    {
                        Item item = Item.FromFileString(line);
                        items.Add(item.Id, item);
                        User owner = FindUserByUsername(item.OwnerUsername);
                        if (owner != null)
                        {
                            owner.AddItem(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid laddning av objekt: {ex.Message}");
            }
        }

        void LoadTrades()
        {
            try
            {
                if (!File.Exists(TRADES_FILE))
                {
                    return;
                }
                string[] lines = File.ReadAllLines(TRADES_FILE);
                foreach (var line in lines)
                {
                    if (line.Trim() != "")
                    {
                        TradeRequest trade = TradeRequest.FromFileString(line);
                        tradeRequests.Add(trade.Id, trade);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid laddning av byten: {ex.Message}");
            }
        }

        // Se till att nya ID:n inte kommer i konflikt med laddad data, fick problem med att ID:n återställdes till 1 vid varje körning
        void UpdateIdCounters()
        {
            // Hitta högsta objekt-ID
            foreach (var item in items.Values)
            {
                if (item.Id >= nextItemId)
                {
                    nextItemId = item.Id + 1;
                }
            }

            // Hitta högsta byte-ID
            foreach (var trade in tradeRequests.Values)
            {
                if (trade.Id >= nextTradeId)
                {
                    nextTradeId = trade.Id + 1;
                }
            }
        }
    }
}