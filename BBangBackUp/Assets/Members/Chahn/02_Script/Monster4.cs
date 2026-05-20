using System.Collections;
using UnityEngine;

public class Monster4 : MonoBehaviour, IChaseRange 
{

    [Header("Settings")]
    public float attackCooltime = 2.0f;

    private bool _chaseRange;
    private bool _canAttack;
    private bool _isAttacking;

    [SerializeField]
    private GameObject prefabBullet;
    private Rigidbody2D _rb;
    [SerializeField]
    private Transform _playerTrm;

    void Start()
    {
        _canAttack = true;
    }

    void Update()
    {
        if (_chaseRange && !_isAttacking)
        {
            if (_canAttack)
            {
                Vector3 dir = (_playerTrm.position - transform.position).normalized;
                transform.localScale = dir.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
                ShootBullet(dir);
            }
        }
    }

    void ShootBullet(Vector3 dir)
    {
        GameObject bullet = Instantiate(prefabBullet, transform.position, Quaternion.identity);
        IceBullet iceBullet = bullet.GetComponent<IceBullet>();
        iceBullet.Shoot(dir);

        _canAttack = false;
        _isAttacking = true;
        StartCoroutine(CoolTimeRoutine());
    }

    private IEnumerator CoolTimeRoutine()
    {
        _isAttacking = false;
        _canAttack = false;
        yield return new WaitForSeconds(attackCooltime);
        _canAttack = true;
    }

    public void SetChasing(bool value) => _chaseRange = value;
    public void SetAttackRange(bool value) { }
}

