using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Monster2 : MonoBehaviour, IChaseRange
{
    [Header("Settings")]
    public float speed = 5f;
    public float attackCooldown = 2.0f;
    public int damage = 3;

    //플레이어가 체이스 범위에 있는지 없는지 *
    private bool _chaseRange;
    //플레이어가 공격 범위에 있는지
    private bool _attackRange;
    //플레이어가 공격을 할수있는지
    private bool _canAttack;
    //플레이어가 공격을 하고있는지
    private bool _isAttacking;

    private Animator _anim;
    private Rigidbody2D _rb;
    private Transform _playerTrm;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _playerTrm = GameObject.FindWithTag("Player").transform;
    }
    void Start()
    {
        _canAttack = true;
    }

    void Update()
    {
        if (_chaseRange && !_isAttacking)
        {
            Vector2 dir = _playerTrm.position - transform.position;
            dir.Normalize();
            DirectionWalk(dir);
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            _anim.SetBool("isWalking", false);
        }
        if (_attackRange && _canAttack && !_isAttacking)
        {
            StartCoroutine(AttackRountine());
        }
    }

    public void DirectionWalk(Vector2 value)
    {
        int flip = value.x >= 0 ? -1 : 1;
        transform.localScale = new Vector3(1 * flip, 1, 1);
        _rb.linearVelocity = value * speed;
        //walking animation clip 
        _anim.SetBool("isWalking", true);

    }

    private IEnumerator AttackRountine()
    {

        _isAttacking = true;
        _anim.SetBool("isWalking", false);
        _anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.4f);
        //attackhitbox enable
        yield return new WaitForSeconds(0.3f);
        //attackhitbox disable
        StartCoroutine(CoolTimeRountine());
    }
    private IEnumerator CoolTimeRountine()
    {
        _isAttacking = false;
        _canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        _canAttack = true;
    }
    public void SetChasing(bool value)
    {
        _chaseRange = value;
    }
    public void SetAttackRange(bool value)
    {
        _attackRange = value;
    }
    private int attackCount;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attackCount++;
            if (attackCount % 3 == 0)
            {
                GameManager.instance.TakeDamage(damage * 3);
                Debug.Log("upgrade damage");
            }
        }
    }
    /*public void SaveZone()
    {
        if (GameManager.instance.isSafeZone == true)
        {
            _chasing = false;
            Looking();
        }
        else
        {
            _chasing = true;
            Chase();
        }

    } */
}
    