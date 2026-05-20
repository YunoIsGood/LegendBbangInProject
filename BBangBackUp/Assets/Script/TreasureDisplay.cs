using UnityEngine;
using UnityEngine.Tilemaps;

public class TreasureDisplay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;//보물의 스프라이트 렌더러 
    private RectTransform canvasRect;//캔버스의 RectTransform 
    private Transform playerTransform;//플레이어의 트랜스폼 
    private UIManager uiManager;//UIManager 

    [Header("보관 데이터")]
    public TreasureData myData; //현재 보물 데이터 

    [Header("타일맵 설정")]
    public Tilemap groundTilemap;//지면 타일맵 
    public Tilemap safeZoneTilemap;//안전 구역 타일맵 

    [Header("UI 설정")]
    public GameObject uiPrefab;// 상호작용 UI 프리팹 (E버튼)
    public float showDistance = 3.0f;//UI가 보이는 최대 거리
    
    private GameObject myUI;//현재 보물과 연결된 UI 오브젝트
    private RectTransform uiRect;//UI 오브젝트의 RectTransform
    public Sprite chestSprite;//보물 상자 스프라이트

    private bool isInteracted = false; // 중복 상호작용 방지(가까운 보물 2개 한번에 먹어지는거 방지)

    void Awake()
    {
        GameObject spawner = GameObject.Find("TreasureSpawner") ?? new GameObject("TreasureSpawner");//TreasureSpawner 오브젝트 찾아서 참조 저장, 없으면 새로 생성
        transform.SetParent(spawner.transform);//보물 오브젝트를 TreasureSpawner 오브젝트의 자식으로 설정(계층 구조 정리)

        spriteRenderer = GetComponent<SpriteRenderer>();//스프라이트 렌더러 가져오기
        groundTilemap = GameObject.Find("Ground")?.GetComponent<Tilemap>();//Ground 타일맵 찾아서 가져오기
        safeZoneTilemap = GameObject.Find("SafeZone")?.GetComponent<Tilemap>();//SafeZone 타일맵 찾아서 가져오기
        
        uiManager = Object.FindFirstObjectByType<UIManager>();//씬 내에서 UIManager 인스턴스 찾아서 참조 저장
        
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();//씬 내에서 Canvas 인스턴스 찾아서 참조 저장
        if (canvas != null) canvasRect = canvas.GetComponent<RectTransform>();//Canvas가 있으면 RectTransform 가져오기
        
        GameObject player = GameObject.FindWithTag("Player");//플레이어 태그로 플레이어 오브젝트 찾아서 저장
        if (player != null) playerTransform = player.transform;//플레이어 트랜스폼 저장
    }

    public void SetTreasure(TreasureData data)//보물 데이터 주입하는 함수
    {
        myData = data;//보물 데이터 저장
        if (spriteRenderer != null) spriteRenderer.sprite = chestSprite;//보물 상자 스프라이트로 설정
        gameObject.name = data.treasureName;//게임 오브젝트 이름을 보물 이름으로 설정(편의성)
    }

    void Update()
    {
        if (groundTilemap == null || playerTransform == null || Camera.main == null || isInteracted) return;//필수 참조가 없거나 이미 상호작용한 보물이면 반환

        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);//보물의 월드 위치를 타일맵 셀 위치로 변환
        bool isExposed = !groundTilemap.HasTile(cellPos) && (safeZoneTilemap == null || !safeZoneTilemap.HasTile(cellPos));//보물이 지면 타일이 없고(노출된 상태) 세이프존 타일도 없으면(세이프존이 아니면) 노출된 것으로 간주
        float distance = Vector2.Distance(transform.position, playerTransform.position);//플레이어와 보물 사이의 거리 계산

        if (isExposed && distance <= showDistance)//보물이 노출된 상태이고 플레이어가 일정 거리 이내에 있으면
        {
            HandleInteractionUI(true);//상호작용 UI 표시

            if (Input.GetKeyDown(KeyCode.E))//플레이어가 E키를 눌렀으면
            {
                Interact();//상호작용
            }
        }
        else
        {
            HandleInteractionUI(false);//상호작용 UI 숨김
        }
    }

    private void HandleInteractionUI(bool show)//상호작용 UI 표시/숨김 함수
    {
        if (show)//UI를 보여줘야 하면
        {
            if (myUI == null && uiPrefab != null)//아직 UI가 생성되지 않았고 UI 프리팹이 있으면
            {
                myUI = Instantiate(uiPrefab, canvasRect);//캔버스의 자식으로 UI 오브젝트 생성
                uiRect = myUI.GetComponent<RectTransform>();//생성된 UI 오브젝트의 RectTransform 저장
            }
            
            if (myUI != null)//UI 오브젝트가 있으면
            {
                myUI.SetActive(true);//UI 오브젝트 활성화하여 보이게 함
                UpdateUIPosition();//UI 위치 업데이트하여 보물이 있는 월드 위치에 맞게 UI 위치 조정
            }
        }
        else if (myUI != null)//UI를 숨겨야 하면 UI 오브젝트가 있으면
        {
            myUI.SetActive(false);//UI 오브젝트 비활성화하여 보이지 않게 함
        }
    }

    private void UpdateUIPosition()//UI 위치 업데이트 함수(보물의 월드 위치에 맞게 UI 위치 조정)
    {
        Vector3 worldPos = transform.position + Vector3.up * 0.8f;//보물 위치에서 약간 위로 UI 위치 조정
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);//보물의 월드 위치를 화면 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, Camera.main, out Vector2 localPoint);//화면 좌표를 캔버스의 로컬 좌표로 변환
        uiRect.anchoredPosition = localPoint;//UI의 RectTransform의 앵커드 포지션을 변환된 로컬 좌표로 설정하여 UI 위치 업데이트
    }

    private void Interact()//보물과 상호작용하는 함수(보물 가까이 가면 나오는 E버튼 눌렀을 때)
    {
        isInteracted = true;//상호작용 했다는 bool값 설정해서 중복 상호작용 방지
        if (myUI != null) myUI.SetActive(false);//상호작용 했을 때 UI 숨김

        if (uiManager != null)//UIManager이 있으면
        {
            uiManager.ShowTreasureResult(myData, this.gameObject);//UIManager의 ShowTreasureResult 함수 호출하여 보물 획득 결과 UI 보여주기(보물 데이터와 현재 보물 오브젝트 전달)
        }
    }

    void OnDestroy() //보물 오브젝트가 파괴될 때
    {
        if (myUI != null) Destroy(myUI);//보물 오브젝트가 파괴될 때 연결된 UI 오브젝트도 함께 파괴하여 남아있지 않도록 함
    }
}