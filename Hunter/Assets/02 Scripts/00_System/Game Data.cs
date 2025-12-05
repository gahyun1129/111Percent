using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "GameData", menuName = "SO/GameData")]
public class GameData : ScriptableObject
{
    [Header("[플레이어 설정]")]
    [Header("플레이어 속도")]
    public float playerSpeed = 1.0f;
    [Header("플레이어 초기 위치")]
    public float playerViewportX = 0.36f;
    public float playerViewportY = 0.19f;

    [Header("[적 설정]")]
    [Header("적 속도")]
    public float enemySpeed = 1.0f;
    [Header("적 초기 위치")]
    public float enemyViewportX = 0.77f;
    public float enemyViewportY = 0.19f;
    [Header("적 이동")]
    public AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0f, 0.3f, 1f, 1f);
    public float maxAccelerationTime = 1f;
    public float stopDistance = 1f;
    public float reactionDelay = 0.5f;
    [Header("적 추적 패턴")]
    [Tooltip("추적 지속 시간 (초)")]
    public float chaseDuration = 3f;
    [Tooltip("휴식 시간 (초)")]
    public float restDuration = 2f;

    [Header("카메라 설정")]
    public float viewportThresholdX = 0.3f;
    public float followSpeed = 3f;
    public float smoothStopDuration = 0.5f;
    [Header("카메라 바운스(관성)")]
    public float bounceDistance = 0.3f;
    public float bounceDuration = 0.4f;
    public Ease bounceEase = Ease.OutCubic;

    [Header("에임 영역")]
    public float aimAreaSize = 3f;
    public Vector2 aimAreaPos = Vector2.zero;
}
