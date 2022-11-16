using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [HideInInspector] public EquipableItem closeItem;
    public Transform leftHand;
    public Transform rightHand;
    public string attackAndGrabButtonLeftHand;
    public string attackAndGrabButtonRightHand;
    public string blockButton, throwButton, rollButton;
    public float throwForce = 100f;
    private bool isHoldingLeft = false;
    private bool isHoldingRight = false;
    private bool activeHandLeft = false;
    private bool activeHandRight = false;
    private bool isThrowing = false;
    private Animator animator;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckAttackOrGrab();
    }

    private void FixedUpdate()
    {
        if(isThrowing)
            Throw();
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

    public void CheckAttackOrGrab()
    {
        if (Input.GetButtonDown(attackAndGrabButtonLeftHand))
        {
            activeHandLeft = true;
            activeHandRight = false;

            Attack();
            GrabClosestItem();
        }
        else if (Input.GetButtonDown(attackAndGrabButtonRightHand))
        {
            activeHandRight = true;
            activeHandLeft = false;

            Attack();
            GrabClosestItem();
        }

        if (Input.GetButton(throwButton))
        {
            if (Input.GetButtonDown(attackAndGrabButtonLeftHand))
            {
                activeHandLeft = true;
                activeHandRight = false;

                isThrowing = true;
            }
            else if (Input.GetButtonDown(attackAndGrabButtonRightHand))
            {
                activeHandRight = true;
                activeHandLeft = false;

                isThrowing = true;
            }
        }

        Block();
        Roll();
    }

    public void Roll()
    {
        if (Input.GetButtonDown(rollButton))
        {
            animator.SetTrigger("Roll");
        }
    }

    public void Block()
    {
        if (Input.GetButton(blockButton))
        {
            animator.SetBool("Block", true);
        }
        else
        {
            animator.SetBool("Block", false);
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
        }

    }

    public void Attack()
    {
        if (activeHandLeft)
        {
            animator.SetTrigger("Sword_Left");
        }

        if (activeHandRight)
        {
            animator.SetTrigger("Sword_Right");
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
        }

        if (closeItem.transform.parent == rightHand)
        {
            closeItem.transform.SetLocalPositionAndRotation(
                Vector3.zero,
                Quaternion.Euler(closeItem.rightHandRotation));
            isHoldingRight = true;
        }
    }
}
