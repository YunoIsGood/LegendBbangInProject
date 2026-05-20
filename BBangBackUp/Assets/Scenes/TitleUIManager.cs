using UnityEngine;
using DG.Tweening;

public class TitleUIIntro : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private RectTransform logo;
    [SerializeField] private RectTransform startButton;
    [SerializeField] private RectTransform exitButton;
    [SerializeField] private RectTransform canvasRect;

    [Header("연출 설정")]
    [SerializeField] private float moveDuration = 0.35f;   // 들어오는 속도
    //[SerializeField] private float delayBetween = 0.12f;   // 다음 UI까지 간격
    [SerializeField] private float outsideOffset = 300f;   // 캔버스 밖으로 얼마나 뺄지

    private Vector2 logoTargetPos;
    private Vector2 startTargetPos;
    private Vector2 exitTargetPos;

    private void Start()
    {
        // 원래 위치 저장
        Time.timeScale = 1;
        logoTargetPos = logo.anchoredPosition;
        startTargetPos = startButton.anchoredPosition;
        exitTargetPos = exitButton.anchoredPosition;

        // 캔버스 왼쪽 밖 시작 위치로 이동
        float startX = -canvasRect.rect.width - outsideOffset;

        logo.anchoredPosition = new Vector2(startX, logoTargetPos.y);
        startButton.anchoredPosition = new Vector2(startX, startTargetPos.y);
        exitButton.anchoredPosition = new Vector2(startX, exitTargetPos.y);

        // 순서대로 등장
        Sequence seq = DOTween.Sequence();
        seq.SetDelay(0.1f);
        seq.Append(logo.DOAnchorPos(logoTargetPos, moveDuration).SetEase(Ease.OutCubic));
        seq.Append(startButton.DOAnchorPos(startTargetPos, moveDuration).SetEase(Ease.OutCubic));
        seq.Append(exitButton.DOAnchorPos(exitTargetPos, moveDuration).SetEase(Ease.OutCubic));
    }
}
