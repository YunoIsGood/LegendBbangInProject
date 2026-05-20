using UnityEngine;

  
public class SafeZone : MonoBehaviour
//세이프존에 들어갈 스크립트
{

    private void OnTriggerStay2D(Collider2D collider)//세이프존에 닿아있는 동안 계속해서 실행되는 함수

    {

    if (collider.gameObject.CompareTag("Player"))//세이프존에 닿아있는 태그가 Player 라면

    {
        GameManager.instance.isSafeZone = true;//GameManager에 isSafeZone을 true로
    }

    }

    private void OnTriggerExit2D(Collider2D collider)//세이프존에서 나갈 때 실행되는 함수

    {
    if (collider.gameObject.CompareTag("Player"))//세이프존에 닿았다가 나간 태그가 Player라면
    {

        GameManager.instance.isSafeZone = false;//GameManager에 isSafeZone을 false로


    }

    }

	}