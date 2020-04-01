using Microsoft.EntityFrameworkCore;
using Othello.Model;
using Othello.Online.DataModel;
using Othello.Online.DataTransferObject;
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

        public async Task<GameDto> GetGameDtoAsync(Guid gameId)
        {
            var game = await _othelloContext.Games.AsNoTracking().SingleOrDefaultAsync(x => x.Id == gameId);
            var dto = new GameDto
            {
                Id = game.Id,
                Name = game.Name,
                GameState = game.GameState,
                UserBlackId = game.UserBlackId,
                UserWhiteId = game.UserWhiteId,
                TurnLengthLimit = game.TurnLengthLimit,
            };

            return dto;
        }

        public async Task<Guid> CreateGameAsync(GameDto gameDto)
        {
            try
            {
                GameManager.Load(gameDto.GameState);
            }
            catch
            {
                new Exception($"Game {gameDto.GameState} is an invalid Othello game state.");
            }

            var game = new Game
            {
                Name = gameDto.Name,
                GameState = gameDto.GameState,
                UserBlackId = gameDto.UserBlackId,
                UserWhiteId = gameDto.UserWhiteId,
                TurnLengthLimit = gameDto.TurnLengthLimit,
            };
            _othelloContext.Games.Add(game);

            await _othelloContext.SaveChangesAsync();

            _othelloContext.Entry(game).State = EntityState.Detached;

            return game.Id;
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

        public async Task UpdateGameAsync(Guid gameId, string play)
        {
            var game = await _othelloContext.Games.SingleOrDefaultAsync(x => x.Id == gameId);

            var gameManager = GameManager.Load(game.GameState);

            if (!gameManager.CanPlay((short)play.ToIndex()))
            {
                gameManager.PlacePiece(null);
                gameManager.NextTurn();
            }
            gameManager.PlacePiece(play.ToIndex());
            gameManager.NextTurn();

            game.GameState = gameManager.SerialiseState();
            game.UpdatedUtc = DateTime.UtcNow;
            _othelloContext.Update(game);

            await _othelloContext.SaveChangesAsync();
        }
    }
}
