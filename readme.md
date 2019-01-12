# News Crawler

## Database Setup

The database layer is built upon [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/).

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
