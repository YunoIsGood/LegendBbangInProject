using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCDiggingAnim : MonoBehaviour
{
    [SerializeField] private Animator anim;
    
    [Header("Related GameObjects")]
    [SerializeField] private GameObject dialogue;
    [SerializeField] private GameObject dialogue2;
    [SerializeField] private GameObject lastDialogue;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject ground;
    
    [SerializeField] private bool lastDialogueActive;
    [SerializeField] private float cooltime = 1f;
    private bool _canClickState;
    

    public void OnEnable() //활성화됐을때 이것들도 활성화될수있게(오류 방지를 위한...)
    {
            dialogue.SetActive(true);   
            dialogue2.SetActive(true);
            lastDialogue.SetActive(false);
            button.SetActive(false);
            lastDialogueActive = false;
            _canClickState = true;

    }

    public void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame && _canClickState && dialogue2.activeSelf == false)
        {
            anim.SetBool("canDig", true);
            StartCoroutine(CoolTimeCoroutine());
            _canClickState = false;
        }

        if (lastDialogueActive)
        {
            button.SetActive(true);          
        }
        
        
    }

    IEnumerator CoolTimeCoroutine()
    {
        yield return new WaitForSeconds(cooltime);
        OnClickStopDig();
    }
    public void OnClickStopDig()
    {
        anim.SetBool("canDig", false);
        StartCoroutine(CoolDialogueCoroutine());
        
    }

    IEnumerator CoolDialogueCoroutine()
    {
        yield return new WaitForSeconds(cooltime);
        gameObject.SetActive(false);
        ground.SetActive(false);
        lastDialogue.SetActive(true);
        lastDialogueActive = true;
        
    }
    
}
