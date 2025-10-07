using System;
using System.Collections.Generic;

namespace TradingSystem
{

    // KLASS: User

    // Representerar en person som använder systemet

    public class User
    {
        // VARFÖR PROPERTIES? 
        // Properties (get/set) skyddar vår data från att ändras felaktigt
        // Detta kallas inkaplsling - vi kontrollerar åtkomst till data

        // Auto-properties: C# skapar automatiskt ett dolt fält åt oss
        public string Username { get; set; }  // Användarens namn
        public string Password { get; set; }  // Hemligt lösenord 

        // Denna lista innehåller alla föremål som denna användare äger
        // VARFÖR List<Item>? För att en användare kan ha MÅNGA föremål (0, 1 eller 100!)
        // Vi använder List istället för array eftersom listor kan växa/krympa dynamiskt
        public List<Item> Items { get; set; }

        // KONSTRUKTOR - Denna körs när vi skapar en ny User

        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Items = new List<Item>(); // Börja med en tom lista av föremål
        }

        // METOD: Lägg till ett föremål till denna användares samling
        // VARFÖR EN METOD? Den organiserar kod - "användare kan lägga till föremål"
        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        // METOD: Ta bort ett föremål (används när bytet slutförs)
        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }

        // ToString() METOD 
        // Detta berättar för C# hur man visar en User som text
        // Mycket användbart för felsökning och att visa information
        public override string ToString()
        {
            return $"User: {Username} (Items: {Items.Count})";
        }

        // METOD: Konvertera användare till ett sparbart strängformat
        // Format: "användarnamn|lösenord"
        // VARFÖR? Vi behöver spara till fil, och filer är bara text!
        public string ToFileString()
        {
            return $"{Username}|{Password}";
        }

        // STATISK METOD: Skapa en User från en sparad sträng
        // VARFÖR STATISK? För att vi skapar en NY användare, inte använder en befintlig
        // Statiska metoder tillhör själva KLASSEN, inte ett objekt
        public static User FromFileString(string fileString)
        {
            // Dela upp strängen vid |-tecknet
            string[] parts = fileString.Split('|');
            return new User(parts[0], parts[1]);
        }
    }
}