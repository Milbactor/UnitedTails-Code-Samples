using UnityEngine;


namespace WhiteKNight
{
    public class SwordVFXView : MonoBehaviour
    {
        private MaterialPropertyBlock _mpb;
        [SerializeField] private Renderer _renderer;

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
        }

        void OnEnable()
        {
            if (this._renderer == null) return;
            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat("_StartTime", Time.time);
            _renderer.SetPropertyBlock(_mpb);
        }
    }

}




