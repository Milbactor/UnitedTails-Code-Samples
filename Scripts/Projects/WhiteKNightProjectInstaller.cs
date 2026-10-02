using Zenject;

namespace WhiteKNight
{
    public class WhiteKNightProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // キャラクター選択情報を保持するモデルをグローバルにバインド
            Container.Bind<ICharacterSelectionModel>()
                .To<CharacterSelectionModel>()
                .AsSingle()
                .NonLazy(); // ゲーム開始時にすぐ作っておく（必要なら）
        }
    }

}

