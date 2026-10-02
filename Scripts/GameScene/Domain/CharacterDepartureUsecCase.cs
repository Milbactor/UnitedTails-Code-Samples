using UniRx;

namespace WhiteKNight
{
    public class CharacterDepartureUsecCase : ICharacterDepartureUseCase
    {
        private ICharacterStateModel _model;
        public ReactiveProperty<bool> IsDead => _model.IsDead;

        public CharacterDepartureUsecCase(ICharacterStateModel model)
        {
            _model = model;
        }
    }
}