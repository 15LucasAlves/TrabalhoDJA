using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public MusicSwitcher musicSwitcher; // Referência ao MusicSwitcher
    public Canvas canvasToShow;          // Referência ao Canvas que você quer mostrar

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            musicSwitcher.SwitchMusic();

            if (canvasToShow != null)
            {
                canvasToShow.gameObject.SetActive(true);
            }
        }
    }
}