using System;
using UnityEngine;

namespace WhiteKNight
{
    public interface IEnemyAttackAreaView
    {
        IObservable<Transform> OnTargetEntered { get; }
        IObservable<Transform> OnTargetExited { get; }
    }
}
