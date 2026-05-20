using System.Collections;
using UnityEngine;

public class enemyNPCAnim : MonoBehaviour
{
    
    [SerializeField] private Transform target;
    [SerializeField] private Transform myStartPos;
    [SerializeField] private Animator anim;
    public float attackdown = 1f;
    public float deadEffectdown = 0.2f;
    public float speed = 300f;
    public bool isAttacking;
    [SerializeField] private bool onMotion = false; //한번만 실행하게 하는 변수
    void OnEnable()
    {
        onMotion = false; //아직 안움직였으니 false
        transform.position = myStartPos.transform.position; //처음 시작 위치
        anim.SetBool("canWalk", true); 
        
    }


    void Update()
    {
        if (onMotion) return; //만약 아직 이미 움직였다면 리턴
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.position) <= 0.01f)
        {
            onMotion = true;
            anim.SetBool("canWalk", false);
            anim.SetBool("canAttack", true);
            isAttacking = true;
        }
        
    }

    public IEnumerator DeadRoutine()
    {
        yield return new WaitForSeconds(deadEffectdown);
        gameObject.SetActive(false);
    }
}
