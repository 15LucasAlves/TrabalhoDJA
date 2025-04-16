using UnityEngine;
using UnityEngine.UI;

namespace StarterAssets
{
    public class PlayerStamina : MonoBehaviour
    {
        [Header("Player Stamina Settings")]
        public float currentStamina;
        public float maxStamina = 20f;
        public float regenRate = 1.5f;
        public float regenDelay = 0.5f;

        [Header("Stamina UI")]
        public Image staminaBarFill; // Drag your blue UI bar here
        public float smoothSpeed = 5f;

        private void Start()
        {
            currentStamina = maxStamina;

            if (staminaBarFill != null)
                staminaBarFill.fillAmount = 1f;
        }

        private void Update()
        {
            UpdateStaminaBar();
        }

        public float CurrentStamina => currentStamina;

        public void RunStaminaSpending()
        {
            currentStamina -= Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }

        public void RegenerateStamina()
        {
            currentStamina += Time.deltaTime * 2.25f;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }

        public void ConsumeStamina(float amount)
        {
            currentStamina -= amount;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }

        private void UpdateStaminaBar()
        {
            if (staminaBarFill != null)
            {
                float target = currentStamina / maxStamina;
                staminaBarFill.fillAmount = Mathf.Lerp(staminaBarFill.fillAmount, target, Time.deltaTime * smoothSpeed);
            }
        }
    }
}
