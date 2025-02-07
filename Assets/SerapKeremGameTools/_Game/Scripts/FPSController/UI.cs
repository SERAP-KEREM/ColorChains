using SerapKeremGameTools._Game._FPSPlayerSystem;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private TextMeshProUGUI staminaText = default;

    private void OnEnable()
    {
        FPSDamage.OnDamage += UpdateHealth;
        FPSDamage.OnHeal += UpdateHealth;
        FPSMovement.OnStaminaChange += UpdateStamina;
    }

    private void OnDisable()
    {
        FPSDamage.OnDamage -= UpdateHealth;
        FPSDamage.OnHeal -= UpdateHealth;
        FPSMovement.OnStaminaChange -= UpdateStamina;

    }

    private void Start()
    {
        UpdateHealth(100);
        UpdateStamina(100);
    }
    private void UpdateHealth(float currentHealth)
    {
        healthText.text = currentHealth.ToString("00");
    } 
    private void UpdateStamina(float currentStamina)
    {
        staminaText.text = currentStamina.ToString("00");
    }
}
