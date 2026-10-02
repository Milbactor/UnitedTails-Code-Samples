using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace WhiteKNight
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField]
        private AssetReferenceGameObject axelReference;

        [SerializeField]
        private AssetReferenceGameObject clausReference;

        [SerializeField]
        private AssetReferenceGameObject siriusReference;

        [SerializeField]
        private AssetReferenceGameObject godfriedReference;

        [SerializeField]
        private AssetReferenceGameObject axelAIReference;

        [SerializeField]
        private AssetReferenceGameObject clausAIReference;

        [SerializeField]
        private AssetReferenceGameObject siriusAIReference;

        [SerializeField]
        private AssetReferenceGameObject godfriedAIReference;

        [SerializeField] private HPView playerHPView;
        [SerializeField] private HPView npcHPView;

        [SerializeField] private LeftEnemiesView leftEnemiesView;

        public override void InstallBindings()
        {
        
            var selection = Container.Resolve<ICharacterSelectionModel>();
            string id = selection.SelectedPlayCharacterId;

            switch (id)
            {
                case "1":
                    Container.Bind<AssetReferenceGameObject>().WithId("PlayerModel")
                    .FromInstance(axelReference) 
                    .AsCached();
                    Container.Bind<ICharacterModel>().WithId("PlayerModel").To<CharacterModel>().AsCached()
                        .WithArguments<string, string, AssetReferenceGameObject>("1", "Axel", axelReference);
                 break;
                case "2":
                    Container.Bind<AssetReferenceGameObject>().WithId("PlayerModel")
                    .FromInstance(clausReference)
                    .AsCached();
                    Container.Bind<ICharacterModel>().WithId("PlayerModel").To<CharacterModel>().AsCached()
                       .WithArguments<string, string, AssetReferenceGameObject>("2", "Claus", clausReference);
                    break;
                case "3":
                    Container.Bind<AssetReferenceGameObject>().WithId("PlayerModel")
                        .FromInstance(siriusReference)
                        .AsCached();
                    Container.Bind<ICharacterModel>().WithId("PlayerModel").To<CharacterModel>().AsCached()
                      .WithArguments<string, string, AssetReferenceGameObject>("3", "Sirius", siriusReference);
                    break;
                case "4":
                    Container.Bind<AssetReferenceGameObject>().WithId("PlayerModel")
                        .FromInstance(godfriedReference)
                        .AsCached();
                    Container.Bind<ICharacterModel>().WithId("PlayerModel").To<CharacterModel>().AsCached()
                      .WithArguments<string, string, AssetReferenceGameObject>("4", "Godfried", godfriedReference);
                    break;
                default:
                    Container.Bind<AssetReferenceGameObject>().WithId("PlayerModel")
                    .FromInstance(clausReference)
                    .AsCached();
                    Container.Bind<ICharacterModel>().WithId("PlayerModel").To<CharacterModel>().AsCached()
                       .WithArguments<string, string, AssetReferenceGameObject>("2", "Claus", clausReference);
                    break;
            }

            string npcId = selection.SelectedNpcCharacterId;
            switch (npcId)
            {
                case "1":
                    Container.Bind<AssetReferenceGameObject>().WithId("NpcModel")
                    .FromInstance(axelAIReference)
                    .AsCached();
                    Container.Bind<ICharacterModel>().WithId("NpcModel").To<CharacterModel>().AsCached()
                        .WithArguments<string, string, AssetReferenceGameObject>("1", "Axel", axelAIReference);
                    break;
                case "2":
                    Container.Bind<AssetReferenceGameObject>().WithId("NpcModel")
                    .FromInstance(clausAIReference)
                    .AsCached();
                    Container.Bind<ICharacterModel>().WithId("NpcModel").To<CharacterModel>().AsCached()
                       .WithArguments<string, string, AssetReferenceGameObject>("2", "Claus", clausAIReference);
                    break;
                case "3":
                    Container.Bind<AssetReferenceGameObject>().WithId("NpcModel")
                        .FromInstance(siriusAIReference)
                        .AsCached();
                    Container.Bind<ICharacterModel>().WithId("NpcModel").To<CharacterModel>().AsCached()
                      .WithArguments<string, string, AssetReferenceGameObject>("3", "Sirius", siriusAIReference);
                    break;
                case "4":
                    Container.Bind<AssetReferenceGameObject>().WithId("NpcModel")
                        .FromInstance(godfriedAIReference)
                        .AsCached();
                    Container.Bind<ICharacterModel>().WithId("NpcModel").To<CharacterModel>().AsCached()
                      .WithArguments<string, string, AssetReferenceGameObject>("4", "Godfried", godfriedAIReference);
                    break;
                default:
                    Container.Bind<AssetReferenceGameObject>().WithId("NpcModel")
                    .FromInstance(axelAIReference)
                    .AsCached();
                    Container.Bind<ICharacterModel>().WithId("NpcModel").To<CharacterModel>().AsCached()
                        .WithArguments<string, string, AssetReferenceGameObject>("1", "Axel", axelAIReference);
                    break;
            }

            // UseCase
            Container.Bind<ICharacterSpawnUseCase>()
                .To<CharacterSpawnUseCase>()
                .AsSingle();

            Container.Bind<ICameraUseCase>()
                .To<CameraUseCase>()
                .AsSingle();

            Container.Bind<ILeftEnemiesUseCase>()
                .To<LeftEnemiesUseCase>()
                .AsSingle();


            //Model
            Container.Bind<ICameraModel>()
                .To<CameraModel>()
                .AsSingle();

            Container.Bind<ILeftEnemiesModel>()
                .To<LeftEnemiesModel>()
                .AsSingle();


            // View TODO いらんのちゃう？
            Container.Bind<CharacterSpawnView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<CameraView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<LeftEnemiesView>()
                .FromComponentInHierarchy()
                .AsSingle();


            // Presenter
            Container.Bind<ICharacterSpawnPresenter>()
                 .To<CharacterSpawnPresenter>()
                .AsSingle();
          
            Container.Bind<ICameraPresenter>()
                .To<CameraPresenter>()
                .AsSingle();

            Container.Bind<ILeftEnemiesVPresenter>()
                .To<LeftEnemiesVPresenter>()
                .AsSingle()
                .NonLazy();

            Container.Bind<EnemyManager>()
               .FromComponentInHierarchy()
               .AsSingle();
        }
    }
}

