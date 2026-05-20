using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering; 
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Mathematics;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤 패턴
    public Bed bed;
    public bool isDead = false;

    [Header("플레이어 능력치")]
    public float currentBattery = 100f; 
    public float maxBattery = 100f;     
    public int currentHealth = 100;     
    public int maxHealth = 100;         
    public int money = 0;               

    [Header("업그레이드 보존 데이터")]
    public int lightUpgradeLevel = 0;
    public int lightUpgradeGold = 10;

    public int speedUpgradeLevel = 0;
    public int speedUpgradeGold = 10;
    public float currentMoveSpeed = 5f; 

    public int attackUpgradeLevel = 0;
    public int attackUpgradeGold = 10;
    public int currentAttackDamage = 10; 

    public int hpUpgradeLevel = 0;
    public int hpUpgradeGold = 50;

    public int batteryUpgradeLevel = 0;
    public int batteryUpgradeGold = 50;

    public int bagUpgradeLevel = 0;
    public int bagUpgradeGold = 80;

    [Header("세이프존")]
    public bool isSafeZone = false;     

    [Header("헤드라이트")]
    public Light2D headLight;           
    public bool isBatteryOff = false;   
    public float baseInnerRadius = 5f;  
    public float baseOuterRadius = 6.5f;

    [Header("포스트 프로세싱 설정")]
    public Volume globalVolume;         
    public float pulseSpeed = 5f;       
    public float maxIntensity = 0.6f;   
    private Vignette vignette;          

    [Header("인벤토리 설정")]
    public List<TreasureData> myInventory = new List<TreasureData>(); 
    public int maxInventorySlots = 4;   
    public GameObject GameOverPanel;    

    void Awake()
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
            SceneManager.sceneLoaded += OnSceneLoaded;
            InitVignette();
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void InitVignette()
    {
        if (globalVolume == null)
        {
            globalVolume = GameObject.FindFirstObjectByType<Volume>();
        }

        if (globalVolume != null && globalVolume.profile.TryGet<Vignette>(out var tmpVignette))
        {
            vignette = tmpVignette;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            headLight = player.GetComponentInChildren<Light2D>(true);
            
            MonoBehaviour playerManager = player.GetComponent("PlayerManager") as MonoBehaviour;
            if (playerManager != null) 
            {
                var field = playerManager.GetType().GetField("moveSpeed");
                if (field != null) field.SetValue(playerManager, currentMoveSpeed);
            }

            PlayerAttack pAttack = player.GetComponent<PlayerAttack>();
            if (pAttack != null) 
            {
                pAttack.playerDamage = currentAttackDamage;
            }
        }

        InitVignette();
        if (vignette != null) vignette.intensity.Override(0); 
    }

    void Start() 
    { 
        isDead = false; 
    }

    void Update()
    {
        currentHealth = math.clamp(currentHealth, 0, maxHealth);
        if (!isSafeZone) UseBattery(3f * Time.deltaTime);

        isBatteryOff = currentBattery <= 0;

        if (headLight != null)
        {
            if (isBatteryOff)
            {
                headLight.pointLightInnerRadius = 0;
                headLight.pointLightOuterRadius = 0;
            }
            else
            {
                headLight.pointLightInnerRadius = baseInnerRadius;
                headLight.pointLightOuterRadius = baseOuterRadius;
            }
        }
        VignetteEffect();
    }

    void VignetteEffect()
    {
        if (vignette == null) return;
        float healthRatio = (float)currentHealth / maxHealth;
        float invHealthRatio = 1f - healthRatio;
        float baseIntensity = invHealthRatio * (maxIntensity * 0.4f);
        float pulse = 0f;

        if (healthRatio <= 0.3f && healthRatio > 0)
        {
            pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            pulse *= invHealthRatio * maxIntensity;
        }
        vignette.color.Override(Color.red);
        vignette.intensity.Override(baseIntensity + pulse);
    }
    
    public void UseBattery(float amount) { if (!isSafeZone) currentBattery = Mathf.Clamp(currentBattery - amount, 0, maxBattery); }
    public bool CanAddItem() { return myInventory.Count < maxInventorySlots; }
    public void AddToInventory(TreasureData data) { if (CanAddItem()) { myInventory.Add(data); } }

    public void AddMoney(int amount) { money += amount; }
    public void TakeDamage(int damage)
    {
        if (isSafeZone || isDead) return;
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        StartCoroutine(Flash());
        if (currentHealth <= 0)
        {
            isDead = true;
            if(vignette != null) vignette.intensity.Override(0);
            Time.timeScale = 0; 
            StartCoroutine(Timer(3)); 
        }
    }

    IEnumerator Flash() 
    {
        if (vignette == null) yield break;
        for (float t = 0; t < 1f; t += Time.deltaTime / 0.01f) { vignette.intensity.Override(Mathf.Lerp(0, 0.25f, t)); yield return null; }
        for (float t = 0; t < 1f; t += Time.deltaTime / 0.3f) { vignette.intensity.Override(Mathf.Lerp(0.25f, 0, t)); yield return null; }
        vignette.intensity.Override(0);
    }

    public void GameOver()
    {
        isDead = false; 
        
        ResetAllUpgrades();

        currentHealth = maxHealth;
        currentBattery = maxBattery;
        money = 0;               
        myInventory.Clear();     
        isSafeZone = false;

        if(vignette != null) { vignette.intensity.Override(0); }
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

   
    public void ResetAllUpgrades()
    {
        maxHealth = 100;
        maxBattery = 100;
        baseInnerRadius = 5f;
        baseOuterRadius = 6.5f;
        maxInventorySlots = 4;

        currentMoveSpeed = 5f;  
        currentAttackDamage = 10; 

        lightUpgradeLevel = 0; lightUpgradeGold = 10;
        speedUpgradeLevel = 0; speedUpgradeGold = 10;
        attackUpgradeLevel = 0; attackUpgradeGold = 10;
        hpUpgradeLevel = 0; hpUpgradeGold = 50;
        batteryUpgradeLevel = 0; batteryUpgradeGold = 50;
        bagUpgradeLevel = 0; bagUpgradeGold = 80;
    }

    IEnumerator Timer(int time)
    {
        yield return new WaitForSecondsRealtime(time); 
        GameOverPanelOpen();
    }
    public void GameOverPanelOpen() { if (GameOverPanel != null) GameOverPanel.SetActive(true); }
    private void OnDestroy() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    public void Sleep()
    {
        currentHealth = maxHealth;
        currentBattery = maxBattery;
        Time.timeScale = 1; 
        StartCoroutine(SleepRoutine());
    }

    private IEnumerator SleepRoutine()
    {
        SoundManager.instance.PlaySFX("SleepSFX");
        if (UIManager.instance != null) UIManager.instance.ImageFadeOut();
        float waitTime = UIManager.instance != null ? UIManager.instance.GetFadeTime() : 1.5f;
        yield return new WaitForSecondsRealtime(waitTime);
        ReloadScene();
    }
    public void ReloadScene() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
}