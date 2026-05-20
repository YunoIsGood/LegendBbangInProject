using System.Collections;
using UnityEngine;

public class PlayerNPCScript : MonoBehaviour
{

    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject lastDialogue;
    [SerializeField]private enemyNPCAnim enemyNPCScript;
    private Animator enemyAnim;
    [SerializeField] Animator playerAnim;
    public float AttackTime = 1f;
    void Start()
    {
        enemyAnim = enemy.GetComponent<Animator>();
    }
    

    public void OnClickDead()
    {
        if (enemyNPCScript.isAttacking)
        {
            StopAllCoroutines();
            enemyAnim.SetBool("canWalk", false);
            enemyAnim.SetBool("canAttack", false);
            playerAnim.SetBool("Attack", true);
            StartCoroutine(DamageRoutine());
        }

       
    }
    IEnumerator DamageRoutine()
    {
        yield return new WaitForSeconds(AttackTime);
        enemyAnim.SetBool("onDead", true);
        yield return StartCoroutine(enemyNPCScript.DeadRoutine());
        if (enemy.activeSelf == false)
        {
            gameObject.SetActive(false);
            lastDialogue.SetActive(true);
        }
    }
}
