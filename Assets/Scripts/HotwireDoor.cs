using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HotwireDoor : MonoBehaviour
{
    // ScriptableObject constants
    public GameConstants gameConstants;

    // Events
    public UnityEvent onHotwireComplete;

    // Parameters
    private float detectionRadius;
    private bool isHotwiring = false;
    private Coroutine hotwireCoroutine;

    // External components
    private GameObject progressIndicator;
    private Animator progressAnimator;

    // Start is called before the first frame update
    void Start()
    {
        progressIndicator = GameObject.Find("ProgressMeter");
        if (progressIndicator != null)
        {
            progressAnimator = progressIndicator.GetComponent<Animator>();
            //progressAnimator.keepAnimatorStateOnDisable = true;
            progressIndicator.SetActive(false);
        }

        // Set constants
        detectionRadius = gameConstants.hotwireDetectionRadius;
    }

    // Subscriber - Hotwire Interaction
    public void HotwireCheck(Vector3 playerPos)
    {
        float distance = Vector3.Distance(transform.position, playerPos);

        if (distance <= detectionRadius)
        {
            if (!isHotwiring)
            {
                StartHotwiring();
            }
        }
        else if (isHotwiring)
        {
            CancelHotwiring();
        }
    }

    private void StartHotwiring()
    {
        isHotwiring = true;

        // Show progress indicator if available
        if (progressIndicator != null)
        {
            progressIndicator.SetActive(true);
        }
        progressAnimator.SetBool("isHotwiring", isHotwiring);
        hotwireCoroutine = StartCoroutine(HotwireProcess());
    }

    private void CompleteHotwiring()
    {
        isHotwiring = false;

        // Hide progress indicator
        if (progressIndicator != null)
        {
            progressIndicator.SetActive(false);
        }
        progressAnimator.SetBool("isHotwiring", isHotwiring);

        onHotwireComplete.Invoke(); // Calls OpenDoor (NextStage)

        gameObject.SetActive(false);
    }

    public void CancelHotwiring()
    {
        isHotwiring = false;
        progressAnimator.SetBool("isHotwiring", isHotwiring);

        if (hotwireCoroutine != null)
        {
            StopCoroutine(hotwireCoroutine);
            hotwireCoroutine = null;
        }

        if (progressIndicator != null)
        {
            progressIndicator.SetActive(false);
        }
    }

    // #------------------ COROUTINES -------------------- 
    private IEnumerator HotwireProcess()
    {
        while (!progressAnimator.GetCurrentAnimatorStateInfo(0).IsName("loading"))
        {
            yield return null;
        }

        //audioSource.PlayOneShot(wireSound);

        //yield return new WaitForSeconds(wireSound.length);

        while (progressAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        // Hotwiring successful
        CompleteHotwiring();
    }

    // #------------------- GAME -------------------#
    public void ResetHotwire()
    {
        // Reset coroutine
        hotwireCoroutine = null;

        // Reset animation
        isHotwiring = false;
        progressAnimator.SetBool("isHotwiring", isHotwiring);
        progressAnimator.Play("idle", 0, 0f);
        progressIndicator.SetActive(false);

        gameObject.SetActive(true);
    }
}
