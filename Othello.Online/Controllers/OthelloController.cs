using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Othello.Online.DataModel;

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
        public async Task CreateGame(Game game)
        {
            await _othelloRepository.CreateGame(game);
        }

        [HttpGet]
        public async Task<Game> GetGameAsync(Guid gameId)
        {
            return await _othelloRepository.GetGameAsync(gameId);
        }
    }
}
