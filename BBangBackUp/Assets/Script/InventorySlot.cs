using UnityEngine;
using UnityEngine.UI;

public partial class InventorySlot : MonoBehaviour//보물 먹을때마다 인벤토리에 이 스크립트 있는 인벤토리슬롯 프리팹이 생성됨
{
    public Image icon; // 보물 아이콘 연결

    public void SetItem(TreasureData data)//인벤토리 슬롯에 보물 데이터 설정하는 함수
    {
        if (data == null)//데이터 없으면
        {
            icon.enabled = false; // 데이터 없으면 아이콘 숨기기
            return;
        }

        icon.enabled = true;//아이콘 켜기
        icon.sprite = data.treasureIcon; // 보물 이미지 입히기
    }
}