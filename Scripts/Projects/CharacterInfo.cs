using System.ComponentModel;

namespace WhiteKNight
{
    public class CharacterInfo
    {
        public string Id { get; }
        public string Name { get; }
        public string StatusSummary { get; }

        public CharacterInfo(string id, string name, string statusSummary)
        {
            Id = id;
            Name = name;
            StatusSummary = statusSummary;
        }
    }
}