using UnityEngine;

public class AttackRange : MonoBehaviour
{
    public Monster1 owner; 

    private void Awake()
    {
        if (owner == null) owner = GetComponentInParent<Monster1>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            owner.SetAttackRange(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            owner.SetAttackRange(false);
        }
    }
}