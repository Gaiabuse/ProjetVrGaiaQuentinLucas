using UnityEngine;

public class GravityActivate : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    
    
    

    private void Gravity()
    {
        rb.useGravity = true;
    }

    private void Kinematic()
    {
        rb.isKinematic = false;
    }
}
