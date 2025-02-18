using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // State
    private float bulletLifetime = 0.5f;

    // Audio
    private AudioSource audioSource;
    public AudioClip bulletSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(DestroyDelay());
    }

    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(bulletLifetime);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyController>().Die();
            Destroy(gameObject);
        }
    }
}
