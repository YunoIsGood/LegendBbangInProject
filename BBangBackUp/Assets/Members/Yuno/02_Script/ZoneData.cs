using UnityEngine;
using System.Collections.Generic;
//존(스테이지) 데이터 관리하는 스크립터블오브젝트트트트트트트틑
[CreateAssetMenu(fileName = "New Zone Data", menuName = "ScriptableObjects/ZoneData")]
public class ZoneData : ScriptableObject
{
    public string zoneName;//존 이름
    public float minY;//존의 최소 Y값
    public float maxY;//존의 최대 Y값
    
    [Header("스폰 설정")]
    public List<TreasureData> zoneTreasures; // 이 구역 전용 보물
    public List<GameObject> zoneMonsters;    // 이 구역 전용 몬스터
    public int treasureCount = 10; // 이 구역에서 스폰할 보물 개수
    public int monsterCount = 5; // 이 구역에서 스폰할 몬스터 개수
}