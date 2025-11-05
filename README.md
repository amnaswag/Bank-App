# 💫 Bank of Sound Star System 💿

Detta är mitt projekt för kursen Systemutveckling, byggt i **Blazor WebAssembly**.
Appen är en banksimulator med lila astetik designad för att visa OOP, tjänster och persistent datalagring.
Enjoy  

☆*: .｡. o(≧▽≦)o .｡.:*☆

## Funktioner

* **Kontohantering:** Skapa och radera konton (Lönekonto/Sparkonto), inklusive PIN-val.
* **Transaktioner:** Sätt in och ta ut pengar, med validering mot övertrassering.
* **Överföringar:** Gör överföringar mellan egna konton.
* **Historik:** Se transaktionshistorik per konto, med **filtrering** på typ och datumintervall, samt **robust sortering** på datum och belopp.
* **Persistens:** All data sparas lokalt i webbläsaren (LocalStorage).

### VG-Tillägg 

1.  **Export/Import av data i JSON-format:** Exportera alla konton och transaktioner till JSON-fil, och importera dem igen med validering.
2.  **Ränta på Sparkonto:** En funktion för att applicera 0.5% ränta på alla upplåsta sparkonton (knapp finns på sidan Konton).
3.  **Åtkomstskydd (PIN-kod):** Konton med PIN-kod låser sitt saldo på kontosidan. Alla transaktioner (utom Insättning) och ränteappliceringar kräver att kontot låses upp med korrekt PIN-kod i den aktuella sessionen (endast UI-låsning).
4.  **Lås skärm för access:** Hela applikationen skyddad vid uppstart med ett förbestämt lösenord ("VG2024")
5.  **UX-Förbättring (Disable-knapp):** Transaktionsknappen på sidan **Ny Transaktion** är dynamiskt inaktiverad om obligatoriska fält saknas eller om kontot är PIN-skyddat och låst.

## Tekniska Val och Motiveringar 

* **Objektorienterad Design (OOP):** Projektet följer strikta OOP-principer genom att separera data (`BankAccount`, `Transaction`) från affärslogik (`AccountService`). Användandet av `IBankAccount` och `IAccountService` säkerställer att klasserna är beroende av abstraktioner istället för konkreta implementationer. Domänmodellerna använder strikta `get; set;` för att säkerställa kompatibilitet med modern C# och `System.Text.Json` serialisering.

* **Service/Repository Mönster:** Logiken är inkapslad i `AccountService` (t.ex. insättning, uttag, övertrassering, räntelogiken). Denna service är beroende av `AccountRepository` och `StorageService` via Dependency Injection (DI) för att hantera datalageroperationer. Detta gör logiken oberoende av var datan lagras.

* **Persistens med LocalStorage:** Valet att använda `IStorageService` implementerat via JS-Interop till **LocalStorage** möjliggör persistent datalagring direkt i webbläsaren mellan sessioner, vilket är lämpligt för en enkel Blazor WASM-app som inte har en backend-server.

* **Robust Historiklogik:** Filtrering, sortering på datum och belopp, samt datumintervall filtreras direkt i Razor-komponenten med hjälp av Linq. Sortering inkluderar sekundära kriterier (`ThenBy(t => t.Id)`) för att säkerställa stabil ordning vid identiska primärvärden (t.ex. samma belopp eller samma datum).

## Kom igång

### Systemkrav

* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Webbläsare (Edge, Chrome, Firefox, etc)

### Installation

1.  **Gå till projektmappen:** Navigera till mappen som innehåller din lösning (d.v.s. till `Bank-App`).
2.  **Bygg och starta appen:**
    ```bash
    dotnet run --project BankApp
    ```
3.  **Öppna:** Öppna din webbläsare och gå till den URL som visas i terminalen (t.ex. `http://localhost:5034` eller `https://localhost:7143`). och skriv in lösenordet "VG2024" 


Lycka till ヾ(•ω•`)o // Amna
