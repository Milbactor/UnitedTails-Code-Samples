using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

namespace WhiteKNight
{
    public class CharacterSpawnView : MonoBehaviour, ICharacterSpawnView
    {
        [SerializeField] CameraView cameraView;

        [SerializeField] Transform playerSpawnPoint;
        [SerializeField] Transform npcSpawnPoint;
        public Transform NpcSpawnPoint => npcSpawnPoint;

        [SerializeField] private HPView playerHPView;
        [SerializeField] private HPView npcHPView;

        [SerializeField] private CharacterNameView playerNameImageView;
        [SerializeField] private CharacterNameView npcNameImageView;

        [SerializeField] private ResultUIView resultUIView;

        [SerializeField] private Transform itemCanvas;
        [SerializeField] private List<PuritusView> puritusViews;

        [SerializeField] public readonly GameObject player;
        [SerializeField] public readonly GameObject npc;

        public async UniTask<Transform> SpawnPlayer(AssetReferenceGameObject playCharacter)
        {
            puritusViews = itemCanvas
                .GetComponentsInChildren<PuritusView>(true)
                .ToList();

            var go = await playCharacter.InstantiateAsync();
             go.GetComponent<PlayerCharacterInstaller>().Construct(
                 cameraView,
                 playerHPView,
                 playerNameImageView,
                 resultUIView,
                 puritusViews
                 );
             go.transform.position = playerSpawnPoint.position;
             return go.transform;
        }

        public async UniTask<Transform> SpawnNpc(AssetReferenceGameObject npcCharacter, Transform targetTransform)
        {
            var go = await npcCharacter.InstantiateAsync();
            go.transform.SetPositionAndRotation(
                npcSpawnPoint.position,
                npcSpawnPoint.rotation);

            var installer = go.GetComponent<AICharacterInstaller>();
            var behaviourInput = go.GetComponent<AIBehaviourInput>();

            behaviourInput.SetPartner(targetTransform);
            installer.Construct(
                cameraView,
                npcHPView,
                npcNameImageView);

            var agent = go.GetComponent<NavMeshAgent>();

            if (NavMesh.SamplePosition(
                    npcSpawnPoint.position,
                    out NavMeshHit hit,
                    5f,
                    NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogWarning(
                    $"No navMesh near NPC SpawnPoint: {npcSpawnPoint.position}");
            }
            return go.transform;
        }
    }
}

