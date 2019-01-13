# News Crawler

## Database Setup

The database layer is built upon [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/).

### Creating a Database Server

This project uses SQL Server. You can use set this up locally or use a Docker container:
```
# docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=Password1*' -p 1433:1433 -v mssql:/var/opt/mssql -d --name sql-server mcr.microsoft.com/mssql/server:2017-latest
```

### Deployment

The following command will deploy the database to localhost:
```
dotnet ef database update --project NewsCrawler.Persistence
```
The connection string can be changed in NewsCrawler.Persistence/appsettings.json.

### Creating a Migration

After making changes run the following to create a migration. Run the update command to apply changes.
```
dotnet ef migrations add < migration of migration > --project NewsCrawler.Persistence
```
