using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Coin : MonoBehaviour
{
    [SerializeField] float spinSpeed = 90f;
    [SerializeField] AudioClip chime;
    bool collected;

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player")) return;
        collected = true;
        AudioSource.PlayClipAtPoint(chime, transform.position);
        GameManager.Instance.AddScore(1);
        Destroy(gameObject);
    }
}