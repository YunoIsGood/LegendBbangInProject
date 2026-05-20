using UnityEngine;
using UnityEngine.UI;

public class DetectorRadarUI : MonoBehaviour
{
    [Header("UI Images")]
    [SerializeField] private CanvasGroup canvasGroup; // UI 전체의 투명도를 조절함
    [SerializeField] private Image sectorImage; // 파란색 부채꼴 영역으로 사용할 이미지임

    [Header("Sector Look")]
    [SerializeField] private float sectorAngle = 85f; // 부채꼴이 얼마나 넓게 보일지 정함
    [SerializeField] private Color farColor = new Color(0.1f, 0.45f, 1f, 0.35f);
    [SerializeField] private Color nearColor = new Color(0.0f, 0.9f, 1f, 0.9f);
    [SerializeField] private float angleOffset = 90f; // UI 이미지의 기본 방향이 어긋나면 이 값을 조절하면됨

    [Header("Fade")]
    [SerializeField] private float fadeSpeed = 5f;

    private float targetAlpha;

    public void Setup(CanvasGroup newCanvasGroup, Image newSectorImage)
    {
        canvasGroup = newCanvasGroup;
        sectorImage = newSectorImage;
        PrepareSectorImage();
        Hide();
    }

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        PrepareSectorImage();
        Hide();
    }

    private void Update()
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed); // Lerp는 현재 값에서 목표 값으로 부드럽게 이동시킴
        bool shouldShow = canvasGroup.alpha > 0.01f;
        if (gameObject.activeSelf != shouldShow)
        {
            gameObject.SetActive(shouldShow); // SetActive는 오브젝트를 켜거나 끄는 메서드임
        }
    }

    public void ShowDirection(Vector2 direction, float closeness)
    {
        if (sectorImage == null)
        {
            return;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true); // SetActive(true)는 꺼져 있던 UI를 다시 보이게함
        }

        targetAlpha = 1f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Mathf.Atan2는 방향 벡터를 각도로 바꿔줌
        sectorImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle + angleOffset - (sectorAngle * 0.5f)); // Quaternion.Euler는 각도 값을 회전값으로 바꿔줌

        float safeCloseness = Mathf.Clamp01(closeness);
        sectorImage.color = Color.Lerp(farColor, nearColor, safeCloseness); // Lerp로 멀 때 색과 가까울 때 색을 섞는다
    }

    public void FadeOut()
    {
        targetAlpha = 0f;
    }

    public void Hide()
    {
        targetAlpha = 0f;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        gameObject.SetActive(false); // SetActive(false)는 UI를 화면에서 숨긴다
    }

    private void PrepareSectorImage()
    {
        if (sectorImage == null)
        {
            return;
        }

        sectorImage.type = Image.Type.Filled;
        sectorImage.fillMethod = Image.FillMethod.Radial360;
        sectorImage.fillOrigin = (int)Image.Origin360.Right;
        sectorImage.fillClockwise = true;
        sectorImage.fillAmount = sectorAngle / 360f;
    }
}