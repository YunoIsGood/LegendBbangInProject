using UnityEngine;

//보물 데이터
[CreateAssetMenu(fileName = "New Treasure", menuName = "ScriptableObjects/TreasureData")]
public class TreasureData : ScriptableObject
{
    public string treasureName; // 보물 이름
    public Sprite treasureIcon; // 보물 이미지
    public int price;           // 보물 가격
    [Header("스폰 가중치 (높을수록 잘 나옴)")]
    public int weight = 10;     
}