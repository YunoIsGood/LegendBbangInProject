using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    public Slider batterySlider;//배터리 UI 슬라이더

    void Start()
    {
        if (GameManager.instance != null)//GameManager 인스턴스가 있으면
        {
            batterySlider.maxValue = GameManager.instance.maxBattery;//배터리슬라이더 최대값은 GameManager의 최대 배터리값으로
            batterySlider.value = GameManager.instance.currentBattery;//배터리슬라이더 현재값은 GameManager의 현재 배터리값으로
        }
    }

    void Update()
    {
        if (GameManager.instance == null || batterySlider == null)
            return;

        batterySlider.maxValue = GameManager.instance.maxBattery;
        batterySlider.value = GameManager.instance.currentBattery;
    }
}