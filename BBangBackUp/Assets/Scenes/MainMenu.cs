using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float waitTime = 0.2f;

    private bool isChangingScene = false;

    private void Start()
    {
        // 처음에는 검은 화면이 안 보이게 투명하게 만들기
        fadeImage.color = new Color(0, 0, 0, 0);

        // 처음에는 버튼 클릭을 막으면 안 됨
        fadeImage.raycastTarget = false;
    }

    public void StartGame()
    {
        if (isChangingScene) return;
        isChangingScene = true;

        // 페이드 중에는 버튼 여러 번 못 누르게 막기
        fadeImage.raycastTarget = true;

        // 화면을 서서히 검게 만들기
        fadeImage
            .DOFade(1f, fadeTime)//fadeImage의 a(투명도)값을 fadeTime초에 거쳐 1로 바꿔라

            .SetEase(Ease.InOutQuad)//페이드 속도의 느낌을 정함(처음에는 살짝 천천히/중간에는 빠르게/끝에는 다시 천천히)
              //안쓰면 조금 딱딱하게 기계적으로 변함(.SetEase(Ease.Linear)이거는 일정한 속도)
            .OnComplete(WaitAndLoadScene);
    }

    private void WaitAndLoadScene()
    {
        DOVirtual.DelayedCall(waitTime, LoadMainScene);
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene("Main");
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }


}