using System.Text.Json.Serialization;

namespace Maersk.FbM.OCT.Controller;

/// <summary>
/// Domain object for informing the caller of a weather alert.
/// </summary>
public class WeatherAlert
{
    public class PropertyDTO
    {
        [JsonInclude]
        public string areaDesc { set; get; }
        [JsonInclude]
        public string status { set; get; }
        [JsonInclude]
        public string severity { set; get; }
        [JsonInclude]
        public string certainty { set; get; }
        [JsonInclude]
        public string urgency { set; get; }
        [JsonInclude]
        public string senderName { set; get; }
        [JsonInclude]
        public string headline { set; get; }
        [JsonInclude]
        public string description { set; get; }
    }
    public class FeatureDTO
    {
        [JsonInclude]
        public string id { set; get; }
        [JsonInclude]
        public PropertyDTO properties { set; get; }
    }
    
    [JsonInclude]
    public List<FeatureDTO> features { set; get; }
}