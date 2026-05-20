using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class KeyboardUI : MonoBehaviour
{
   
    public Image[] tabImages;
    void Start()
    {
    }
    void Update()
    { 
        if(Keyboard.current.wKey.wasPressedThisFrame)
        {
            for (int i = 0; i < tabImages.Length; i++)
            {
                tabImages[i].color = Color.gray;
            }
            tabImages[0].color = Color.white;
        }
        if(Keyboard.current.aKey.wasPressedThisFrame)
        {
            for (int i = 0; i < tabImages.Length; i++)
            {
                tabImages[i].color = Color.gray;
            }
            tabImages[1].color = Color.white;
        }
        if(Keyboard.current.sKey.wasPressedThisFrame)
        {
            for (int i = 0; i < tabImages.Length; i++)
            {
                tabImages[i].color = Color.gray;
            }
            tabImages[2].color = Color.white;
        }
        if(Keyboard.current.dKey.wasPressedThisFrame)
        {
            for (int i = 0; i < tabImages.Length; i++)
            {
                tabImages[i].color = Color.gray;
            }
            tabImages[3].color = Color.white;
        }
        
        
    
    }
}
