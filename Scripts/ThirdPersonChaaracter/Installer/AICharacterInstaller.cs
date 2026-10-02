using System.Collections.Generic;
using UnityEngine;

namespace WhiteKNight
{
    public class AICharacterInstaller : ThirdPersonCharacterInstaller_Base
    {
        [SerializeField] private AIBehaviourInput behaviourInput;
        [SerializeField] private AICharacterView characterView;
        [SerializeField] private CharacterObstacleDetectionController obstacleController;
        [SerializeField] private AICharacterAttackView attackView;
        [SerializeField] private SwordAttackView swordAttackView;
        [SerializeField] private AICharacterDamageView damageView;
        [SerializeField] private CharacterDepartureView departureView;
        [SerializeField] private AICharacterSetting characterSetting;
        [SerializeField] private RotatingSpinAttackEffectView rotatingSpinAttackEffectView;
        [SerializeField] private List<PuritusView> puritusViews = new List<PuritusView>();

        public override void Construct(
            CameraView cameraView, 
            HPView hpView, 
            CharacterNameView characterNameImageView, 
            ResultUIView resultUIView = null,
            List<PuritusView> puritusViews = null)
        {
            groundCheckController = GetComponent<CharactorGroundCheckController>();
            var combatSetting = characterSetting.CombatSetting;

            var characterLifeCycleNotifier = new CharacterDeathNotifier();

            var stateModel = new ThirdPersonStateModel(characterSetting);
            var aiStateModel = new AIStateModel();

            //No Puritus for AI
           /* puritusViews.Clear();
            var puritusList = GameObject.FindGameObjectsWithTag("Recovery");
            foreach (var puritus in puritusList)
            {
                PuritusView view;
                bool exist = puritus.TryGetComponent<PuritusView>(out view);
                if (exist) { puritusViews.Add(view); }
            }*/

            var movementUseCase = new AICharacterMovementUseCase(
                groundCheckController,
                 stateModel,
                 aiStateModel,
                 characterLifeCycleNotifier
                );
            
            var equipUseCase = new AIEquipUseCase(
                stateModel,
                aiStateModel,
                combatSetting,
                characterLifeCycleNotifier);
          

            var attackUseCase = new AICharacterAttackUseCase(
                stateModel,
                aiStateModel,
                combatSetting,
                behaviourInput,
                characterLifeCycleNotifier
                );

            var damageUseCase = new AICharacterDamageUseCase(
                stateModel,
                combatSetting,
                characterLifeCycleNotifier
                );

            var approachUseCase = new AIApproachUseCase(
                aiStateModel,
                stateModel,
                combatSetting,
                 characterLifeCycleNotifier
                );

            var jumpDecisionUseCase = new AIJumpDecisionUseCase(
                aiStateModel,
                stateModel,
                behaviourInput,
                obstacleController,
                combatSetting
                );
            var moveCommandUseCase = new AIMoveCommandUseCase(
                aiStateModel, 
                groundCheckController
                );

            var ascendingUseCase = new CharacterDepartureUsecCase(stateModel);

          //  var recoveryUseCase = new ThirdPersonRecoveryUseCase(stateModel);

            var hpUseCase = new HPUseCase(combatSetting);

            //var recoveryPresenter = new ThirdPersonRecoveryPresenter(recoveryUseCase, hpUseCase, puritusViews);

            var presenter = new AICharacterMovementPresenter(
                movementUseCase,
                attackUseCase,
                approachUseCase,
                moveCommandUseCase,
                jumpDecisionUseCase,
                characterView,
                attackView,
                behaviourInput,
                characterLifeCycleNotifier
             );

            var attackPresenter = new AICharacterAttackPresenter(
                attackUseCase,
                equipUseCase,
                attackView,
                behaviourInput,
                swordAttackView,
                characterLifeCycleNotifier,
                rotatingSpinAttackEffectView
                );

            var damagePresenter = new AICharacterDamagePresenter(
                damageUseCase,
                hpUseCase,
                damageView,
                characterLifeCycleNotifier
                );

            var equipPresenter = new AIEquipPresenter( 
                equipUseCase, 
                attackView,
                behaviourInput,
                characterLifeCycleNotifier
                );

            var approachPresenter = new AIApproachPresenter(
                behaviourInput,
                attackView,
                approachUseCase,
                aiStateModel,
                characterLifeCycleNotifier
                );

            var ascendingPresenter = new CharacterDeparturePresenter(ascendingUseCase, departureView);

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