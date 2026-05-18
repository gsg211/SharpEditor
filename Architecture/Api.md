# Documentație API

Acest document descrie endpoint-urile expuse de backend, formatele JSON necesare pentru cereri (Requests) și structura răspunsurilor (Responses).

---

## Autentificare și Autorizare (`/auth`)

Toate endpoint-urile din afara acestui modul necesită trimiterea token-ului JWT primit la Login prin header-ul de tip Bearer:
`Authorization: Bearer <token_jwt>`

### 1. Înregistrare Utilizator

Creează un cont nou în sistem. Acest endpoint nu loghează automat utilizatorul.

* **URL:** `/auth/register`
* **Metodă:** `POST`
* **Request Body (JSON):**

```json
{
  "username": "ion",
  "email": "ion@test.com",
  "password": "parola123"
}

```

* **Response (201 Created):**

```json
{
  "token": null,
  "message": "User registered successfully."
}

```

### 2. Autentificare Utilizator (Login)

Validează credențialele și generează token-ul de acces.

* **URL:** `/auth/login`
* **Metodă:** `POST`
* **Request Body (JSON):**

```json
{
  "email": "ion@test.com",
  "password": "parola123"
}

```

* **Response (200 OK):**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Login successful."
}

```

---

## Management Documente (`/shared-docs`)

### 3. Creare Document

Creează un document nou. Utilizatorul curent devine automat Owner.

* **URL:** `/shared-docs`
* **Metodă:** `POST`
* **Headers:** `Authorization: Bearer <TOKEN>`
* **Request Body (JSON):**

```json
{
  "title": "Documentul lui Ion",
  "content": "Hello world"
}

```

* **Response (201 Created):**

```json
{
  "shareId": 1,
  "permission": "Owner",
  "document": {
    "id": 1,
    "title": "Documentul lui Ion",
    "content": "Hello world",
    "version": 1,
    "createdAt": "2026-05-18T12:52:39.962Z",
    "updatedAt": "2026-05-18T12:52:39.962Z"
  }
}

```

### 4. Obținere Listă Documente

Returnează o listă sumară cu toate documentele la care utilizatorul are acces (create de el sau partajate cu el).

* **URL:** `/shared-docs`
* **Metodă:** `GET`
* **Headers:** `Authorization: Bearer <TOKEN>`
* **Response (200 OK):**

```json
[
  {
    "shareId": 1,
    "title": "Documentul lui Ion",
    "permission": "Owner"
  }
]

```

### 5. Obținere Document Specific

Returnează detaliile complete ale unui document pe baza ID-ului, dacă utilizatorul are drepturi de acces.

* **URL:** `/shared-docs/{documentId}`
* **Metodă:** `GET`
* **Headers:** `Authorization: Bearer <TOKEN>`
* **Response (200 OK):**

```json
{
  "shareId": 1,
  "permission": "Owner",
  "document": {
    "id": 1,
    "title": "Documentul lui Ion",
    "content": "Hello world",
    "version": 1,
    "createdAt": "2026-05-18T12:52:39.962Z",
    "updatedAt": "2026-05-18T12:52:39.962Z"
  }
}

```

### 6. Actualizare Document (Editare)

Modifică conținutul unui document. Implementează concurență optimistă pe baza câmpului `version`.

* **URL:** `/shared-docs/{documentId}`
* **Metodă:** `PUT`
* **Headers:** `Authorization: Bearer <TOKEN>`
* **Request Body (JSON):**

```json
{
  "content": "Continut actualizat",
  "version": 1
}

```

* **Response (200 OK):** *(Versiunea crește automat cu +1)*

```json
{
  "shareId": 1,
  "permission": "Owner",
  "document": {
    "id": 1,
    "title": "Documentul lui Ion",
    "content": "Continut actualizat",
    "version": 2,
    "createdAt": "2026-05-18T12:52:39.962Z",
    "updatedAt": "2026-05-18T12:52:40.533Z"
  }
}

```

#### Erori posibile la editare:

* **409 Conflict:** Trimis când versiunea specificată de frontend nu se potrivește cu versiunea actuală de pe server.

```json
{
  "error": "Version conflict: client has v1, server is at v3."
}

```

* **403 Forbidden:** Trimis când utilizatorul încearcă să modifice documentul, dar are doar permisiuni de tip `ReadOnly`.

```json
{
  "error": "You have ReadOnly permission."
}

```

### 7. Ștergere Document

Dacă apelantul este Owner, documentul și partajările asociate sunt șterse definitiv. Dacă este un colaborator, i se elimină doar dreptul de acces personal.

* **URL:** `/shared-docs/{documentId}`
* **Metodă:** `DELETE`
* **Headers:** `Authorization: Bearer <TOKEN>`
* **Response:**
* `204 No Content` (fără body) în caz de succes.
* `404 Not Found` dacă documentul nu există sau nu este accesibil.



---

## Partajare și Permisiuni (`/shared-docs/{id}/shares`)

> ⚠️ **Notă:** Toate acțiunile din această secțiune pot fi executate exclusiv de către Proprietarul (**Owner**) documentului.

### 8. Partajare Document (Adăugare Colaborator)

Acordă acces unui alt utilizator. Valorile permise pentru permisiuni sunt: `ReadOnly` sau `ReadWrite`.

* **URL:** `/shared-docs/{documentId}/shares`
* **Metodă:** `POST`
* **Headers:** `Authorization: Bearer <TOKEN_OWNER>`
* **Request Body (JSON):**

```json
{
  "userId": 2,
  "permission": "ReadOnly"
}

```

* **Response (201 Created / 200 OK):**

```json
{
  "userId": 2,
  "username": "maria",
  "permission": "ReadOnly"
}

```

### 9. Listare Utilizatori cu Access

Afișează toți utilizatorii care au primit acces la documentul respectiv.

* **URL:** `/shared-docs/{documentId}/shares`
* **Metodă:** `GET`
* **Headers:** `Authorization: Bearer <TOKEN_OWNER>`
* **Response (200 OK):**

```json
[
  {
    "userId": 2,
    "username": "maria",
    "permission": "ReadOnly"
  }
]

```

### 10. Modificare Permisiuni Colaborator

Schimbă nivelul de acces al unui colaborator existent.

* **URL:** `/shared-docs/{documentId}/shares/{targetUserId}`
* **Metodă:** `PUT`
* **Headers:** `Authorization: Bearer <TOKEN_OWNER>`
* **Request Body (JSON):**

```json
{
  "permission": "ReadWrite"
}

```

* **Response (200 OK):**

```json
{
  "userId": 2,
  "username": "maria",
  "permission": "ReadWrite"
}

```

### 11. Revocare Acces (Eliminare Colaborator)

Elimină complet dreptul unui utilizator de a mai vizualiza sau edita documentul.

* **URL:** `/shared-docs/{documentId}/shares/{targetUserId}`
* **Metodă:** `DELETE`
* **Headers:** `Authorization: Bearer <TOKEN_OWNER>`
* **Response:** `204 No Content` (fără body).

*Orice tentativă ulterioară a utilizatorului eliminat de a accesa documentul va returna codul **404 Not Found**:*

```json
{
  "error": "Document 1 not found or not accessible."
}
```