using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello.Online.DataModel;
using Othello.Online.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Othello.Online.IntegrationTest
{
    [TestClass]
    public class GameCreateAndUpdateTests
    {
        private OthelloRepository _othelloRepository;
        private OthelloContext _othelloContext;
        private string _tamenori = "C4,E3,F6,E6,F5,C5,F4,G6,F7,D3";

        public GameCreateAndUpdateTests()
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
        public async Task CreateGame_ValidGame_GameSavedAsync()
        {
            await SetupUsersAsync();

            var users = await _othelloRepository.GetUsersAsync();

            var game = new GameDto
            {
                Name = "New game",
                GameState = _tamenori,
                UserBlackId = users[0].Id,
                UserWhiteId = users[1].Id,
            };

            var gameId = await _othelloRepository.CreateGameAsync(game);
            var gameDto = await _othelloRepository.GetGameDtoAsync(gameId);

            Assert.AreEqual(game.GameState, gameDto.GameState);
        }

        [TestMethod]
        public async Task UpdateGame_ValidPlay_GameStateUpdatedAsync()
        {
            await SetupUsersAsync();

            var users = await _othelloRepository.GetUsersAsync();

            var gameDto = new GameDto
            {
                Name = "New game",
                GameState = "C4,E3,F6,E6,F5,C5,F4,G6,F7",
                UserBlackId = users[0].Id,
                UserWhiteId = users[1].Id,
            };

            var gameId = await _othelloRepository.CreateGameAsync(gameDto);
            await _othelloRepository.UpdateGameAsync(gameId, "D3");

            var savedDto = await _othelloRepository.GetGameDtoAsync(gameId);

            Assert.AreEqual(_tamenori, savedDto.GameState);
        }

        [TestMethod]
        public async Task UpdateGame_InvalidPlay_GameStateUnchangedAsync()
        {
            await SetupUsersAsync();

            var users = await _othelloRepository.GetUsersAsync();

            var gameDto = new GameDto
            {
                Name = "New game",
                GameState = "C4,E3,F6,E6,F5,C5,F4,G6,F7",
                UserBlackId = users[0].Id,
                UserWhiteId = users[1].Id,
            };

            var gameId = await _othelloRepository.CreateGameAsync(gameDto);

            await Assert.ThrowsExceptionAsync<Exception>(async () => await _othelloRepository.UpdateGameAsync(gameId, "D1"));
        }
    }
}
