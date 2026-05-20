using UnityEngine;
using UnityEngine.InputSystem; // 이 줄이 없으면 Keyboard 오류가 납니다!

public class AttackBuff : MonoBehaviour
{
    // 유니티 인스펙터에서 수정 가능한 변수들
    public int itemcoin2 = 3;         // 아이템 개수
    public float timeFloating = 120f;
    


    private float endTime = 0f;      // 종료 시간 저장 (초기값 -1)
    private bool isBuffActive = false; // 버프 활성화 상태 스위치

    void Update()
    {
        // 1. 아이템 사용 로직 (2번 키)
        if (Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("2");
            // 아이템이 있고, 현재 버프가 꺼져 있을 때만 실행
            if (itemcoin2 > 0 && isBuffActive == false)
            {
                itemcoin2--;
                isBuffActive = true;
                endTime += Time.deltaTime;

                Debug.Log("공격력 업! (2분 동안 지속)");
            }
            else if (itemcoin2 <= 0)
            {
                Debug.Log("아이템이 부족합니다.");
            }
        }

        // 2. 시간 체크 로직 (매 프레임 감시)
        if (endTime > timeFloating)
        {
            isBuffActive = false; // 스위치 끄기
            endTime = -1f;        // 시간 초기화
            Debug.Log("공격력 업 종료");
        }
    }
}