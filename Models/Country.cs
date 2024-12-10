using System.Text.Json.Serialization;

namespace IP2C_consumer.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TwoLetterCode { get; set; }
        public string ThreeLetterCode { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public ICollection<IpAddress> IPAddresses { get; set; }
    }
}
