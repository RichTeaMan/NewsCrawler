# News Crawler

## Database Setup

The database layer is built upon [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) with [Npgsql](http://www.npgsql.org/efcore/)
for PostgreSQL integration.

### Creating a Database Server

This project uses [PostgreSQL](https://www.postgresql.org/). You can use set this up locally or use a Docker container:
```bash
# docker run --name postgres -p 5432:5432 -v /postgres:/var/lib/postgresql/data -e POSTGRES_PASSWORD=Password1* -d postgres
```

### Deployment

The following command will deploy the database to localhost:
```
dotnet ef database update --project NewsCrawler.Persistence --startup-project NewsCrawler.WebUI
```
The connection string can be changed in NewsCrawler.Persistence/appsettings.json.

### Creating a Migration

After making changes run the following to create a migration. Run the update command to apply changes.
```
dotnet ef migrations add < name of migration > --project NewsCrawler.Persistence --startup-project NewsCrawler.WebUI
```
