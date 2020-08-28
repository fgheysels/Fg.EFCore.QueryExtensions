using Fg.EFCore.QueryExtensions.Tests.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Fg.EFCore.QueryExtensions.Tests
{
    public class DataPagingTests : IDisposable
    {
        private readonly ITestOutputHelper _unitTestOutput;

        private readonly NavyDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterExpressionTests"/> class.
        /// </summary>
        public DataPagingTests(ITestOutputHelper unitTestOutput)
        {
            _unitTestOutput = unitTestOutput;

            var connection = new SqliteConnection("data source=:memory:");

            connection.Open();

            var options = new DbContextOptionsBuilder<NavyDbContext>()
                          .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddXunit(_unitTestOutput)))
                          .UseSqlite(connection).Options;

            _dbContext = new NavyDbContext(options);

            _dbContext.Database.EnsureCreated();

            _dbContext.Vessels.Add(new Vessel { Id = 1, Name = "Enterprise" });
            _dbContext.Vessels.Add(new Vessel { Id = 2, Name = "Nautilus" });
            _dbContext.Vessels.Add(new Vessel { Id = 3, Name = "Boaty McBoatface" });
            _dbContext.Vessels.Add(new Vessel { Id = 5, Name = "Nautica" });

            _dbContext.SaveChanges();
        }


        [Fact]
        public async Task CanRetrievePagedResults()
        {
            var result = await _dbContext.Vessels.OrderBy(v => v.Id).ToPagedResultAsync(1, 2);

            Assert.Equal(1, result.PageNumber);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(4, result.TotalNumberOfItems);

            Assert.Contains(result.Items, v => v.Id == 1);
            Assert.Contains(result.Items, v => v.Id == 2);
        }

        [Fact]
        public async Task PagedResultsWithLastIncompletePage()
        {
            var firstPage = await _dbContext.Vessels.ToPagedResultAsync(1, 3);

            Assert.Equal(1, firstPage.PageNumber);
            Assert.Equal(3, firstPage.Items.Count());
            Assert.Equal(4, firstPage.TotalNumberOfItems);

            var secondPage = await _dbContext.Vessels.ToPagedResultAsync(2, 3);

            Assert.Equal(2, secondPage.PageNumber);
            Assert.Equal(1, secondPage.Items.Count());
            Assert.Equal(4, secondPage.TotalNumberOfItems);
        }

        [Fact]
        public async Task CanRetrievePagedResultsOnFilteredQuery()
        {
            var result = await _dbContext.Vessels
                                         .Where(DbFilterExpression.LikeOneOf<Vessel>(nameof(Vessel.Name), new[] { "Nau%", "%stat%" }))
                                         .ToPagedResultAsync(1, 2);

            Assert.Equal(1, result.PageNumber);
            Assert.Equal(2, result.TotalNumberOfItems);
        }

        [Fact]
        public async Task PagedResultsOnEmptyResultsetDoesNotFail()
        {
            var result = await _dbContext.Vessels
                                         .Where(v => false)
                                         .ToPagedResultAsync(1, 10);

            Assert.NotNull(result);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(0, result.TotalNumberOfItems);
            Assert.Empty(result.Items);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
