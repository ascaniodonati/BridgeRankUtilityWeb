namespace BridgeRankUtilityWeb
{
    public enum TournamentType
    {
        Pair, Team
    }

    public class Tournament
    {
        public List<Player> Players { get; set; }
        public string Code { get; set; }
        public TournamentType Type { get; set; }
        public DateTime Date { get; set; }
        public string URL { get; set; }

        public void AssignPoints()
        {
            Players = Players.OrderBy(s => s.Position).ToList();

            int[] positions = Players.Select(s => s.Position).ToArray();
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
                Players[i].Points = points[i];
            }

            RicalcolaPunti();
        }

        public void RicalcolaPunti()
        {
            this.Players.ForEach(s =>
            {
                switch (s.Position)
                {
                    case 1:
                        s.Points += 3;
                        break;
                    case 2:
                        s.Points += 2;
                        break;
                    case 3:
                        s.Points += 1;
                        break;
                }
            });
        }
    }

    public class Player
    {
        public string Name { get; set; }
        public string FIGBCode { get; set; }
        public int Category { get; set; }   //va da 1 a 4
        public int Position { get; set; }
        public int Points { get; set; }
    }
}
