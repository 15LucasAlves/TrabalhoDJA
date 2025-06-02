using UnityEngine;

public class portao_boss : MonoBehaviour
{
    [Header("Descent Settings")]
    public float descentAmount = 3f;
    public float descentSpeed = 2f;

    private bool shouldMoveDown = false;
    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (shouldMoveDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, descentSpeed * Time.deltaTime);
        }
    }

    public void ActivateDescent()
    {
        if (!shouldMoveDown)
        {
            targetPosition = transform.position + Vector3.down * descentAmount;
            shouldMoveDown = true;
        }
    }
}
