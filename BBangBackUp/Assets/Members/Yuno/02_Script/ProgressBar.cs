using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Header("References")]
    public Slider progressSlider;     // UI 슬라이더
    public Transform playerTransform; // 플레이어 위치
    public Transform startPoint;      // 맵의 시작 지점 (Y축)
    public Transform endPoint;        // 맵의 끝 지점 (Y축)

    void Update()
    {
        if (playerTransform == null || startPoint == null || endPoint == null) return;//없으면 반환

        // 1. 현재 플레이어의 Y 위치가 시작점과 끝점 사이에서 어느 정도 비율인지 계산 (0 ~ 1)
        float totalDistance = endPoint.position.y - startPoint.position.y;
        float currentDistance = playerTransform.position.y - startPoint.position.y;
        
        // 0과 1 사이로 값을 제한 (Clamping)
        float progress = Mathf.Clamp01(currentDistance / totalDistance);

        // 2. 슬라이더 값에 적용
        progressSlider.value = progress;
    }
}