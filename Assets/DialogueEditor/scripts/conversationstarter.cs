using DialogueEditor;
using StarterAssets;
using UnityEngine;

public class conversationstarter : MonoBehaviour
{
    [SerializeField] private NPCConversation myConversation;
    [SerializeField] private Animator npcAnimator; // Reference to the NPC's Animator

    private PlayerAttackSystem playerAttackSystem;
    private ThirdPersonController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerAttackSystem = other.GetComponent<PlayerAttackSystem>();
            playerController = other.GetComponent<ThirdPersonController>();

            if (playerAttackSystem != null)
            {
                playerAttackSystem.DisableAttacks();  // Disable attacks
            }

            ConversationManager.Instance.StartConversation(myConversation);

            if (playerController != null)
            {
                playerController.LockCameraOnTarget(transform);  // Lock camera to NPC
            }

            // Start talking animation
            if (npcAnimator != null)
            {
                npcAnimator.SetBool("isTalking", true);
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ConversationManager.Instance.EndConversation();

            if (playerAttackSystem != null)
            {
                playerAttackSystem.EnableAttacks();  // Re-enable attacks
            }

            if (playerController != null)
            {
                playerController.UnlockCamera();  // Unlock camera
            }

            // Stop talking animation
            if (npcAnimator != null)
            {
                npcAnimator.SetBool("isTalking", false);
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void EndConversation()
    {
        // Re-enable attacks and unlock camera after conversation ends
        if (playerAttackSystem != null)
        {
            playerAttackSystem.EnableAttacks();
        }

        if (playerController != null)
        {
            playerController.UnlockCamera();
        }

        // Stop talking animation
        if (npcAnimator != null)
        {
            npcAnimator.SetBool("isTalking", false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
