using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello.Online.DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Othello.Online.IntegrationTest
{
    [TestClass]
    public class UnitTest1
    {
        private OthelloRepository _othelloRepository;
        private OthelloContext _othelloContext;

        public UnitTest1()
        {
            var connectionString = "Server=.;Database=Othello;Integrated Security=true;";

            var optionsBuilder = new DbContextOptionsBuilder<OthelloContext>();
            optionsBuilder
                .UseSqlServer(connectionString, providerOptions => providerOptions.CommandTimeout(30))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            _othelloContext = new OthelloContext(optionsBuilder.Options);
            _othelloContext.Database.EnsureCreated();

            _othelloRepository = new OthelloRepository(_othelloContext);
        }

        private async Task SetupUsersAsync()
        {
            var numberUsers = await _othelloContext.Users.CountAsync();

            var users = new List<User>
            {
                new User { Name = "Patrick" },
                new User { Name = "Luis" },
            };

            for (var i = numberUsers; i < 2; i++)
            {
                _othelloContext.Users.Add(users[i]);
            }

            await _othelloContext.SaveChangesAsync();
        }

        [TestMethod]
        public async Task CreateGame_ValidGame_()
        {
            await SetupUsersAsync();

            var users = await _othelloRepository.GetUsersAsync();

            var game = new Game
            {
                Name = "New game",
                State = "c4,e3,f6,e6,f5,c5,f4,g6,f7,d3",
                UserBlackId = users[0].Id,
                UserWhiteId = users[1].Id,
            };

            var savedGame = await _othelloRepository.CreateGame(game);

            await _othelloContext.Entry(savedGame).ReloadAsync();

            Assert.AreEqual(game.State, savedGame.State);
        }
    }
}
