using System.Globalization;

namespace BridgeRankUtilityWeb.Utility
{
    public static class Utility
    {
        /// <summary>
        /// Ottengo il valore di un argomento in un link a partire dalla sua chiave
        /// </summary>
        /// <param name="link"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetURLArgs(string link, string key)
        {
            try
            {
                string[] args = link.Substring(link.IndexOf('?') + 1).Split('&');

                foreach (string arg in args)
                {
                    string[] splittedArg = arg.Split('=');

                    if (splittedArg[0] == key)
                    {
                        return splittedArg[1];
                    }
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Metodo di estensione utilizzato per il parsing del nome, visto che inizialmente l'HTML lo stupra
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="codeShowed">Nel caso alla fine compaia il codice dell'associazione [FO217], abilitare questo metodo</param>
        /// <returns></returns>
        public static string ParseName(this string _name)     //placucci federico[F0217] -> Placucci Federico
        {
            string name = _name;

            if (name.Contains("["))
            {
                name = name.Substring(0, name.IndexOf('['));
            }

            name = string.Join(" ", name.Split(new char[0], StringSplitOptions.RemoveEmptyEntries));

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);

        }

        public static int ParsePosition(this string _position, TournamentType type)
        {
            string position = _position;
            char symbol = new char();

            if (type == TournamentType.Pair) { symbol = '°'; }
            else if (type == TournamentType.Team) { symbol = 'ª'; }

            position = position.Substring(0, position.IndexOf(symbol));
            return int.Parse(position.Trim());
        }
    }
}
