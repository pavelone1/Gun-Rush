using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject squadMemberPrefab;

    [Header("Squad Health")]
    [SerializeField, Range(0.01f, 2f)] private float squadHealthPercent = 0.5f;

    [Header("Formation Size Limits")]
    [SerializeField] private float maxFormationRadius = 2.8f;
    [SerializeField] private float innerRingRadius = 1.2f;
    [SerializeField] private float outerRingSpacing = 0.75f;

    [Header("Crowding")]
    [SerializeField] private float minNeighborSpacing = 0.55f;
    [SerializeField] private float maxNeighborSpacing = 1.15f;

    [Header("Ring Capacity")]
    [SerializeField] private int baseRingCapacity = 6;
    [SerializeField] private int extraCapacityPerRing = 4;

    [Header("Player Scaling")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float playerMinScale = 0.75f;
    [SerializeField] private float playerMaxScale = 1.0f;
    [SerializeField] private Vector3 playerScaleVector = Vector3.one;

    [Header("Squad Scaling")]
    [SerializeField] private float squadMinScale = 0.75f;
    [SerializeField] private float squadMaxScale = 1.0f;
    [SerializeField] private Vector3 squadScaleVector = Vector3.one;

    [Header("Shared Scaling")]
    [SerializeField] private int scaleStartCount = 4;
    [SerializeField] private int scaleFullCount = 20;
    [SerializeField] private float scaleLerpSpeed = 4f;

    private float currentPlayerScale = 1f;
    private float currentSquadScale = 1f;

    private readonly List<GameObject> squadMembers = new List<GameObject>();

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AddSquadMember();
        }

        CleanupMissingMembers();
        UpdateScaling();
    }

    public void AddSquadMember()
    {
        if (player == null || squadMemberPrefab == null) return;

        GameObject member = Instantiate(squadMemberPrefab, player.position, Quaternion.identity);

        WeaponState playerWeaponState = player.GetComponent<WeaponState>();
        WeaponState memberWeaponState = member.GetComponent<WeaponState>();

        if (playerWeaponState != null && memberWeaponState != null)
        {
            memberWeaponState.CopyFrom(playerWeaponState);
        }

        ApplySquadHealthFromPlayer(member);

        squadMembers.Add(member);
        RebuildFormation();
    }

    public void UpgradePlayerHealth(float amount)
    {
        if (amount <= 0f || player == null) return;

        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth == null) return;

        playerHealth.AddMaxHealth(amount, amount);

        float squadAmount = amount * squadHealthPercent;

        for (int i = squadMembers.Count - 1; i >= 0; i--)
        {
            GameObject member = squadMembers[i];

            if (member == null)
            {
                squadMembers.RemoveAt(i);
                continue;
            }

            Health memberHealth = member.GetComponent<Health>();
            if (memberHealth != null)
            {
                memberHealth.AddMaxHealth(squadAmount, squadAmount);
            }
        }
    }

    private void ApplySquadHealthFromPlayer(GameObject member)
    {
        if (member == null || player == null) return;

        Health playerHealth = player.GetComponent<Health>();
        Health memberHealth = member.GetComponent<Health>();

        if (playerHealth == null || memberHealth == null) return;

        float squadMaxHealth = playerHealth.MaxHealth * squadHealthPercent;
        memberHealth.SetHealth(squadMaxHealth, true);
        memberHealth.Died += HandleSquadMemberDied;
    }

    private void HandleSquadMemberDied(Health deadHealth)
    {
        if (deadHealth == null) return;

        squadMembers.Remove(deadHealth.gameObject);
        RebuildFormation();
    }

    private void CleanupMissingMembers()
    {
        bool removedAny = false;

        for (int i = squadMembers.Count - 1; i >= 0; i--)
        {
            if (squadMembers[i] == null)
            {
                squadMembers.RemoveAt(i);
                removedAny = true;
            }
        }

        if (removedAny)
        {
            RebuildFormation();
        }
    }

    private void RebuildFormation()
    {
        int total = squadMembers.Count;

        for (int i = 0; i < total; i++)
        {
            GameObject member = squadMembers[i];
            if (member == null) continue;

            Vector3 offset = GetPackedOffsetForIndex(i, total);
            SquadFollower follower = member.GetComponent<SquadFollower>();

            if (follower != null)
            {
                follower.Initialize(player, offset);
            }
        }
    }

    private Vector3 GetPackedOffsetForIndex(int index, int totalMembers)
    {
        int ring = 0;
        int indexInRing = index;
        int ringCapacity = GetRingCapacity(ring);

        while (indexInRing >= ringCapacity)
        {
            indexInRing -= ringCapacity;
            ring++;
            ringCapacity = GetRingCapacity(ring);
        }

        int membersInThisRing = GetMembersInRing(ring, totalMembers);
        float ringRadius = GetDynamicRingRadius(ring, membersInThisRing);

        float angleDeg;

        if (ring == 0)
        {
            if (indexInRing == 0) angleDeg = 110f;
            else if (indexInRing == 1) angleDeg = 250f;
            else
            {
                float[] orderedAngles = GetOrderedAnglesForRing(membersInThisRing);
                angleDeg = orderedAngles[Mathf.Clamp(indexInRing, 0, orderedAngles.Length - 1)];
            }
        }
        else
        {
            float[] orderedAngles = GetOrderedAnglesForRing(membersInThisRing);
            angleDeg = orderedAngles[Mathf.Clamp(indexInRing, 0, orderedAngles.Length - 1)];
        }

        float angleRad = angleDeg * Mathf.Deg2Rad;
        float x = Mathf.Sin(angleRad) * ringRadius;
        float z = Mathf.Cos(angleRad) * ringRadius;

        return new Vector3(x, 0f, z);
    }

    private void ApplyScale(float playerScale, float squadScale)
    {
        if (playerTransform != null)
        {
            playerTransform.localScale = new Vector3(
                playerScaleVector.x * playerScale,
                playerScaleVector.y * playerScale,
                playerScaleVector.z * playerScale
            );
        }

        foreach (var member in squadMembers)
        {
            if (member != null)
            {
                member.transform.localScale = new Vector3(
                    squadScaleVector.x * squadScale,
                    squadScaleVector.y * squadScale,
                    squadScaleVector.z * squadScale
                );
            }
        }
    }

    private void UpdateScaling()
    {
        int total = squadMembers.Count + 1;

        float t = Mathf.InverseLerp(scaleStartCount, scaleFullCount, total);
        float targetPlayerScale = Mathf.Lerp(playerMaxScale, playerMinScale, t);
        float targetSquadScale = Mathf.Lerp(squadMaxScale, squadMinScale, t);

        currentPlayerScale = Mathf.Lerp(currentPlayerScale, targetPlayerScale, Time.deltaTime * scaleLerpSpeed);
        currentSquadScale = Mathf.Lerp(currentSquadScale, targetSquadScale, Time.deltaTime * scaleLerpSpeed);

        ApplyScale(currentPlayerScale, currentSquadScale);
    }

    private int GetRingCapacity(int ring)
    {
        return baseRingCapacity + ring * extraCapacityPerRing;
    }

    private int GetMembersInRing(int targetRing, int totalMembers)
    {
        int remaining = totalMembers;

        for (int ring = 0; ring <= targetRing; ring++)
        {
            int capacity = GetRingCapacity(ring);

            if (ring == targetRing)
                return Mathf.Min(remaining, capacity);

            remaining -= capacity;
        }

        return 0;
    }

    private float GetDynamicRingRadius(int ring, int membersInRing)
    {
        if (membersInRing <= 1)
            return innerRingRadius + ring * outerRingSpacing;

        float desiredSpacing = Mathf.Lerp(
            maxNeighborSpacing,
            minNeighborSpacing,
            Mathf.InverseLerp(1f, 12f, membersInRing)
        );

        float circumferenceNeeded = membersInRing * desiredSpacing;
        float computedRadius = circumferenceNeeded / (2f * Mathf.PI);
        float baseRadius = innerRingRadius + ring * outerRingSpacing;
        float finalRadius = Mathf.Max(baseRadius * 0.75f, computedRadius);

        return Mathf.Min(finalRadius, maxFormationRadius + ring * outerRingSpacing);
    }

    private float[] GetOrderedAnglesForRing(int count)
    {
        List<float> baseAngles = new List<float>();

        for (int i = 0; i < count; i++)
        {
            float angle = (360f / count) * i;
            baseAngles.Add(angle);
        }

        baseAngles.Sort((a, b) => AnglePriority(a).CompareTo(AnglePriority(b)));
        return baseAngles.ToArray();
    }

    private float AnglePriority(float angle)
    {
        float diffFromSide = Mathf.Min(
            Mathf.Abs(Mathf.DeltaAngle(angle, 90f)),
            Mathf.Abs(Mathf.DeltaAngle(angle, 270f))
        );

        float frontBackPenalty = Mathf.Min(
            Mathf.Abs(Mathf.DeltaAngle(angle, 0f)),
            Mathf.Abs(Mathf.DeltaAngle(angle, 180f))
        );

        return diffFromSide + frontBackPenalty * 0.5f;
    }
}