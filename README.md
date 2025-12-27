# 🛒 FreshBite: Intelligent Enterprise E-Commerce

![.NET 8](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![Onion Architecture](https://img.shields.io/badge/Architecture-Onion-green?style=for-the-badge)
![AI RAG](https://img.shields.io/badge/AI-RAG%20Powered-orange?style=for-the-badge)
![Redis](https://img.shields.io/badge/Redis-Caching-red?style=for-the-badge)

## 📖 Overview

**FreshBite** is a cutting-edge E-Commerce solution that bridges the gap between **Enterprise Stability** and **Modern AI Innovation**.

Built on a robust **Onion Architecture** backend, it ensures scalability and maintainability using industry-standard patterns (Repository, Unit of Work, Specification). On the frontend, it delivers a **Next-Gen User Experience** featuring an **AI Shopping Assistant** capable of semantic product discovery using **Retrieval-Augmented Generation (RAG)**.

---

## 🚀 Key Features

### 🧠 1. AI & Modern UX (The Innovation)
- **🤖 Smart AI Assistant:** A floating chat widget powered by **Llama 3 (via Groq)** & **Qdrant Vector DB**.
- **🔍 Semantic Search (RAG):** Understands user intent (e.g., *"Healthy breakfast under 50 EGP"*) and retrieves context-aware results.
- **⚡ Interactive UI:** Built with **Angular 17 Signals**, featuring auto-scroll, suggestion chips, and HTML-formatted AI responses.
- **🛒 Conversational Commerce:** Users can **add products to the cart directly** within the chat interface.

### 🏗️ 2. Enterprise Backend (The Foundation)
- **✅ Clean Architecture (Onion):** Strict separation of concerns for high testability and scalability.
- **✅ Advanced Patterns:** Implements **Repository Pattern**, **Generic Repository**, **Unit of Work**, and **Specification Pattern** for flexible data querying.
- **✅ Performance Optimization:** High-performance caching using **Redis** with custom Attributes for dynamic cache invalidation.
- **✅ Security:** Robust **JWT Authentication** & Authorization system integrated with ASP.NET Identity.
- **✅ Error Handling:** Centralized Global Exception Handling Middleware.

---

## 🛠️ Tech Stack

| Layer | Technology |
|:------|:------------|
| **Backend Framework** | ASP.NET Core 8 Web API |
| **Frontend** | Angular 17+ (Signals, Standalone Components) |
| **AI Engine** | Groq API (Llama 3.1), Qdrant (Vector DB) |
| **Database** | SQL Server (EF Core), Redis (Caching) |
| **Architecture** | Onion Architecture (Clean Arch) |
| **Patterns** | Repository, Unit of Work, Specification, CQRS-style Services |

---

## 🧩 Project Architecture
The solution follows a strict **Onion Architecture** to decouple dependencies:

```text
ECommerceApp/
│
├── API/                    # Entry Point
│ ├── Controllers/          # API Endpoints
│ ├── Middleware/           # Global Exception Handling
│ ├── Extensions/           # Program.cs Cleanups
│ └── Factories/            # Validation Responses
│
├── Infrastructure/         # External Concerns
│ ├── Persistence/          # EF Core, Migrations, Seeding
│ ├── Identity/             # Auth Configuration
│ ├── Repositories/         # Repositories Implementation
│ └── AI/                   # Groq & Vector DB Services
│
├── Core/                   # The Heart (Business Logic)
│ ├── Domain/               # Entities & Enterprise Rules
│ ├── Service/              # Business Services & Specifications
│ └── Service.Abstractions/ # Interfaces (Contracts)
│
└── Shared/                 # Cross-Cutting Concerns (DTOs)
