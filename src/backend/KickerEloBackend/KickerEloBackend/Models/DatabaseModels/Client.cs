using System;

namespace KickerEloBackend.Models.DatabaseModels
{
    internal class Client : AbstractEntityClass
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public DateTime CreationDate { get; set; }
        public string ClientToken { get; set; }
    }
}
