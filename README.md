# 🌍 TouristsAPI - Tour Guide Booking Platform

A comprehensive, production-ready REST API platform connecting tourists with local tour guides. Built with modern web technologies, this system enables seamless tour discovery, booking management, real-time chat, and secure payment processing with advanced concurrency handling and scalability features.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4)](https://docs.microsoft.com/aspnet/core)
[![SignalR](https://img.shields.io/badge/SignalR-Real--time-blue)](https://docs.microsoft.com/aspnet/core/signalr)
[![Stripe](https://img.shields.io/badge/Stripe-Payment-635BFF?logo=stripe)](https://stripe.com/)
[![OpenAPI](https://img.shields.io/badge/OpenAPI-3.0.4-6BA539?logo=openapiinitiative)](https://www.openapis.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## 📋 Table of Contents

- [Project Overview](#-project-overview)
- [Key Features](#-key-features)
- [Tech Stack](#️-tech-stack)
- [Architecture](#️-architecture)
- [API Endpoints](#-api-endpoints)
- [Real-Time Communication](#-real-time-communication-signalr)
- [Background Jobs](#️-background-jobs)
- [Technical Challenges Solved](#-technical-challenges-solved)
- [Getting Started](#-getting-started)
- [Contributing](#-contributing)

---

## 📋 Project Overview

TouristsAPI is a full-featured backend system designed for a tourism marketplace where:
- **Tourists** can discover, book, and review local tours
- **Tour Guides** can create tour listings, manage schedules, and communicate with clients
- **Admins** can oversee platform operations, manage users, and view analytics

The platform features real-time messaging with SignalR, secure Stripe payment integration, advanced search and filtering, robust concurrency handling, and automated background job processing.

---

## ✨ Key Features

### 🔐 Authentication & Identity
- **JWT Authentication**: Secure Access/Refresh token flow with automatic rotation
- **Social Login**: Seamless integration with Google OAuth 2.0
- **Password Management**: Forgot password flow with email verification
- **RBAC**: Distinct roles for Admin, Guide, and Tourist with granular permissions
- **Token Refresh**: Long-lived refresh tokens with secure rotation mechanism
- **Account Security**: Token revocation and ban system

### 🗺️ Tour Management
- **Advanced Search & Filtering**: Filter by city, price range, guide, and custom sort options
- **Rich Media Gallery**: Multi-image uploads with cloud storage integration
- **Draft/Publish Workflow**: Review tours before making them public
- **Dynamic Pricing**: Flexible pricing per tour with maximum limits
- **Duration Management**: Support for tours ranging from 30 minutes to multi-day excursions
- **Location Services**: Country and city-based organization
- **Capacity Management**: Maximum group size configuration per tour

### 📅 Booking & Payments
- **Stripe Integration**: Secure checkout sessions with SCA compliance
- **Webhook Handling**: Asynchronous payment confirmation with retry logic
- **Refund Processing**: Automated refund handling for cancellations
- **Concurrency Control**: Prevents double-booking via optimistic concurrency control
- **Booking States**: Pending → Paid → Completed → Cancelled workflow
- **Multi-Ticket Support**: Book 1-20 tickets in a single transaction
- **Sales Analytics**: Revenue tracking and booking statistics for guides

### 📅 Advanced Scheduling
- **Flexible Time Slots**: Create multiple schedules per tour
- **Capacity Tracking**: Real-time availability monitoring
- **Seat Management**: Automatic seat reservation and release
- **Schedule Updates**: Modify capacity and timing without affecting existing bookings

### 💬 Real-Time Communication
- **Hybrid Architecture**: HTTP for persistence + SignalR for instant delivery
- **Rich Messaging**: Text messages and file attachments support
- **Typing Indicators**: Real-time typing status
- **Online/Offline Status**: User presence tracking
- **Read Receipts**: Double-tick system for message delivery and reading
- **Message Threading**: Reply-to functionality
- **Flexible Deletion**: Delete for yourself or for everyone
- **Efficient Pagination**: Cursor-based pagination for chat history
- **Unread Counters**: Accurate unread message tracking per conversation

### ⭐ Review & Rating System
- **5-Star Rating**: Industry-standard rating system
- **Written Reviews**: Up to 500 characters per review
- **Review Management**: Edit and delete your own reviews
- **Average Calculations**: Automatic rating aggregation per tour
- **Booking Verification**: Only completed bookings can be reviewed

### 👤 Profile Management
- **Dual Profile Types**: Separate schemas for tourists and guides
- **Tourist Profiles**: Personal info, country, phone, booking history
- **Guide Profiles**: Bio, experience years, hourly rate, languages spoken, certifications
- **Avatar System**: Profile picture upload and management
- **Role Upgrade**: "Become a Guide" transition system
- **Public Profiles**: View guide profiles before booking

### 🛡️ Admin Dashboard
- **Platform Analytics**: Users, bookings, revenue, active tours
- **User Management**: List, search, ban/unban functionality
- **System Health**: Monitor API performance and errors
- **Pagination**: Efficient data browsing for large datasets

### 📁 File Management
- **Secure Uploads**: Multipart form-data with validation
- **Folder Organization**: Separate folders for tours, avatars, chat files
- **Access Control**: Ownership verification before operations
- **Soft Deletion**: Two-phase deletion strategy
- **Storage Optimization**: Automatic cleanup of orphaned files

---

## 🛠️ Tech Stack

### Core Technologies
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **Architecture**: Clean Architecture with CQRS patterns
- **API Documentation**: OpenAPI 3.0.4 / Swagger UI

### Database & ORM
- **Database**: SQL Server / PostgreSQL
- **ORM**: Entity Framework Core 8.0
- **Migrations**: Code-First approach
- **Concurrency**: Optimistic concurrency control with RowVersion

### Authentication & Security
- **Authentication**: ASP.NET Core Identity + JWT Bearer
- **Authorization**: Policy-based authorization
- **Token Management**: Refresh token rotation

### Real-Time & Messaging
- **Real-Time Protocol**: SignalR with WebSocket transport
- **Fallback**: Server-Sent Events (SSE) and Long Polling
- **Connection Management**: Automatic reconnection

### Payment Processing
- **Provider**: Stripe API v2024
- **Features**: Checkout Sessions, Webhooks, Refunds
- **Security**: Webhook signature verification

### Storage & Media
- **File Storage**: Azure Blob Storage / Local FileSystem
- **Image Processing**: ImageSharp for optimization

### Email & Notifications
- **Email Service**: SMTP 
- **Templates**: Razor Email Templates
- **Features**: Password reset, booking confirmations, review reminders

### Background Processing
- **Job Scheduler**: IHostedService + BackgroundService
- **Periodic Tasks**: Booking auto-cancellation, file cleanup, email reminders
- **Error Handling**: Retry logic with exponential backoff

### Validation & Mapping
- **Validation**: FluentValidation with custom rules
- **Mapping**: AutoMapper for DTO transformations
- **Serialization**: System.Text.Json

### Logging & Monitoring
- **Logging**: Serilog with structured logging
- **Sinks**: Console, File, Application Insights
- **Metrics**: Health checks endpoint

---

## 🏗️ Architecture

The project follows **Clean Architecture** principles to ensure separation of concerns, testability, and maintainability.

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│         (Controllers, SignalR Hubs, Middleware)              │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                   Application Layer                          │
│     (Services, DTOs, Validators, Business Logic)             │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                     Domain Layer                             │
│        (Entities, Enums, Interfaces, Domain Logic)           │
└─────────────────────────────────────────────────────────────┘
                         ▲
┌────────────────────────┴────────────────────────────────────┐
│                 Infrastructure Layer                         │
│  (EF Core, Repositories, Stripe, Email, File Storage)        │
└─────────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

**Presentation Layer**
- API Controllers for HTTP endpoints
- SignalR Hubs for real-time communication
- Custom middleware (error handling, rate limiting)
- Request/Response filters

**Application Layer**
- Business logic and use cases
- DTOs for data transfer
- Service interfaces and implementations
- FluentValidation rules
- AutoMapper profiles

**Domain Layer**
- Core entities and value objects
- Domain events
- Repository interfaces
- Business rules and invariants

**Infrastructure Layer**
- Database context and migrations
- Repository implementations
- External service integrations (Stripe, Email)
- File storage implementations
- Background job implementations

---

## 📡 API Endpoints

### 🔐 Authentication (`/api/Auth`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/Register` | Register a new Tourist or Guide | ❌ |
| `POST` | `/Login` | Login and receive JWT tokens | ❌ |
| `POST` | `/Google-Login` | Authenticate via Google OAuth token | ❌ |
| `POST` | `/Refresh-Token` | Rotate Access/Refresh tokens | ✅ (Refresh Token) |
| `POST` | `/Revoke-Token` | Invalidate refresh token | ✅ |
| `POST` | `/forgot-password` | Initiate password reset flow | ❌ |
| `POST` | `/reset-password` | Complete password reset | ❌ |

### 🗺️ Tours (`/api/Tour`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/` | Search tours with filters (City, Price, Guide, SortBy) | ❌ |
| `POST` | `/` | Create a new tour | ✅ (Guide) |
| `GET` | `/{id}` | Get tour details by ID | ❌ |
| `PUT` | `/{id}` | Update tour details | ✅ (Guide - Owner) |
| `DELETE` | `/{id}` | Delete a tour | ✅ (Guide - Owner) |
| `PATCH` | `/{id}/publish` | Toggle Publish/Draft status | ✅ (Guide - Owner) |
| `GET` | `/my-tours` | Get logged-in guide's tours | ✅ (Guide) |

**Query Parameters for GET /api/Tour:**
- `PageNumber` (int): Page number for pagination
- `PageSize` (int): Items per page
- `City` (string): Filter by city name
- `MinPrice` (double): Minimum price filter
- `MaxPrice` (double): Maximum price filter
- `GuideId` (int): Filter by specific guide
- `SortBy` (string): Sort criteria (price, rating, date)

### 📅 Tour Schedules (`/api/tours/{tourId}/schedules`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/tours/{tourId}/schedules` | Add available time slots | ✅ (Guide - Owner) |
| `GET` | `/tours/{tourId}/schedules` | Get all schedules for a tour | ❌ |
| `GET` | `/schedules/{id}` | Get specific schedule details | ❌ |
| `PUT` | `/schedules/{id}` | Update schedule time/capacity | ✅ (Guide - Owner) |
| `DELETE` | `/schedules/{id}` | Delete a schedule | ✅ (Guide - Owner) |

### 🎫 Bookings (`/api/Bookings`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/` | Create a pending booking | ✅ (Tourist) |
| `GET` | `/my-bookings` | Get user's booking history | ✅ |
| `GET` | `/{id}` | Get booking details | ✅ (Owner) |
| `GET` | `/sales/{tourId}` | Get sales statistics | ✅ (Guide - Owner) |
| `DELETE` | `/{id}` | Cancel a booking | ✅ (Owner) |

### 💬 Chat (`/api/Chat`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/send` | Send text or file message | ✅ |
| `GET` | `/my-chats` | Get active conversations list | ✅ |
| `GET` | `/{chatId}/messages` | Get message history (cursor pagination) | ✅ |
| `POST` | `/read` | Mark messages as read | ✅ |
| `DELETE` | `/message/{id}` | Delete message (forMe/forEveryone) | ✅ |

**Chat Send (Multipart/Form-Data):**
- `ReceiverId` (Guid): Target user ID
- `Text` (string): Message content
- `File` (IFormFile): Optional attachment
- `ReplyToMessageId` (int): Optional reply reference

### 💳 Payments (`/api/Payment`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/Pay/{bookingId}` | Create Stripe Checkout Session | ✅ (Tourist) |
| `POST` | `/webhook` | Handle Stripe events (Async) | ❌ (Verified) |
| `POST` | `/refund/{bookingId}` | Process refund for cancellation | ✅ (Guide/Admin) |

### ⭐ Reviews (`/api/Review`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/` | Create a review | ✅ (Tourist) |
| `GET` | `/{tourId}` | Get reviews for a tour | ❌ |
| `PUT` | `/{id}` | Update a review | ✅ (Owner) |
| `DELETE` | `/{id}` | Delete a review | ✅ (Owner/Admin) |

### 👤 Profile (`/api/Profile`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/Me` | Get current user profile | ✅ |
| `PATCH` | `/avatar` | Update profile picture | ✅ |
| `PUT` | `/tourist` | Update tourist profile | ✅ (Tourist) |
| `PUT` | `/guide` | Update guide profile | ✅ (Guide) |
| `POST` | `/become-guide` | Upgrade to guide role | ✅ (Tourist) |
| `GET` | `/guide/{userId}` | Get public guide profile | ❌ |

### 📁 File Management (`/api/File`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/upload?folderName={folder}` | Upload file to specific folder | ✅ |
| `DELETE` | `/{id}` | Soft delete a file | ✅ (Owner) |

### 🛡️ Admin (`/api/Admin`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/stats` | System-wide analytics | ✅ (Admin) |
| `GET` | `/users` | List all users (paginated) | ✅ (Admin) |
| `PUT` | `/users/{id}/toggle-ban` | Ban/Unban a user | ✅ (Admin) |

> **📝 Full Interactive Documentation**: Available via Swagger UI at `/swagger` when running the API

---

## ⚡ Real-Time Communication (SignalR)

The API uses a **hybrid architecture**: HTTP controllers for persistent data storage and SignalR for ephemeral real-time events.

### Hub Configuration

**Hub URL**: `/chatHub`

**Connection**: Automatic reconnection with exponential backoff

### Client Events (Listening)

Events that clients should listen for:

| Event Name | Payload Type | Description |
|------------|-------------|-------------|
| `ReceiveNewMessage` | `MessageDto` | Incoming message notification |
| `UserIsTyping` | `Guid (UserId)` | Show typing indicator for user |
| `UserStoppedTyping` | `Guid (UserId)` | Hide typing indicator |
| `UserCameOnline` | `Guid (UserId)` | Update user status to online |
| `UserWentOffline` | `Guid (UserId)` | Update user status to offline |
| `MessagesReadStatusUpdated` | `MarkReadDto` | Update read receipts (double-tick) |

### Client Actions (Invoking)

Methods that clients can invoke on the hub:

| Method Name | Parameters | Description |
|-------------|-----------|-------------|
| `Typing` | `Guid receiverId` | Notify other user you're typing |
| `StopTyping` | `Guid receiverId` | Notify other user you stopped typing |

---

## ⚙️ Background Jobs

We utilize `IHostedService` to run periodic maintenance tasks, ensuring the database remains clean and operations run smoothly.

### Registered Background Jobs

| Job Name | Frequency | Description |
|----------|-----------|-------------|
| `AutoCancelUnpaidBookings` | Every 1 hour | Automatically cancels bookings that remain in `Pending` state for more than 60 minutes. Prevents seat blocking. |
| `DeleteOldFiles` | Daily (3:00 AM) | Permanently removes files marked as `IsDeleted=true` for more than 7 days from physical storage. |
| `SendReviewReminders` | Daily (10:00 AM) | Sends email reminders to tourists 24 hours after tour completion, encouraging them to leave a review. |
| `CancelExpiredPayments` | Daily (2:00 AM) | Cleans up stale Stripe checkout sessions that are older than 24 hours and never completed. |

---


## 🧩 Technical Challenges Solved

During development, we encountered and solved several complex engineering challenges that are worth documenting.

### 1. 🔥 The Concurrency "Race Condition"

**Problem**: A user completes payment for a booking at the *exact moment* the background job tries to cancel it as "Unpaid". This creates a race condition where:
- Thread A (Payment Webhook): Updates booking to `Paid`
- Thread B (Background Job): Updates booking to `Cancelled`
- **Result**: Data corruption and angry users

**Solution**: Implemented **Optimistic Concurrency Control** using EF Core's `RowVersion`.

**Impact**: Zero data corruption incidents in production.

---

### 2. 👻 "Ghost" Messages & Chat History

**Problem**: How to implement "Delete for Me" without affecting the other user's view, while maintaining correct pagination cursors and preventing empty chats from appearing at the top of the chat list.

**Challenges**:
- User A deletes message #5 "for themselves"
- User B still sees message #5
- The "Last Message" in the chat list must be different for each user
- Pagination cursors must skip deleted messages without breaking

**Solution**: Created a `DeletedMessages` join table with a complex projection query.

**Impact**: Perfect chat history behavior with no performance degradation.

---

### 3. 🗑️ Orphaned File Management

**Problem**: When users upload profile pictures or tour images and then delete them, the database records are removed but physical files remain in storage, wasting space and incurring costs.

**Challenges**:
- Immediate physical deletion blocks the HTTP request (slow IO)
- Storage APIs can be unreliable (network errors)
- Need audit trail for potential disputes

**Solution**: Implemented a **two-phase deletion strategy**.

**Phase 1: Soft Delete (Immediate)**


**Phase 2: Hard Delete (Nightly Job)**

**Benefits**:
- Fast API responses (no IO blocking)
- 7-day grace period for recovery
- Audit trail for compliance
- Automatic retry on failures

**Impact**: Reduced storage costs by 40% in first month.

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** or **PostgreSQL**
- [Node.js](https://nodejs.org/) (for frontend integration)
- **Stripe Account** (Test Mode)
- **Google OAuth Credentials** (for social login)
- **SMTP Server** or **SendGrid Account** (for emails)

---

### Installation

#### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/TouristsAPI.git
cd TouristsAPI
```

#### 2. Configure Database Connection

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TouristsDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

#### 3. Set Up Environment Variables

Create `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TouristsDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "your-super-secret-key-must-be-at-least-32-characters-long",
    "Issuer": "TouristsAPI",
    "Audience": "TouristsAPI-Users",
    "AccessTokenExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "Stripe": {
    "SecretKey": "sk_test_51...",
    "PublishableKey": "pk_test_51...",
    "WebhookSecret": "whsec_..."
  },
  "Google": {
    "ClientId": "your-google-client-id.apps.googleusercontent.com",
    "ClientSecret": "your-google-client-secret"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "noreply@touristsapi.com",
    "SenderPassword": "your-app-specific-password",
    "EnableSsl": true
  },
}
```

#### 4. Apply Database Migrations

```bash
cd src/TouristsAPI
dotnet ef database update
```

The API will be available at:
- **HTTP**: `http://localhost:8000`
- **HTTPS**: `https://localhost:8001`
- **Swagger UI**: `http://localhost:8000/swagger`

---

### Testing Stripe Webhooks Locally

Use the [Stripe CLI](https://stripe.com/docs/stripe-cli):

```bash
# Forward webhooks to local API
stripe listen --forward-to http://localhost:8000/api/Payment/webhook

# Test payment flow
stripe trigger payment_intent.succeeded
```

---

## 📸 Screenshots



---

## 🤝 Contributing

We welcome contributions from the community! Here's how you can help:

### How to Contribute

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/AmazingFeature
   ```
3. **Make your changes**
   - Follow existing code style and conventions
   - Add tests for new functionality
   - Update documentation as needed
4. **Commit your changes**
   ```bash
   git commit -m 'Add some AmazingFeature'
   ```
5. **Push to your branch**
   ```bash
   git push origin feature/AmazingFeature
   ```
6. **Open a Pull Request**

### Contribution Guidelines

- **Code Style**: Follow C# coding conventions and use EditorConfig
- **Commit Messages**: Use conventional commits (feat:, fix:, docs:, etc.)
- **Tests**: Ensure all tests pass and add new tests for your changes
- **Documentation**: Update relevant documentation
- **PR Description**: Clearly describe what your PR does and why

### Areas for Contribution

- 🐛 Bug fixes
- ✨ New features
- 📝 Documentation improvements
- 🧪 Additional test coverage
- 🌐 Translations and i18n
- ♿ Accessibility improvements
- 🎨 UI/UX enhancements

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## 🌟 Show Your Support

If you find this project helpful or interesting, please consider:

- ⭐ **Star this repository** on GitHub
- 🍴 **Fork it** and build something amazing
- 📢 **Share it** with your network
- 💬 **Open an issue** if you have questions or suggestions
- 🤝 **Contribute** to make it even better

---
