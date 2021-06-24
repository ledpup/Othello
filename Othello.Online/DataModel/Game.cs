using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Othello.Online.DataModel
{
    public class Game
    {
        public Game()
        {
            var now = DateTime.UtcNow;
            CreatedUtc = now;
            UpdatedUtc = now;
            TurnLengthLimit = TimeSpan.FromMinutes(5);
        }

        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string GameState { get; set; }
        public Guid UserBlackId { get; set; }
        public virtual User UserBlack { get; set; }
        public Guid UserWhiteId { get; set; }
        public virtual User UserWhite { get; set; }
        public TimeSpan TurnLengthLimit { get; set; }
        public bool Finished { get; set; }


        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
    }
}
