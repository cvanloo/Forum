# Forum

## Dependencies:

* .NET 5.0
* MariaDB 10.5

## Directories

Mail is stored to `C:\tmp\mail`.

Threads are uploaded to `C:\tmp\upload`.

## Database configuration

The database user and password are configured in `Forum/appsettings.json`.

The user `root` and password `toor` are preconfigured.

## Setup database:

	dotnet ef database update

Run:

	dotnet run