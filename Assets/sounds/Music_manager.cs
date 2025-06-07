using UnityEngine;

public class MusicSwitcher : MonoBehaviour
{
    public AudioClip defaultMusic;   // Música inicial
    public AudioClip triggerMusic;   // Música ao ser acionada
    public Canvas canvasToShow;      // Canvas para ativar junto com a música

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = defaultMusic;
        audioSource.loop = true;
        audioSource.Play();

        if (canvasToShow != null)
        {
            canvasToShow.gameObject.SetActive(false); // Desativa o canvas no início
        }
    }

    // Método chamado para trocar a música
    public void SwitchMusic()
    {
        if (triggerMusic != null && audioSource.clip != triggerMusic)
        {
            audioSource.clip = triggerMusic;
            audioSource.Play();

            if (canvasToShow != null)
            {
                canvasToShow.gameObject.SetActive(true);  // Ativa o canvas
            }
        }
    }
}