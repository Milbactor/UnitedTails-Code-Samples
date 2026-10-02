using Cysharp.Threading.Tasks;

namespace WhiteKNight
{
    public interface ICharacterSpawnPresenter
    {
        UniTask Spawn();
        void SetCameraView(CameraView cameraView);
    }
}