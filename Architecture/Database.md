# Diagrama relatie

```mermaid
erDiagram
    USER {
        uniqueidentifier Id PK
        string Username
        string PasswordHash "Stored on Server"
    }
    USER_DOCUMENT {
        uniqueidentifier UserId FK
        uniqueidentifier DocumentId FK
        string PermissionLevel "Read/Write"
    }
    DOCUMENT {
        uniqueidentifier Id PK
        string Title
        string Content
        number Version 
        datetime LastUpdated
    }
    USER ||--o{ USER_DOCUMENT : "owns/edits"
    DOCUMENT ||--o{ USER_DOCUMENT : "associated with"
```

### Explicatie
USER -> tine doar hash-ul parolei, username-ul si  id-ul


USER_DOCUMENT-> face legatura intre document in sine si user. detine permisiunile sale per document


DOCUMENT-> reprezinta documentul in sine


[[Architecture.md|Inapoi]]