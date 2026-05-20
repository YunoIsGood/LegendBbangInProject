using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPromptText;
    [SerializeField] private MonoBehaviour playerMovement;
    [SerializeField] private PlayerAttack playerAttack;
    
    private bool isPlayerInShopRange = false;
    private bool isShopOpen = false;

    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button shopButton;
    [SerializeField] private TextMeshProUGUI messageText;
    private Coroutine messageCoroutine;

    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI batteryText;

    [Header("업그레이드 UI")]
    [SerializeField] private TextMeshProUGUI lightUpgradeText;
    [SerializeField] private TextMeshProUGUI speedUpgradeText;
    [SerializeField] private TextMeshProUGUI attackUpgradeText;
    [SerializeField] private TextMeshProUGUI hpUpgradeText;
    [SerializeField] private TextMeshProUGUI batteryUpgradeText;
    [SerializeField] private TextMeshProUGUI bagUpgradeText;

    // ★ 로컬 변수였던 레벨 및 가격 정보는 전부 GameManager로 이동했으므로 삭제함

    void Start()
    {
        ClearMessageText();
        UpdateAllUI();
        SetShopPanelActive(false);
    }

    private void Update()
    {
        if (GameManager.instance != null)
        {
            if (healthText != null)
                healthText.text = $"{GameManager.instance.currentHealth}/{GameManager.instance.maxHealth}";
            if (batteryText != null)
                batteryText.text = $"{GameManager.instance.currentBattery:F0}/{GameManager.instance.maxBattery}";
        }

        if (isPlayerInShopRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isShopOpen) CloseShop();
            else OpenShop();
        }

        if (isShopOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    public void OpenShop()
    {
        SetShopPanelActive(true);
        isShopOpen = true;
        SetShopPromptActive(false);
        SetPlayerMovementEnabled(false);
        UpdateAllUI();
    }

    public void CloseShop()
    {
        SetShopPanelActive(false);
        isShopOpen = false;
        if (isPlayerInShopRange) SetShopPromptActive(true);
        SetPlayerMovementEnabled(true);
    }

    private void SetShopPromptActive(bool active) { if (shopPromptText != null) shopPromptText.SetActive(active); }
    private void SetShopPanelActive(bool active) { if (shopPanel != null) shopPanel.SetActive(active); }
    private void SetPlayerMovementEnabled(bool enabled) { if (playerMovement != null) playerMovement.enabled = enabled; }

    private void UpdateAllUI()
    {
        if (GameManager.instance == null) return;

        // ★ GameManager.instance 를 통해 변수 접근하도록 모두 변경
        if (lightUpgradeText != null)
            lightUpgradeText.text = $"시야 범위: {GameManager.instance.baseInnerRadius:F1}\n레벨: {GameManager.instance.lightUpgradeLevel}\n{GameManager.instance.lightUpgradeGold}G";

        if (speedUpgradeText != null)
            speedUpgradeText.text = $"스피드: {GameManager.instance.currentMoveSpeed:F1}\n레벨: {GameManager.instance.speedUpgradeLevel}\n{GameManager.instance.speedUpgradeGold}G";

        if (attackUpgradeText != null)
            attackUpgradeText.text = $"공격력: {GameManager.instance.currentAttackDamage}\n레벨: {GameManager.instance.attackUpgradeLevel}\n{GameManager.instance.attackUpgradeGold}G";

        if (hpUpgradeText != null)
            hpUpgradeText.text = $"체력: {GameManager.instance.maxHealth}\n레벨: {GameManager.instance.hpUpgradeLevel}\n{GameManager.instance.hpUpgradeGold}G";

        if (batteryUpgradeText != null)
            batteryUpgradeText.text = $"배터리 용량: {GameManager.instance.maxBattery}\n레벨: {GameManager.instance.batteryUpgradeLevel}\n{GameManager.instance.batteryUpgradeGold}G";

        if (bagUpgradeText != null)
            bagUpgradeText.text = $"가방 용량: {GameManager.instance.maxInventorySlots}\n레벨: {GameManager.instance.bagUpgradeLevel}\n{GameManager.instance.bagUpgradeGold}G";
    }

    // ====== 업그레이드 적용 함수들 (전부 GameManager 변수 사용) ======

    public void BagUpgrade()
    {
        if (!TrySpendGold(GameManager.instance.bagUpgradeGold)) return;
        
        GameManager.instance.bagUpgradeLevel += 1;
        GameManager.instance.maxInventorySlots += 2;
        GameManager.instance.bagUpgradeGold += 100;
        
        UpdateAllUI();
        UpdateInventoryUIIfNeeded();
    }

    public void HPUpgrade()
    {
        if (!TrySpendGold(GameManager.instance.hpUpgradeGold)) return;
        
        GameManager.instance.hpUpgradeLevel += 1;
        GameManager.instance.maxHealth += 10;
        GameManager.instance.hpUpgradeGold += 30;
        
        UpdateAllUI();
    }

    public void BatteryUpgrade()
    {
        if (!TrySpendGold(GameManager.instance.batteryUpgradeGold)) return;

        GameManager.instance.batteryUpgradeLevel += 1;
        GameManager.instance.maxBattery += 10;
        GameManager.instance.batteryUpgradeGold += 30;
        
        UpdateAllUI();
    }

    public void LightUpgrade()
    {
        if (!TrySpendGold(GameManager.instance.lightUpgradeGold)) return;

        GameManager.instance.lightUpgradeLevel += 1;
        GameManager.instance.baseInnerRadius += 1.15f;
        GameManager.instance.baseOuterRadius += 1.15f;
        GameManager.instance.lightUpgradeGold += 20;

        UpdateAllUI();
    }

    public void SpeedUpgrade()
    {
        if (!TrySpendGold(GameManager.instance.speedUpgradeGold)) return;

        GameManager.instance.speedUpgradeLevel += 1;
        GameManager.instance.speedUpgradeGold += 20;
        GameManager.instance.currentMoveSpeed += 1f;

        // 현재 필드에 있는 플레이어에게도 즉시 적용
        var pManager = playerMovement.GetComponent("PlayerManager");
        if (pManager != null)
        {
            var field = pManager.GetType().GetField("moveSpeed");
            if (field != null) field.SetValue(pManager, GameManager.instance.currentMoveSpeed);
        }
        
        UpdateAllUI();
    }

    public void AttackUpgrade()
    {
        if (!TrySpendGold(GameManager.instance.attackUpgradeGold)) return;

        GameManager.instance.attackUpgradeLevel += 1;
        GameManager.instance.attackUpgradeGold += 20;
        GameManager.instance.currentAttackDamage += 2;

        if (playerAttack != null)
        {
            playerAttack.playerDamage = GameManager.instance.currentAttackDamage;
        }
        
        UpdateAllUI();
    }

    // =========================================================

    private bool TrySpendGold(int price)
    {
        if (GameManager.instance == null) return false;

        if (GameManager.instance.money < price)
        {
            ShowMessage("돈이 부족합니다");
            return false;
        }

        ClearRunningMessage();
        GameManager.instance.money -= price;
        return true;
    }

    private void ShowMessage(string msg)
    {
        if (messageText == null) return;
        ClearRunningMessage();
        messageCoroutine = StartCoroutine(ShowMessageForSeconds(msg, 2f));
    }

    private IEnumerator ShowMessageForSeconds(string message, float seconds)
    {
        messageText.text = message;
        yield return new WaitForSeconds(seconds);
        ClearMessageText();
        messageCoroutine = null;
    }

    private void ClearMessageText() { if (messageText != null) messageText.text = ""; }
    private void ClearRunningMessage()
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }
        ClearMessageText();
    }

    private void UpdateInventoryUIIfNeeded()
    {
        UIManager ui = Object.FindFirstObjectByType<UIManager>();
        if (ui != null) ui.UpdateInventoryUI();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInShopRange = true;
            SetShopPromptActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInShopRange = false;
            SetShopPromptActive(false);
            if (isShopOpen) CloseShop();
        }
    }
}