using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WhiteKNight
{
    public interface ICharacterSpawnView
    {
        UniTask<Transform> SpawnNpc(AssetReferenceGameObject npcCharacter, Transform targetTransform);
        UniTask<Transform> SpawnPlayer(AssetReferenceGameObject playCharacter);
    }
}