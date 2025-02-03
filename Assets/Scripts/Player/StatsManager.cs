using System;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;


    [Header("Stats")]
    public float maxHealth = 100;
    public float health;
    public float hunger;
    public float thirst;
    [Space]
    [Header("Hunger & Thirst Depletion")]
    [SerializeField] private float hungerDepletionRate = 1f; // 1 punkt na sekundę
    [SerializeField] private float thirstDepletionRate = 1f;
    [SerializeField] private float defaultHungerDepletionInterval = 5f;
    [SerializeField] public float defaultThirstDepletionInterval = 5f;
    [Space]
    [Header("Health Drain Settings")]
    [SerializeField] private int lowHungerThreshold = 10;
    [SerializeField] private int lowThirstThreshold = 10;
    [SerializeField] private float healthDrainInterval = 5f;
    [SerializeField] private int healthDrainAmount = 1;
    [Space]
    [Header("References")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image foodBar;
    [SerializeField] private Image waterBar;
    
    //*Internal variables
    
    //Timers
    private float hungerTimer;
    private float thirstTimer;
    private float healthDrainTimer;
    //Depletion Intervals
    [HideInInspector] public float hungerDepletionInterval;
    [HideInInspector] public float thirstDepletionInterval;

    private void Start()
    {
        
        hungerDepletionInterval = defaultHungerDepletionInterval;
        thirstDepletionInterval = defaultThirstDepletionInterval;
        
        health = maxHealth;
        hunger = 100;
        thirst = 100;
    }

    private void UpdateBars()
    {
        healthBar.fillAmount = health / maxHealth;
        foodBar.fillAmount = hunger / 100;
        waterBar.fillAmount = thirst / 100;
    }
    
    private void ChangeHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateBars();
    }
    
    private void Update()
    {
        HandleStatDepletion();
        HandleHealthDrain();
        
        if(health <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Player died");
        }
    }

    private void HandleHealthDrain()
    {
        bool shouldDrainHealth = hunger <= lowHungerThreshold || thirst <= lowThirstThreshold;

        if (shouldDrainHealth)
        {
            healthDrainTimer += Time.deltaTime;

            if (healthDrainTimer >= healthDrainInterval)
            {
                ChangeHealth(-healthDrainAmount);
                healthDrainTimer = 0f;
            }
        }
        else
        {
            healthDrainTimer = 0f;
        }
    }
    
    private void HandleStatDepletion()
    {
        hungerTimer += Time.deltaTime;
        thirstTimer += Time.deltaTime;

        if (hungerTimer >= hungerDepletionInterval && hunger > 0)
        {
            hunger -= (int)hungerDepletionRate;
            hungerTimer = 0;
            UpdateBars();
        }

        if (thirstTimer >= thirstDepletionInterval && thirst > 0)
        {
            thirst -= (int)thirstDepletionRate;
            thirstTimer = 0;
            UpdateBars();
        }
        
    }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
}
