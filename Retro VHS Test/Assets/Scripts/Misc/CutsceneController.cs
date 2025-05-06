using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [Header("Animation")]
    public Animator playerAnimator;
    public Animator knifeAnimator;
    public GameObject knifeObject;

    [Header("Audio")]
    public AudioClip stabSFX;
    public AudioSource audioSource;

    [Header("Cutscene Timing")]
    public float turnAnimationDuration = 2.0f;
    public float stabAnimationDuration = 1.5f;

    [Header("Scene Management")]
    public string creditsSceneName = "Credits";

    [Header("Player Control")]
    public PlayerMovement playerMovement;

    private bool cutsceneStarted = false;

    void Start()
    {
        if (knifeObject != null)
            knifeObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!cutsceneStarted && other.CompareTag("Player"))
        {
            cutsceneStarted = true;

            if (playerMovement != null)
                playerMovement.enabled = false;

            StartCoroutine(PlayCutscene());
        }
    }

    private IEnumerator PlayCutscene()
    {
        if (playerAnimator != null)
            playerAnimator.SetTrigger("Turn");

        yield return new WaitForSeconds(turnAnimationDuration);

        if (knifeObject != null)
            knifeObject.SetActive(true);

        if (knifeAnimator != null)
            knifeAnimator.SetTrigger("Stab");

        if (audioSource != null && stabSFX != null)
            audioSource.PlayOneShot(stabSFX, 1.0f);

        yield return new WaitForSeconds(stabAnimationDuration);

        SceneManager.LoadScene(creditsSceneName);
    }
}
