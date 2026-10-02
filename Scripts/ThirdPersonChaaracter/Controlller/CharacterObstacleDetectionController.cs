using UnityEngine;

namespace WhiteKNight
{
    public class CharacterObstacleDetectionController : MonoBehaviour, IObstacleDetectionController
    {
        private float detectDistance = 0.1f;
        private float sideDistance = 2f;

        public bool IsFrontBlocked()
        {
            RaycastHit hit;
            return Physics.Raycast(transform.position, transform.forward, out hit, detectDistance);
            
        }

        public bool IsLeftFree()
        {
            return !Physics.Raycast(transform.position, -transform.right, sideDistance);
        }

        public bool IsRightFree()
        {
            return !Physics.Raycast(transform.position, transform.right, sideDistance);
        }
    }


}
