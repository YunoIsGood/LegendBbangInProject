using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealitemWonyoung : MonoBehaviour
{
    public int itemcoin = 3;
    public int healitem = 50;

    private void Update()
    {
        if(Keyboard.current. digit1Key .wasPressedThisFrame)
        {
            if(itemcoin > 0)
            {
                itemcoin--;
                Debug.Log($"플레이어 체력 {healitem} 회복");
            }
            else
            {
                Debug.Log($"아이템이 부족합니다!");
            }
        }
    }
}

