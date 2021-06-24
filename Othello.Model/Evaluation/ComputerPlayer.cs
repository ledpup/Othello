﻿using Othello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Othello.Model.Evaluation
{
	public class ComputerPlayer
	{
	    public readonly Dictionary<string, float>[] Weights;
	    public readonly short NumberOfGamePhases;
	    private const short TurnsPerPhase = 6;

        public ComputerPlayer(IPlayerUiSettings playerUiSettings)
        {
            PlayerUiSettings = playerUiSettings;

            NumberOfGamePhases = 60 / TurnsPerPhase;

            MaxSearchDepth = new int[NumberOfGamePhases];
            MaxSearchTime = playerUiSettings.MaxSearchTime;
            Weights = new Dictionary<string, float>[NumberOfGamePhases];
            for (var i = 0; i < NumberOfGamePhases; i++)
            {
                MaxSearchDepth[i] = PlayerUiSettings.MaxSearchDepth;
                Weights[i] = new Dictionary<string, float>();
                Strategies.ForEach(x => Weights[i].Add(x, 1));
            }

            SetDefaults();

            TranspositionTable = new Dictionary<GameState, float>();
        }

        public SearchAlgorithms.SearchMethod Search
        { 
            get
            {
                switch (PlayerUiSettings.SearchMethod)
                {
                    case 0:
                        return SearchAlgorithms.NegaMax;
                    case 1:
                        return SearchAlgorithms.AlphaBetaNegaMax;
                    case 2:
                        return SearchAlgorithms.NegaScout;
                }
                return SearchAlgorithms.NegaMax;
            }
            set
            {
                if (value == SearchAlgorithms.NegaMax)
                        PlayerUiSettings.SearchMethod = 0;
                if (value == SearchAlgorithms.AlphaBetaNegaMax)
                    PlayerUiSettings.SearchMethod = 1;
                if (value == SearchAlgorithms.NegaScout)
                    PlayerUiSettings.SearchMethod = 2;
            }
        }
        public int BaseSearchDepth
        {
            get { return PlayerUiSettings.MaxSearchDepth; }
            set
            {
                PlayerUiSettings.MaxSearchDepth = value > 1 ? value : 2;
                for (var i = 0; i < NumberOfGamePhases; i++)
                {
                    MaxSearchDepth[i] = PlayerUiSettings.MaxSearchDepth;
                }
                SetDefaults();
            }   
        }
		
		public IPlayerUiSettings PlayerUiSettings;

        public static Dictionary<GameState, float> TranspositionTable;

	    public readonly List<string> Strategies = new List<string>
	                                    {"Pieces", "Mobility", "PotentialMobility", "Pattern", };

	    private readonly int[] MaxSearchDepth;
        public int MaxSearchTime;

        private void SetDefaults()
        {
            MaxSearchDepth[8] = 10;
            MaxSearchDepth[9] = 10;

            Weights[0]["Pieces"] = .1f;
            Weights[1]["Pieces"] = .1f;
            Weights[2]["Pieces"] = .1f;
            Weights[3]["Pieces"] = .1f;
            Weights[4]["Pieces"] = .1f;
            Weights[5]["Pieces"] = .1f;

            Weights[6]["Pieces"] = .5f;
            Weights[7]["Pieces"] = .5f;

            Weights[8]["Mobility"] = .1f;
            Weights[8]["PotentialMobility"] = .1f;

            Weights[9]["Mobility"] = .1f;
            Weights[9]["PotentialMobility"] = .1f;
        }

	    int Phase(short turn)
	    {
			if (turn >= 60)
				return NumberOfGamePhases - 1;
            return turn / TurnsPerPhase;
	    }

        public Dictionary<string,float> GetWeights(short turn)
        {
            return Weights[Phase(turn)];
        }

        public int GetSearchDepth(short turn)
        {
            return MaxSearchDepth[Phase(turn)];
        }

        public void Draw()
        {
            for (var i = 0; i < NumberOfGamePhases; i++)
            {
                Console.WriteLine("Phase {0}", i);
                foreach (var weight in Weights[i])
                    Console.WriteLine("{0}  {1}", weight.Key, weight.Value);
            }
        }

	    public int NumberOfWeights
	    {
            get { return NumberOfGamePhases*Strategies.Count; }
	    }
	}
}
