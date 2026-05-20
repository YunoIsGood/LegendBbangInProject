using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 추가: SceneManager 사용을 위함
using DG.Tweening;                // 추가: DOFade(DOTween) 사용을 위함

public class UIManager : MonoBehaviour
{
    [Header("기본 상태 UI")]
    public TextMeshProUGUI moneyText;   // 돈 텍스트
    public Image hpBar;                 // 체력 바 이미지
    public Image batteryBar;            // 배터리 바 이미지

    [Header("게임 오버 설정")]
    [SerializeField] private GameObject sceneGameOverPanel; // 씬 내 게임오버 패널
    [SerializeField] private Button restartButton;          // 게임오버 패널 내 재시작 버튼

    [Header("보물 획득 UI")]
    [SerializeField] private GameObject resultPanel;        // 보물 획득 결과 패널
    [SerializeField] private Image resultImage;             // 보물 아이콘 이미지
    [SerializeField] private TextMeshProUGUI resultNameText; // 보물 이름 텍스트
    [SerializeField] private TextMeshProUGUI resultPriceText;// 보물 가격 텍스트
    [SerializeField] private Button keepButton;             // 보물 획득 후 "Keep" 버튼
    [SerializeField] private TextMeshProUGUI warningText;   // 인벤토리 가득 찼을 때 경고 텍스트

    [Header("연출 설정")]
    [SerializeField] private Animator uiChestAnimator;       // UI 내 보물상자 애니메이터
    [SerializeField] private GameObject rewardContent;       // 아이템 정보 + 버튼들을 묶은 부모 오브젝트
    [SerializeField] private float animationDuration = 1.0f; // 애니메이션이 재생되는 시간
    [SerializeField] private GameObject questUi;             // 퀘스트 UI

    [Header("UI 모음 (ESC / 세팅)")]
    [SerializeField] private GameObject escUIPanel;
    [SerializeField] private GameObject settingUIPanel;

    [Header("페이드 아웃")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeTime = 1.5f;

    private TreasureData currentFoundData;      // 현재 발견한 보물 데이터
    private GameObject currentWorldTreasure;    // 현재 발견한 보물의 월드 오브젝트 참조
    private TreasureManager treasureManager;    // 보물 관리 매니저 참조

    public static UIManager instance;

    void Awake()
    {
        instance = this;
        // 씬 내에서 TreasureManager 인스턴스 찾아서 참조 저장
        treasureManager = Object.FindFirstObjectByType<TreasureManager>();
        
        // ESC 패널 초기 비활성화
        if (escUIPanel != null) escUIPanel.SetActive(false);

        fadeImage.gameObject.SetActive(true);
    }

    void Start() 
    {
        // 게임매니저 초기 설정 연결
        if (GameManager.instance != null)
        {
            GameManager.instance.GameOverPanel = sceneGameOverPanel;

            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners(); 
                restartButton.onClick.AddListener(GameManager.instance.GameOver); 
            }
        }

        // 각 UI 패널 초기 상태 설정
        if (resultPanel != null) resultPanel.SetActive(false);
        if (questUi != null) questUi.SetActive(false);
        
        UpdateInventoryUI();

        // 페이드 인 효과 연출 시작 (검은 화면 -> 투명)
        if (fadeImage != null)
        {
             ImageFadeIn();
        }
    }

    void Update() 
    {
        if (GameManager.instance == null) return;

        // 1. 상시 스탯 UI 업데이트
        moneyText.text = GameManager.instance.money.ToString();
        hpBar.fillAmount = (float)GameManager.instance.currentHealth / GameManager.instance.maxHealth;
        batteryBar.fillAmount = (float)GameManager.instance.currentBattery / GameManager.instance.maxBattery;

        // 2. 탭(Tab) 키 입력 처리: 퀘스트 UI 토글
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            bool isActive = questUi.gameObject.activeSelf;
            questUi.SetActive(!isActive);

            // 퀘스트 UI가 켜지면 일시정지(0), 꺼지면 원래대로(1)
            Time.timeScale = isActive ? 1.0f : 0.0f;
        }

        // 3. ESC 키 입력 처리: 메뉴 UI 토글
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool isActive = escUIPanel.activeSelf;
            escUIPanel.SetActive(!isActive);

            Time.timeScale = isActive ? 1f : 0f;
            
            // ESC 메뉴를 닫을 때 세팅 패널도 함께 닫히도록 예외 처리
            if (isActive == false)
            {
                if (settingUIPanel != null) settingUIPanel.SetActive(false);
            }
        }

    }

    private void FadeEnd()
    {
        if (fadeImage != null) fadeImage.raycastTarget = false;
    }

    #region 보물 시스템 관련 메서드

    public void UpdateInventoryUI()
    {
        if (warningText == null) return;

        int currentCount = GameManager.instance.myInventory.Count;
        int maxCount = GameManager.instance.maxInventorySlots;

        if (currentCount < maxCount) 
        {
            keepButton.interactable = true;
            warningText.text = $"{currentCount}/{maxCount}";
        } 
        else 
        {
            keepButton.interactable = false;
            warningText.text = "<color=red>가방이 가득 찼습니다!</color>";
        }
    }

    public void ShowTreasureResult(TreasureData data, GameObject worldObj) 
    {
        currentFoundData = data;
        currentWorldTreasure = worldObj;

        resultNameText.text = data.treasureName;
        resultPriceText.text = $"가격: {data.price}G";
        resultImage.sprite = data.treasureIcon;

        UpdateInventoryUI();

        if (uiChestAnimator != null)
        {
            uiChestAnimator.gameObject.SetActive(true);
            uiChestAnimator.SetTrigger("UIOpen");
        }

        resultPanel.SetActive(true);
        rewardContent.SetActive(false); 

        Time.timeScale = 0f; 
        StartCoroutine(RevealRewardRoutine());
    }

    private IEnumerator RevealRewardRoutine()
    {
        yield return new WaitForSecondsRealtime(animationDuration);
        
        if (uiChestAnimator != null)
        {
            uiChestAnimator.gameObject.SetActive(false);
        }

        rewardContent.SetActive(true);
        SoundManager.instance.PlaySFX("Tada");
    }

    public void ClickKeep() 
    {
        if (currentFoundData != null && GameManager.instance.CanAddItem())
        {
            GameManager.instance.AddToInventory(currentFoundData);
            UpdateInventoryUI();
            RemoveTreasureFromWorld();
        }
        CloseResult();
    }

    public void ClickDiscard() 
    {
        RemoveTreasureFromWorld();
        CloseResult();
    }

    private void RemoveTreasureFromWorld()
    {
        if (currentWorldTreasure != null)
        {
            if (treasureManager != null)
                treasureManager.RemoveTreasureFromList(currentWorldTreasure.transform);
            
            Destroy(currentWorldTreasure);
        }
    }

    void CloseResult()
    {
        resultPanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    public void SellAllTreasures() 
    {   
        var inventory = GameManager.instance.myInventory;
        if (inventory.Count <= 0) return;
        
        int totalProfit = 0;
        foreach (var item in inventory) 
        {
            totalProfit += item.price;
        }

        GameManager.instance.AddMoney(totalProfit);
        inventory.Clear();
        UpdateInventoryUI();

        Object.FindFirstObjectByType<InventoryManager>()?.SendMessage("RefreshInventory", SendMessageOptions.DontRequireReceiver);
    }

    #endregion

    #region ESC 버튼 및 세팅 UI 관련 메서드

    public void OnClickPlay()
    {
        if (escUIPanel != null) escUIPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickSetting()
    {
        if (settingUIPanel != null) settingUIPanel.SetActive(true);
    }

    public void OnSettingExit()
    {
        if (settingUIPanel != null) settingUIPanel.SetActive(false);
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
    }

    public void OnClickExit()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("HomeScene");
    }

    public void OnClickScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Debug.Log("화면 전환");
    }

    // GameManager에서 페이드 타임을 참조하기 위한 함수 추가
    public float GetFadeTime()
    {
        return fadeTime;
    }

    // 게임 시작 및 씬 로드 후 실행: 검은 화면 -> 투명하게 밝아짐
    public void ImageFadeIn()
    {
        if (fadeImage == null) return;

        fadeImage.DOKill(); // 기존 트윈 제거로 꼬임 방지

        fadeImage.color = new Color(0, 0, 0, 1); // 완전 검은색 상태 설정
        fadeImage.raycastTarget = true;

        fadeImage
            .DOFade(0f, fadeTime)
            .SetEase(Ease.InOutQuad)
            .OnComplete(FadeEnd);
    }

    // 침대 취침 등 실행: 투명한 화면 -> 검은색으로 어두워짐
    public void ImageFadeOut()
    {
        if (fadeImage == null) return;

        fadeImage.DOKill(); // 기존 트윈 제거로 꼬임 방지

        fadeImage.raycastTarget = true; 

        fadeImage
            .DOFade(1f, fadeTime)
            .SetEase(Ease.InOutQuad);
    }

    #endregion
}