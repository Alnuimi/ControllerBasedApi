# Controller-Based API

A concise README for a controller-based, RESTful API built around controllers that return action results and support content negotiation.

## Overview
This API uses controller classes to group related endpoints. Each controller exposes actions (methods) that return standardized action results (HTTP status, headers, body). Content negotiation determines response format (JSON by default).

## Features
- RESTful endpoint conventions (GET, POST, PUT, DELETE)
- Action results with consistent status codes and error payloads
- Content negotiation (Accept/Content-Type)
- Dependency injection for controller services
- Clear separation: Controllers → Services → Models → Persistence

## Architecture
- Controllers: handle request validation, call services, return action results
- Services: business logic, transaction boundaries
- Models/DTOs: request and response shapes
- Middleware: authentication, logging, error handling, content negotiation

## Getting started
Prerequisites:
- Runtime (.NET9.0)
- Package manager and build tools

Quick start:
1. Install dependencies
2. Run the application
3. Open API at http://localhost:5000/ or configured port

Example requests:
- Options /api/products
- HEAD /api/products/{{productId}}
- GET  /api/products/{{productId}}
- GET  /api/products?page=4&pageSize=4
- POST /api/products
  Content-Type: application/json

  {
    "name": "New Product",
    "price": 19.99
  }

- PUT /api/products/{{productId}}
  Content-Type: application/json

  {
    "name": "Updated Product Name",
    "price": 29.99
  }

- PATCH /api/products/{{productId}}
  Content-Type: application/json-patch+json

  [
    { "op": "replace", "path": "/price", "value": 24.99 }
  ]

- DELETE /api/products/{{productId}}

Curl example:
curl -H "Accept: application/json" http://localhost:5000/items

## Conventions
- Successful GET: 200 OK
- Resource created: 201 Created (Location header)
- No content: 204 No Content
- Validation error: 400 Bad Request (standard error DTO)
- Not found: 404 Not Found
- Not found: 406 Not Acceptable
- Server error: 500 Internal Server Error


## Content Negotiation
- Default response format: application/json
- Honor Accept header; fall back to default when unsupported
- Use Content-Type for request bodies

