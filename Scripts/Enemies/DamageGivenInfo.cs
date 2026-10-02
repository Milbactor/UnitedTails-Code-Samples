using UnityEngine;

namespace WhiteKNight
{
    public readonly struct DamageGivenInfo
    {
        public GameObject Target { get; }
        public Vector3 HitPoint { get; }
        public float Power { get; }

        public DamageGivenInfo(GameObject target, Vector3 hitPoint, float damage)
        {
            Target = target;
            HitPoint = hitPoint;
            Power = damage;
        }
    }
}