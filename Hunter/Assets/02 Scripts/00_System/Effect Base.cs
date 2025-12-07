using UnityEngine;
using System.Collections;

public class EffectBase : MonoBehaviour
{
    private EffectType _type;
    private EffectManager _manager;

    public void Initialize(EffectManager manager, EffectType type)
    {
        _manager = manager;
        _type = type;
    }

    public void Play(Vector3 position, Vector3 scale)
    {
        transform.position = position;
        transform.localScale = scale;

        gameObject.SetActive(true);

        // 자식들에 있는 모든 파티클/애니메이션을 다시 시작시킴
        // (Set Active true가 되면 보통 자동 재생되지만, 확실하게 하기 위함)
        foreach (var ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }

        // 가장 긴 이펙트 시간 계산
        float duration = GetMaxDuration();

        StopAllCoroutines();
        StartCoroutine(DisableRoutine(duration));
    }

    private float GetMaxDuration()
    {
        float maxDuration = 0f;

        // 1. 자식들 중 파티클 시스템 체크
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particles)
        {
            if (!ps.main.loop)
            {
                // duration + startLifetime(최대)가 실제 보여지는 시간
                float time = ps.main.duration + ps.main.startLifetime.constantMax;
                if (time > maxDuration) maxDuration = time;
            }
        }

        // 2. 필요하다면 오디오나 애니메이터 시간 체크 로직 추가 가능
        // (예: AudioSource.clip.length 등)

        // 최소 1초는 보장 (너무 짧으면 이상하니까)
        return Mathf.Max(maxDuration, 1.0f);
    }

    private IEnumerator DisableRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        _manager.ReturnEffectToPool(_type, this);
    }
}

public enum EffectType
{
    Bird_Success,
    Bird_Fail,
}