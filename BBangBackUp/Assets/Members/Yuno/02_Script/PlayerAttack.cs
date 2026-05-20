using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Hitboxes")]
    public GameObject rightAttackHitbox;
    public GameObject leftAttackHitbox;
    public GameObject upAttackHitbox;
    public GameObject downAttackHitbox;

    [Header("Attack Settings")]
    public int playerDamage = 10;       // 플레이어 공격력
    public float attackDuration = 0.2f; // 콜라이더가 켜져 있을 시간
    public float attackCooldown = 0.5f; // 공격 속도(쿨타임)

    private bool isAttacking = false;
    public bool IsAttacking => isAttacking; // 외부에서 읽을 수 있도록 프로퍼티 추가
    private GameObject currentHitbox = null; // 현재 활성화된 힛박스를 저장할 변수

    public static PlayerAttack instance;
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {  
        // 모든 힛박스를 처음에는 비활성화
        if (leftAttackHitbox) leftAttackHitbox.SetActive(false);
        if (rightAttackHitbox) rightAttackHitbox.SetActive(false);
        if (upAttackHitbox) upAttackHitbox.SetActive(false);
        if (downAttackHitbox) downAttackHitbox.SetActive(false);
    }

    void OnAttack()
    {
        if (PlayerManager.Instance.isShovel || GameManager.instance.isDead || isAttacking) return;

        StartCoroutine(PerformAttack());
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;

        // 공격 시작 시 플레이어 이동 속도를 0으로 만듦
        PlayerManager.Instance.originalMoveSpeed = PlayerManager.Instance.moveSpeed;
        PlayerManager.Instance.moveSpeed = 0f;

        // 1. 방향 우선순위 판단
        currentHitbox = GetAttackHitbox();

        // 2. 결정된 단 하나의 힛박스만 활성화
        if (currentHitbox != null)
        {
            currentHitbox.SetActive(true);
        }

        // 애니메이션 제어
        PlayerManager.Instance.animator.SetTrigger("Attack");

        // 3. 지정된 시간(공격 판정 시간)만큼 대기
        yield return new WaitForSeconds(attackDuration);

        // 4. 활성화했던 힛박스 비활성화 및 초기화
        if (currentHitbox != null)
        {
            currentHitbox.SetActive(false);
            currentHitbox = null;
        }

        // 원래 속도로 안전하게 복구
        PlayerManager.Instance.moveSpeed = PlayerManager.Instance.originalMoveSpeed;

        // 5. 남은 쿨타임만큼 대기
        yield return new WaitForSeconds(attackCooldown - attackDuration);

        isAttacking = false;
    }

    private GameObject GetAttackHitbox()
{
    if (PlayerManager.Instance.isUp)    return upAttackHitbox;
    if (PlayerManager.Instance.isDown)  return downAttackHitbox;
    if (PlayerManager.Instance.isRight) return rightAttackHitbox;
    if (PlayerManager.Instance.isLeft)  return leftAttackHitbox;

    // 만에 하나 예외 상황 시 기본값으로 다운 힛박스 반환
    return downAttackHitbox; 
}
}