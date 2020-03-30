using Microsoft.EntityFrameworkCore;
using Othello.Online.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Othello.Online
{
    public class OthelloRepository
    {
        OthelloContext _othelloContext;
        public OthelloRepository(OthelloContext othelloContext)
        {
            _othelloContext = othelloContext;
        }

        public async Task<Game> GetGameAsync(Guid gameId)
        {
            var game = await _othelloContext.Games.SingleOrDefaultAsync(x => x.Id == gameId);
            return game;
        }

        public async Task<Game> CreateGame(Game game)
        {
            //var gameManager = Othello.Model.GameManager.Load(game.State);

            try
            {
                Model.GameManager.Load(game.State);
            }
            catch (Exception ex)
            {
                new Exception($"Invalid game. Exception: {ex}");
            }


            _othelloContext.Games.Add(game);
            await _othelloContext.SaveChangesAsync();

            return game;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var users = await _othelloContext.Users.ToListAsync();
            return users;
        }

        public async Task CreateUserAsync(User user)
        {
            _othelloContext.Users.Add(user);
            await _othelloContext.SaveChangesAsync();
        }
    }
}
