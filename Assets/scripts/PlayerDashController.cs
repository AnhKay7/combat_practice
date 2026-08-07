using System;
using UnityEngine;

public class PlayerDashController : MonoBehaviour
{
    [Header("Dash Stats")]
    [field: SerializeField] public float DashSpeed { get; private set; } = 25f;
    [field: SerializeField] public float DashDuration { get; private set; } = 0.2f;
    [field: SerializeField] public float DashCooldown { get; private set; } = 0.5f;
    [field: SerializeField] public int MaxAirDashes { get; private set; } = 1;

    private float dashCooldownEndTime;
    private int remainingAirDashes;
    public bool CanDash(bool isGrounded)
    {
        if (Time.time < dashCooldownEndTime)
            return false;

        if (!isGrounded && remainingAirDashes <= 0)
            return false;

        return true;
    }

    public void ConsumeDash(bool isGrounded)
    {
        dashCooldownEndTime = Time.time + DashCooldown;

        if (!isGrounded)
        {
            remainingAirDashes--;
        }
    }
    public void ResetAirDashes()
    {
        remainingAirDashes = MaxAirDashes;
    }

    public void Reset()
    {
        remainingAirDashes = MaxAirDashes;
        dashCooldownEndTime = -100f;
    }
}

