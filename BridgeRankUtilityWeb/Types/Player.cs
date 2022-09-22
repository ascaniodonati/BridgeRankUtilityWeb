using Newtonsoft.Json;

namespace BridgeRankUtilityWeb.Types
{
    public class Player
    {
        public string Nome { get; set; }
        public string FIGBCode { get; set; }
        public int Categoria { get; set; }   //va da 1 a 4
        public int Posizione { get; set; }
        public int Punti { get; set; }

        [JsonIgnore]
        public new string ToString => $"{Nome},{FIGBCode},{Categoria}";

        [JsonIgnore]
        public bool IsEditing { get; set; } = false;
    }
}
