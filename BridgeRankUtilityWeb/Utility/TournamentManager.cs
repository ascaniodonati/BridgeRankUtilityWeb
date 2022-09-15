using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BridgeRankUtilityWeb.Utility
{
    public class TournamentManager
    {
        public static List<Tournament> TOURNAMENTS = new List<Tournament>();

        public static void DeleteTournamentFromLink(string link)
        {
            TOURNAMENTS.Remove(GetTournamentByLink(link));
        }

        private static HtmlNode HtmlGetTable(HtmlDocument doc)
        {
            HtmlNode div = doc.DocumentNode.SelectNodes("//div").Where(s =>
                    s.Attributes.Contains("class") &&
                    s.Attributes["class"].Value.Contains("DivCentroSZ")).FirstOrDefault();

            HtmlNode table = div.SelectSingleNode("./table");
            return table;
        }

        public static void CreateTournamentFromDoc(HtmlDocument doc, string link, TournamentType type)
        {
            switch (type)
            {
                case TournamentType.Pair:
                    CreatePairTournamentFromDoc(doc, link);
                    break;
                case TournamentType.Team:
                    CreateTeamTournamentFromDoc(doc, link);
                    break;
            }
        }

        public static bool TournamentAlreadyExist(string link)
        {
            return TOURNAMENTS.Where(s => s.URL == link).Any();
        }

        public static Tournament GetTournamentByLink(string link)
        {
            return TOURNAMENTS.Where(s => s.URL == link).FirstOrDefault();
        }

        private static void CreatePairTournamentFromDoc(HtmlDocument doc, string link)
        {
            HtmlNode table = HtmlGetTable(doc);
            List<Player> playersInTournament = new List<Player>();
            List<Player> playersInTournament2 = new List<Player>();

            foreach (HtmlNode child in table.Descendants())
            {

                if (child.Attributes.Contains("class") && child.Attributes["class"].Value.Contains(" ALTbase25"))
                {
                    string NAME1 = child.ChildNodes.Where(s =>
                    s.Attributes.Contains("class") &&
                    s.Attributes["class"].Value.Contains("POSbase0")).FirstOrDefault().InnerText;

                    string FIGBCODE1 = child.ChildNodes.Where(s =>
                        s.Attributes.Contains("class") &&
                        s.Attributes["class"].Value.Contains("COLceleste")).FirstOrDefault().InnerText;

                    string NAME2 = child.ChildNodes.Where(s =>
                        s.Attributes.Contains("class") &&
                        s.Attributes["class"].Value.Contains("POSbase0")).Last().InnerText;

                    string FIGBCODE2 = child.ChildNodes.Where(s =>
                        s.Attributes.Contains("class") &&
                        s.Attributes["class"].Value.Contains("COLceleste")).Last().InnerText;

                    int POSITION = child.ChildNodes.Where(s =>
                        s.Attributes.Contains("class") &&
                        s.Attributes["class"].Value.Contains("COLgray")).FirstOrDefault().InnerText.ParsePosition(TournamentType.Pair);

                    Player p1 = new Player
                    {
                        Name = NAME1.ParseName(),
                        FIGBCode = FIGBCODE1.Trim(),
                        Category = PlayersManager.GetCategoryFromCode(FIGBCODE1.Trim()),
                        Position = POSITION
                    };

                    Player p2 = new Player
                    {
                        Name = NAME2.ParseName(),
                        FIGBCode = FIGBCODE2.Trim(),
                        Category = PlayersManager.GetCategoryFromCode(FIGBCODE2.Trim()),
                        Position = POSITION
                    };

                    playersInTournament.Add(p1);
                    playersInTournament.Add(p2);
                }
            }

            Tournament t = new Tournament
            {
                Players = playersInTournament,
                Code = Utility.GetURLArgs(link, "cod"),
                Date = DateTime.Parse(Utility.GetURLArgs(link, "data")),
                Type = TournamentType.Pair,
                URL = link
            };

            t.AssignPoints();
            TOURNAMENTS.Add(t);

            Console.WriteLine();
        }

        private static void CreateTeamTournamentFromDoc(HtmlDocument doc, string link)
        {
            HtmlNode table = HtmlGetTable(doc);
            List<Player> playersInTournament = new List<Player>();
            int POSITION = 0;

            foreach (HtmlNode child in table.Descendants())
            {
                if (child.Attributes.Contains("class"))
                {
                    Debug.WriteLine(child.Attributes["class"].Value);
                }

                if (child.Attributes.Contains("class") && child.Attributes["class"].Value.Contains("TitoloSezOver"))
                {
                    POSITION = child.InnerText.ParsePosition(TournamentType.Team);
                }
                else if (child.Attributes.Contains("class") && child.Attributes["class"].Value.Contains("ALTbase25"))
                {
                    try
                    {
                        string NAME = child.ChildNodes.Where(s =>
                            s.Attributes.Contains("class") &&
                            s.Attributes["class"].Value.Contains("COLnavy")).FirstOrDefault().InnerText;

                        string FIGBCODE = child.ChildNodes.Where(s =>
                            s.Attributes.Contains("class") &&
                            s.Attributes["class"].Value.Contains("COLverdeMarcio")).FirstOrDefault().InnerText.Trim();

                        playersInTournament.Add(new Player
                        {
                            Name = NAME.ParseName(),
                            FIGBCode = FIGBCODE,
                            Category = PlayersManager.GetCategoryFromCode(FIGBCODE),
                            Position = POSITION
                        });
                    }
                    catch
                    {
                        //throw new Exception("Errore: " + child.InnerHtml);
                        Debug.WriteLine($"Errore: {child.InnerHtml}");
                    }
                }

            }

            Tournament t = new Tournament
            {
                Players = playersInTournament,
                Code = Utility.GetURLArgs(link, "cod"),
                Date = DateTime.Parse(Utility.GetURLArgs(link, "data")),
                Type = TournamentType.Team,
                URL = link
            };

            t.AssignPoints();
            TOURNAMENTS.Add(t);

        }

        public static IEnumerable<Player> Classifica()
        {
            IEnumerable<Player> classifica =
                 from t in TOURNAMENTS
                 from p in t.Players
                 where p.Category != 9
                 group p by new { p.FIGBCode, p.Name, p.Category } into g
                 orderby g.Key.Category, g.Sum(s => s.Points) descending
                 select new Player { FIGBCode = g.Key.FIGBCode, Name = g.Key.Name, Category = g.Key.Category, Points = g.Sum(s => s.Points) };

            return classifica;
        }

        public static void SaveLocalTournament(string path)
        {
            string JSON_TOURNAMENT = JsonConvert.SerializeObject(TOURNAMENTS);

            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine(JSON_TOURNAMENT);
            sw.Close();
        }
    }
}
