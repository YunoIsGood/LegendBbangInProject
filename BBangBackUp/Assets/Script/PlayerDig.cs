using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerDig : MonoBehaviour
{
    public Tilemap groundTilemap;//지면 타일맵
    public Tilemap safeZoneTilemap;//세이프존 타일맵
    public LayerMask treasureLayer;//보물 레이어 마스크
    public float digRadius = 1.5f;//파는 반경
    public float collectRadius = 1.2f;//보물 수집 반경
    public ParticleSystem digEffect;//파는 효과 파티클

    private CinemachineImpulseSource source;

    void Awake()
    {
        source = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        if (Time.timeScale == 0) return;//게임이 일시정지 상태면 반환

        if (Input.GetKeyDown(KeyCode.C)) DigGround();//C키를 누르면 땅 파기
        if (Input.GetKeyDown(KeyCode.E)) TryCollectTreasure();//E키를 누르면 보물 수집 시도
    }

    void DigGround()//땅 파는 함수
    {
        if (groundTilemap == null) return;//지면 타일맵이 없으면 반환
        if (GameManager.instance.currentBattery <= 0) return;//배터리가 없으면 반환

        Vector3Int playerCellPos = groundTilemap.WorldToCell(transform.position);//플레이어의 월드 위치를 타일맵 셀 위치로 변환
        int range = Mathf.CeilToInt(digRadius);//파는 반경을 정수로 올림해서 범위 계산(소수점으로 하면 타일단위로 못파니까)
        
        bool hasDigSomething = false; // 실제로 타일을 팠는지 체크하는 변수

        for (int x = -range; x <= range; x++)//플레이어 주변의 타일(x좌표)들을 검사하기 위한 루프
        {
            for (int y = -range; y <= range; y++)//플레이어 주변의 타일(y좌표)들을 검사하기 위한 루프
            {
                Vector3Int tilePos = new Vector3Int(playerCellPos.x + x, playerCellPos.y + y, 0);//검사할 타일의 셀 위치 계산
                
                if (groundTilemap.HasTile(tilePos)) //검사할 타일 위치에 지면 타일이 있는지 확인
                {
                    if (Vector2.Distance(transform.position, groundTilemap.GetCellCenterWorld(tilePos)) <= digRadius)//검사할 타일 위치가 플레이어로부터 파는 반경 이내에 있는지 확인
                    {
                        if (safeZoneTilemap != null && safeZoneTilemap.HasTile(tilePos)) continue;//세이프존 타일이 있으면 파지 않음
                        PlayerManager.Instance.PlayerShovel();
                        groundTilemap.SetTile(tilePos, null);//타일을 파서 제거
                        hasDigSomething = true; // 타일을 하나라도 팠다면 true
                    }
                }
            }
        }

        if (hasDigSomething)//실제로 타일을 팠다면 배터리 소모 및 효과 재생새랭생
        {
            GameManager.instance.UseBattery(2.5f);//배터리 소모
            source.GenerateImpulse();

            if (digEffect != null)//파는 효과가 있으면 재생
            {
                ParticleSystem effectInstance = Instantiate(digEffect, transform.position, Quaternion.identity);//플레이어 위치에 파는 효과 생성
                Destroy(effectInstance.gameObject, effectInstance.main.duration + 0.5f);//효과가 끝나면 파는 효과 오브젝트 파괴
            }
        }
    }

    void TryCollectTreasure()//보물 수집 시도하는 함수
    {
        Collider2D[] treasures = Physics2D.OverlapCircleAll(transform.position, collectRadius, treasureLayer);//플레이어 위치를 중심으로 보물 레이어에 해당하는 콜라이더들을 수집 반경 내에서 모두 검사하여 배열로 반환
        if (treasures.Length <= 0) return;//수집할 보물이 없으면 반환

        Collider2D closestTreasure = null;//가장 가까운 보물 콜라이더를 저장할 변수
        float minDistance = Mathf.Infinity;//가장 가까운 보물까지의 최소 거리를 저장할 변수(처음에는 무한대로 설정)

        foreach (Collider2D t in treasures)//수집 반경 내에 있는 모든 보물 콜라이더에 대해 반복
        {
            Vector3Int cellPos = groundTilemap.WorldToCell(t.transform.position);//보물의 월드 위치를 타일맵 셀 위치로 변환하여 보물이 땅 속에 있는지 체크
            if (groundTilemap.HasTile(cellPos)) continue;//보물이 땅 속에 있으면 수집하지 않음

            float dist = Vector2.Distance(transform.position, t.transform.position);//플레이어와 보물 사이의 거리 계산
            if (dist < minDistance)//현재 보물이 지금까지 발견된 가장 가까운 보물보다 더 가까우면
            {
                minDistance = dist;//최소 거리 업데이트
                closestTreasure = t;//가장 가까운 보물 콜라이더 업데이트
            }
        }

        if (closestTreasure != null)//가장 가까운 보물이 있으면
        {
            TreasureDisplay display = closestTreasure.GetComponent<TreasureDisplay>();//가장 가까운 보물에서 TreasureDisplay 컴포넌트 가져오기
            if (display != null && display.myData != null)//TreasureDisplay와 보물 데이터가 있으면
            {
                UIManager uiMgr = Object.FindFirstObjectByType<UIManager>();//씬에 존재하는 UIManager 인스턴스 찾기
                if (uiMgr != null)//UIManager이 있으면
                {
                    uiMgr.ShowTreasureResult(display.myData, closestTreasure.gameObject);//UIManager의 ShowTreasureResult 함수 호출하여 보물 획득 결과 UI 보여주기(보물 데이터와 현재 보물 오브젝트 전달)
                }
            }
        }
    }
}