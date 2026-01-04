# 🛒 FreshBite: Intelligent Enterprise E-Commerce

![.NET 8](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=for-the-badge)
![Angular](https://img.shields.io/badge/Angular-17+-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![Onion Architecture](https://img.shields.io/badge/Architecture-Onion-2ea44f?style=for-the-badge)
![AI RAG](https://img.shields.io/badge/AI-RAG%20Powered-FF6B35?style=for-the-badge)
![Redis](https://img.shields.io/badge/Cache-Redis-DC382D?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

---

## 📖 Overview

**FreshBite** is a state-of-the-art E-Commerce platform that seamlessly merges **Enterprise Stability** with **Modern AI Innovation**. 

The **Backend** leverages a robust **Onion Architecture** (Clean Architecture principles) ensuring enterprise-grade scalability, maintainability, and testability. It implements industry-standard design patterns including **Repository Pattern**, **Unit of Work**, **Specification Pattern**, and comprehensive **JWT-based security**.

The **Frontend** (Angular 17+) delivers a next-generation user experience with an **AI Shopping Assistant** — a conversational interface powered by **Llama 3.1 (via Groq)** and **Qdrant Vector Database** for semantic product discovery using **Retrieval-Augmented Generation (RAG)**.

**Result:** An intelligent e-commerce experience where customers can have natural conversations to discover products, get personalized recommendations, and complete purchases — all while maintaining enterprise-level performance and security.

---

## ⭐ Standout Features

### 🧠 AI & Modern UX
- **🤖 Intelligent Shopping Assistant:** Conversational AI that understands customer intent beyond keyword matching
- **🔍 Semantic Product Discovery (RAG):** Retrieves context-aware results (e.g., *"Healthy breakfast under 50 EGP"*)
- **💬 Interactive Chat Widget:** Real-time, HTML-formatted responses with product suggestions
- **🛒 Seamless Cart Integration:** Add products to cart directly from AI suggestions
- **⚡ Modern Frontend Stack:** Angular 17+ with Signals and Standalone Components

### 🏗️ Enterprise Backend Architecture
- **✅ Onion Architecture:** Strict separation of concerns across 5 layers with zero cross-layer dependencies
- **✅ Advanced Design Patterns:** 
  - Generic Repository Pattern for reusable data access
  - Unit of Work for transaction management
  - Specification Pattern for complex, type-safe queries
  - Dependency Injection container for loose coupling
- **✅ High Performance:** Redis caching with custom attributes for dynamic invalidation
- **✅ Security First:** 
  - JWT authentication with refresh token rotation
  - ASP.NET Core Identity integration
  - Role-based & claim-based authorization
- **✅ Production Ready:**
  - Global exception handling middleware
  - Comprehensive input validation (FluentValidation)
  - Structured error responses
  - Automated data seeding

---

## 🛠️ Technology Stack

### Backend Framework
| Component | Technology | Version |
|:----------|:-----------|:--------|
| **Framework** | ASP.NET Core Web API | 8.0 |
| **ORM** | Entity Framework Core | 8.0+ |
| **Authentication** | ASP.NET Core Identity + JWT | - |
| **Validation** | FluentValidation | 12.1+ |
| **Mapping** | AutoMapper | 12.0+ |
| **Testing** | xUnit + Moq + FluentAssertions | Latest |

### Data & Infrastructure
| Component | Technology | Purpose |
|:----------|:-----------|:--------|
| **Primary Database** | SQL Server | Product catalog, orders, users |
| **Cache Layer** | Redis | Session caching, performance optimization |
| **Vector Database** | Qdrant | Semantic search & embeddings |
| **API** | REST (OpenAPI/Swagger) | Standard HTTP endpoints |

### AI & Integration
| Component | Technology | Purpose |
|:----------|:-----------|:--------|
| **LLM Engine** | Groq API (Llama 3.1) | AI assistant & semantic understanding |
| **Embeddings** | Ollama | Local vector embeddings generation |
| **Payment Processing** | Stripe | Secure payment handling |
| **Vector Search** | Qdrant Client | Semantic similarity retrieval |

### Frontend (Reference)
- **Framework:** Angular 17+
- **State Management:** Angular Signals
- **HTTP Client:** RxJS + Angular HttpClient
- **Component Style:** Standalone Components

---

## 🧩 Architecture & Design

### Onion Architecture Layers

```
┌─────────────────────────────────────────────────────────────────┐
│                     PRESENTATION LAYER                           │
│              (Controllers, DTOs, API Attributes)                │
│           Infrastructure\Presentation (Presentation.csproj)     │
└─────────────────────────────────────────────────────────────────┘
                                ↓
┌─────────────────────────────────────────────────────────────────┐
│                     APPLICATION LAYER                            │
│         (Business Logic, Services, Specifications)              │
│                  Core\Service (Service.csproj)                  │
└─────────────────────────────────────────────────────────────────┘
                                ↓
┌─────────────────────────────────────────────────────────────────┐
│                    ABSTRACTION LAYER                             │
│                  (Interface Contracts)                           │
│         Core\ServiceAbstraction (ServiceAbstraction.csproj)     │
└─────────────────────────────────────────────────────────────────┘
                                ↓
┌─────────────────────────────────────────────────────────────────┐
│                  INFRASTRUCTURE LAYER                            │
│    (Data Access, Repositories, External Services)               │
│         Infrastructure\Persistence (Persistence.csproj)        │
└─────────────────────────────────────────────────────────────────┘
                                ↓
┌─────────────────────────────────────────────────────────────────┐
│                      DOMAIN LAYER                                │
│        (Entities, Business Rules, Enterprise Logic)             │
│                   Core\Domain (Domain.csproj)                   │
└─────────────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

| Layer | Responsibility | Projects |
|:------|:--------------|:---------|
| **Presentation** | API endpoints, request/response handling, validation attributes | `Presentation.csproj` |
| **Application** | Business logic, data transformation, service orchestration, specifications | `Service.csproj` |
| **Abstraction** | Service & repository interface definitions, contracts | `ServiceAbstraction.csproj` |
| **Infrastructure** | Data access, repositories, EF Core contexts, validations, external integrations | `Persistence.csproj` |
| **Domain** | Entities, business rules, aggregate roots, exceptions, value objects | `Domain.csproj` |
| **Shared** | DTOs, enums, common utilities, error models | `Shared.csproj` |
| **Entry Point** | DI configuration, middleware setup, program bootstrapping | `E-CommerceApi.csproj` |

### Design Patterns Implemented

#### 1. **Repository Pattern**
```
GenericRepository<TEntity, TKey> → Abstraction → Service Layer
```
- Generic base class for standard CRUD operations
- Specialized repositories for complex entities (ProductRepository, BasketRepository)
- Reduces code duplication and maintains consistency

#### 2. **Unit of Work Pattern**
```
UnitOfWork → Multiple Repositories → Single Transaction
```
- Manages multiple repositories as a single transaction
- Ensures data consistency across related operations
- Simplifies complex business transactions

#### 3. **Specification Pattern**
```
Specification<TEntity> → Repository → Dynamic Queries
```
- Encapsulates query logic in reusable, type-safe objects
- Separates filtering, sorting, and pagination concerns
- Makes queries testable and composable
- Examples: `ProductTypeAndBrandSpecifications`, `OrderWithIncludesSpecifications`

#### 4. **Dependency Injection**
```
Interface → Service → Constructor Injection → DI Container
```
- All dependencies registered in `Program.cs`
- Loose coupling between layers
- Easy testing with mock implementations

---

## 📁 Project Structure

### Directory Tree

```
FreshBite/Back-End/
│
├── E-CommerceApi/                          # 🚀 Entry Point & Bootstrapping
│   ├── Program.cs                          # Application startup configuration
│   ├── appsettings.json                    # Configuration & secrets
│   ├── Extensions/
│   │   ├── CoreServicesExtensions.cs       # Core service registration
│   │   ├── InfrastructureServicesExtensions.cs  # Infrastructure setup
│   │   ├── WebApiServices.cs               # Web API configuration
│   │   ├── WebApiManageMiddlwares.cs       # Middleware pipeline
│   │   └── FluentValidationsDI.cs          # Validation registration
│   ├── Middlewares/
│   │   └── GlobalExceptionHandelingMiddleware.cs  # Centralized error handling
│   └── Factories/
│       └── ApiResponseFactory.cs           # Response formatting
│
├── Core/                                    # 💚 Business Logic & Contracts
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── ProductModule/
│   │   │   │   ├── Product.cs             # Product aggregate
│   │   │   │   ├── ProductBrand.cs
│   │   │   │   └── ProductType.cs
│   │   │   ├── OrderModule/
│   │   │   │   ├── Order.cs               # Order aggregate
│   │   │   │   ├── OrderItem.cs
│   │   │   │   ├── DeliveryMethod.cs
│   │   │   │   └── Address.cs
│   │   │   ├── BasketModule/
│   │   │   │   ├── CustomerBasket.cs      # Shopping cart aggregate
│   │   │   │   └── BasketItems.cs
│   │   │   └── IdentityModule/
│   │   │       ├── User.cs                # User aggregate (extends IdentityUser)
│   │   │       ├── RefreshToken.cs        # JWT token management
│   │   │       └── Address.cs
│   │   ├── Contracts/
│   │   │   ├── IGenericRepository.cs      # Base repository interface
│   │   │   ├── IProductRepository.cs
│   │   │   ├── IBasketRepository.cs
│   │   │   ├── IRefreshTokenRepository.cs
│   │   │   ├── ICacheRepository.cs
│   │   │   ├── IUnitOfWork.cs             # Transaction management
│   │   │   ├── ISpecification.cs          # Specification pattern interface
│   │   │   └── IDataSeeding.cs
│   │   └── Exceptions/
│   │       ├── GenericNotFoundException.cs
│   │       ├── UnauthorizeException.cs
│   │       └── ValidationException.cs
│   │
│   ├── Service/
│   │   ├── Implementations/
│   │   │   ├── AuthenticationService.cs     # JWT & identity management
│   │   │   ├── ProductService.cs            # Product operations
│   │   │   ├── BasketService.cs             # Shopping cart logic
│   │   │   ├── OrderService.cs              # Order processing
│   │   │   ├── PaymentService.cs            # Stripe integration
│   │   │   ├── CacheService.cs              # Redis operations
│   │   │   ├── FileService.cs               # File management
│   │   │   ├── RefreshTokenServices.cs      # Token lifecycle
│   │   │   └── OllamaEmbeddingService.cs    # AI embeddings
│   │   ├── Specifications/
│   │   │   ├── BaseSpecification.cs         # Base class for all specs
│   │   │   ├── ProductTypeAndBrandSpecifications.cs
│   │   │   └── OrderWithIncludesSpecifications.cs
│   │   └── MappingProfiles/
│   │       ├── ProductsMapping.cs
│   │       ├── OrderMapping.cs
│   │       ├── BasketMappingProfile.cs
│   │       └── RefreshTokenMapping.cs
│   │
│   └── ServiceAbstraction/
│       └── Contracts/
│           ├── IAuthenticationService.cs
│           ├── IProductService.cs
│           ├── IBasketService.cs
│           ├── IOrderService.cs
│           ├── IPaymentService.cs
│           ├── ICacheService.cs
│           ├── IRefreshTokenServices.cs
│           ├── IEmbeddingService.cs
│           └── IVectorService.cs
│
├── Infrastructure/                         # 🔌 External Concerns & Data Access
│   ├── Persistence/
│   │   ├── Data/
│   │   │   ├── StoreDbContext.cs           # Main database context
│   │   │   ├── Configurations/             # EF Core entity configurations
│   │   │   ├── Migrations/                 # Database schema versions
│   │   │   └── DataSeeding.cs              # Initial data population
│   │   ├── Identity/
│   │   │   ├── IdentityStoreDbContext.cs   # User & authentication context
│   │   │   └── Migrations/                 # Identity schema versions
│   │   ├── Repositories/
│   │   │   ├── GenericRepository.cs        # Base CRUD operations
│   │   │   ├── ProductRepository.cs        # Product-specific queries
│   │   │   ├── BasketRepository.cs         # Shopping cart operations
│   │   │   ├── RefreshTokenRepository.cs   # Token management
│   │   │   ├── CacheRepository.cs          # Redis caching wrapper
│   │   │   ├── UnitOfWork.cs               # Transaction coordination
│   │   │   └── SpecificationEvaluators.cs  # Specification pattern evaluator
│   │   ├── Validations/
│   │   │   └── ProductsValidations/
│   │   │       ├── CreateProductDtoValidations.cs
│   │   │       └── UpdateProductDtoValidation.cs
│   │   └── Implementations/
│   │       ├── VectorDbService.cs          # Qdrant integration
│   │       └── OllamaService.cs            # Ollama embeddings
│   │
│   └── Presentation/
│       ├── Controllers/
│       │   ├── ApiController.cs            # Base controller with common logic
│       │   ├── AuthenticationController.cs # /api/authentication
│       │   ├── ProductsController.cs       # /api/products
│       │   ├── BasketController.cs         # /api/basket
│       │   ├── OrdersController.cs         # /api/orders
│       │   ├── PaymentsController.cs       # /api/payments (webhooks)
│       │   ├── SearchController.cs         # /api/search (AI)
│       │   └── AdminController.cs          # /api/admin (admin operations)
│       └── Attributes/
│           └── RedisCacheAttribute.cs      # Caching decorator
│
├── Shared/                                  # 📦 Cross-Cutting DTOs & Utilities
│   ├── Dtos/
│   │   ├── ProductDto/
│   │   │   ├── ProductsResultDto.cs
│   │   │   ├── CreatedProductDto.cs
│   │   │   ├── BrandResultDto.cs
│   │   │   └── TypeResultDto.cs
│   │   ├── OrderDto/
│   │   │   ├── OrderResultDto.cs
│   │   │   ├── OrderRequest.cs
│   │   │   ├── OrderItemDto.cs
│   │   │   ├── AddressDto.cs
│   │   │   └── DeliverMethodResult.cs
│   │   ├── BasketDto/
│   │   │   ├── BasketDto.cs
│   │   │   └── BasketItemsDto.cs
│   │   ├── IdentityDto/
│   │   │   ├── LoginDto.cs
│   │   │   ├── RegisterDto.cs
│   │   │   ├── UserResultDto.cs
│   │   │   ├── TokenRequestDto.cs
│   │   │   └── RefreshTokenDto.cs
│   │   └── AiSearch/
│   │       ├── ProductSearchResponse.cs
│   │       ├── RagResponseDto.cs
│   │       ├── ProductDtos.cs
│   │       └── UpdateProductDto.cs
│   ├── Common/
│   │   └── JwtOptions.cs                  # JWT configuration
│   ├── ErrorModels/
│   │   ├── ErrorDetails.cs
│   │   ├── ValidationError.cs
│   │   └── ValidationErrorResponse.cs
│   ├── Enums/
│   │   └── SortingOptions.cs
│   └── PaginatedResult.cs                 # Generic pagination wrapper
│
└── Testing/                                 # ✅ Comprehensive Test Suite
    └── Tests/
        ├── Services/
        │   └── AuthenticationServiceTests.cs  # Service layer tests
        ├── Repositories/
        │   └── RefreshTokenRepositoryTests.cs  # Repository tests
        └── Fixtures/
            └── TestFixture.cs             # Test base class & setup
```

---

## 🚀 Quick Start

### Prerequisites
- **Runtime:** .NET 8 SDK or later
- **Database:** SQL Server (Local or remote instance)
- **Optional:** Redis (for caching), Qdrant (for vector search)
- **IDE:** Visual Studio 2022 or VS Code with C# extension

### Installation & Setup

#### 1. Clone the Repository
```bash
git clone https://github.com/Eslamrabei/FreshBite.git
cd FreshBite/Back-End
```

#### 2. Restore Dependencies
```bash
dotnet restore
```

#### 3. Configure Database Connection
Edit `E-CommerceApi/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=E_CommerceDb;Trusted_Connection=true;TrustServerCertificate=true;",
    "IdentityConnection": "Server=YOUR_SERVER;Database=E_CommerceDb.Identity;Trusted_Connection=true;TrustServerCertificate=true;",
    "RedisConnection": "localhost:6379"
  },
  "JwtOptions": {
    "Issuer": "https://localhost:7279",
    "Audience": "AngularApp",
    "ExpirationInDays": 1,
    "SecretKey": "your-256-bit-hex-key-64-characters-long"
  }
}
```

#### 4. Apply Database Migrations
```bash
cd E-CommerceApi
dotnet ef database update --project ../Infrastructure/Persistence/Persistence.csproj
```

#### 5. Run the Application
```bash
dotnet run
```

The API will be available at **`https://localhost:7279`**

---

## ⚙️ Configuration

### appsettings.json Structure

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  
  "ConnectionStrings": {
    "DefaultConnection": "SQL Server connection string",
    "IdentityConnection": "Identity database connection",
    "RedisConnection": "Redis server connection"
  },
  
  "JwtOptions": {
    "Issuer": "JWT token issuer",
    "Audience": "JWT token audience",
    "ExpirationInDays": 1,
    "SecretKey": "256-bit hexadecimal key (64 chars minimum)"
  },
  
  "StripSettings": {
    "SecretKey": "Stripe API secret key",
    "EndPointSecret": "Stripe webhook signing secret"
  },
  
  "URLS": {
    "BaseUrl": "https://localhost:7279/",
    "FrontUrl": "http://localhost:4200"
  },
  
  "Groq": {
    "ApiKey": "Groq API key for Llama access",
    "ModelUrl": "https://api.groq.com/openai/v1/chat/completions"
  },
  
  "QdrantClient": {
    "Host": "localhost",
    "Port": 6334
  }
}
```

### Environment Variables (Production)
For security, override sensitive settings via environment variables:
```bash
export ConnectionStrings__DefaultConnection="..."
export JwtOptions__SecretKey="..."
export StripSettings__SecretKey="..."
export Groq__ApiKey="..."
```

---

## 📡 API Endpoints

### Authentication (`/api/authentication`)
| HTTP | Endpoint | Description | Auth Required |
|:----:|:---------|:------------|:-------------:|
| POST | `/register` | Register new user account | ❌ |
| POST | `/login` | User login with email & password | ❌ |
| POST | `/refresh-token` | Refresh JWT access token | ❌ |
| GET | `/current-user` | Get authenticated user info | ✅ |

**Example Request:**
```bash
curl -X POST https://localhost:7279/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePassword123!"
  }'
```

### Products (`/api/products`)
| HTTP | Endpoint | Description | Auth Required |
|:----:|:---------|:------------|:-------------:|
| GET | `/` | List products with pagination & filters | ❌ |
| GET | `/{id}` | Get product details | ❌ |
| POST | `/` | Create new product | ✅ Admin |
| PUT | `/{id}` | Update product | ✅ Admin |
| DELETE | `/{id}` | Delete product | ✅ Admin |

**Query Parameters:**
- `pageIndex` - Page number (0-based)
- `pageSize` - Items per page
- `sort` - Sort order (name, price)
- `brandId` - Filter by brand
- `typeId` - Filter by type

### Shopping Basket (`/api/basket`)
| HTTP | Endpoint | Description | Auth Required |
|:----:|:---------|:------------|:-------------:|
| GET | `/` | Get current basket | ✅ |
| POST | `/` | Add/update basket item | ✅ |
| DELETE | `/{itemId}` | Remove basket item | ✅ |

### Orders (`/api/orders`)
| HTTP | Endpoint | Description | Auth Required |
|:----:|:---------|:------------|:-------------:|
| GET | `/` | List user orders | ✅ |
| GET | `/{id}` | Get order details | ✅ |
| POST | `/` | Create new order | ✅ |

### AI Search (`/api/search`)
| HTTP | Endpoint | Description | Auth Required |
|:----:|:---------|:------------|:-------------:|
| POST | `/` | Semantic product search (RAG) | ❌ |

**Request Body:**
```json
{
  "query": "Healthy breakfast under 50 EGP",
  "limit": 10
}
```

### Payments (`/api/payments`)
| HTTP | Endpoint | Description | Auth Required |
|:----:|:---------|:------------|:-------------:|
| POST | `/webhook` | Stripe webhook handler | ✅ Webhook Secret |

---

## 🗄️ Database

### Context Separation

**StoreDbContext** (E-Commerce Core)
- Products, ProductBrands, ProductTypes
- Orders, OrderItems, DeliveryMethods

**IdentityStoreDbContext** (Authentication)
- Users (extends IdentityUser)
- RefreshTokens
- Roles, Claims, UserRoles

### EF Core Migrations

```bash
# Create a new migration
dotnet ef migrations add AddNewFeature \
  --project Infrastructure/Persistence/Persistence.csproj \
  --context StoreDbContext

# Apply all pending migrations
dotnet ef database update \
  --project Infrastructure/Persistence/Persistence.csproj

# Revert to specific migration
dotnet ef database update PreviousMigrationName \
  --project Infrastructure/Persistence/Persistence.csproj

# Remove last migration
dotnet ef migrations remove --context StoreDbContext
```

### Data Seeding
Initial data is automatically seeded on first application startup via `DataSeeding.cs`:
```csharp
await app.UseSeedDataAsync();
```

---

## 🧪 Testing

The project includes comprehensive unit and integration tests covering services, repositories, and specifications.

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test Testing/Tests/Tests.csproj

# Run with verbose output
dotnet test -v n

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

### Test Structure

```
Testing/Tests/
├── Services/
│   └── AuthenticationServiceTests.cs
├── Repositories/
│   └── RefreshTokenRepositoryTests.cs
└── Fixtures/
    └── TestFixture.cs         # Base class with common setup
```

### Testing Patterns

- **Unit Tests:** Services, repositories, specifications with mocked dependencies
- **Integration Tests:** Repository patterns with mocked DbContext
- **Fixtures:** AutoFixture for test data generation
- **Mocking:** Moq for dependency injection
- **Assertions:** FluentAssertions for readable test expressions

### Test Example

```csharp
[Fact]
public async Task LoginAsync_WithValidCredentials_ReturnsUserResult()
{
    // Arrange
    var loginDto = new LoginDto { Email = "test@example.com", Password = "Pwd123!" };
    var user = new User { Email = loginDto.Email, DisplayName = "Test User" };
    
    _mockUserManager
        .Setup(um => um.FindByEmailAsync(loginDto.Email))
        .ReturnsAsync(user);
    
    // Act
    var result = await _sut.LoginAsync(loginDto);
    
    // Assert
    result.Should().NotBeNull();
    result.Email.Should().Be(loginDto.Email);
}
```

---

## 🔐 Security

### Authentication & Authorization
- **JWT Tokens:** Secure, stateless authentication with configurable expiration
- **Refresh Token Rotation:** Secure refresh token lifecycle management
- **Password Security:** Bcrypt hashing via ASP.NET Core Identity
- **Password Complexity:** Enforced complexity requirements
- **CORS:** Configured for specific frontend origins

### Authorization Levels
- **Public:** Unauthenticated access (search, product listing)
- **User:** Authenticated users (basket, orders, profile)
- **Admin:** Administrative operations (product management)

### Input Validation
- **FluentValidation:** Automatic DTO validation
- **Data Annotations:** Additional attribute-based validation
- **SQL Injection Prevention:** Parameterized queries via EF Core
- **Error Responses:** Structured validation error feedback

### Best Practices
- No sensitive information in error messages
- Comprehensive exception logging
- Secure default headers configuration
- Regular security updates for dependencies

---

## 📊 Performance Optimization

### Caching Strategy
- **Redis Distributed Cache:** Offload frequently accessed data
- **Cache Attributes:** Custom `[RedisCache]` for automatic invalidation
- **Query Optimization:** Specification pattern for efficient filtering
- **Async/Await:** All I/O operations are non-blocking

### Database Optimization
- **Entity Configurations:** Proper indexing and relationships
- **EF Core Best Practices:** Efficient loading strategies (Include, ThenInclude)
- **Query Specifications:** Type-safe, composable queries

---

## 🎨 Code Style & Standards

### C# Conventions
- **Language Version:** C# 12 with nullable reference types enabled
- **Naming:** PascalCase for public members, camelCase for locals/parameters
- **Async Methods:** Suffix with `Async` (e.g., `GetProductAsync`)
- **Interfaces:** Prefix with `I` (e.g., `IProductService`)
- **Exceptions:** Typed exceptions, minimal try-catch blocks

### Code Organization
- **XML Documentation:** Public members have summary comments
- **Using Statements:** Organized alphabetically
- **Line Length:** Max 120 characters for readability
- **DRY Principle:** No code duplication via abstractions

### Dependency Injection
All dependencies injected via constructor:
```csharp
public class ProductService(
    IGenericRepository<Product, int> repository,
    IMapper mapper,
    ILogger<ProductService> logger) : IProductService
{
    // ...
}
```

---

## 📦 NuGet Package Dependencies

### Core Framework
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0+" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0+" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0+" />
```

### Data Access & ORM
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0+" />
<PackageReference Include="StackExchange.Redis" Version="2.6+" />
```

### Validation & Mapping
```xml
<PackageReference Include="FluentValidation" Version="12.1+" />
<PackageReference Include="AutoMapper" Version="12.0+

