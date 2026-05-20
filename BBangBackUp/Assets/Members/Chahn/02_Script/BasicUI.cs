using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicUI : MonoBehaviour
{
    public GameObject tutorialUI;
    
    void Start()
    {
        tutorialUI.SetActive(false);
    }
    public void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            tutorialUI.SetActive(true);
        }
    }

}
