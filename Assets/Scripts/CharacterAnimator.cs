using UnityEngine;

namespace DefaultNamespace
{
    public class CharacterAnimator : MonoBehaviour
    {
        [Tooltip("characterName need match to SpeakerName for work")]
        public string CharacterName;
        [SerializeField] private Animator animator;

        public void PlayTalk() => animator.SetBool("isTalking", true);
        public void PlayIdle() => animator.SetBool("isTalking", false);
        
        public void TriggerAnimation(string triggerName) => animator.SetTrigger(triggerName);
        
        public void SetCharacterPosition(Vector3 position) => transform.position = position;
        public void SetCharacterRotation(Quaternion rotation) => transform.rotation = rotation;
        
        public void SetActiveCharacter(bool active)=> gameObject.SetActive(active);
    }
}