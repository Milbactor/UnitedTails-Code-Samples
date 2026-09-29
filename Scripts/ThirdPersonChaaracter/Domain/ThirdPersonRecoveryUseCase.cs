using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class ThirdPersonRecoveryUseCase
    {
        private readonly ICharacterStateModel _characterStateModel;
        public ReactiveProperty<float> CurrentHP => _characterStateModel.HP;

        public ThirdPersonRecoveryUseCase(ICharacterStateModel characterStateModel)
        {
            _characterStateModel = characterStateModel;
        }

        public void RecoverHP(float amount)
        {
            Debug.Log($"Before HP: {_characterStateModel.HP.Value}");
            Debug.Log($"Model MaxHP: {_characterStateModel.MaxHP}");
            Debug.Log($"Recovery: {amount}");

            _characterStateModel.HP.Value =
                Mathf.Min(
                    _characterStateModel.HP.Value + amount,
                    _characterStateModel.MaxHP);

            Debug.Log($"After HP: {_characterStateModel.HP.Value}");
        }
    }
}