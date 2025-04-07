using UnityEngine;

namespace StarterAssets
{
    public class PlayerStamina : MonoBehaviour
    {
        [Header("Player Stamina Settings")]
        public float currentStamina;
        public float maxStamina = 20f;
        public float regenRate = 1.5f;            
        public float regenDelay = 0.5f;          


        private void Start()
        {
            currentStamina = maxStamina;
        }

        public float CurrentStamina => currentStamina;


        public void RunStaminaSpending()
        {
            currentStamina -= Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
        public void RegenerateStamina()
        {
            currentStamina += Time.deltaTime * 2.25f; // Regain slower than loss rate
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
        public void ConsumeStamina(float amount)
        {
            currentStamina -= amount;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 200, 20), "Stamina: " + currentStamina.ToString("F1"));
        }
    }
}
