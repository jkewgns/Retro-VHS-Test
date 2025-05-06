using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class PlayVoiceline : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;

    [Header("Subtitle Settings")]
    public TMP_Text subtitleText;

    [System.Serializable]
    public class SubtitleLine
    {
        public string text;
        public float time;
    }

    [Header("Subtitle Lines")]
    public SubtitleLine[] subtitles;

    [Header("Input Settings")]
    private PlayerInput playerInput;
    private InputAction interactAction;

    [Header("State Settings")]
    private bool hasPlayed = false;

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found in the scene!");
            return;
        }

        interactAction = playerInput.actions["Interact"];
    }

    void OnEnable()
    {
        if (interactAction != null)
            interactAction.performed += PlayAudio;
    }

    void OnDisable()
    {
        if (interactAction != null)
            interactAction.performed -= PlayAudio;
    }

    void PlayAudio(InputAction.CallbackContext context)
    {
        if (audioSource == null || hasPlayed) return;

        audioSource.Play();
        hasPlayed = true;

        if (subtitleText != null)
            StartCoroutine(ShowSubtitles());
    }

    IEnumerator ShowSubtitles()
    {
        subtitleText.gameObject.SetActive(true);

        foreach (SubtitleLine subtitle in subtitles)
        {
            yield return new WaitForSeconds(subtitle.time);
            subtitleText.text = subtitle.text;
        }

        yield return new WaitForSeconds(2f);
        subtitleText.text = "";
        subtitleText.gameObject.SetActive(false);
    }
}
