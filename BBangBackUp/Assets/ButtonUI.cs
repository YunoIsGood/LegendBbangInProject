using UnityEngine;

public class ButtonUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject TutorialUI;
    public void OnClickButton()
    {
        TutorialUI.SetActive(false);
    }
}
