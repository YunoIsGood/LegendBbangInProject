using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TreasureDetector : MonoBehaviour
{
    [Header("Scan Input")]
    [SerializeField] private KeyCode scanKey = KeyCode.Space; // [SerializeField]는 private 변수도 Inspector에서 조절할 수 있게 해줌
    [SerializeField] private float scanDuration = 1f; // 스캔이 켜져 있는 시간임
    [SerializeField] private float scanCooldown = 1.5f; // 스캔 후 다시 사용할 때까지 기다리는 시간임

    [Header("Scan Range")]
    [SerializeField] private float detectRange = 5f; // 보물을 찾을 수 있는 최대 거리임
    [SerializeField] private string treasureTag = "Treasure"; // 이 태그가 붙은 오브젝트를 보물로 찾음

    [Header("Battery")]
    [SerializeField] private float batteryCostPerScan = 5f; // 스캔 1번에 사용할 배터리 양임

    [Header("References")]
    [SerializeField] private DetectorRadarUI radarUI; // 오른쪽 아래 원형 감지 UI임
    [SerializeField] private ScanWaveEffect scanWaveEffect; // 플레이어 주변에 퍼지는 파장 효과임

    private float nextScanTime;
    private Coroutine scanCoroutine; // Coroutine은 여러 프레임에 걸쳐 실행되는 작업임

    private void Awake()
    {
        CreateMissingReferences();
    }

    private void Update()
    {
        if (Input.GetKeyDown(scanKey))
        {
            TryScan();
        }
    }

    private void TryScan()
    {
        if (Time.time < nextScanTime)
        {
            return;
        }

        if (!HasEnoughBattery())
        {
            if (radarUI != null)
            {
                radarUI.Hide();
            }

            Debug.Log("Battery is too low to use the treasure detector.");
            return;
        }

        nextScanTime = Time.time + scanCooldown;

        if (scanWaveEffect != null)
        {
            scanWaveEffect.Play(transform.position, detectRange, scanDuration);
        }

        if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
        }

        scanCoroutine = StartCoroutine(ScanRoutine()); // StartCoroutine은 Coroutine을 시작하는 함수임
    }

    private IEnumerator ScanRoutine()
    {
        float timer = 0f;

        while (timer < scanDuration)
        {
            UseScanBattery(Time.deltaTime);
            ShowNearestTreasure();
            timer += Time.deltaTime;
            yield return null;
        }

        if (radarUI != null)
        {
            radarUI.FadeOut();
        }

        scanCoroutine = null;
    }

    private void ShowNearestTreasure()
    {
        Transform nearestTreasure = FindNearestTreasure();

        if (nearestTreasure == null)
        {
            if (radarUI != null)
            {
                radarUI.FadeOut();
            }

            return;
        }

        Vector2 toTreasure = nearestTreasure.position - transform.position;
        float distance = Vector2.Distance(transform.position, nearestTreasure.position); // Vector2.Distance는 두 위치 사이의 거리를 구함
        float closeness = 1f - Mathf.Clamp01(distance / detectRange);

        if (radarUI != null)
        {
            radarUI.ShowDirection(toTreasure, closeness);
        }
    }

    private Transform FindNearestTreasure()
    {
        GameObject[] treasures = FindTreasureObjects();
        Transform nearest = null;
        float nearestDistance = Mathf.Infinity;

        for (int i = 0; i < treasures.Length; i++)
        {
            if (treasures[i] == null)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, treasures[i].transform.position); // Vector2.Distance는 플레이어와 보물의 거리를 비교할 때 사용함
            if (distance <= detectRange && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = treasures[i].transform;
            }
        }

        return nearest;
    }

    private GameObject[] FindTreasureObjects()
    {
        try
        {
            return GameObject.FindGameObjectsWithTag(treasureTag);
        }
        catch (UnityException)
        {
            Debug.LogWarning("Treasure tag does not exist. Add a tag named Treasure in Unity.");
            return new GameObject[0];
        }
    }

    private bool HasEnoughBattery()
    {
        if (GameManager.instance == null)
        {
            return true;
        }

        return GameManager.instance.currentBattery >= batteryCostPerScan;
    }

    private void UseScanBattery(float deltaTime)
    {
        if (GameManager.instance == null)
        {
            return;
        }

        float batteryCostThisFrame = (batteryCostPerScan / scanDuration) * deltaTime;
        GameManager.instance.UseBattery(batteryCostThisFrame);
    }

    private void CreateMissingReferences()
    {
        if (radarUI == null)
        {
            radarUI = FindFirstObjectByType<DetectorRadarUI>();
        }

        if (radarUI == null)
        {
            radarUI = CreateRadarUI();
        }

        if (scanWaveEffect == null)
        {
            scanWaveEffect = GetComponentInChildren<ScanWaveEffect>();
        }

        if (scanWaveEffect == null)
        {
            GameObject waveObject = new GameObject("ScanWaveEffect");
            waveObject.transform.SetParent(transform);
            waveObject.transform.localPosition = Vector3.zero;
            scanWaveEffect = waveObject.AddComponent<ScanWaveEffect>();
        }
    }

    private DetectorRadarUI CreateRadarUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject radarObject = new GameObject("DetectorRadarUI");
        radarObject.transform.SetParent(canvas.transform, false);

        RectTransform radarRect = radarObject.AddComponent<RectTransform>();
        radarRect.anchorMin = new Vector2(1f, 0f);
        radarRect.anchorMax = new Vector2(1f, 0f);
        radarRect.pivot = new Vector2(1f, 0f);
        radarRect.anchoredPosition = new Vector2(-40f, 40f);
        radarRect.sizeDelta = new Vector2(350f, 350f);

        CanvasGroup canvasGroup = radarObject.AddComponent<CanvasGroup>();
        Image background = radarObject.AddComponent<Image>();
        background.sprite = CreateCircleSprite(64);
        background.color = new Color(0f, 0f, 0f, 0.35f);

        GameObject sectorObject = new GameObject("DirectionSector");
        sectorObject.transform.SetParent(radarObject.transform, false);

        RectTransform sectorRect = sectorObject.AddComponent<RectTransform>();
        sectorRect.anchorMin = Vector2.zero;
        sectorRect.anchorMax = Vector2.one;
        sectorRect.offsetMin = Vector2.zero;
        sectorRect.offsetMax = Vector2.zero;

        Image sectorImage = sectorObject.AddComponent<Image>();
        sectorImage.sprite = background.sprite;
        sectorImage.color = new Color(0.1f, 0.6f, 1f, 0.75f);

        DetectorRadarUI createdRadar = radarObject.AddComponent<DetectorRadarUI>();
        createdRadar.Setup(canvasGroup, sectorImage);
        return createdRadar;
    }

    private Sprite CreateCircleSprite(int size)
    {
        Texture2D texture = new Texture2D(size, size);
        Color clear = new Color(1f, 1f, 1f, 0f);
        Color white = Color.white;
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.48f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center); // Vector2.Distance는 픽셀과 중심 사이 거리를 구함
                texture.SetPixel(x, y, distance <= radius ? white : clear);
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f)); // Sprite.Create는 Texture2D를 UI Image가 쓸 수 있는 Sprite로 바꿈
    }
}