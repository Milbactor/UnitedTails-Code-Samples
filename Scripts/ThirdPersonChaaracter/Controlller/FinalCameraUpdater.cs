using UnityEngine;
using Cinemachine;

namespace WhiteKNight
{
    // Script Execution Order で、このスクリプトを一番最後（+9999）に設定してください
    [DefaultExecutionOrder(99999)]
    public class FinalCameraUpdater : MonoBehaviour
    {
        private CinemachineBrain _brain;

        private void Awake()
        {
            _brain = GetComponent<CinemachineBrain>();
            if (_brain == null) _brain = Camera.main.GetComponent<CinemachineBrain>();
        }

        // LateUpdate は Update の後に必ず呼ばれる。
        // その中でも Execution Order が最後なら、文字通り「大トリ」になる。
        private void LateUpdate()
        {
            if (_brain != null && _brain.isActiveAndEnabled)
            {
                // ここでシャッターを切る
                // この瞬間、PlayerもAIも移動を終えている（はず）
                _brain.ManualUpdate();
            }
        }
    }
}