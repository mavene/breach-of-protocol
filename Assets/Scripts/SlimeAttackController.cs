using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SlimeAttackController : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator DestroyDelay()
{
    yield return new WaitForSeconds(bulletLifetime);

    if (gameObject == null)
    {
        Debug.Log("Bullet already null before Destroy.");
        yield break; // ✅ Prevents execution if already destroyed
    }

    Debug.Log("Destroying Bullet: " + gameObject.name);

    StopAllCoroutines(); // ✅ Prevents lingering coroutines from running
    // gameObject.SetActive(false); // ✅ Ensures it cannot be referenced before destruction
    // Destroy(this.gameObject);
}

}
