using System.Collections;
using UnityEngine;

public class Monster3 : MonoBehaviour, IChaseRange
{
    [Header("Settings")]
    public float speed = 5f;
    public float attackCooltime = 3.0f;
    public int damage = 3;

    //플레이어가 체이스 범위에 있는지 없는지 
    private bool _chaseRange;
    //플레이어가 공격 범위에 있는지
    private bool _canAttack;
    //플레이어가 공격을 하고있는지
    public bool _isAttacking;

    [SerializeField]
    private GameObject prefabWeapon;
    [SerializeField]
    private WeaponSpawn _weaponSpawner;
    private Animator _anim;
    private Rigidbody2D _rb;
    private Transform _playerTrm;
    

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _playerTrm = GameObject.FindWithTag("Player").transform;

    }
    void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
        _canAttack = true;
    }

    void Update()
    {
        /* if (_chaseRange && !_isAttacking)
         {
             AttackDirection();
         } */
        AttackDirection();
    }
    void AttackDirection()
    {
        Vector3 dir;
        if ( _chaseRange)
        {
            if (_canAttack && !_isAttacking)
            {
                dir = _playerTrm.position - transform.position;
                dir.Normalize();
                transform.localScale = dir.x > 0 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
                _weaponSpawner.SpawnWeapon(dir);
            }
            else
            {
                dir = _playerTrm.position - transform.position;
                dir.Normalize();
                transform.localScale = dir.x > 0 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
            }
        }
    }
    public void SetChasing(bool value)
    {
        _chaseRange = value;
    }

    public void SetAttackRange(bool value)
    {
       
    }

   
    public IEnumerator CoolTimeRountine()
    {
        _isAttacking = false;
        _canAttack = false;
        yield return new WaitForSeconds(attackCooltime);
        _canAttack = true;
    }

}
