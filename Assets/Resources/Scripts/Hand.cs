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
    private bool isHoldingItem = false;
    private bool activeHandLeft = false;
    private bool activeHandRight = false;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckAttackOrGrab();
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
    }

    public void Attack()
    {
        if (activeHandLeft)
        {
            Debug.Log("Attacking Left");
            animator.SetTrigger("Sword_Left");
            //closeItem.Attack_Left();
        }

        if (activeHandRight)
        {
            Debug.Log("Attacking Right");
            animator.SetTrigger("Sword_Right");
           //closeItem.Attack_Right();
        }
    }

    public void GrabClosestItem()
    {
        Debug.Log("Grabbing");

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
            isHoldingItem = true;
        }

        if (closeItem.transform.parent == rightHand)
        {
            closeItem.transform.SetLocalPositionAndRotation(
                Vector3.zero,
                Quaternion.Euler(closeItem.rightHandRotation));
            isHoldingItem = true;
        }
    }
}
