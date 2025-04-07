using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class make_player_crouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StarterAssetsInputs input = other.GetComponent<StarterAssetsInputs>();
            if (input != null)
            {
                input.forcedCrouch = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StarterAssetsInputs input = other.GetComponent<StarterAssetsInputs>();
            if (input != null)
            {
                input.forcedCrouch = false;
            }
        }
    }
}
