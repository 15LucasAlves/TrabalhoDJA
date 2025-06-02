using UnityEngine;

public class portao_trigger : MonoBehaviour
{
    public portao_boss gateScript; // Drag the gate object here in the Inspector

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gateScript.ActivateDescent();
        }
    }
}
