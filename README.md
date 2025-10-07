#  Trading System - README

##  Hur man använder programmet

### Användarguide - Steg för steg

#### Första gången du använder programmet:

1. **Registrera ett konto**
   - Välj alternativ 1 från huvudmenyn
   - Ange önskat användarnamn
   - Ange ett lösenord
   - Systemet sparar automatiskt din användare

2. **Logga in**
   - Välj alternativ 2
   - Ange ditt användarnamn och lösenord
   - Du kommer nu till inloggad meny

3. **Ladda upp en vara**
   - Välj alternativ 1 (Upload Item)
   - Ange varans namn (t.ex. "Pokemon-kort")
   - Ange beskrivning (t.ex. "Charizard 1:a upplagan")
   - Varan sparas automatiskt

4. **Bläddra bland varor**
   - Välj alternativ 2 (Browse Items)
   - Du ser alla varor som ANDRA användare har lagt upp
   - Notera ID-numret [1], [2], etc. för varor du är intresserad av

5. **Begär en byteshandel**
   - Välj alternativ 3 (Request Trade)
   - Ange ID-numret för varan du vill ha
   - En bytesförfrågan skickas till varans ägare

6. **Hantera bytesförfrågningar**
   - Välj alternativ 4 (View Trade Requests) för att se förfrågningar på DINA varor
   - Välj alternativ 5 (Accept Trade) och ange Trade ID för att acceptera
   - Välj alternativ 6 (Deny Trade) och ange Trade ID för att neka
   - När du accepterar, överförs varan automatiskt till den som begärde bytet

7. **Se färdiga byteshandlar**
   - Välj alternativ 7 (View Completed Trades)
   - Här ser du alla accepterade och nekade byteshandlar

8. **Logga ut**
   - Välj alternativ 8 (Logout)
   - Du kommer tillbaka till huvudmenyn

### Datapersistens

Programmet sparar automatiskt all data till dessa filer:
- `users.txt` - Alla registrerade användare
- `items.txt` - Alla uppladade varor
- `trades.txt` - Alla bytesförfrågningar


---

## Implementationsval 

### 1. Klassstruktur - Varför fyra separata klasser?

Jag har skapat fyra huvudklasser: `User`, `Item`, `TradeRequest`, och `TradingSystem`.

**Varför separata klasser?**

Varje klass representerar en tydlig och tydlig entitet i systemet:
- **User** - En person som använder systemet
- **Item** - En vara som kan bytas
- **TradeRequest** - En bytesförfrågan mellan två användare
- **TradingSystem** - Systemet som hanterar allt


**Fördelar med separata klasser:**
- Lättare att hitta och fixa buggar (om något är fel med items, kollar jag i Item-klassen)
- Lättare att förstå koden (varje klass har ett tydligt syfte)
- Lättare att utöka systemet senare (vill man lägga till fler funktioner för Users, ändrar man bara User-klassen)

---

### 2. Komposition vs Arv - Varför valde jag komposition?

**Komposition används:**
- User HAS-A lista med Items
- TradingSystem HAS-A lista med Users
- TradingSystem HAS-A lista med Items

**Varför INTE arv (inheritance)?**

Arv används när man har en "IS-A" relation:
- Hund IS-A Djur 
- Katt IS-A Djur 

I mitt system har jag "HAS-A" relationer:
- User HAS items (User ÄR INTE en Item)
- TradingSystem HAS users (TradingSystem ÄR INTE en User)


---

### 3. Properties vs Public Fields - Varför properties?

**Jag använder properties med get och set istället för publika fält.**

**Varför?**

1. Om jag senare vill lägga till validering, kan jag göra det utan att ändra hur andra använder klassen


Exempel:
Om jag vill lägga till validering senare kan jag ändra propertyn utan att bryta befintlig kod. Till exempel kräva att användarnamn är minst 3 tecken.

Just nu använder jag auto-properties för enkelhetens skull, men strukturen gör det lätt att lägga till logik senare.

---

### 4. List<T> vs Array - Varför dynamiska listor?

**Jag använder List istället för Array.**

**Varför listor?**

1. **Dynamisk storlek** - Jag vet inte hur många användare som kommer registrera sig. Det kan vara 1, det kan vara 1000. Med array måste jag välja en fast storlek på förhand.

2. **Enklare metoder** - List har inbyggda metoder:
   - `.Add()` - Lägg till element
   - `.Remove()` - Ta bort element
   - `.Count` - Antal element
   
   Med array måste jag hålla koll på index och storlek själv.

3. **Mindre risk för buggar** - Med array kan jag råka skriva utanför gränserna. List hanterar detta automatiskt.

Exempel:
Med Array måste jag gissa storlek - vad händer vid user 101 om jag satt storlek 100?
Med List växer den automatiskt - kan växa oändligt!

---

### 5. Enum för TradeStatus - Varför inte strings?

**Jag använder enum TradeStatus med värdena Pending, Accepted, Denied.**

**Varför enum?**

1.  Kompilatorn förhindrar felaktiga värden. Med string kan jag skriva "Pendingg" (stavfel) utan att kompilatorn upptäcker det förrän runtime. Med enum får jag kompileringsfel direkt.

2.  När jag skriver TradeStatus. får jag en lista med alla giltiga värden

3.  Kod som kollar om status är Pending blir tydligare än att jämföra strings

4.  "Pending" vs "pending" vs "PENDING" - enum löser detta

**Det finns exakt tre möjliga statusar för en byteshandel** - enum är perfekt för detta!

---

### 6. Access Modifiers - Varför private och public?

**Jag använder private för fält och public för metoder i TradingSystem-klassen.**

**Varför privata fält och publika metoder?**

1. **Inkapsling** - Skyddar intern data från att manipuleras direkt
2. **Kontrollerad åtkomst** - All interaktion sker genom metoder som kan validera input

Exempel:
Om users var public kunde någon göra system.users.Clear() och radera ALLA användare! Eller lägga till null och krascha systemet!

Med private users och public metoder är det säkert - metoden Register validerar input först.

---

### 8. File I/O - Varför textfiler istället för databas?

**Jag använder File.WriteAllLines och File.ReadAllLines för att spara och ladda data.**

**Varför textfiler?**

1. Ingen extern databas behövs
2. Kan öppna users.txt i anteckningar och se innehållet


**Format valt: Pipe-separated (|)**
Exempel: username|password

**Varför | som separator?**
- Lätt att dela upp med `.Split('|')`
- Mer robust än komma eller mellanslag

**Jag hanterar | i användardata:**
Ersätter | med mellanslag om det finns i användarens input för att förhindra att filformatet går sönder.

---

### 9. Try-Catch - Varför felhantering?

**Jag omger fil-operationer med try-catch.**

**Varför är detta nödvändigt?**

1. **Filen kan vara låst** - Om filen är öppen i annat program
2. **Disken kan vara full** - Inget utrymme att spara
3. **Ingen behörighet** - Användaren kanske inte får skriva till mappen
4. **Filen kan vara skadad** - Vid inläsning kan data vara korrupt

**Utan try-catch:**
Programmet skulle KRASCHA helt och användaren förlorar allt arbete.

**Med try-catch:**
Programmet fångar felet, visar ett meddelande, och fortsätter köra.

**Jag använder också try-catch för användarinput:**
När användaren ska ange ett ID (nummer) kan de skriva "abc" istället. int.Parse() skulle krascha, men try-catch fångar detta och ber om korrekt input.

---

### 10. For vs Foreach 

**Jag använder for-loop när jag behöver index.**
Exempel: När jag sparar till fil och behöver placera varje user på rätt rad-index.

**Jag använder foreach-loop när jag bara behöver elementet.**
Exempel: När jag söker efter en specifik användare bryr jag mig inte om indexet.

---

### 11. ToString() Override - Varför implementera detta?

**Jag överrider ToString() i alla klasser.**

**Varför?**

1. Kan skriva Console.WriteLine(user) istället för att manuellt formatera
2. Alla objekt visar sig själva på samma sätt
3. Formattering finns på ETT ställe

**Utan ToString():**
Måste skriva formattering varje gång jag vill visa en user.

**Med ToString():**
Console.WriteLine(user) - enkelt och rent!

---

### 12. Switch vs If-Else - Varför switch för menyer?

**Jag använder switch för menyer.**

**Varför switch?**

1. Tydligare när man jämför samma variabel mot många värden
2. Marginellt snabbare för många case
3. Det är standard för meny-navigation
4. Gör det tydligt när varje case slutar

---
Den veckan vi gick i igenom Git så var jag bortrest och när jag väl kom tillbaka så ville jag bara fokusera på att koda eftersom jag var efter så när jag 
väl kom till Git och började läsa så var jag redan färdig med allt och ville kolla upp hur jag skulle lägga upp det på Github. Vilket 100% är mitt fel.
Jag vet hur man gör och om vi kan komprimissa så kommer jag använda mycket git under vårt grupparbete vilket jag kan visa att jag kan göra det där men det
är såklart upp till dig hur du vill gå tillväga.


# Git 
bash
# Första gången# 
git init

git add TradingSystem.cs README.md .gitignore

git commit -m "Initial commit: Complete trading system implementation"

# Efter varje funktionalitet# 
git add TradingSystem.cs

git commit -m "Added user registration and login"
git add TradingSystem.cs

git commit -m "Implemented item upload and browsing"
git add TradingSystem.cs

git commit -m "Added trade request functionality"

git add TradingSystem.cs

git commit -m "Implemented accept/deny trades with item transfer"

git add TradingSystem.cs

git commit -m "Added auto-save and auto-load functionality"

git add README.md

git commit -m "Completed README"

# Koppla till GitHub 
git remote add origin https://github.com/dittanvändarnamn/trading-system.gitorigin 
git branch -M main
git push -u origin main
