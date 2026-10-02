using UnityEngine;

namespace WhiteKNight
{

    public class ThirdPersonEnemyDetector : EnemyDetector_Base
    {
        [SerializeField] private ThirdPersonCharacterInput _input;
   
        protected override void SetTarget(Transform nearest)
        {
            _input.SetTarget(nearest);
        }

    }
}

