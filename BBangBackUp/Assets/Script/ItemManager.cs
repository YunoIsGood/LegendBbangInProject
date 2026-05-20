using UnityEngine;
using UnityEngine.InputSystem;

public class ItemManager : MonoBehaviour
{


  public int itemcoin = 3;//힐 아이템 개수
 public int healitem = 50;//힐량
 private int nowHeal = 0;//얼마 줘야하는지 저장소
 


 public int itemcoin2 = 3;         // 딜 아이템 개수
 public float timeFloating = 120f;//딜템 시간
 public int dmg; 
 public int dmgUp = 10;       //데미지
 private float endTime = 0f;      // 종료 시간 저장 (초기값 -1)
 private bool isBuffActive = false; // 버프 활성화 상태 스위치


 private int itemcoin3 = 1;              //스피드 아이템 개수
 public float timeFloating_Speed = 120f; // 스업 지속시간
 public float speedUp = 10f;             //스업 속도
 private float endTime_Speed = 0f;
 private bool isBuffActiveSpeed = false;//버프 활성화 상태 스위치



    

    void Update()
 {


     //힐
     if (Keyboard.current.digit1Key.wasPressedThisFrame)
     {
         if (GameManager.instance.currentHealth >= GameManager.instance.maxHealth)
         {
             Debug.Log("체력이 가득합니다");
         }

         else if (itemcoin <= 0)
         {
             Debug.Log("아이템이 부족합니다!");
         }

         else if (GameManager.instance.currentHealth + healitem <= GameManager.instance.maxHealth)
         {
             GameManager.instance.currentHealth += healitem;
             Debug.Log($"플레이어 체력 {healitem} 회복, 현재 체력 {GameManager.instance.currentHealth}");
             itemcoin--;
         }

         else
         {
             nowHeal = GameManager.instance.maxHealth - GameManager.instance.currentHealth;
             GameManager.instance.currentHealth = GameManager.instance.maxHealth;
             Debug.Log($"플레이어 체력 {nowHeal} 회복, 현재 체력 {GameManager.instance.currentHealth}");
             itemcoin--;
         }
     }





     //공업
     if (isBuffActive == true && itemcoin2 > 0)//공업 종료
     {
         endTime += Time.deltaTime;
         if (endTime > timeFloating)
         {
             isBuffActive = false;
             Debug.Log("공업 종료");
             endTime = 0f;
             dmg -= dmgUp;
             PlayerAttack.instance.playerDamage = dmg;

         }
     }

     if (Keyboard.current.digit2Key.wasPressedThisFrame)//공업 시작
     {
         if (itemcoin2 > 0)
         {
             itemcoin2--;
             isBuffActive = true;
             Debug.Log("공격력 업!");
             dmg += dmgUp;
             PlayerAttack.instance.playerDamage = dmg;

         }
         else
         {
             Debug.Log("아이템이 부족합니다");
         }


     }
     //스피드 업
     if (Keyboard.current.digit3Key.wasPressedThisFrame)
     {
         if (itemcoin3 > 0)
         {
             itemcoin3--;
             isBuffActiveSpeed = true;
             PlayerManager.Instance.moveSpeed += speedUp;
             Debug.Log("스피드 업");

         }
         else
             Debug.Log("아이템이 부족합니다");

     }
     if (isBuffActiveSpeed == true)
     {

         endTime_Speed += Time.deltaTime;
         if (endTime_Speed > timeFloating_Speed)
         {
             isBuffActiveSpeed = false;
             Debug.Log("스피드업 종료");
             endTime_Speed = 0f;
             PlayerManager.Instance.moveSpeed -= speedUp;
         }
     }



 }
}
