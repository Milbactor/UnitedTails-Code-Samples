using System.Collections.Generic;

namespace WhiteKNight
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly Dictionary<string, CharacterInfo> _characters = new()
    {
        {
            "klaus", new CharacterInfo(
                "klaus", "クラウス", "ATK: 98 / DEF: 80 / SPD: ★★★★☆")
        },
        {
            "sirius", new CharacterInfo(
                "sirius", "シリウス", "ATK: 70 / DEF: 90 / SPD: ★★★☆☆")
        },
        {
            "axel", new CharacterInfo(
                "axel", "アクセル", "ATK: 88 / DEF: 60 / SPD: ★★★★★")
        }
    };

        public CharacterInfo GetCharacterInfo(string characterId)
        {
            if (_characters.TryGetValue(characterId, out var info))
                return info;

            return new CharacterInfo(characterId, "不明", "データなし");
        }
    }
}