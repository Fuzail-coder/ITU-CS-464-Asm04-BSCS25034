using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    [SerializeField] int lifeAmount = 1;
    [SerializeField] float spinSpeed = 90f;
    [SerializeField] AudioClip effect;

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance.AddLife(lifeAmount))
        {
            if (effect != null)
                SfxPlayer.Instance.Play(effect);

            Destroy(gameObject);
        }
    }
}