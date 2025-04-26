# BerAuto
Rendszerfejlesztés haladó módszerei nevű tárgy beadandó feladatának repoja

## JWT autentikáció használata

Az alábbiakban bemutatjuk, hogyan használható a JWT autentikáció a rendszerben, beleértve a bejelentkezést és a frissítő token használatát.

### Bejelentkezés
A bejelentkezéshez a `/api/auth/login` végpontot kell használni. A kéréshez egy `POST` metódust kell küldeni az alábbi JSON formátumban:

```json
{
  "email": "felhasznalo@example.com",
  "password": "jelszo"
}
```

**Válasz:**
- Sikeres bejelentkezés esetén a szerver visszaad egy JWT tokent és egy frissítő tokent:
  ```json
  {
    "success": true,
    "data": {
      "token": "JWT_TOKEN",
      "refreshToken": "REFRESH_TOKEN"
    }
  }
  ```
- Sikertelen bejelentkezés esetén hibaüzenetet kapunk:
  ```json
  {
    "success": false,
    "message": "Hibás email vagy jelszó"
  }
  ```

### Token frissítése
A token frissítéséhez a `/api/auth/refresh` végpontot kell használni. A kéréshez egy `POST` metódust kell küldeni az alábbi JSON formátumban:

```json
{
  "refreshToken": "REFRESH_TOKEN"
}
```

**Válasz:**
- Sikeres frissítés esetén a szerver visszaad egy új JWT tokent és egy új frissítő tokent:
  ```json
  {
    "success": true,
    "data": {
      "token": "NEW_JWT_TOKEN",
      "refreshToken": "NEW_REFRESH_TOKEN"
    }
  }
  ```
- Sikertelen frissítés esetén hibaüzenetet kapunk:
  ```json
  {
    "success": false,
    "message": "Érvénytelen vagy lejárt frissítő token"
  }
  ```

### Tesztelés
A token érvényességének teszteléséhez használható a `/api/auth/test` végpont. Ehhez egy `GET` kérést kell küldeni, amely tartalmazza a JWT tokent a `Authorization` fejlécben:

```
Authorization: Bearer JWT_TOKEN
```

**Válasz:**
- Sikeres hitelesítés esetén a szerver visszaadja a felhasználó adatait:
  ```json
  {
    "message": "Authentication successful! Your token is valid.",
    "userDetails": {
      "id": "USER_ID",
      "name": "USER_NAME",
      "email": "USER_EMAIL",
      "role": "USER_ROLE"
    }
  }
  ```
- Sikertelen hitelesítés esetén hibaüzenetet kapunk:
  ```json
  {
    "message": "Unauthorized"
  }
  ```