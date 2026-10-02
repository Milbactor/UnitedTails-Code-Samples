using UnityEngine;

namespace WhiteKNight
{
    public abstract class EnemyDetector_Base : MonoBehaviour
    {
        [SerializeField] protected float detectRadius = 6f;
        [SerializeField] protected LayerMask enemyLayer;

        protected abstract void SetTarget(Transform nearest);

        protected virtual void Update()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, enemyLayer);

            Transform nearest = null;
            float nearestSqr = float.MaxValue;

            foreach (var hit in hits)
            {
                float sqr = (hit.transform.position - transform.position).sqrMagnitude;
                if (sqr < nearestSqr)
                {
                    nearestSqr = sqr;
                    nearest = hit.transform;
                }
            }
            SetTarget(nearest);
        }

    }
}

