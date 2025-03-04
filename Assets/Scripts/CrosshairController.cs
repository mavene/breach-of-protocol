using UnityEngine;
using System.Collections;

public class CrosshairController : MonoBehaviour
{
    // "Movement"
    private Vector2 crosshairPos;

    // Animation
    private Animator crosshairAnimator;

    // Start is called before the first frame update
    void Start()
    {
        crosshairAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        crosshairPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = crosshairPos;
    }

    public void PlayCrosshairAnimation()
    {
        crosshairAnimator.SetBool("isShooting", true);
        StartCoroutine(CrosshairProgress());
    }

    private IEnumerator CrosshairProgress()
    {
        while (!crosshairAnimator.GetCurrentAnimatorStateInfo(0).IsName("shooting"))
        {
            yield return null;
        }

        while (crosshairAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        crosshairAnimator.SetBool("isShooting", false);
    }
}
