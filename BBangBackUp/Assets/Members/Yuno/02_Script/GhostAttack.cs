using UnityEngine;
using System.Collections;

public class GhostAttack : MonoBehaviour
{
    public GhostScript owner; 
    private bool isAttack = true; // 공격 가능 여부

    private void Awake()
    {
        if (owner == null) owner = GetComponentInParent<GhostScript>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isAttack) 
        {
            if (GameManager.instance != null && owner != null)
            {
                GameManager.instance.TakeDamage(owner.damage);
                
                isAttack = false;
                StartCoroutine(CoolTime(1f));
            }
        }
    }

    IEnumerator CoolTime(float time)
    {
        yield return new WaitForSeconds(time);
        isAttack = true;
    }
}