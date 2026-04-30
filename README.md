# RuleWayCase - E-Commerce Product API

## Description
This project is a .NET 8 Web API for managing products and categories with business rules.

## Features
- Product CRUD operations
- Category management
- Stock-based "IsLive" validation
- Filter endpoint (search + stock range)

## Technologies
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core (InMemory DB)

## Endpoints

### Product
- POST /api/products
- GET /api/products
- GET /api/products/{id}
- PUT /api/products/{id}
- DELETE /api/products/{id}

### Filter
- GET /api/products/filter?search=...&minStock=...&maxStock=...

### Category
- POST /api/products/category

## 🧪 Example

### Create Category
{
  "name": "Electronics",
  "minStockQuantity": 10
}

### Create Product
{
  "title": "iPhone",
  "description": "Apple phone",
  "stockQuantity": 13,
  "categoryId": 1
}
