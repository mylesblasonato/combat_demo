using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hand : MonoBehaviour
{
    [HideInInspector] public EquipableItem closeItem, leftItem, rightItem;
    public float health = 100;
    public Transform leftHand;
    public Transform rightHand;
    public float throwForce = 100f;
    public float hitForce = 10f;
    public LayerMask interactable;
    private bool isHoldingLeft = false;
    private bool isHoldingRight = false;
    private bool activeHandLeft = false;
    private bool activeHandRight = false;
    private bool isThrowing = false;
    private bool isHoldingThrow = false;
    private bool isAttackingLeft = false;
    private bool isAttackingRight = false;
    private Animator animator;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animator = GetComponent<Animator>();

        leftItem = leftHand.GetComponent<EquipableItem>();
        rightItem = rightHand.GetComponent<EquipableItem>();
    }

    private void Update()
    {
        if(health > 0)
            Raycast();
    }

    private void FixedUpdate()
    {
        if(isThrowing && health > 0)
            Throw();
    }

    public void LeftAttackInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            activeHandLeft = true;
            activeHandRight = false;

            Attack();
            GrabClosestItem();
        }

        if (context.performed && isHoldingThrow)
        {
            activeHandLeft = true;
            activeHandRight = false;

            isThrowing = true;
        }
    }

    public void RightAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            activeHandRight = true;
            activeHandLeft = false;

            Attack();
            GrabClosestItem();
        }

        if (context.performed && isHoldingThrow)
        {
            activeHandRight = true;
            activeHandLeft = false;

            isThrowing = true;
        }
    }

    public void ThrowInput(InputAction.CallbackContext context)
    {
        if(context.performed)
            isHoldingThrow = true;
        else if (context.canceled)
            isHoldingThrow = false;

    }

    public void BlockInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            animator.SetBool("Block", true);
        else
            animator.SetBool("Block", false);
    }

    public void RollInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            animator.SetTrigger("Roll");
    }

    private void OnCollisionEnter(Collision collision)
    {
        var tempEquippedItem = collision.transform.GetComponent<EquipableItem>();
        if (tempEquippedItem != null)
        {
            closeItem = tempEquippedItem;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        var tempEquippedItem = collision.transform.GetComponent<EquipableItem>();
        if (tempEquippedItem != null)
        {
            closeItem = null;
        }
    }

    public void Throw()
    {
        if (activeHandLeft && isHoldingLeft)
        {
            animator.SetTrigger("Throw_Left"); 
            closeItem.GetComponent<Rigidbody>().isKinematic = false;
            closeItem.GetComponent<Collider>().enabled = true;
            closeItem.GetComponent<Rigidbody>().AddForce((transform.forward * throwForce) + transform.up * Time.deltaTime, ForceMode.Impulse);
            closeItem.transform.parent = null;
            isHoldingLeft = false;
            isThrowing = false;
            isHoldingThrow = false;
            leftItem = leftHand.GetComponent<EquipableItem>();
        }

        if (activeHandRight & isHoldingRight)
        {
            animator.SetTrigger("Throw_Right");          
            closeItem.GetComponent<Rigidbody>().isKinematic = false;
            closeItem.GetComponent<Collider>().enabled = true;
            closeItem.GetComponent<Rigidbody>().AddForce((transform.forward * throwForce) + transform.up * Time.deltaTime, ForceMode.Impulse);
            closeItem.transform.parent = null;
            isHoldingRight = false;
            isThrowing = false;
            isHoldingThrow = false;
            rightItem = rightHand.GetComponent<EquipableItem>();
        }

    }

    public void Hit()
    {      
        if (health <= 0)
        {
            health = 0;
            animator.SetTrigger("Death");
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }
        else
        {
            animator.SetTrigger("Hit");
            health -= 10;
        }
    }

    public void Attack()
    {
        if (activeHandLeft)
        {
            animator.SetTrigger("Sword_Left");
            isAttackingLeft = true;
        }

        if (activeHandRight)
        {
            animator.SetTrigger("Sword_Right");
            isAttackingRight = true;
        }
    }

    private void Raycast()
    {
        if (closeItem == null) return;
        var heldItem = closeItem.GetComponent<EquipableItem>();

        if (isAttackingLeft && heldItem.isHitting)
        {
            leftItem.transform.GetComponent<Collider>().enabled = true;
            heldItem.hitTarget.GetComponent<Rigidbody>().AddForce(transform.forward * (isHoldingLeft ? closeItem.power : hitForce) * Time.deltaTime, ForceMode.Impulse);
            heldItem.hitTarget.GetComponent<Hand>().Hit();
            isAttackingLeft = false;
        }

        if (isAttackingRight && heldItem.isHitting)
        {
            rightItem.transform.GetComponent<Collider>().enabled = true;
            heldItem.hitTarget.GetComponent<Rigidbody>().AddForce(transform.forward * (isHoldingRight ? closeItem.power : hitForce) * Time.deltaTime, ForceMode.Impulse);
            heldItem.hitTarget.GetComponent<Hand>().Hit();
            isAttackingRight = false;
        }
    }

    public void GrabClosestItem()
    {

        if (closeItem == null) return;

        if(activeHandLeft && closeItem.transform.parent != rightHand)
            closeItem.transform.parent = leftHand;

        if (activeHandRight && closeItem.transform.parent != leftHand)
            closeItem.transform.parent = rightHand;

        closeItem.transform.GetComponent<Rigidbody>().isKinematic = true;
        closeItem.transform.GetComponent<Collider>().enabled = false;

        if (closeItem.transform.parent == leftHand)
        {
            closeItem.transform.SetLocalPositionAndRotation(
                Vector3.zero,
                Quaternion.Euler(closeItem.leftHandRotation));
            isHoldingLeft = true;
            leftItem = closeItem;
        }

        if (closeItem.transform.parent == rightHand)
        {
            closeItem.transform.SetLocalPositionAndRotation(
                Vector3.zero,
                Quaternion.Euler(closeItem.rightHandRotation));
            isHoldingRight = true;
            rightItem = closeItem;
        }
    }
}
