# 🍉 FruitKart

A full-stack microservices e-commerce platform for fresh fruit delivery, built with ASP.NET Core 10 and React.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)
![React](https://img.shields.io/badge/React-20232A?style=flat&logo=react&logoColor=61DAFB)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?style=flat&logo=typescript&logoColor=white)
![Supabase](https://img.shields.io/badge/Supabase-3ECF8E?style=flat&logo=supabase&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker&logoColor=white)
![Azure](https://img.shields.io/badge/Azure-0078D4?style=flat&logo=microsoftazure&logoColor=white)

---

## About

FruitKart is a microservices-based e-commerce application where customers can browse a fruit catalogue, add items to cart, place orders, and track delivery status. It includes an AI-powered chatbot that answers questions about the menu and order history in real time.

The project was built to demonstrate distributed systems design patterns including API Gateway routing, inter-service communication, JWT-based auth across services, and RAG-style LLM context injection.

---

## Architecture

```
┌─────────────────────────────────────────────────────┐
│                React Frontend (Vite)                 │
└─────────────────────────┬───────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────┐
│            FruitKartGateway (Ocelot)                 │
│                    Port: 5000                        │
└──┬──────────┬──────────┬──────────┬─────────────────┘
   │          │          │          │
   ▼          ▼          ▼          ▼
Account    Menu       Order      Image     ChatBot
Service    Service    Service    Service   Service
```

All client requests go through the **Ocelot API Gateway**, which routes to the appropriate downstream service. Services communicate directly with each other for inter-service calls (e.g. OrderService calls MenuService to deduct stock before confirming an order).

---

## Tech Stack

### Backend
| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| Language | C# |
| ORM | Entity Framework Core (Npgsql) |
| Database | PostgreSQL via Supabase |
| API Gateway | Ocelot |
| Auth | JWT Bearer Tokens (shared class library) |
| Email | Brevo (Sendinblue) |
| Images | Cloudinary |
| AI / LLM | Groq API — LLaMA 3.3 70B |

### Frontend
| Layer | Technology |
|---|---|
| Framework | React 18 + Vite |
| Language | JavaScript / JSX |
| State Management | Redux Toolkit + RTK Query |
| Routing | React Router v6 |

---

## Services

| Service | Responsibility | Default Port |
|---|---|---|
| `AccountService` | Registration, login, JWT issuance | 5001 |
| `MenuServiceAPI` | Fruit catalogue, stock management, CRUD | 5142 |
| `OrderService` | Order creation, stock deduction, status updates | 5268 |
| `ImageService` | Cloudinary image upload/delete | 5003 |
| `ChatBotService` | AI chatbot with live menu + order context (RAG) | 5004 |
| `FruitKartGateway` | Ocelot reverse proxy and route aggregation | 5000 |
| `JWTAuth` | Shared class library — JWT configuration and validation | — |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [Supabase account](https://supabase.com/) (free tier works)
- [Cloudinary account](https://cloudinary.com/) (free tier works)
- [Brevo account](https://www.brevo.com/) (free tier works)
- [Groq API key](https://console.groq.com/) (free tier works)

---

## Setup

### 1. Clone the repository

```bash
git clone https://github.com/JayTuli/FruitKart.git
cd FruitKart
```

### 2. Create appsettings.json for each service

Each service needs its own `appsettings.json` inside its project folder. These files are gitignored — create them locally using the templates below.

---

#### `FruitKart_Backend/AccountService/appsettings.json`
```json
{
  "ConnectionStrings": {
    "AccountConnection": "Host=YOUR_SUPABASE_HOST;Database=postgres;Username=YOUR_SUPABASE_USERNAME;Password=YOUR_SUPABASE_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_JWT_SECRET_MIN_32_CHARS",
    "ValidIssuer": "FruitKart-issuer",
    "ValidAudience": "FruitKart-Audience"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

#### `FruitKart_Backend/MenuServiceAPI/appsettings.json`
```json
{
  "ConnectionStrings": {
    "MenuConnection": "Host=YOUR_SUPABASE_HOST;Database=postgres;Username=YOUR_SUPABASE_USERNAME;Password=YOUR_SUPABASE_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_JWT_SECRET_MIN_32_CHARS",
    "ValidIssuer": "FruitKart-issuer",
    "ValidAudience": "FruitKart-Audience"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

#### `FruitKart_Backend/OrderService/appsettings.json`
```json
{
  "Brevo": {
    "ApiKey": "YOUR_BREVO_API_KEY",
    "FromEmail": "your@email.com",
    "FromName": "FruitKart"
  },
  "ConnectionStrings": {
    "OrderConnection": "Host=YOUR_SUPABASE_HOST;Database=postgres;Username=YOUR_SUPABASE_USERNAME;Password=YOUR_SUPABASE_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
  },
  "ServiceUrls": {
    "MenuService": "http://localhost:5142"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_JWT_SECRET_MIN_32_CHARS",
    "ValidIssuer": "FruitKart-issuer",
    "ValidAudience": "FruitKart-Audience"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

#### `FruitKart_Backend/ImageService/appsettings.json`
```json
{
  "ConnectionStrings": {
    "ImageServiceDb": "Host=YOUR_SUPABASE_HOST;Database=postgres;Username=YOUR_SUPABASE_USERNAME;Password=YOUR_SUPABASE_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
  },
  "Cloudinary": {
    "CloudName": "YOUR_CLOUDINARY_CLOUD_NAME",
    "ApiKey": "YOUR_CLOUDINARY_API_KEY",
    "ApiSecret": "YOUR_CLOUDINARY_API_SECRET"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_JWT_SECRET_MIN_32_CHARS"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

#### `FruitKart_Backend/ChatBotService/appsettings.json`
```json
{
  "Groq": {
    "ApiKey": "YOUR_GROQ_API_KEY",
    "Model": "llama-3.3-70b-versatile",
    "SystemPrompt": "You are FruitKart AI Assistant, a helpful chatbot for FruitKart - a premium online fruit delivery service."
  },
  "MenuService": {
    "BaseUrl": "http://localhost:5142"
  },
  "OrderService": {
    "BaseUrl": "http://localhost:5268"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_JWT_SECRET_MIN_32_CHARS",
    "ValidIssuer": "FruitKart-issuer",
    "ValidAudience": "FruitKart-Audience"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

#### `FruitKart_Backend/FruitKartGateway/appsettings.json`
```json
{
  "JwtSettings": {
    "SecretKey": "YOUR_JWT_SECRET_MIN_32_CHARS",
    "ValidIssuer": "FruitKart-issuer",
    "ValidAudience": "FruitKart-Audience"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

> **Note:** Use the same `SecretKey` value across all services — JWT tokens are issued by AccountService and validated by every other service using this shared key.

---

### 3. Run EF Core migrations

Run this for each service that has a database (AccountService, MenuServiceAPI, OrderService, ImageService):

```bash
cd FruitKart_Backend/AccountService
dotnet ef database update

cd ../MenuServiceAPI
dotnet ef database update

cd ../OrderService
dotnet ef database update

cd ../ImageService
dotnet ef database update
```

---

### 4. Run the backend services

Open a terminal for each service and run:

```bash
# Terminal 1
cd FruitKart_Backend/AccountService && dotnet run

# Terminal 2
cd FruitKart_Backend/MenuServiceAPI && dotnet run

# Terminal 3
cd FruitKart_Backend/OrderService && dotnet run

# Terminal 4
cd FruitKart_Backend/ImageService && dotnet run

# Terminal 5
cd FruitKart_Backend/ChatBotService && dotnet run

# Terminal 6 — start gateway last
cd FruitKart_Backend/FruitKartGateway && dotnet run
```

### 5. Run the frontend

```bash
cd mangopedia-react
npm install
npm run dev
```

Frontend runs at `http://localhost:5173`

---

## Features

- **Authentication** — Register, login, role-based access (User / Admin) with JWT
- **Fruit Catalogue** — Browse fruits with images, prices, and live stock counts
- **Cart & Checkout** — Add to cart, place orders with automatic stock deduction
- **Order Management** — Track order status (Confirmed → Ready For Pickup → Completed)
- **Admin Panel** — Add, edit, delete menu items and manage orders
- **AI Chatbot** — Floating chat panel powered by LLaMA 3.3 70B via Groq; fetches live menu and user order data before each response (RAG pattern)
- **Image Service** — Cloudinary-backed image upload for menu items
- **Email Notifications** — Transactional emails via Brevo on order confirmation

---

## Project Structure

```
FruitKart/
├── FruitKart_Backend/
│   ├── AccountService/          # Auth, user management
│   ├── MenuServiceAPI/          # Fruit catalogue, stock
│   ├── OrderService/            # Orders, email notifications
│   ├── ImageService/            # Cloudinary image handling
│   ├── ChatBotService/          # Groq LLM + RAG chatbot
│   ├── FruitKartGateway/        # Ocelot API Gateway
│   └── JWTAuth/                 # Shared JWT class library
└── mangopedia-react/            # React + Vite frontend
    └── src/
        ├── components/          # Reusable UI components
        ├── pages/               # Route-level page components
        ├── store/               # Redux store, slices, RTK Query APIs
        ├── routes/              # Role-based route guards
        └── utility/             # JWT parsing, constants, helpers
```
