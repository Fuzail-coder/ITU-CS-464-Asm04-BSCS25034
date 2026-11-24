using System;
using UnityEngine;

// The player's hearts, with a moment of invulnerability after every hit.
public class Health : MonoBehaviour
{
    [SerializeField] int max = 3;
    [SerializeField] float iFrames = 1f;
    public bool shielded;
    int current;
    float invulnerableUntil;
    public event Action<int, int> Damaged;
    public event Action<int, int> Healed;

    public int Current => current;
    public int Max => max;

    void Awake() => current = max;

    // True if the hit landed. A player who is down, or still invulnerable, can't be hit.
    public bool TakeDamage(int amount)
    {
        Debug.Log("TakeDamage called");
        if (current <= 0 || Time.time < invulnerableUntil) return false;
        invulnerableUntil = Time.time + iFrames;
        if (shielded)
        {
            shielded = false;
            return false;
        }
        current -= amount;
        Damaged?.Invoke(current, max);
        GameManager.Instance.UpdateUI();
        if (current <= 0) GameManager.Instance.PlayerDied();
        return true;
    }

    public void Restore(float protection)
    {
        current = max;
        invulnerableUntil = Time.time + protection;
        Healed?.Invoke(current, max);
        GameManager.Instance.UpdateUI();
    }
}