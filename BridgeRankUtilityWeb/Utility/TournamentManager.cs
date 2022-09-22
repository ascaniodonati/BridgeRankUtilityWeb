using BridgeRankUtilityWeb.Enums;
using BridgeRankUtilityWeb.Types;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BridgeRankUtilityWeb.Utility
{
    public class TournamentUtility
    {
        public static List<Tournament> TOURNAMENTS = new List<Tournament>();

#pragma warning disable
        public async static Task<Tournament> GetTournamentFromDoc(string url)
        {
            HtmlDocument doc = new HtmlDocument();
            HtmlNode rootNode;

            if (!url.Contains("http")) { return new Tournament { Type = TournamentType.NoLink }; }

            using (HttpClient hc = new HttpClient())
            {
                string html = await hc.GetStringAsync(url);
                doc.LoadHtml(html);
            }

            //Recuperiamo il titolo
            HtmlNode titleNode =
                doc.DocumentNode.SelectNodes("//div").Where(s =>
                    s.Attributes.Contains("class") &&
                    s.Attributes["class"].Value.Contains("TitoloTorGareClass"))
                    .FirstOrDefault();

            if (titleNode == null) { return new Tournament { Type = TournamentType.NotFound }; }
            string title = titleNode.InnerText.ToLower();

            //Creiamo il node con dentro la tabella delle classifiche
            HtmlNode div = doc.DocumentNode.SelectNodes("//div").Where(s =>
                s.Attributes.Contains("class") &&
                s.Attributes["class"].Value.Contains("DivCentroSZ")).FirstOrDefault();

            rootNode = div.SelectSingleNode("./table");

            if (title.Contains("squadre"))
            {
                return CreateTeamTournamentFromDoc(rootNode, url);
            }
            else if (title.Contains("coppie") || title.Contains("simultaneo"))
            {
                return CreatePairTournamentFromDoc(rootNode, url);
            }
            else
            {
                return new Tournament { Type = TournamentType.NotFound };
            }
        }


        private static Tournament CreatePairTournamentFromDoc(HtmlNode rootNode, string url)
        {
            List<Player> playersInTournament = new List<Player>();
            List<Player> playersInTournament2 = new List<Player>();

            var rankPositions =
                rootNode.Descendants().Where(x =>
                    x.Attributes.Contains("class") &&
                    x.Attributes["class"].Value.Contains(" ALTbase25"));

            Console.WriteLine(rankPositions.Count());

            foreach (HtmlNode child in rankPositions)
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
                    Nome = NAME1.ParseName(),
                    FIGBCode = FIGBCODE1.Trim(),
                    Categoria = PlayersManager.GetCategoryFromCode(FIGBCODE1.Trim()),
                    Posizione = POSITION
                };

                Player p2 = new Player
                {
                    Nome = NAME2.ParseName(),
                    FIGBCode = FIGBCODE2.Trim(),
                    Categoria = PlayersManager.GetCategoryFromCode(FIGBCODE2.Trim()),
                    Posizione = POSITION
                };

                playersInTournament.Add(p1);
                playersInTournament.Add(p2);
            }

            Tournament t = new Tournament
            {
                Players = playersInTournament,
                Code = Utility.GetURLArgs(url, "cod"),
                Date = DateTime.Parse(Utility.GetURLArgs(url, "data")),
                Type = TournamentType.Pair,
                URL = url
            };

            t.AssignPoints();
            return t;
        }

        private static Tournament CreateTeamTournamentFromDoc(HtmlNode rootNode, string link)
        {
            List<Player> playersInTournament = new List<Player>();
            int POSITION = 0;

            foreach (HtmlNode child in rootNode.Descendants())
            {
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
                            Nome = NAME.ParseName(),
                            FIGBCode = FIGBCODE,
                            Categoria = PlayersManager.GetCategoryFromCode(FIGBCODE),
                            Posizione = POSITION
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
            return t;

        }

        public static IEnumerable<Player> Classifica()
        {
            IEnumerable<Player> classifica =
                 from t in TOURNAMENTS
                 from p in t.Players
                 where p.Categoria != 9
                 group p by new { p.FIGBCode, p.Nome, p.Categoria } into g
                 orderby g.Key.Categoria, g.Sum(s => s.Punti) descending
                 select new Player
                 {
                     FIGBCode = g.Key.FIGBCode,
                     Nome = g.Key.Nome,
                     Categoria = g.Key.Categoria,
                     Punti = g.Sum(s => s.Punti)
                 };

            return classifica;
        }
    }
}
