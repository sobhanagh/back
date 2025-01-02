# Gamatrain Backend

Gamatrain Backend is an ASP.NET Core-based RESTful API designed to support the Gamatrain platform. This backend handles user management, course management, governance tokens, and other core functionalities of the Gamatrain ecosystem.

---

## Table of Contents
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

---

## Features
- User Authentication and Management
- Course Management (CRUD Operations)
- Database Integration using Entity Framework Core
- Lightweight and Scalable RESTful API
- Unit Testing for Key Endpoints

---

## Technology Stack
- **Backend Framework:** ASP.NET Core
- **Database:** PostgreSQL
- **Authentication:** JWT (JSON Web Token)
- **Development Tools:** 
  - Entity Framework Core
  - Swagger (API Documentation)

---

## Getting Started

### Prerequisites
Before you begin, ensure you have the following installed:
- [.NET SDK 7.0 or later](https://dotnet.microsoft.com/download)

### Installation
1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/gamatrain-backend.git
   cd gamatrain-backend


### Run
1. **Production**
    ```bash
    dotnet publish -c Release -o out      
