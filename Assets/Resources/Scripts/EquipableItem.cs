using UnityEngine;

public class EquipableItem : MonoBehaviour
{
    public string weaponName;

    public Vector3 leftHandRotation;
    public Vector3 rightHandRotation;

    public string leftAnimation;
    public string rightAnimation;

    public float size, power;

    public LayerMask interactableLayer;

    public bool isHitting = false;
    public GameObject hitTarget;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != interactableLayer) return;
        isHitting = true;
        hitTarget = collision.gameObject;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer != interactableLayer) return;
        isHitting = false;
        hitTarget = null;
    }

    public void Attack_Left(Animator animator)
    {
        animator.SetTrigger(leftAnimation);
    }   
    
    public void Attack_Right(Animator animator)
    {
        animator.SetTrigger(rightAnimation);
    }
}