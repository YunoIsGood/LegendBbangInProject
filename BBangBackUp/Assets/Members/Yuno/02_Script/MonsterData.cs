using UnityEngine;
//몬스터 데이터 저장하는 스크립터블 오브젝트
[CreateAssetMenu(fileName = "NewMonsterData", menuName = "ScriptableObjects/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("Stat Settings")]
    public string monsterName;//몬스터 이름
    public int maxHp;//최대 체력
    public float speed = 5f;//이동 속도
    public int damage;//공격력
    public float attackCooldown = 1.0f;//공격 쿨타임

    [Header("Visual Settings")]
    public GameObject modelPrefab; // 필요한 경우 외형도 SO에서 관리
}