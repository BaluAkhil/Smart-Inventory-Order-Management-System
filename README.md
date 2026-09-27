Order Management System
1. Project Overview
Order Management System backend developed using ASP.NET Core and Entity Framework Core. The API allows customers to register and log in, browse products, manage their shopping cart, and place orders. Administrators can manage the product catalog and update order statuses.
Main Features
Customer registration and login
JWT-based authentication
Role-based authorization for administrators
Public product listing and product details
Product search and pagination
Customer-specific shopping cart
Order placement from the current cart
Customer order history
Admin order management
Stock validation and concurrency handling
Soft deletion of products
Centralized exception handling
Entity Framework Core migrations
Swagger documentation
2. Technology Stack
Technology	Purpose
.NET 10 / ASP.NET Core	Web API and application framework
Entity Framework Core	ORM and database access
Microsoft SQL Server	Relational database
JWT	Authentication and authorization
Swagger 	API documentation and testing
The project documentation supplied with the project describes the API as an ASP.NET Core application and the setup prerequisites specify the .NET 10 SDK.
3. Setup and Installation
Prerequisites
.NET 10 SDK
SQL Server
Entity Framework Core CLI
Install the EF Core CLI if it is not already available:
dotnet tool install --global dotnet-ef
Install Packages : 
The project uses the following NuGet packages:
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.IdentityModel.Tokens
dotnet add package Swashbuckle.AspNetCore
dotnet add package BCrypt.Net-Next
Database Configuration
Update the connection string in appsettings.json according to the local SQL Server setup.
"DbConnection":"Server=localhost;Database=OrderDb;Trusted_Connection=
                             True;TrustServerCertificate=True"
Create the Database (Use Manager Console)
Add-Migration 
Update-Database
Run the Application
dotnet ruu
https://localhost:7281/swagger
4. Default Test Accounts
Role	Email	Password
Admin	akhilbalu2477@gmail.com	Admin@123
Customer	baluakhil4444@gmail.com	Customer@123
		
On database initialization, the application creates one administrator account and one customer account for local testing.
The application also seeds approximately 10 sample products across several categories. This makes it possible to test product listing, search, pagination, cart operations, and order placement without manually entering initial data.
These seeded credentials are intended for local development/testing only and should be changed or removed before production deployment.
5. Swagger API Documentation
Swagger is configured for testing the API directly from the browser. The captured Swagger UI shows the API grouped into Auth, Cart, Orders, and Product sections, with protected operations displaying lock icons
Testing Protected APIs
1.	Call POST /api/Auth/login using one of the test accounts.
2.	Copy the JWT token returned by the login endpoint.
3.	Click the Authorize button in Swagger.
4.	Paste the token into the authorization field. Swagger adds the Bearer prefix.
5.	Execute the protected endpoints from Swagger

6. API Endpoints
Authentication
Method	Endpoint	Access	Purpose
POST	/api/Auth/register	Public	Register a new customer
POST	/api/Auth/login	Public	Login and receive a JWT token
Products
Method	Endpoint	Access	Purpose
GET	/api/Product	Public	List products with search and pagination
GET	/api/Product/{id}	Public	Get product details
POST	/api/Product	Admin	Create a product
PUT	/api/Product/{id}	Admin	Update a product
DELETE	/api/Product/{id}	Admin	Deactivate a product
Cart
Method	Endpoint	Access	Purpose
GET	/api/Cart	Authenticated customer	Get the current user's cart
POST	/api/Cart/items	Authenticated customer	Add an item to the cart
PUT	/api/Cart/items/{cartItemId}	Authenticated customer	Update cart item quantity
DELETE	/api/Cart/items/{cartItemId}	Authenticated customer	Remove an item from the cart
Orders
Method	Endpoint	Access	Purpose
POST	/api/Orders	Authenticated customer	Place an order from the current cart
GET	/api/Orders	Authenticated customer	Get the current user's order history
GET	/api/Orders/{id}	Customer/Admin	Get a specific order
GET	/api/Orders/admin/all	Admin	Get all orders, with optional status filtering
PUT	/api/Orders/{id}/status	Admin	Update order status

7. Centralized Exception Handling
The application uses a central ExceptionMiddleware instead of building error responses repeatedly inside controllers.
NotFoundException — used when a requested resource does not exist.
BadRequestException — used for invalid request or business conditions.
ConflictException — used when the requested operation conflicts with the current state.

Exception	HTTP Status
NotFoundException	404 Not Found
BadRequestException	400 Bad Request
ConflictException	409 Conflict
Unexpected exception	500 Internal Server Error

