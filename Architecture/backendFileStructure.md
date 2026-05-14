```
src/
├── API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── SharedDocsController.cs
│   │   └── SharingController.cs
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs
│   └── DTOs/
│       ├── RegisterRequest.cs
│       ├── LoginRequest.cs
│       ├── DocumentCreateDto.cs
│       ├── DocumentUpdateDto.cs
│       ├── ShareRequest.cs
│       ├── SharedDocumentDto.cs
│       ├── DocumentDto.cs
│       └── ShareDto.cs
├── Core/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Document.cs
│   │   ├── SharedDocument.cs
│   │   └── SharePermission.cs        ← enum 
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IDocumentService.cs
│   │   ├── IUserRepository.cs
│   │   ├── IDocumentRepository.cs
│   │   ├── ISharedDocumentRepository.cs
│   │   └── IUnitOfWork.cs
│   └── Services/
│       ├── AuthService.cs
│       └── DocumentService.cs
├── Infrastructure/
│   ├── Data/
│   │   ├── MyDbContext.cs
│   ├── Repositories/
│   │   ├── UserRepository.cs
│   │   ├── DocumentRepository.cs
│   │   ├── SharedDocumentRepository.cs
│   │   └── UnitOfWork.cs
│   └── Security/
│       ├── PasswordHasher.cs
│       └── JwtService.cs
└── Common/
    └── Exceptions/
        ├── NotFoundException.cs
        ├── ForbiddenException.cs
        └── ConflictException.cs
```