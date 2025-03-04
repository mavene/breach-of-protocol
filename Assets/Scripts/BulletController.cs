using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour, IDeflectable
{
    // State
    private float bulletLifetime = 1f;

    // Audio
    private AudioSource audioSource;
    public AudioClip bulletSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(DestroyDelay());
    }

    public void Deflect(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().velocity = direction * 7f;
        gameObject.tag = "DeflectedProjectile";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && gameObject.CompareTag("DeflectedProjectile"))
        {
            if (other.gameObject.GetComponent<EnemyController>() != null) other.gameObject.GetComponent<EnemyController>().Die();
            if (other.gameObject.GetComponent<SlimeController>() != null) other.gameObject.GetComponent<SlimeController>().Die();
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(bulletLifetime);
        Destroy(gameObject);
    }
}
