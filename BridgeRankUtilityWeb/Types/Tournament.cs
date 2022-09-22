using BridgeRankUtilityWeb.Enums;

namespace BridgeRankUtilityWeb.Types
{
    public class Tournament
    {
        public List<Player> Players { get; set; }
        public string Code { get; set; }
        public TournamentType Type { get; set; }
        public DateTime Date { get; set; }
        public string URL { get; set; }

        public void GetPlayers()
        {

        }

        public void AssignPoints()
        { 
            Players = Players.OrderBy(s => s.Posizione).ToList();

            int[] positions = Players.Select(s => s.Posizione).ToArray();
            List<int> newPosition = new List<int>();
            List<int> points = new List<int>();

            int currentNumber = 0;

            foreach (int number in positions)
            {
                if (currentNumber != number)
                {
                    newPosition.Add(newPosition.Count());
                    currentNumber = number;
                }
                else
                {
                    newPosition.Add(newPosition.Last());
                }

                points.Add(positions.Count() - newPosition.Last());
            }

            for (int i = 0; i < points.Count(); i++)
            {
                Players[i].Punti = points[i];
            }

            RicalcolaPunti();
        }

        public void RicalcolaPunti()
        {
            Players.ForEach(s =>
            {
                switch (s.Posizione)
                {
                    case 1:
                        s.Punti += 3;
                        break;
                    case 2:
                        s.Punti += 2;
                        break;
                    case 3:
                        s.Punti += 1;
                        break;
                }
            });
        }


    }
}
