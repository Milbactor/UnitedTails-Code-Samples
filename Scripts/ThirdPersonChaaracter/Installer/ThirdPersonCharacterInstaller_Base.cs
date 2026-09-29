using System.Collections.Generic;
using UnityEngine;

namespace WhiteKNight
{
    public class ThirdPersonCharacterInstaller_Base:  MonoBehaviour
    {
        [SerializeField] protected CharacterID characterID;
        [SerializeField] protected CharactorGroundCheckController groundCheckController;
        protected ThirdPersonCharacterMovementUseCase movementUseCase;
        protected ThirdPersonStateModel model;
        protected ThirdPersonCharacterAttackUseCase attackUseCase;
        protected ThirdPersonCharacterDamageUseCase damageUseCase;
        protected HPUseCase hpUseCase;
        protected CharacterDepartureUsecCase departureUsecCase;

        public virtual void Construct(
            CameraView cameraView, 
            HPView hpView, 
            CharacterNameView characterNameImageView,
            ResultUIView resultUIView = null,
            List<PuritusView> puritusViews = null
            )
        { }
    }
}