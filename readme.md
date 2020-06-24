# Abstract

This project contains some extensions to EF Core (3)

It seems not to be possible to create a LINQ expression in Entity Framework Core which translates into a SQL `LIKE` exprression which matches against multiple possible values.  That's why I've decided to create this project and make it available via NuGet.

# Generating a LIKE expression against multiple possible values

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