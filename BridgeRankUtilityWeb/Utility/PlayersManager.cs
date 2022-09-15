namespace BridgeRankUtilityWeb.Utility
{
    public static class PlayersManager
    {
        private static List<Player> PLAYERS = new List<Player>();

        public static void ImportPlayers(string localUrl)
        {
            StreamReader sr = new StreamReader(localUrl);

            while (!sr.EndOfStream)
            {
                string[] row = sr.ReadLine().Split(';');

                PLAYERS.Add(new Player
                {
                    Name = row[0],
                    FIGBCode = row[1],
                    Category = int.Parse(row[2])
                });
            }
        }

        public static int GetCategoryFromCode(string FIGBCode)
        {
            IEnumerable<Player> player = PLAYERS.Where(s => s.FIGBCode == FIGBCode);

            if (player.Any())
            {
                return player.FirstOrDefault().Category;
            }

            //Se non trovo il giocatore nel file assegno 9 come categoria
            return 9;
        }

        public static void AddPlayer(Player p)
        {
            PLAYERS.Add(p);
        }
    }
}
