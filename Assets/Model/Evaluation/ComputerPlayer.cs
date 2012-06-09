using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.Model.Evaluation
{
	public class ComputerPlayer
	{
	    public readonly Dictionary<string, float>[] Weights;
	    public readonly short NumberOfGamePhases;
	    private const short TurnsPerPhase = 6;

	    public readonly List<string> Strategies = new List<string>
	                                    {"Pieces", "Mobility", "PotentialMobility", "Parity", "Pattern", };

	    public bool UseOpeningBook;

	    private readonly int[] SearchDepth;

        public ComputerPlayer(bool useOpeningBook)
        {
            UseOpeningBook = useOpeningBook;

            NumberOfGamePhases = 60 / TurnsPerPhase;

            SearchDepth = new int[NumberOfGamePhases];
            Weights = new Dictionary<string, float>[NumberOfGamePhases];
            for (var i = 0; i < NumberOfGamePhases; i++)
            {
                SearchDepth[i] = 5;
                Weights[i] = new Dictionary<string, float>();
                Strategies.ForEach(x => Weights[i].Add(x, 1));
            }

            SetDefaults();
        }

        private void SetDefaults()
        {
            SearchDepth[8] = 10;
            SearchDepth[9] = 10;

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
            return turn >= 60 ? NumberOfGamePhases - 1 : turn / TurnsPerPhase;
	    }

        public Dictionary<string,float> GetWeights(short turn)
        {
            return Weights[Phase(turn)];
        }

        public int GetSearchDepth(short turn)
        {
            return SearchDepth[Phase(turn)];
        }

        public void Draw()
        {
            for (var i = 0; i < NumberOfGamePhases; i++)
            {
                Console.WriteLine("Phase {0}", i);
                foreach (var weight in Weights[i])
                    Console.WriteLine("{0}     {1}", weight.Key, weight.Value);
            }
        }

	    public int NumberOfWeights
	    {
            get { return NumberOfGamePhases*Strategies.Count; }
	    }
	}
}
