using BridgeRankUtilityWeb.Enums;
using HtmlAgilityPack;
using System.Net;

namespace BridgeRankUtilityWeb.Utility
{
    public static class HtmlUtility
    {
        public static TournamentType? FindTournamentTypeFromURL(string url)
        {
            HtmlDocument doc = new HtmlDocument();
            string title = "";

            if (!url.Contains("http")) { return null; }

            using (WebClient wc = new WebClient())
            {
                string html = wc.DownloadString(url);
                doc.LoadHtml(html);
            }

#pragma warning disable

            HtmlNode titleNode =
                doc.DocumentNode.SelectNodes("//div").Where(s =>
                    s.Attributes.Contains("class") &&
                    s.Attributes["class"].Value.Contains("TitoloTorGareClass"))
                    .FirstOrDefault();

            title = titleNode.InnerText.ToLower();

            if (title.Contains("squadre"))
            {
                return TournamentType.Team;
            }
            else if (title.Contains("coppie") || title.Contains("simultaneo"))
            {
                return TournamentType.Pair;
            }

            return null;
        }
    }
}
