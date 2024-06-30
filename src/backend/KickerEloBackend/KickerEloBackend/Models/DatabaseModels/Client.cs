using System;

namespace KickerEloBackend.Models.DatabaseModels
{
    internal class Client
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public string ClientToken { get; set; }
    }
}
