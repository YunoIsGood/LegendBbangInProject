using UnityEngine;
using System.Collections;

public class GhostScript : MonoBehaviour, IDamageable
{
    public MonsterData data;
    public Transform playerTrm;
    [HideInInspector] public float moveSpeed; // GhostAttack에서 접근할 수 있도록 하거나 유지
    [HideInInspector] public int damage;
    public bool isChasing = false;
    private const string PLAYER_TAG = "Player";
    private int currenthealth;

    void Start()
    {
        GameObject player = GameObject.FindWithTag(PLAYER_TAG);
        if (player != null) playerTrm = player.transform;
        currenthealth = data.maxHp;
        damage = data.damage;
        moveSpeed = data.speed;
    }

    void Update()
    {
        if (playerTrm == null) return;

        Vector3 moveDir = (playerTrm.position - transform.position).normalized;
        if(isChasing)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        currenthealth -= damage;
        Debug.Log("몬스터가 받은 데미지:" + damage);
        if (currenthealth <= 0)
        {
            Destroy(gameObject);
            Debug.Log("뒤짐!");
        }
    }
}