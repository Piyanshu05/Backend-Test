================================================================================
               SUPPORT TICKET MANAGEMENT SYSTEM - README
================================================================================

A REST API built with ASP.NET Core 8 for managing support tickets. 
It supports user authentication (JWT), role-based access control, 
ticket CRUD operations, ticket assignment, status tracking, and commenting.




================================================================================
 TECH STACK
================================================================================

  - Framework       : ASP.NET Core 8 Web API
  - Language         : C#
  - Database         : SQL Server (EF core-Code First)
  - Authentication   : JWT (JSON Web Tokens)
  - Password Hashing : BCrypt
  - API Docs         : Swagger (Swashbuckle)

NuGet Packages Used:

  Package                                          
  ------------------------------------------------
  Microsoft.EntityFrameworkCore.SqlServer           
  Microsoft.EntityFrameworkCore.Tools               
  Microsoft.AspNetCore.Authentication.JwtBearer     
  BCrypt.Net-Next                                   
Swashbuckle.AspNetCore                            



================================================================================
  DATABASE SETUP
================================================================================

This project uses EF Core Code-First approach. The database and tables are 
AUTOMATICALLY CREATED when you run the project for the first time.

Tables Created:
  - roles              : Stores user roles
  - users              : Stores user accounts
  - tickets            : Stores support tickets
  - ticket_comments    : Stores comments on tickets
  - ticket_status_logs : Logs ticket status changes

Default Roles (Seeded Automatically):
  ID    Role Name
  ---   ---------
   1    MANAGER
   2    SUPPORT
   3    USER

Password: "Password@123" - for all users

