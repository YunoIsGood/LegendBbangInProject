using System.Collections;
using UnityEngine;

public class ScanWaveEffect : MonoBehaviour
{
    [Header("Wave Look")]
    [SerializeField] private Color startColor = new Color(0.2f, 0.8f, 1f, 0.65f);
    [SerializeField] private Color endColor = new Color(0.2f, 0.8f, 1f, 0f);
    [SerializeField] private float lineWidth = 0.08f;
    [SerializeField] private int circlePointCount = 80;

    private LineRenderer lineRenderer;
    private Coroutine waveCoroutine; // Coroutine은 1초 동안 크기와 투명도를 천천히 바꿀 때 사용함

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = circlePointCount;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        DrawCircle(0.1f);
        lineRenderer.enabled = false;
    }

    public void Play(Vector3 centerPosition, float radius, float duration)
    {
        transform.position = centerPosition;

        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
        }

        waveCoroutine = StartCoroutine(WaveRoutine(radius, duration)); // StartCoroutine은 Coroutine을 실행함
    }

    private IEnumerator WaveRoutine(float radius, float duration)
    {
        float timer = 0f;
        lineRenderer.enabled = true;

        while (timer < duration)
        {
            float progress = timer / duration;
            float currentRadius = Mathf.Lerp(0.1f, radius, progress); // Lerp는 작은 값에서 큰 값으로 부드럽게 바꿈
            DrawCircle(currentRadius);
            lineRenderer.startColor = Color.Lerp(startColor, endColor, progress); // Lerp로 색을 점점 투명하게 만듬
            lineRenderer.endColor = Color.Lerp(startColor, endColor, progress); // Lerp로 선 끝 색도 같이 바꿈

            timer += Time.deltaTime;
            yield return null;
        }

        lineRenderer.enabled = false;
        waveCoroutine = null;
    }

    private void DrawCircle(float radius)
    {
        for (int i = 0; i < circlePointCount; i++)
        {
            float angle = ((float)i / circlePointCount) * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}