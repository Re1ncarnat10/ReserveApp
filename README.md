# Reserve App Backend

Backend (.NET/C#) aplikacji do rezerwacji zasobow. Udostepnia API do obslugi uzytkownikow, zasobow, rezerwacji oraz historii wypozyczen (UserHistory).

## Spis tresci
- [Opis projektu](#opis-projektu)
- [Diagramy](#diagramy)
- [Wymagania funkcjonalne](#wymagania-funkcjonalne)
- [Wymagania niefunkcjonalne](#wymagania-niefunkcjonalne)
- [Wymagania systemowe](#wymagania-systemowe)
- [Instalacja](#instalacja)
- [Uruchomienie](#uruchomienie)
- [Struktura projektu](#struktura-projektu)
- [Glowne funkcje](#glowne-funkcje)
- [Przykladowe scenariusze uzycia](#przykladowe-scenariusze-uzycia)
- [Wspoltworzenie](#wspoltworzenie)
- [Licencja](#licencja)
- [Kontakt](#kontakt)

---

## Opis projektu

REST API zapewniajace zarzadzanie uzytkownikami, zasobami, rezerwacjami oraz historia wypozyczen (UserHistory). Pozwala uzytkownikom na sledzenie historii wszystkich wypozyczen, a administratorowi na przeglad dzialan uzytkownikow.

---

## Diagramy

### Struktura bazy danych (fragment)

```mermaid
erDiagram
    USER ||--o{ USER_HISTORY : posiada
    USER ||--o{ USER_RESOURCE : wypozycza
    USER_RESOURCE }o--|| RESOURCE : dla
    USER_HISTORY }o--|| RESOURCE : dotyczy

    USER {
        string Id PK
        string Name
        string Surname
        string Department
    }
    USER_RESOURCE {
        int UserResourceId PK
        string UserId FK
        int ResourceId FK
        string Status
        datetime RentalStartTime
        datetime RentalEndTime
    }
    USER_HISTORY {
        int UserHistoryId PK
        string UserId FK
        int ResourceId FK
        string ResourceName
        string ResourceDescription
        string ResourceType
        string ResourceImage
        datetime ApprovedAt
        datetime RentalStartTime
        datetime RentalEndTime
    }
    RESOURCE {
        int ResourceId PK
        string Name
        string Description
        string Type
        string Image
        bool Availability
    }
```

### Sequence diagram – Resource Request

```mermaid
sequenceDiagram
    participant U as User
    participant FE as Frontend
    participant API as Backend
    participant DB as Database

    U->>FE: Selects resource, sets dates
    FE->>API: POST /api/UserResource/request (userId, resourceId, rentalStartTime, rentalEndTime)
    API->>DB: Check resource availability
    alt Resource available
        DB-->>API: OK
        API->>DB: Add UserResource (Status: Pending)
        DB-->>API: Confirm add
        API-->>FE: 201 Created (pending request)
        FE-->>U: Show success, waiting for approval
    else Resource not available
        DB-->>API: Not available
        API-->>FE: 409 Conflict (not available)
        FE-->>U: Show unavailable message
    end
```

### Flowchart – Resource Request

```mermaid
flowchart TD
    A[User selects resource] --> B[POST /UserResource/request]
    B --> C{Resource available?}
    C -- Yes --> D[Create UserResource with Pending status]
    D --> E[Wait for admin approval]
    C -- No --> F[Show not available message]
```

### Use Case diagram – Resource Request

```mermaid
usecaseDiagram
    actor User
    actor Admin

    User --> (Request resource)
    Admin --> (Approve or reject request)
    User --> (Receive request status notification)
```

### Sequence diagram – User History

```mermaid
sequenceDiagram
    participant FE as Frontend
    participant API as Backend
    participant DB as Database

    FE->>API: GET /api/User/me/history
    API->>DB: Get UserHistory for userId
    DB-->>API: History list
    API-->>FE: JSON with history
```

---

## Wymagania funkcjonalne

1. **Obsluga uzytkownikow:**
   - Rejestracja, logowanie, edycja danych, zarzadzanie rolami.
2. **Zarzadzanie zasobami:**
   - Dodawanie, edycja, usuwanie zasobow.
3. **System rezerwacji:**
   - Tworzenie, zatwierdzanie, anulowanie rezerwacji.
4. **Historia wypozyczen (UserHistory):**
   - Tworzenie rekordu historii przy zatwierdzeniu wypozyczenia.
   - Endpoint GET /api/User/me/history zwracajacy liste historii uzytkownika.
   - Dane: nazwa zasobu, opis, typ, obraz, daty wypozyczenia.
5. **Panel admina:**
   - Przeglad historii wypozyczen uzytkownikow
   - Zarzadzanie uzytkownikami i zasobami.
6. **Powiadomienia e-mail**

---

## Wymagania niefunkcjonalne

- Wydajnosc: odpowiedz <1s dla 95% zadan
- Bezpieczenstwo: JWT, hash + salt, ochrona przed SQLi/XSS/CSRF
- Skalowalnosc: rozdzielenie warstw, wsparcie dla chmury
- Backupy bazy, monitoring bledow
- Testy jednostkowe i integracyjne

---

## Wymagania systemowe

- .NET 6+
- SQL Server lub PostgreSQL
- Entity Framework

---

## Instalacja

```sh
git clone https://github.com/Re1ncarnat10/ReserveApp.git
cd ReserveApp
dotnet restore
```

---

## Uruchomienie

```sh
dotnet run
```
API na http://localhost:5000

---

## Struktura projektu

- `Controllers/UserController.cs` – endpointy uzytkownika (w tym historia)
- `Models/UserHistory.cs` – model historii wypozyczen
- `Dtos/UserHistoryDto.cs` – DTO historii wypozyczen
- `Services/UserService.cs` – logika pobierania historii uzytkownika
- `Data/AppDbContext.cs` – konfiguracja encji UserHistory

---

## Glowne funkcje

- Rejestracja, logowanie, obsluga JWT
- Zarzadzanie zasobami i uzytkownikami
- Rezerwacje
- Historia wypozyczen (UserHistory)
- Powiadomienia e-mail
- Audyt operacji

---

## Przykladowe scenariusze uzycia

- Uzytkownik loguje sie i sprawdza historie wypozyczen.
- Uzytkownik requestuje zasob – oczekuje na akceptacje lub odrzucenie.
- Administrator przeglada historie wszystkich uzytkownikow.

---

## Wspoltworzenie

1. Fork repozytorium
2. Nowa galaz (`feature/nazwa`)
3. Zmiany + testy
4. Pull Request

---

## Licencja

MIT

---

## Kontakt

Masz pytania? Issues lub [Twoj_email@example.com]
