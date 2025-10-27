using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class VRGrabObject : XRGrabInteractable
{
    private Rigidbody rb;
    private Vector3 lastPosition;
    private Vector3 velocity;
    private Vector3 lastAngularVelocity;
    private Transform grabbingHand;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        rb.isKinematic = true;

        // On enregistre la main ou le contrôleur qui attrape
        grabbingHand = args.interactorObject.transform;
        lastPosition = grabbingHand.position;
        lastAngularVelocity = Vector3.zero;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        rb.isKinematic = false;
        grabbingHand = null;

        // On applique la dernière vitesse mesurée avant le lâcher
        rb.linearVelocity = velocity;
        rb.angularVelocity = lastAngularVelocity;
    }

    private void FixedUpdate()
    {
        // Si l’objet est actuellement tenu, on calcule la vitesse à chaque frame
        if (isSelected && grabbingHand != null)
        {
            // Vitesse linéaire : déplacement de la main
            velocity = (grabbingHand.position - lastPosition) / Time.fixedDeltaTime;

            // Vitesse angulaire approximative
            lastAngularVelocity = grabbingHand.rotation.eulerAngles * Mathf.Deg2Rad / Time.fixedDeltaTime;

            lastPosition = grabbingHand.position;
        }
    }
}