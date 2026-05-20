using UnityEngine;
using System.Collections;

public class PlayerAttackArea : MonoBehaviour
{
    // 부모(PlayerAttack)의 공격력을 가져오기 위한 참조
    public PlayerAttack playerAttack;

    [Header("Knockback Settings")]
    public float knockbackForce = 8f;     // 밀려나는 힘의 세기
    public float knockbackDuration = 0.15f; // 밀려나는 시간 (초)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 대미지 전달 (인터페이스 방식 유지)
        IDamageable target = collision.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(playerAttack.playerDamage);
            Debug.Log(collision.name + "에게 대미지를 입혔습니다.");
        }

        // 2. 몬스터 스크립트를 안 건드리고 물리 엔진(Rigidbody2D)만 뺏어서 강제로 밀기
        Rigidbody2D monsterRb = collision.GetComponent<Rigidbody2D>();
        
        // 몬스터에게 Rigidbody2D가 있고, 플레이어 본인이 아닐 때만 실행
        if (monsterRb != null && !collision.CompareTag("Player"))
        {
            // 공격자(플레이어)로부터 몬스터를 향하는 방향 계산
            Vector2 knockbackDirection = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;

            // 몬스터를 밀어내는 코루틴 실행 (이 스크립트가 붙은 오브젝트에서 실행)
            StartCoroutine(ForceKnockback(monsterRb, knockbackDirection));
        }
    }

    // 몬스터의 물리 강제 제어 코루틴 (수정됨)
    private IEnumerator ForceKnockback(Rigidbody2D rb, Vector2 direction)
    {
        float timer = 0f;

        // knockbackDuration(예: 0.15초) 동안 매 프레임마다 강제로 속도를 덮어씌움
        while (timer < knockbackDuration)
        {
            // 몬스터가 도중에 죽어서 삭제되면 에러가 날 수 있으니 체크
            if (rb == null) break;
            rb.linearVelocity = direction * knockbackForce; 

            timer += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 넉백이 끝난 후 미끄러지지 않게 속도를 0으로 초기화
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}