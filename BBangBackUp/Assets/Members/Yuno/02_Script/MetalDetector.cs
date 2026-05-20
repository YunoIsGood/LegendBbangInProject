using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    [Header("설정")]
    public Transform arrowUI;       
    public float detectRange = 15f; 
    public float rotationSpeed = 15f;
    
    // 추적 속도 (값이 높을수록 딱 붙어서 따라옵니다. 30~50 추천)
    [Range(10f, 100f)]
    public float followSmoothness = 40f; 

    private TreasureManager tm;
    private Vector3 offset = Vector3.zero;

    void Start()
    {
        tm = Object.FindFirstObjectByType<TreasureManager>();
        
        // 실행 시 오프셋을 강제로 0으로 만들어 중앙에 맞춤
        offset = Vector3.zero; 

        // 화살표가 자식으로 있다면 위치 계산이 꼬일 수 있으므로 
        // 부모 관계를 해제하는 것이 끊김 방지에 더 좋습니다.
        if (arrowUI != null && arrowUI.parent == transform)
        {
            arrowUI.SetParent(null);
        }
    }

    // 위치 추적은 LateUpdate에서 부드럽게(Lerp) 처리
    void LateUpdate()
    {
        if (arrowUI == null) return;

        // 현재 위치에서 목표 위치(플레이어)까지 아주 빠르게 따라가도록 설정
        // 이 방식이 단순히 position을 맞추는 것보다 훨씬 부드럽습니다.
        Vector3 targetPos = transform.position + offset;
        arrowUI.position = Vector3.Lerp(arrowUI.position, targetPos, Time.deltaTime * followSmoothness);
    }

    void Update()
    {
        if (tm == null || arrowUI == null) return;

        // 1. 가장 가까운 보물 찾기
        float minDistance = Mathf.Infinity;
        Transform target = null;

        foreach (Transform t in tm.activeTreasures)
        {
            if (t == null) continue;
            float dist = Vector2.Distance(transform.position, t.position);
            if (dist < minDistance && dist <= detectRange)
            {
                minDistance = dist;
                target = t;
            }
        }

        // 2. 회전 로직
        if (target != null)
        {
            if (!arrowUI.gameObject.activeSelf) arrowUI.gameObject.SetActive(true);

            Vector2 dir = target.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90f);
            
            // 회전도 Slerp를 통해 부드럽게 처리
            arrowUI.rotation = Quaternion.Slerp(arrowUI.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            if (arrowUI.gameObject.activeSelf) arrowUI.gameObject.SetActive(false);
        }
    }
}