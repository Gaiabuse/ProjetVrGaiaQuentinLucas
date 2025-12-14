using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace Fight
{
    public class Note : MonoBehaviour
    {
        [SerializeField] private float damages = 2f;
        [SerializeField] private float hitSpeed = 1f;
        [SerializeField] private int maxHit = 5;

        [SerializeField] private float inputActivation = 0.5f;
    
        private bool _canHit = true;
        protected bool InTrigger = false;
    
        protected InputDevice LeftHand;
        protected InputDevice RightHand;

        protected virtual void Awake()
        {
            StartCoroutine(WaitForDamages());
            LeftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            RightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                InTrigger = true;
                StartCoroutine(PlayerInTrigger());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            InTrigger = false;
        }

        protected virtual IEnumerator PlayerInTrigger()
        {
            while (InTrigger)
            {
                bool rightPressed = RightHand.TryGetFeatureValue(CommonUsages.trigger, out float triggerValueRight) && triggerValueRight > 0.1f;
                bool leftPressed  = LeftHand.TryGetFeatureValue(CommonUsages.trigger, out float triggerValueLeft) && triggerValueLeft > 0.1f;
                if (rightPressed || leftPressed) 
                {
                    FightManager.INSTANCE.AddAnxiety(- damages);
                    if (rightPressed)
                    {
                        RightHand.SendHapticImpulse(0, 0.5f, 0.1f);
                    }
                    else
                    {
                        LeftHand.SendHapticImpulse(0, 0.5f, 0.1f);
                    }
                    // ca aurait pu être pas mal, si t'as de longs fights, de ne pas détruire, mais ajouter à une pool
                    Destroy(gameObject);
                }
            
                yield return null;
            }
        }

        IEnumerator WaitForDamages()
        {
            if (_canHit)
            {
                for (int i = 0; i < maxHit; i++)
                {
                    yield return new WaitForSeconds(hitSpeed);
                    FightManager.INSTANCE.AddAnxiety(damages);
                }
                // pareil ici, maybe un peu de pooling
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
