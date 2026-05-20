using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel; // 가방 패널
    public GameObject slotPrefab;     // 아까 만든 슬롯 프리팹
    public Transform content;         // Grid Layout Group이 있는 곳

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))//I누르면 인벤토리 오픈
        {
            bool isActive = inventoryPanel.activeSelf;//현재 켜져 있는지 확인
            inventoryPanel.SetActive(!isActive);//반대로 바꿔줌 (켜져 있으면 끄고 꺼져 있으면 켬)

            if (!isActive)//가방을 연 상태라면
            {
                RefreshInventory();//인벤토리 내용물 새로고침
            }
        }
    }

    void RefreshInventory()//인벤토리 내용물 새로고침하는 함수(보물 팔았을때)
    {
        foreach (Transform child in content)//인벤토리로 들어가는 슬롯들 훑어서
        {
            Destroy(child.gameObject);//인벤토리 모두 삭제
        }

        foreach (TreasureData data in GameManager.instance.myInventory)//GameManager 안에 있는 인벤토리를 훑고 그에 맞게 인벤토리 수정
        {
            GameObject newSlot = Instantiate(slotPrefab, content);//슬롯 프리팹 생성
            newSlot.GetComponent<InventorySlot>().SetItem(data);//슬롯에 아이템 정보 전달
        }
    }
}