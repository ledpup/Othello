using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Othello.Online.DataTransferObject
{
    public class GameDto
    {
        public GameDto()
        {
            TurnLengthLimit = TimeSpan.FromMinutes(5);
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string GameState { get; set; }
        public Guid UserBlackId { get; set; }
        public Guid UserWhiteId { get; set; }
        public bool Finished { get; set; }
        public TimeSpan TurnLengthLimit { get; set; }
    }
}
