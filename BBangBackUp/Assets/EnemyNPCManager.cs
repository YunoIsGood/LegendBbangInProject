using UnityEngine;

public class EnemyNPCManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private GameObject enemyNPC;
    [SerializeField] private GameObject playerNPC;
    [SerializeField] private GameObject lastDialogueUI;
    [SerializeField] private GameObject buttonUI;
    [SerializeField] private bool enableState;
    [SerializeField] private bool enableState2;
    [SerializeField] private bool enableState3;
    void OnEnable()
    {
        dialogueUI.SetActive(true); //dialogueUI가 이 오브젝트가 활성화 되면 같이 활성화된다
        playerNPC.SetActive(true);
        lastDialogueUI.SetActive(false); 
        enemyNPC.SetActive(false);
        buttonUI.SetActive(false);
        enableState = false;
        enableState2 = false;
        enableState3 = false;
    }

    void Update()
    {
        if (dialogueUI.activeSelf == false) //계속 활성화되면 안돼서 방지 코드
        {

            if (!enableState)
            {
                enemyNPC.SetActive(true);
                enableState = true;
            }
        }

        if (playerNPC.activeSelf == false) //계속 활성화되면 안돼서 방지 코드
        {
            if (!enableState2)
            {
                lastDialogueUI.SetActive(true); //활성화
                enableState2 = true;
            }
        }

        if (lastDialogueUI.activeSelf == false && enableState2)
        {
            if (!enableState3)
            {
                buttonUI.SetActive(true);
                enableState3 = true;
            }
        }
    }
}
