using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Othello.Model
{
	public struct GameBoard
	{
        public GameBoard(ulong playerPieces, ulong opponentPieces)
            : this()
        {
            PlayerPieces = playerPieces;
            OpponentPieces = opponentPieces;
        }

        public ulong PlayerPieces;
        public ulong OpponentPieces;
	}
}
