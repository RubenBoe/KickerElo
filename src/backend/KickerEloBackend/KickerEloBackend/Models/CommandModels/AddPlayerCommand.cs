

namespace KickerEloBackend.Models.CommandModels
{
    internal class AddPlayerCommand
    {
        public string ClientToken { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
    }
}
