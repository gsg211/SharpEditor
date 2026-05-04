# Arhitectura

```mermaid
graph TB
    subgraph Client_Side [Client]
        UI[Editor UI]
        State[Local Document State]
        AuthStore[JWT Storage]
    end


    subgraph Server_Side [Server]
        DocController[Document Controller]
        Repo[Document Repository]
        Auth[JWT Authentication]
    end

    subgraph Data_Layer [Storage]
        DB[(SQL Server)]
    end

    %% Connections
    UI <--> State

    AuthStore -.-> Auth
    

    DocController <--> Repo
    Repo <--> DB

```

---

## Diagrame UML



### Diagrama de secventa autentificare 
```mermaid
sequenceDiagram
    participant Client
    participant Server
    actor DB@{ "type": "database" } as Database

    Client ->> Server: Authentication

    Server ->> DB: Check credentials
    DB -->> Server: response

    Server->>Server: generate JWT
    Server -->> Client: JWT

    Client ->> Server: GET Document
    Server ->> Server: validate request 
    Server ->> DB: GET document
    DB -->> Server: document
    Server -->> Client: document
```

---

### Diagrama caz utilizari
```mermaid
graph LR
    %% Actor
    User((User))

    subgraph Document_Management_System [Document Editor System]
        UC1(Authentication / Login)
        UC2(Create New Document)
        UC3(View Document List)
        UC4(Edit Content)
        UC5(Save Changes)
        UC6(Delete Personal Document)
        UC7(Share Document)
        
        %% Dependencies
        UC4 -.->|include| UC5
    end

    %% Connections
    User --> UC1
    User --> UC3
    User --> UC2
    User --> UC4
    User --> UC6
    User --> UC7
```

---

### Diagrama activitate -> salvare document 
```mermaid
stateDiagram-v2
    [*] --> EditingInProgress
    EditingInProgress --> ClickSave
    
    ClickSave --> VerifyAuthentication
    
    state VerifyAuthentication <<choice>>
    VerifyAuthentication --> CheckPermissions : Valid Token
    VerifyAuthentication --> PromptLogin : Token Expired / Missing

    CheckPermissions --> CompareVersions : Has Write Access
    CheckPermissions --> PermissionError : Read-Only Access

    state CompareVersions <<choice>>
    CompareVersions --> UpdateDatabase : Versions Match
    CompareVersions --> ConflictDetected : Server Version > Local Version

    UpdateDatabase --> SuccessMessage
    SuccessMessage --> [*]

    ConflictDetected --> ShowConflictDialog
    ShowConflictDialog --> ReloadDocument : Choose 'Overwrite with Server Version'
    ReloadDocument --> [*]
    
    PermissionError --> [*]
```
vezi  [Baza de date](Database.md)
vezi  [Backend](Backend.md)
