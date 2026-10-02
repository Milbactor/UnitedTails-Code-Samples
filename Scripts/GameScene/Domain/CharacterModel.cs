using UnityEngine.AddressableAssets;

namespace WhiteKNight
{
    public class CharacterModel : ICharacterModel
    {
        public string Id { get; }
        public string DisplayName { get; }
        public AssetReferenceGameObject PrefabReference { get; }

        public CharacterModel(string id, string displayName, AssetReferenceGameObject prefab)
        {
            Id = id;
            DisplayName = displayName;
            PrefabReference = prefab;
        }
    }

}

