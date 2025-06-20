using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour, IGameEventObserver
{
    public Slider healthBar;
    public Slider energyBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI energyText;

    private float maxHealth = 100f;
    private float maxEnergy = 100f;

    private float currentHealth;
    private float currentEnergy;
    public GameObject gameOverCanvas;
    private bool isGameOver = false;

    void Start()
    {
        Time.timeScale = 1f;
        gameOverCanvas.SetActive(false);

        currentHealth = maxHealth;
        currentEnergy = maxEnergy;

        healthBar.maxValue = maxHealth;
        energyBar.maxValue = maxEnergy;

        healthBar.value = currentHealth;
        energyBar.value = currentEnergy;

        UpdateUI();

        InvokeRepeating(nameof(DecreaseEnergy), 1f, 1f);
        
        // Observer Pattern 적용 - 이벤트 구독
        GameEventSystem.Instance.Subscribe(GameEventType.PlayerHealthChanged, this);
        GameEventSystem.Instance.Subscribe(GameEventType.PlayerEnergyChanged, this);
    }
    
    void OnDestroy()
    {
        // 메모리 누수 방지를 위한 구독 해제
        if (GameEventSystem.Instance != null)
        {
            GameEventSystem.Instance.UnsubscribeAll(this);
        }
    }
    
    // Observer Pattern 구현
    public void OnGameEvent(GameEventData eventData)
    {
        switch (eventData.eventType)
        {
            case GameEventType.PlayerHealthChanged:
                Debug.Log($"🏥 Health changed event received: {eventData.data}");
                break;
            case GameEventType.PlayerEnergyChanged:
                Debug.Log($"⚡ Energy changed event received: {eventData.data}");
                break;
        }
    }

    void DecreaseEnergy()
    {
        float oldEnergy = currentEnergy;
        currentEnergy -= 1f;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        energyBar.value = currentEnergy;

        UpdateUI();
        
        // Observer Pattern - 에너지 변화 이벤트 발생
        if (oldEnergy != currentEnergy)
        {
            GameEventSystem.Instance.TriggerEvent(GameEventType.PlayerEnergyChanged, new { oldValue = oldEnergy, newValue = currentEnergy });
        }
    }

    void UpdateUI()
    {
        healthText.text = $"{currentHealth} / {maxHealth}";
        energyText.text = $"{currentEnergy} / {maxEnergy}";
    }

    void Update()
    {
        if (!isGameOver && (currentHealth <= 0f || currentEnergy <= 0f))
        {
            isGameOver = true;
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
		EffectManager.Instance.PlayGameOver(transform.position);
		isGameOver = true;
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
        
        // Observer Pattern - 게임 오버 이벤트 발생
        GameEventSystem.Instance.TriggerEvent(GameEventType.GameOver, new { health = currentHealth, energy = currentEnergy });
    }

	void EndGame()
	{
		
	}

	public bool IsGameOver()
	{
		return isGameOver;
	}

    public void AddEnergy(float amount)
    {
        if (isGameOver) return;

        float oldEnergy = currentEnergy;
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        energyBar.value = currentEnergy;
        UpdateUI();
        
        // Observer Pattern - 에너지 변화 이벤트 발생
        if (oldEnergy != currentEnergy)
        {
            GameEventSystem.Instance.TriggerEvent(GameEventType.PlayerEnergyChanged, new { oldValue = oldEnergy, newValue = currentEnergy });
        }
    }

    public void AddHealth(float amount)
    {
        if (isGameOver) return;

        float oldHealth = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        healthBar.value = currentHealth;
        UpdateUI();
        
        // Observer Pattern - 체력 변화 이벤트 발생
        if (oldHealth != currentHealth)
        {
            GameEventSystem.Instance.TriggerEvent(GameEventType.PlayerHealthChanged, new { oldValue = oldHealth, newValue = currentHealth });
        }
    }

    public void TakeDamage(float amount)
    {
        if (isGameOver) return;

        float oldHealth = currentHealth;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        healthBar.value = currentHealth;
        UpdateUI();
        
        // Observer Pattern - 체력 변화 이벤트 발생
        if (oldHealth != currentHealth)
        {
            GameEventSystem.Instance.TriggerEvent(GameEventType.PlayerHealthChanged, new { oldValue = oldHealth, newValue = currentHealth });
        }
    }
}

