using UnityEngine;

public class RotateOnKeyPress : MonoBehaviour
{
    public KeyCode rotateKey = KeyCode.R;
    public Vector3 rotationAxis = Vector3.forward; // Para 2D: Z (Vector3.forward)
    public float rotationSpeed = 100f;

    void Update()
    {
        if (Input.GetKey(rotateKey))
        {
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
        }
    }
}
