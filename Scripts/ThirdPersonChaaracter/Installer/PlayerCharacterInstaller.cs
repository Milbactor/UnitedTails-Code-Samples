using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace WhiteKNight
{
    public class PlayerCharacterInstaller : ThirdPersonCharacterInstaller_Base
    {
        [SerializeField] private ThirdPersonCharacterInput playerInput;
        [SerializeField] private ThirdPersonCharacterView characterView;
        [SerializeField] private ThirdPersonCharacterAttackView attackView;
        [SerializeField] private SwordAttackView swordAttackView;
        [SerializeField] private ThirdPersonCharacterDamageView damageView;
        [SerializeField] private CharacterDepartureView departureView;
        [SerializeField] private ActorStateProvider actorStateProvider;
        [SerializeField] private CharacterSetting characterSetting;
        [SerializeField] private RotatingSpinAttackEffectView rotationSpinAttackEffectView;
        [SerializeField] private List<PuritusView> puritusViews;
        [SerializeField] private ResultUIView resultUIView;


        public override void Construct(
            CameraView cameraView, 
            HPView hpView,
            CharacterNameView characterNameImageView,
            ResultUIView resultUIView,
            List<PuritusView> puritusViews
            )
        {
  
            characterID = characterID == null ? GetComponent<CharacterID>() : characterID;
            groundCheckController = GetComponent<CharactorGroundCheckController>();
            var groundStateUpdater = new CharacterGroundStateUpdater(groundCheckController);
            model = new ThirdPersonStateModel(characterSetting);
            this.puritusViews = puritusViews;
            attackUseCase = new ThirdPersonCharacterAttackUseCase(model, characterSetting.CombatSetting);
            movementUseCase = new ThirdPersonCharacterMovementUseCase(groundStateUpdater, model, characterSetting.CombatSetting);
            damageUseCase = new ThirdPersonCharacterDamageUseCase(model, characterSetting.CombatSetting);
            departureUsecCase = new CharacterDepartureUsecCase(model);
            hpUseCase = new HPUseCase(characterSetting.CombatSetting);

            var recoveryUseCase = new ThirdPersonRecoveryUseCase(model);

            var recoveryPresenter = new ThirdPersonRecoveryPresenter(recoveryUseCase, hpUseCase, this.puritusViews);

            var presenter = new ThirdPersonCharacterMovementPresenter(
                movementUseCase,
                playerInput,
                characterView,
                actorStateProvider,
                cameraView
             );

            var attackPresenter = new ThirdPersonCharacterAttackPresenter(
                attackUseCase,
                playerInput,
                attackView,
                swordAttackView,
                rotationSpinAttackEffectView
                );

            var damagePresenter = new ThirdPersonDamagePresenter(
                damageUseCase,
                hpUseCase,
                damageView
                );

            var deathPresenter = new ThirdPersonCharacterGameOverPresenter(
                damageUseCase,
                cameraView,
                resultUIView,
                playerInput
                );

            var departurePresenter = new CharacterDeparturePresenter(
                departureUsecCase,
                departureView
                );

            var hpPresenter = new HPPresenter(
                hpUseCase, 
                hpView, 
                characterSetting.CombatSetting
                );

            var nameImagePresenter = new CharacterNameImagePresenter(
                characterNameImageView,
                characterSetting.CombatSetting
                );
        }
    }
}