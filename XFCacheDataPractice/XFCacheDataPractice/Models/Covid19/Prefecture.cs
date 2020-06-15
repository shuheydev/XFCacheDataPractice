using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace XFCacheDataPractice.Models.Covid19
{
    public class Prefecture
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name_ja")]
        public string Name_Ja { get; set; }
        [JsonPropertyName("name_en")]
        public string Name_En { get; set; }
        [JsonPropertyName("lat")]
        public float Lat { get; set; }
        [JsonPropertyName("lng")]
        public float Lng { get; set; }
        [JsonPropertyName("population")]
        public int Population { get; set; }
        [JsonPropertyName("last_updated")]
        public Last_Updated Last_Updated { get; set; }
        [JsonPropertyName("cases")]
        public int Cases { get; set; }
        [JsonPropertyName("deaths")]
        public int Deaths { get; set; }
        [JsonPropertyName("pcr")]
        public int Pcr { get; set; }
    }

    public class Last_Updated
    {
        [JsonPropertyName("cases_date")]
        public int Cases_Date { get; set; }
        [JsonPropertyName("deaths_date")]
        public int Deaths_Date { get; set; }
        [JsonPropertyName("pcr_date")]
        public int Pcr_Date { get; set; }
    }
}
