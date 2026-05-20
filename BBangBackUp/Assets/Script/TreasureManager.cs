using UnityEngine;
using System.Collections.Generic;

public class TreasureManager : MonoBehaviour
{
    [Header("구역(스테이지) 설정")]
    public List<ZoneData> zones = new List<ZoneData>();//구역(스테이지) 데이터 리스트
    
    [Header("프리팹")]
    public GameObject treasurePrefab;//보물 프리팹
    
    [Header("활성화된 보물 리스트")]
    public List<Transform> activeTreasures = new List<Transform>();//현재 활성화된 보물들의 트랜스폼 리스트

    void Start() 
    { 
        SpawnEverything(); //게임 시작 시 보물과 몬스터 스폰
    }

    public void SpawnEverything()
    {
        if (zones.Count == 0 || treasurePrefab == null) return;

        foreach (var zone in zones)
        {
            for (int i = 0; i < zone.treasureCount; i++)
            {
                Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), Random.Range(zone.minY, zone.maxY), 0);
                GameObject newTreasure = Instantiate(treasurePrefab, spawnPos, Quaternion.identity);

                if (zone.zoneTreasures.Count > 0)
                {
                    // 가중치 기반으로 랜덤 보물 선택
                    TreasureData selectedData = GetRandomTreasure(zone.zoneTreasures);
                    newTreasure.GetComponent<TreasureDisplay>().SetTreasure(selectedData);
                }
                
                activeTreasures.Add(newTreasure.transform);
            }

            for (int i = 0; i < zone.monsterCount; i++)
            {
                if (zone.zoneMonsters.Count == 0) break;
                Vector3 spawnPos = new Vector3(Random.Range(-15f, 15f), Random.Range(zone.minY, zone.maxY), 0);
                Instantiate(zone.zoneMonsters[Random.Range(0, zone.zoneMonsters.Count)], spawnPos, Quaternion.identity);
            }
        }
    }

    // ★ 가중치(weight) 기반 룰렛 휠 선택 함수
    private TreasureData GetRandomTreasure(List<TreasureData> treasureList)
    {
        int totalWeight = 0;

        // 1. 리스트에 있는 모든 보물의 weight를 더해 총합 가중치를 구합니다.
        foreach (TreasureData treasure in treasureList)
        {
            totalWeight += treasure.weight; // ★ weight로 변경
        }

        if (totalWeight <= 0) return treasureList[0];

        // 2. 0부터 총합 가중치 사이의 숫자를 무작위로 뽑습니다.
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        // 3. 가중치를 누적해가며 당첨된 보물을 찾습니다.
        foreach (TreasureData treasure in treasureList)
        {
            currentWeight += treasure.weight; // ★ weight로 변경

            if (randomValue < currentWeight)
            {
                return treasure;
            }
        }

        return treasureList[treasureList.Count - 1];
    }

    public void RemoveTreasureFromList(Transform t)
    {
        if (activeTreasures.Contains(t))
        {
            activeTreasures.Remove(t);
        }
    }
}