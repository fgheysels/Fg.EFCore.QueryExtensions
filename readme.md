# Abstract

This project contains some extensions to EF Core (3)

It seems not to be possible to create a LINQ expression in Entity Framework Core which translates into a SQL `LIKE` exprression which matches against multiple possible values.  That's why I've decided to create this project and make it available via NuGet.

# Installation

Get it via NuGet:

```
PM> Install-Package Fg.EfCore.QueryExtensions
```

# Features

## Generating a LIKE expression against multiple possible values

It seems not to be possible to create a LINQ expression in Entity Framework Core 3 which translates into a SQL clause that looks like this:

```sql
WHERE ([name] LIKE 'foo%' OR [name] LIKE 'bar%')
```

I thought the above clause could be generated with an expression that looks like the one below, but apparently, that is not true

```csharp
var names = new string[] {"foo%", "bar%"};

var results = dbContext.Persons.Where( p => names.Any(n => p.Name.Startswith(n)));
```

This project contains a helper methods which allows you to write this LINQ query:

```csharp
var names = new string[] {"foo%", "bar%"};

var results = await dbContext.Persons.Where(DbFilterExpression.LikeOneOf(nameof(Person.Name), names)).ToListAsync();
```

## Pagination / Create paged query results

An extension method is available which allows to easily create paged query results:

```csharp
DataPage<Person> result = dbContext.Persons.ToPagedResultAsync(pageNumber: 1, pageSize: 20);
```

This is an extension method on `IQueryable` so you're able to use it filtered expessions as well:

```csharp
DataPage<Person> result = dbContext.Persons.Where(p => p.Name.Startswith("Fre")).ToPagedResultAsync(pageNumber: 1, pageSize: 20);
```

> The `ToPagedResult` extension method will execute the query so it is important to have this method as the last part of your query expression.