# DS Assignment

This project is an implementation of a .NET Core 8 application using Visual Studio 2022 and SQL Server 2022.

## Prerequisites

Before you begin, ensure you have the following installed:
- Visual Studio 2022
- .NET Core 8
- SQL Server 2022

## Getting Started

1. Clone the repository to your local machine:
   ```bash
   git clone https://github.com/n4b1l007/DS-Assignment.git

2. Setup the Database (Described bellow) 

3. Open the solution file located in the `src` folder using Visual Studio 2022.

## Project Structure

- The solution is organized as follows:
- `src` folder contains the solution file and all the projects.
- `sql` folder contains the SQL script ds-db.sql for database setup.


## Database Setup
Open SQL Server Management Studio (SSMS) and connect to your SQL Server instance.
Execute the ds-db.sql script located in the sql folder to create the necessary database tables and stored procedures.
## Configuration
In the `DsApi` project, locate the appsettings.json file.
Update the connection string to match your SQL Server instance.
## Running the Application
Ensure your SQL Server instance is running and accessible.
Build and run both `DsWebApp` and `DsApi` project in Visual Studio.


The application should now be running and accessible.