using System;
using System.Collections.Generic;
using UniRx;

namespace WhiteKNight
{
    public class ThirdPersonRecoveryPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposables
            = new CompositeDisposable();

        public ThirdPersonRecoveryPresenter(
            ThirdPersonRecoveryUseCase useCase,
            IHPUseCase HPUseCase,
            List<PuritusView> puritusViews)
        {
            foreach (var puritus in puritusViews)
            {
                puritus.OnPuritusObtained
                    .Subscribe(recovery =>
                    {
                        UnityEngine.Debug.Log($"Recovery received: {recovery}");

                        useCase.RecoverHP(recovery);

                        UnityEngine.Debug.Log(
                            $"RecoveryUseCase HP: {useCase.CurrentHP.Value}");

                        HPUseCase.CurrentHP.Value = useCase.CurrentHP.Value;

                        UnityEngine.Debug.Log(
                            $"HPUseCase HP: {HPUseCase.CurrentHP.Value}");
                    })
                    .AddTo(_disposables);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}