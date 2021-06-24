using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Othello.Online.DataModel;
using Othello.Online.DataTransferObject;

namespace Othello.Online.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OthelloController : ControllerBase
    {
        private readonly ILogger<OthelloController> _logger;
        readonly OthelloContext _othelloContext;
        OthelloRepository _othelloRepository;

        public OthelloController(ILogger<OthelloController> logger)
        {
            _logger = logger;

            var connectionString = "Server=.;Database=Othello;Integrated Security=true;";

            var optionsBuilder = new DbContextOptionsBuilder<OthelloContext>();
            optionsBuilder
                .UseSqlServer(connectionString, providerOptions => providerOptions.CommandTimeout(30))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            _othelloContext = new OthelloContext(optionsBuilder.Options);
            _othelloContext.Database.EnsureCreated();

            _othelloRepository = new OthelloRepository(_othelloContext);
        }

        [HttpPost]
        public async Task CreateGame(GameDto game)
        {
            await _othelloRepository.CreateGameAsync(game);
        }

        [HttpPatch]
        public async Task UpdateGame(Guid gameId, string play)
        {
            await _othelloRepository.UpdateGameAsync(gameId, play);
        }

        [HttpGet]
        public async Task<GameDto> GetGameAsync(Guid gameId)
        {
            var dto = await _othelloRepository.GetGameDtoAsync(gameId);

            return dto;
        }
    }
}
