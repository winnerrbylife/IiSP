using System;
using System.Text.Json.Serialization;

namespace IPR1_2.Models
{
    public class Rate
    {
        [JsonPropertyName("Cur_ID")]
        public int CurId { get; set; }

        [JsonPropertyName("Date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("Cur_Abbreviation")]
        public string CurAbbreviation { get; set; } = string.Empty;

        [JsonPropertyName("Cur_Scale")]
        public int CurScale { get; set; }

        [JsonPropertyName("Cur_Name")]
        public string CurName { get; set; } = string.Empty;

        [JsonPropertyName("Cur_OfficialRate")]
        public decimal CurOfficialRate { get; set; }
    }
}