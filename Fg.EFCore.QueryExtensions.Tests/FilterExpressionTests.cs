using System;
using System.Linq;
using System.Threading.Tasks;
using Fg.EFCore.QueryExtensions.Tests.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Fg.EFCore.QueryExtensions.Tests
{
    public class FilterExpressionTests
    {

        private readonly ITestOutputHelper _unitTestOutput;
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterExpressionTests"/> class.
        /// </summary>
        public FilterExpressionTests(ITestOutputHelper unitTestOutput)
        {

            _unitTestOutput = unitTestOutput;
        }

        [Fact]
        public async Task CanQueryWithMultiLike()
        {
            using (var connection = new SqliteConnection("data source=:memory:"))
            {
                connection.Open();

                var options = new DbContextOptionsBuilder<NavyDbContext>()
                              .UseLoggerFactory(LoggerFactory.Create(builder=>builder.AddXunit(_unitTestOutput)))
                    .UseSqlite(connection).Options;

                var dbContext = new NavyDbContext(options);

                dbContext.Database.EnsureCreated();

                dbContext.Vessels.Add(new Vessel { Id = 1, Name = "Enterprise" });
                dbContext.Vessels.Add(new Vessel { Id = 2, Name = "Nautilus" });
                dbContext.Vessels.Add(new Vessel { Id = 3, Name = "Boaty McBoatface" });
                dbContext.Vessels.Add(new Vessel { Id = 5, Name = "Nautica" });

                dbContext.SaveChanges();

                var results = await dbContext.Vessels
                                             .Where(FilterExpression.LikeOneOf<Vessel>(nameof(Vessel.Name), new[] { "%auti%", "%boat%" }))
                                             .ToListAsync();

                Assert.Contains(results, v => v.Name.Equals("Nautilus"));
                Assert.Contains(results, v => v.Name.Equals("Nautica"));
                Assert.Contains(results, v => v.Name.Equals("Boaty McBoatface"));
            }
        }

    }
}
