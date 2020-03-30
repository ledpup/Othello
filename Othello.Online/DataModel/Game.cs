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
            CreatedUtc = DateTime.UtcNow;
            UpdatedUtc = DateTime.UtcNow;
        }

        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }

        public Guid UserBlackId { get; set; }
        public virtual User UserBlack { get; set; }
        public Guid UserWhiteId { get; set; }
        public virtual User UserWhite { get; set; }

        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
    }
}
