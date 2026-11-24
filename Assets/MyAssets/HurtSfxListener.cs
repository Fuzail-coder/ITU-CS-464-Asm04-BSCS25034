using UnityEngine;

public class HurtSfxListener : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] AudioClip hurtClip;

    void OnEnable() => health.Damaged += OnDamaged;
    void OnDisable() => health.Damaged -= OnDamaged;

    void OnDamaged(int current, int max) => SfxPlayer.Instance.Play(hurtClip);
}