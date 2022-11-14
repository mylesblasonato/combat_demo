using UnityEngine;

public class EquipableItem : MonoBehaviour
{
    public string weaponName;

    public Vector3 leftHandRotation;
    public Vector3 rightHandRotation;

    public string leftAnimation;
    public string rightAnimation;

    private GameObject player;
    private Animator animator;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = player.GetComponent<Animator>();
    }

    public void Attack_Left()
    {
        animator.SetTrigger(leftAnimation);
    }   
    
    public void Attack_Right()
    {
        animator.SetTrigger(rightAnimation);
    }
}