using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HeadLightController : MonoBehaviour
{
    public Light2D headLight;//헤드라이트

    void Update()//매 프레임마다 헤드라이트 상태 업데이트
    {
        if (GameManager.instance == null || headLight == null)//GameManager 인스턴스나 헤드라이트가 없으면 반환
            return;

        headLight.enabled = GameManager.instance.currentBattery > 0f;//배터리가 0보다 크면 키고 아니면끔
    }
}