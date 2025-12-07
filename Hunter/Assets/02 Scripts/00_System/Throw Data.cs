using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThrowData", menuName = "SO/ThrowData")]
public class ThrowData : ScriptableObject
{
    [Header("조준선 및 조준")]
    [Header("Control Feel (조작감)")]
    public float sensitivity;
    public float aimSmoothing;
    public float minDragDistance;

    [Header("Physics Settings")]
    public float minPower = 5f;
    public float powerMultiplier = 10f;
    public float maxPower = 25f;
    public float gravity = -25f;
    public float fallGravityMultiplier = 2.5f;
    public float initialSpeedBoost = 1.2f;

    [Header("Trajectory Visuals")]
    public int dotCount = 30;
    public float dotSpacing = 0.05f;
    public float minAlpha = 0.2f;
    public float flowSpeed = 2f;

    [Header("Aim Constraints")]
    public float minAngle = -10f;
    public float maxAngle = 85f;

    [Header("투사체")]
    [Header("Stats & Damage")]
    public float damage = 10f;
    public float lifeTime = 5f;

    [Header("Speed & Logic")]
    public float speedMultiplier = 2.0f;

    [Header("Physics Logic")]
    public Vector2 ricochetForce = new Vector2(-5f, 10f);
    public float ricochetTorque = 360f;
    public float groundBounciness = 0.5f;
    public float groundFriction = 2.0f;
    public float stopThreshold = 4.0f;

    [Header("적 피격")]
    [Header("Stats")]
    public float Health = 100f;

    [Header("Damage Effect (Visual)")]
    public Vector2 punchScaleStrength = new Vector2(0.3f, 0.3f);
    public float punchDuration = 0.3f;
    public float flashDuration = 0.15f;
}
