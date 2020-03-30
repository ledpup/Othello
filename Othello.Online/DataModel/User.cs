using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Othello.Online.DataModel
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("UserBlackId")]
        public virtual ICollection<Game> GamesAsBlack { get; set; }

        [ForeignKey("UserWhiteId")]
        public virtual ICollection<Game> GamesAsWhite { get; set; }
    }
}
