using System;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace WhiteKNight
{
    public class SEPlayerForUI : MonoBehaviour
    {
        [SerializeField] private SoundEffectId SoundEffectId;
        [SerializeField] private Button _button;
        [SerializeField] private bool _isButtonClick = true;

        private void Start()
        {
            if (_isButtonClick == false) return;
            if (_button == null) return;
            _button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PlaySE();
                })
                .AddTo(this);
        }

        public void PlaySE()
        {
            SoundManager.Instance?.PlaySE(SoundEffectId);
        }
    }

    [Serializable]
    public sealed class SoundEffectEntry
    {
        public SoundEffectId Id;
        public AudioClip Clip;
        public float Volume = 1f;
        public float Pitch = 1f;
        public bool Loop;
        public AudioMixerGroup Mixer;
    }

    public enum SoundEffectId
    {
        Cancel,
        Click01,
        Click02,
        Decide,
        EnemyDamage,
        EnemyDead,
        EnemyHit01,
        EnemyShout,
        Equip,
        Footstep,
        Jump,
        Knife,
        KnightDead,
        Land,
        SmallExplosion2,
        SpinAttack,
        SpinJump,
        Swing,
        Swing2,
        Unequip,
        Recovery
    }
}
