using System;

namespace TradingSystem
{

    // ENUM: TradeStatus

    // VARFÖR ENUM? Vi har exakt 3 statusar - inte oändliga möjligheter
    // Enums förhindrar stavfel och gör koden tydligare än att använda strängar

    public enum TradeStatus
    {
        Pending,   // Väntar på svar
        Accepted,  // Bytet genomfört!
        Denied     // Nekat
    }


    // CLASS: TradeRequest

    // Representerar ett byteserbjudande mellan två användare

    public class TradeRequest
    {
        public int Id { get; set; }  // Unikt ID för denna bytesförfrågan

        // Vem begär bytet?
        public string RequesterUsername { get; set; }

        // Vem äger föremålet som efterfrågas?
        public string OwnerUsername { get; set; }

        // Vilket föremål efterfrågas?
        public int ItemId { get; set; }

        // Vad är nuvarande status?
        // Vi använder TradeStatus enum istället för string för typsäkerhet
        public TradeStatus Status { get; set; }

        // KONSTRUKTOR
        public TradeRequest(int id, string requesterUsername, string ownerUsername, int itemId)
        {
            Id = id;
            RequesterUsername = requesterUsername;
            OwnerUsername = ownerUsername;
            ItemId = itemId;
            Status = TradeStatus.Pending; // Börjar alltid som Pending
        }

        // ToString() för enkel visning
        public override string ToString()
        {
            return $"[Byte #{Id}] {RequesterUsername} vill ha föremål #{ItemId} från {OwnerUsername} - Status: {Status}";
        }

        // Sparformat: "id|requester|owner|itemId|status"
        public string ToFileString()
        {
            // Konvertera enum till sträng (Pending, Accepted, eller Denied)
            return $"{Id}|{RequesterUsername}|{OwnerUsername}|{ItemId}|{Status}";
        }

        // Ladda från sparad sträng
        public static TradeRequest FromFileString(string fileString)
        {
            string[] parts = fileString.Split('|');
            int id = int.Parse(parts[0]);
            int itemId = int.Parse(parts[3]);

            // Skapa bytesförfrågan
            TradeRequest trade = new TradeRequest(id, parts[1], parts[2], itemId);

            // Tolka status-strängen tillbaka till enum
            // Enum.Parse konverterar "Pending"-text till TradeStatus.Pending
            trade.Status = (TradeStatus)Enum.Parse(typeof(TradeStatus), parts[4]);

            return trade;
        }
    }
}