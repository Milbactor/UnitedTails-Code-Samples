using UnityEngine;

namespace WhiteKNight
{
    public interface IHPView
    {
        void SetHPRate(float rate);
        void Initialize(Color color);
    }
}

