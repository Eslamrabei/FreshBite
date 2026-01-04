# 🛒 FreshBite - Backend API

A modern, enterprise-grade e-commerce backend built with **ASP.NET Core 8**, featuring intelligent product search with AI integration, secure authentication, order management, and a robust architecture following **Clean Architecture** principles.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Key Features](#key-features)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Database Setup](#database-setup)
- [Testing](#testing)
- [Contributing](#contributing)

---

## 🎯 Overview

**FreshBite Backend** is a scalable, production-ready e-commerce API that provides comprehensive functionality for:
- User authentication and authorization (JWT-based)
- Product catalog management with AI-powered search
- Shopping basket and order processing
- Payment integration (Stripe)
- Vector database search using Qdrant
- Caching strategies with Redis
- Comprehensive error handling and validation

The project is designed with **testability** and **maintainability** at its core, utilizing dependency injection, repository patterns, and specification-based queries.

---

## 🏗️ Architecture

The backend follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│           (Controllers, Attributes, DTOs)                    │
│          Infrastructure\Presentation                         │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                      Service Layer                           │
│    (Business Logic, Specifications, Mappings)               │
│              Core\Service                                    │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                  Abstraction Layer                           │
│               (Interface Contracts)                          │
│          Core\ServiceAbstraction                             │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                  Infrastructure Layer                        │
│        (Repositories, DbContext, Validations)               │
│        Infrastructure\Persistence                           │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    Domain Layer                              │
│      (Entities, Contracts, Exceptions)                      │
│              Core\Domain                                     │
└─────────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

| Layer | Purpose | Location |
|-------|---------|----------|
| **Presentation** | HTTP endpoints, request handling, response formatting | `Infrastructure/Presentation` |
| **Service** | Business logic, data transformation, orchestration | `Core/Service` |
| **Abstraction** | Interface definitions, contracts | `Core/ServiceAbstraction` |
| **Persistence** | Data access, repositories, validations | `Infrastructure/Persistence` |
| **Domain** | Entities, business rules, exceptions | `Core/Domain` |
| **Shared** | DTOs, enums, common utilities | `Shared` |
| **API** | Bootstrapping, DI configuration, middleware | `E-CommerceApi` |

---

## 💻 Technology Stack

### Core Framework
- **ASP.NET Core 8** - Web framework
- **Entity Framework Core 8** - ORM for data access
- **Microsoft.AspNetCore.Identity** - Authentication & authorization

### Data & Caching
- **SQL Server** - Primary database
- **Redis** - Distributed caching
- **Qdrant** - Vector database for semantic search

### AI & Integration
- **Groq API** - AI language model integration
- **Ollama** - Local embedding generation
- **Stripe** - Payment processing

### Validation & Mapping
- **FluentValidation** - Data validation framework
- **AutoMapper** - Object-to-object mapping

### Testing
- **xUnit** - Unit testing framework
- **Moq** - Mocking library
- **FluentAssertions** - Assertion library
- **AutoFixture** - Test data generation

---

## 📁 Project Structure

### Core Layer

#### `Core/Domain`
Contains business entities and contracts:
```
Domain/
├── Entities/
│   ├── ProductModule/        # Product, ProductBrand, ProductType
│   ├── OrderModule/          # Order, OrderItem, DeliveryMethod
│   ├── BasketModule/         # CustomerBasket, BasketItems
│   └── IdentityModule/       # User, RefreshToken, Address
├── Contracts/                # Repository & service interfaces
└── Exceptions/               # Custom exception definitions
```

**Key Entities:**
- `Product` - Product catalog items
- `Order` - Customer orders
- `CustomerBasket` - Shopping cart
- `User` - User accounts with identity
- `RefreshToken` - JWT token management

#### `Core/Service`
Business logic and service implementations:
```
Service/
├── Implementations/
│   ├── AuthenticationService    # Auth, JWT, refresh tokens
│   ├── ProductService           # Product queries & management
│   ├── BasketService            # Shopping cart operations
│   ├── OrderService             # Order processing
│   ├── PaymentService           # Payment processing
│   ├── CacheService             # Caching logic
│   ├── FileService              # File operations
│   ├── RefreshTokenServices     # Token lifecycle management
│   └── OllamaEmbeddingService   # AI embeddings
├── Specifications/              # Query specifications (pattern)
│   ├── BaseSpecification
│   ├── ProductTypeAndBrandSpecifications
│   └── OrderWithIncludesSpecifications
└── MappingProfiles/             # AutoMapper configurations
```

**Specifications Pattern:**
The project uses the Specification Pattern for complex queries, allowing reusable, testable query logic separate from repositories.

#### `Core/ServiceAbstraction`
Interface definitions for all services:
```
ServiceAbstraction/
└── Contracts/
    ├── IAuthenticationService
    ├── IProductService
    ├── IBasketService
    ├── IOrderService
    ├── IPaymentService
    ├── ICacheService
    ├── IRefreshTokenServices
    ├── IEmbeddingService
    └── IVectorService
```

### Infrastructure Layer

#### `Infrastructure/Persistence`
Data access and persistence logic:
```
Persistence/
├── Data/
│   ├── StoreDbContext.cs        # Main database context
│   ├── Configurations/          # EF Core entity configurations
│   ├── Migrations/              # Database schema migrations
│   └── DataSeeding.cs           # Initial data seeding
├── Identity/
│   ├── IdentityStoreDbContext   # Identity database context
│   └── Migrations/              # Identity schema migrations
├── Repositories/
│   ├── GenericRepository<T>     # Generic CRUD operations
│   ├── ProductRepository        # Product-specific queries
│   ├── BasketRepository         # Shopping cart queries
│   ├── RefreshTokenRepository   # Token management
│   ├── CacheRepository          # Redis caching
│   └── UnitOfWork               # Transaction management
└── Validations/
    └── ProductsValidations/     # FluentValidation rules
```

#### `Infrastructure/Presentation`
HTTP API controllers and attributes:
```
Presentation/
├── Controllers/
│   ├── ApiController            # Base controller
│   ├── AuthenticationController # Auth endpoints
│   ├── ProductsController       # Product endpoints
│   ├── BasketController         # Basket endpoints
│   ├── OrdersController         # Order endpoints
│   ├── PaymentsController       # Payment webhooks
│   ├── SearchController         # AI search endpoints
│   └── AdminController          # Admin operations
└── Attributes/
    └── RedisCacheAttribute      # Caching decorator
```

### Entry Point

#### `E-CommerceApi`
Application bootstrapping and configuration:
```
E-CommerceApi/
├── Program.cs                   # Application startup
├── Extensions/
│   ├── CoreServicesExtensions
│   ├── InfrastructureServicesExtensions
│   ├── WebApiServices
│   ├── WebApiManageMiddlwares
│   └── FluentValidationsDI
├── Middlewares/
│   └── GlobalExceptionHandelingMiddleware
└── Factories/
    └── ApiResponseFactory
```

### Shared Layer

#### `Shared`
Common DTOs and utilities:
```
Shared/
├── Dtos/
│   ├── ProductDto/              # Product DTOs
│   ├── OrderDto/                # Order DTOs
│   ├── BasketDto/               # Basket DTOs
│   ├── IdentityDto/             # Auth DTOs
│   └── AiSearch/                # Vector search DTOs
├── Common/                      # Configuration models
├── ErrorModels/                 # Error response models
└── Enums/                       # Shared enumerations
```

### Testing

#### `Testing/Tests`
Comprehensive unit and integration tests:
```
Tests/
├── Services/                    # Service layer tests
├── Repositories/                # Repository layer tests
└── Fixtures/                    # Test base classes & fixtures
```

---

## ✨ Key Features

### 1. **Authentication & Authorization**
- JWT-based authentication
- Refresh token rotation
- Role-based access control
- Secure password hashing with Identity

### 2. **Product Management**
- Full CRUD operations
- Product categorization by type and brand
- Product search with pagination
- Vector-based semantic search integration

### 3. **Shopping Experience**
- Dynamic basket management
- Multiple items per basket
- Basket persistence

### 4. **Order Processing**
- Complete order lifecycle
- Multiple delivery methods
- Order tracking and history
- Address management

### 5. **Payment Integration**
- Stripe payment processing
- Webhook handling for payment events
- Payment status tracking

### 6. **AI-Powered Search**
- Vector embeddings via Ollama
- Semantic similarity search with Qdrant
- RAG (Retrieval-Augmented Generation) responses
- Groq API integration for enhanced results

### 7. **Caching Strategy**
- Redis-based distributed caching
- Automatic cache invalidation
- Cache decorator attributes

### 8. **Data Validation**
- FluentValidation for DTOs
- Automatic validation on API requests
- Comprehensive error responses

### 9. **Error Handling**
- Global exception middleware
- Structured error responses
- Business exception types (NotFoundException, UnauthorizeException, ValidationException)

### 10. **Logging & Monitoring**
- ASP.NET Core logging framework
- Structured error tracking

---

## 🚀 Getting Started

### Prerequisites
- **.NET 8 SDK** or later
- **SQL Server** (Local or remote instance)
- **Redis** (optional, for caching)
- **Qdrant** (optional, for vector search)
- **Visual Studio 2022** or **VS Code**

### Installation

1. **Clone the repository:**
```bash
git clone https://github.com/Eslamrabei/FreshBite.git
cd FreshBite/Back-End
```

2. **Restore NuGet packages:**
```bash
dotnet restore
```

3. **Configure the database connection** in `E-CommerceApi/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;DataBase=E_CommerceWepApiNewFeature;Trusted_Connection=true;",
    "IdentityConnection": "Server=YOUR_SERVER;DataBase=E_CommerceWepApiNewFeature.Identity;Trusted_Connection=true;",
    "RedisConnection": "localhost"
  }
}
```

4. **Apply database migrations:**
```bash
cd E-CommerceApi
dotnet ef database update --project ../Infrastructure/Persistence/Persistence.csproj
```

5. **Run the application:**
```bash
dotnet run
```

The API will be available at `https://localhost:7279`

---

## ⚙️ Configuration

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "SQL Server connection string",
    "IdentityConnection": "Identity database connection",
    "RedisConnection": "Redis connection string"
  },
  "JwtOptions": {
    "Issuer": "JWT issuer",
    "Audience": "JWT audience",
    "ExpirationInDays": 1,
    "SecretKey": "Encryption key (256-bit hex)"
  },
  "StripSettings": {
    "SecretKey": "Stripe API key",
    "EndPointSecret": "Stripe webhook secret"
  },
  "URLS": {
    "BaseUrl": "API base URL",
    "FrontUrl": "Frontend URL for CORS"
  },
  "Groq": {
    "ApiKey": "Groq API key",
    "ModelUrl": "Groq API endpoint"
  },
  "QdrantClient": {
    "Host": "Qdrant server host",
    "Port": "Qdrant server port"
  }
}
```

### Environment Variables
Configure sensitive values via environment variables in production:
- `ConnectionStrings:DefaultConnection`
- `ConnectionStrings:IdentityConnection`
- `JwtOptions:SecretKey`
- `StripSettings:SecretKey`
- `Groq:ApiKey`

---

## 📡 API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/authentication/register` | Register new user |
| POST | `/api/authentication/login` | User login |
| POST | `/api/authentication/refresh-token` | Refresh JWT token |
| GET | `/api/authentication/current-user` | Get current user info |

### Products
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | List products (paginated) |
| GET | `/api/products/{id}` | Get product details |
| POST | `/api/products` | Create product (Admin) |
| PUT | `/api/products/{id}` | Update product (Admin) |
| DELETE | `/api/products/{id}` | Delete product (Admin) |

### Basket
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/basket` | Get basket contents |
| POST | `/api/basket` | Add/update basket item |
| DELETE | `/api/basket/{itemId}` | Remove basket item |

### Orders
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | List user orders |
| GET | `/api/orders/{id}` | Get order details |
| POST | `/api/orders` | Create order |

### Search
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/search` | AI-powered semantic search |

### Payments
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/payments/webhook` | Stripe webhook handler |

---

## 🗄️ Database Setup

### Database Schema

#### StoreDbContext (Products & Orders)
- **Products** - Product catalog
- **ProductBrands** - Brand categories
- **ProductTypes** - Product type categories
- **Orders** - Customer orders
- **OrderItems** - Line items in orders
- **DeliveryMethods** - Shipping options

#### IdentityStoreDbContext (Users)
- **Users** - User accounts (extends IdentityUser)
- **RefreshTokens** - JWT refresh tokens
- **UserRoles** - User role mappings

### Running Migrations

#### Create a new migration:
```bash
dotnet ef migrations add MigrationName --project Infrastructure/Persistence/Persistence.csproj
```

#### Apply migrations:
```bash
dotnet ef database update --project Infrastructure/Persistence/Persistence.csproj
```

#### Revert to previous migration:
```bash
dotnet ef database update PreviousMigrationName --project Infrastructure/Persistence/Persistence.csproj
```

### Data Seeding
Initial data is seeded via `DataSeeding.cs` which runs automatically on application startup:
```csharp
await app.UseSeedDataAsync();
```

---

## 🧪 Testing

### Running Tests

#### Run all tests:
```bash
dotnet test
```

#### Run specific test project:
```bash
dotnet test Testing/Tests/Tests.csproj
```

#### Run with code coverage:
```bash
dotnet test /p:CollectCoverage=true
```

### Test Structure

**Unit Tests** cover:
- Service business logic
- Repository data access patterns
- Specification query builders

**Integration Tests** cover:
- Repository operations with mocked DbContext
- Service orchestration

### Test Fixtures
`TestFixture` base class provides:
- AutoFixture for test data generation
- Mock setup helpers
- Common assertions

---

## 🔐 Security Features

1. **JWT Token Management**
   - Configurable expiration
   - Secure refresh token rotation
   - Token revocation support

2. **Password Security**
   - Bcrypt hashing via Identity
   - Password complexity requirements
   - Secure password reset

3. **Authorization**
   - Role-based access control
   - Claim-based authorization
   - Endpoint-level authorization attributes

4. **Data Validation**
   - Input sanitization via FluentValidation
   - SQL injection prevention via parameterized queries
   - CORS configuration for frontend

5. **Error Handling**
   - No sensitive information in error messages
   - Comprehensive exception logging

---

## 🎨 Code Style & Conventions

- **Language:** C# 12 with nullable reference types enabled
- **Naming:** PascalCase for classes/methods, camelCase for variables
- **Async/Await:** All I/O operations are async
- **Exception Handling:** Typed exceptions, minimal try-catch
- **Comments:** XML documentation on public members
- **Dependencies:** Constructor injection via DI container

---

## 📦 NuGet Dependencies

### Core
- Microsoft.EntityFrameworkCore (8.0+)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore

### Validation
- FluentValidation (12.1+)
- FluentValidation.DependencyInjectionExtensions

### Mapping
- AutoMapper (12.0+)

### Caching
- StackExchange.Redis

### Testing
- xunit (2.4+)
- Moq (4.15+)
- FluentAssertions (6.0+)
- AutoFixture (4.17+)

---

## 🐛 Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure proper authentication credentials

### JWT Token Errors
- Verify `JwtOptions` configuration
- Check token expiration time
- Ensure secret key is 256-bit (64 hex characters)

### Redis Connection Issues
- Verify Redis server is running (default: localhost:6379)
- Check `ConnectionStrings:RedisConnection` setting

### Migration Issues
```bash
# Reset database (caution: deletes all data)
dotnet ef database drop --project Infrastructure/Persistence/Persistence.csproj
dotnet ef database update --project Infrastructure/Persistence/Persistence.csproj
```

---

## 🤝 Contributing

1. Create a feature branch: `git checkout -b feature/YourFeature`
2. Follow the code style conventions
3. Write unit tests for new features
4. Ensure all tests pass: `dotnet test`
5. Commit with clear messages: `git commit -m "Add YourFeature"`
6. Push and create a Pull Request

---

## 📄 License

This project is part of the FreshBite e-commerce platform. Check the repository for license details.

---

## 👤 Author

**Eslam Rabei** - [GitHub](https://github.com/Eslamrabei)

---

## 📞 Support

For issues and questions, please open an issue on [GitHub Issues](https://github.com/Eslamrabei/FreshBite/issues).

---

## 🗺️ Project Roadmap

- [x] Core e-commerce functionality
- [x] JWT authentication & refresh tokens
- [x] Product catalog with search
- [x] Shopping basket & orders
- [x] Stripe payment integration
- [x] Redis caching
- [x] Vector database search (Qdrant)
- [x] AI integration (Groq, Ollama)
- [ ] GraphQL API support
- [ ] WebSocket notifications
- [ ] Advanced analytics dashboard
- [ ] Multi-language support
- [ ] Microservices architecture

---

**Last Updated:** November 2024  
**Version:** 1.0.0

