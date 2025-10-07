using System;

namespace TradingSystem
{
    //
    // KLASS: Item
    // 
    // VARFÖR EN KLASS? För att samla all relaterad data och funktionalitet på ETT ställe
    // Istället för att ha lösa variabler överallt, grupperar vi allt som hör till ett föremål
    // 
    // Representerar något som kan bytas (som ett vapen eller bok)
    // En klass är som en RITNING/MALL för att skapa objekt - här definierar vi 
    // vilka EGENSKAPER (properties) och FUNKTIONER (metoder) som alla Item-objekt ska ha
    public class Item
    {
        // PROPERTIES (Egenskaper) - Data som varje Item-objekt lagrar
        // Detta är INKAPSLING - vi skyddar datan med get/set (kan styra åtkomst)

        // VARFÖR INKAPSLING? Så vi senare kan lägga till validering/kontroll
        // T.ex: "set { if (value > 0) Id = value; }" för att förhindra negativa ID

        // VARFÖR int för Id? Varje föremål behöver ett UNIKT nummer (som en streckkod)
        // Detta hjälper oss att skilja föremål åt även om de har samma namn
        // int är en DATATYP som lagrar heltal 
        // VARFÖR INTE string? int är snabbare att jämföra och tar mindre minne
        public int Id { get; set; }

        // string är DATATYP för text - kan innehålla bokstäver, siffror och symboler
        // VARFÖR string? Namn kan vara vad som helst - flexibelt för alla typer av föremål
        public string Name { get; set; }        // Vad är det? "Vapen"


        public string Description { get; set; }  // Mer detaljer: "Ett svärd"

        // VARFÖR OwnerUsername? Vi måste veta VEM som äger föremålet för att:
        // 1. Visa vem man byter med
        // 2. Förhindra att folk byter sina egna föremål med sig själva
        // 3. Kunna uppdatera ägarskap efter ett byte
        public string OwnerUsername { get; set; } // Vem äger det?

        // CONSTRUCTOR (Konstruktor): En SPECIELL METOD som körs när vi skapar ett nytt objekt
        // Den har SAMMA NAMN som klassen och har INGEN returtyp (inte ens void)
        // 
        // VARFÖR CONSTRUCTOR? För att GARANTERA att varje Item skapas med ALL nödvändig data

        // 
        // VARFÖR PARAMETRAR? Så vi kan skicka in olika värden när vi skapar olika objekt
        // T.ex: new Item(1, "Svärd", "Ett vasst svärd", "Anna")
        //       new Item(2, "Bok", "En gammal bok", "Erik")
        public Item(int id, string name, string description, string ownerUsername)
        {
            // Här använder vi TILLDELNING (assignment) för att sätta värden på properties

            // VARFÖR? För att KOPIERA värden från parametrar till objektets egenskaper
            // När objektet skapats kan vi komma åt dessa värden via item.Name, item.Id etc
            Id = id;
            Name = name;
            Description = description;
            OwnerUsername = ownerUsername;
        }

        // METOD: En funktion som tillhör klassen - definierar vad objektet kan GÖRA
        // override betyder att vi SKRIVER ÖVER en metod från basklassen Object
        // 
        // VARFÖR ToString()? För att enkelt kunna visa föremålet i konsolen eller UI
        // Utan denna metod skulle Console.WriteLine(item) bara visa "TradingSystem.Item"
        // Nu visar det: "[1] Svärd - Ett vasst svärd (Owner: Anna)" - MYCKET mer användbart!
        // 
        // RETURTYP: string (metoden ger tillbaka text)
        public override string ToString()
        {
            // STRING INTERPOLATION med $"" - enkelt sätt att sätta ihop text med värden
            // Vi använder {} för att infoga variabelvärden direkt i strängen

            return $"[{Id}] {Name} - {Description} (Owner: {OwnerUsername})";
        }

        // METOD för att konvertera till sparbart format för FILHANTERING
        // Format: "id|namn|beskrivning|ägare"
        // 
        // VARFÖR DENNA METOD? För att kunna SPARA föremål till en textfil
        // Vi kan inte spara C#-objekt direkt i en textfil - måste göra om till text först!


        public string ToFileString()
        {
            // Vi ersätter | med ett mellanslag om det finns i namn/beskrivning
            // för att förhindra att vårt sparformat går sönder

            // Replace() är en STRING-METOD som byter ut tecken
            string safeName = Name.Replace("|", " ");
            string safeDesc = Description.Replace("|", " ");

            // Returnerar en formaterad sträng - detta är SERIALISERING (göra data till text)
            // VARFÖR? Så datan kan sparas på disk och överleva när programmet stängs av
            return $"{Id}|{safeName}|{safeDesc}|{OwnerUsername}";
        }

        // STATISK METOD: Tillhör KLASSEN själv, inte individuella objekt
        // Vi kallar på den så här: Item.FromFileString() - inte genom ett objekt

        // Detta är en FACTORY METHOD - skapar och returnerar nya Item-objekt
        // Används för DESERIALISERING (göra text tillbaka till objekt)
        // VARFÖR? För att kunna LADDA sparade föremål när programmet startar igen
        public static Item FromFileString(string fileString)
        {
            // Split() är en STRING-METOD som delar upp text i en ARRAY
            // ARRAY är en lista med fast storlek där vi lagrar flera värden
            // VARFÖR Split? För att dela upp "1|Svärd|Vasst|Anna" i sina 4 delar
            string[] parts = fileString.Split('|');

            // INDEXERING: Vi använder [0], [1] etc för att komma åt element i arrayen
            // parts[0] = id, parts[1] = name, parts[2] = description, parts[3] = owner
            // VARFÖR DENNA ORDNING? Måste matcha ordningen vi sparade i ToFileString()!

            // PARSING: Konvertera text till rätt datatyp (string -> int)
            // VARFÖR Parse? Id läses som text "1" från filen, men vi behöver det som talet 1
            // vi kan inte blanda text och tal utan konvertering
            int id = int.Parse(parts[0]);

            // Vi använder NEW-nyckelordet för att skapa ett nytt objekt från klassen
            // och anropar KONSTRUKTORN med de värden vi parsade från filen
            // Nu kan annan kod göra: Item item = Item.FromFileString(textFromFile);
            return new Item(id, parts[1], parts[2], parts[3]);
        }
    }
}