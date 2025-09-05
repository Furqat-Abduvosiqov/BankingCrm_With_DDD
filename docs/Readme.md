# Banking CRM – A Domain-Driven Design Journey

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)  
[![EF Core](https://img.shields.io/badge/EF%20Core-8.0-green)](https://learn.microsoft.com/en-us/ef/core/)  
[![DDD](https://img.shields.io/badge/DDD-Clean%20Architecture-orange)](https://domainlanguage.com/ddd/)  
[![License](https://img.shields.io/badge/license-MIT-lightgrey)](https://opensource.org/licenses/MIT)

---

## Why this project?

Modern banking systems are huge, complex, and interconnected. A CRM (Customer Relationship Management) for banking is not just about storing customer names — it touches orders, payments, notifications, analytics, and identity.

This project is a reference implementation that shows how to bring order to that complexity using Domain-Driven Design (DDD), Clean Architecture, and best practices like the Specification Pattern and Repository Pattern.

Think of it as a blueprint for building robust, scalable, and future-proof enterprise systems.

---

## What’s inside?

Here’s what this CRM can do:

- Customer management → profiles, addresses, preferences
- Product catalog → accounts, services, categories, inventory
- Order lifecycle → from creation to payment and shipment
- Payment processing → transactions, invoices, refunds
- Shipping → carriers, tracking, delivery updates
- Notifications → email, SMS, templated messages
- Analytics → dashboards, reports, business intelligence
- Identity & Access → users, roles, permissions

---

## Architecture at a glance

We use Clean Architecture with DDD to keep things modular and testable:

- Domain Layer → where business rules live (Entities, Value Objects, Aggregates, Events, Specs)
- Application Layer → orchestrates behavior (CQRS, services, workflows)
- Infrastructure Layer → persistence and external systems (EF Core, Repositories, Adapters)
- API Layer → the entry point (REST/gRPC APIs, Auth, Minimal APIs)

## Project Layout

```
src/
 ├── BankingCRM.Customers     # Customer management
     ├── BankingCRM.Customers.Domain         # Core domain logic
     ├── BankingCRM.Customers.Application    # CQRS, orchestrations
     ├── BankingCRM.Customers.Infrastructure # EF Core, persistence, integrations
     ├── BankingCRM.Customers.API            # Web API endpoints
     
tests/
   ├── BankingCRM.Customers 
        └── Architecture.Tests         # Architecture and layering tests
        └── BankingCRM.Customers.UnitTests      # Unit tests for domain and application logic
        └── BankingCRM.Customers.IntegrationTests # Integration tests for infrastructure and end-to-end scenarios
        
docs/
   └── README.md                     # This documentation file
   └── ARCHITECTURE.md               # Detailed architecture overview
```

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/)
- [Docker](https://www.docker.com/) (optional, for local setup)

### Run locally

```bash
# Clone the repository
git clone https://github.com/your-org/banking-crm.git
cd banking-crm

# Apply database migrations
dotnet ef database update --project src/BankingCRM.Infrastructure

# Run the API
dotnet run --project src/BankingCRM.API
```

The API will be available at: `https://localhost:5001/swagger`

---

## Testing

```bash
dotnet test
```

- Unit tests cover domain rules, value objects, and specifications
- Integration tests ensure EF Core and services work as expected

## Contributing

We’d love your help.

1. Fork the repo
2. Create a feature branch (`git checkout -b feature/my-feature`)
3. Commit changes (`git commit -m 'Add awesome feature'`)
4. Push your branch (`git push origin feature/my-feature`)
5. Open a Pull Request

---

## Disclaimer

This project is intended for **educational and reference purposes only**.  
It demonstrates how to structure a complex system using **Domain-Driven Design** and **Clean Architecture** principles.  
It is **not production-ready** and should be carefully adapted, reviewed, and tested before being used in any real banking or financial environment.

---
## License
This project is licensed under the MIT License. See [MIT License](https://opensource.org/licenses/MIT) for details.


