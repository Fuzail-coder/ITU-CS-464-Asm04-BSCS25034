using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] Image lag;
    [SerializeField] Gradient colour;
    [SerializeField] Health health;
    [SerializeField] float lagDelay = 0.4f;
    [SerializeField] float lagSpeed = 0.8f;
    float target = 1f;
    float lagStartsAt;

    void Awake()
    {
        if (health == null) health = FindFirstObjectByType<Health>();
    }

    void OnEnable()
    {
        health.Damaged += Set;
        health.Healed += Set;
    }

    void OnDisable()
    {
        health.Damaged -= Set;
        health.Healed -= Set;
    }

    void Start()
    {
        target = 1f;
        lag.fillAmount = 1f;
        Set(health.Current, health.Max);
    }

    public void Set(int current, int max)
    {
        float next = Mathf.Clamp01((float)current / max);
        Debug.Log("Set called: " + current + "/" + max);
        if (next < target) lagStartsAt = Time.unscaledTime + lagDelay;
        else lag.fillAmount = next;
        target = next;
        fill.fillAmount = target;
        fill.color = colour.Evaluate(target);
        lag.color = Color.Lerp(fill.color, Color.white, 0.55f);
    }

    void Update()
    {
        if (Time.unscaledTime < lagStartsAt) return;
        lag.fillAmount = Mathf.MoveTowards(lag.fillAmount, target, lagSpeed * Time.unscaledDeltaTime);
    }
}