using UnityEngine.AddressableAssets;

namespace WhiteKNight
{
    public interface ICharacterModel
    {
        string DisplayName { get; }
        string Id { get; }
        AssetReferenceGameObject PrefabReference { get; }
    }
}