using System.Text.Json.Serialization;

namespace IP2C_consumer.Models
{
    public class IpAddress
    {
        public int Id { get; set; }
        public int CountryId { get; set; }

        [JsonIgnore]
        public Country Country { get; set; }
        public string IP { get; set; }            
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }        
}
