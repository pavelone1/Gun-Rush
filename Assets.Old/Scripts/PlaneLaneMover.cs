using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLaneMover : MonoBehaviour
{
    [Header("Lane Settings")]
    [SerializeField] private float laneOffset = 3.2f;
    [SerializeField] private float laneChangeSpeed = 12f;

    private int currentLane = 0; // Left = -1, Center = 0, Right = 1
    private float targetX;

    private void Start()
    {
        targetX = transform.position.x;
    }

    private void Update()
    {
        HandleLaneInput();
        MoveToTargetLane();
    }

    private void HandleLaneInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
        {
            currentLane = Mathf.Clamp(currentLane - 1, -1, 1);
            UpdateTargetX();
        }

        if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
        {
            currentLane = Mathf.Clamp(currentLane + 1, -1, 1);
            UpdateTargetX();
        }
    }

    private void UpdateTargetX()
    {
        targetX = currentLane * laneOffset;
    }

    private void MoveToTargetLane()
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = new Vector3(targetX, currentPosition.y, currentPosition.z);

        transform.position = Vector3.Lerp(
            currentPosition,
            targetPosition,
            laneChangeSpeed * Time.deltaTime
        );
    }

    public int GetCurrentLane()
    {
        return currentLane;
    }
}