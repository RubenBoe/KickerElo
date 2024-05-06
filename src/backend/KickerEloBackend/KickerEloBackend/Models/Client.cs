using System.Text.Json.Serialization;

namespace KickerEloBackend.Models
{
    internal class Client
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string ClientName { get; set; }
    }
}
