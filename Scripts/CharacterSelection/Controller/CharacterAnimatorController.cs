using System;
using UniRx;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace WhiteKNight
{
    public class CharacterAnimatorController : MonoBehaviour, ICharacterAnimatorController
    {
        private Subject<Unit> _onAnimationFinished = new Subject<Unit>();
        public IObservable<Unit> OnAnimationFinished => _onAnimationFinished;

        [System.Serializable]
        private class CharacterAnimatorBinding
        {
            public string characterId;
            public Transform characterTransform;
            public Animator animator;
            public Vector3 centerPosition;
        }

        [SerializeField]
        private CharacterAnimatorBinding[] bindings;

        public void PlaySelectSequence(string characterId)
        {
            var binding = System.Array.Find(bindings, b => b.characterId == characterId);
            if (binding == null)
            {
                Debug.LogWarning($"Animator binding not found for ID: {characterId}");
                return;
            }
            //  binding.characterTransform.position = binding.centerPosition; //It is possible that character adjust position in future
            binding.animator.SetTrigger("Selected");
        }

        public void PlayDeselectSequence(string characterId)
        {
            var binding = System.Array.Find(bindings, b => b.characterId == characterId);
            if (binding == null)
            {
                Debug.LogWarning($"Animator binding not found for ID: {characterId}");
                return;
            }

          //  binding.characterTransform.position = binding.centerPosition;
            binding.animator.SetTrigger("Unselected");
        }

        public void PlayHoveredSequence(string characterId)
        {
            var binding = System.Array.Find(bindings, b => b.characterId == characterId);
            if (binding == null)
            {
                Debug.LogWarning($"Animator binding not found for ID: {characterId}");
                return;
            }

            //binding.characterTransform.position = binding.centerPosition;
            binding.animator.SetBool("IsHovered", true);
        }

        public void PlayUnhoveredSequence(string characterId)
        {
            var binding = System.Array.Find(bindings, b => b.characterId == characterId);
            if (binding == null)
            {
                Debug.LogWarning($"Animator binding not found for ID: {characterId}");
                return;
            }

            //binding.characterTransform.position = binding.centerPosition;
            binding.animator.SetBool("IsHovered", false);
        }
    }
}