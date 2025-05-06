using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditsScroller : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 50f;
    public float maxScrollSpeed = 100f;
    public float targetY = 1874f;

    [Header("Transition Settings")]
    public Animator transition;
    public float transitionTime = 1f;

    private float originalScrollSpeed;

    void Start()
    {
        originalScrollSpeed = scrollSpeed;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            scrollSpeed = Mathf.Lerp(scrollSpeed, maxScrollSpeed, Time.deltaTime * 5f);
        }
        else
        {
            scrollSpeed = Mathf.Lerp(scrollSpeed, originalScrollSpeed, Time.deltaTime * 5f);
        }

        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

        if (transform.position.y >= targetY)
        {
            LoadNextLevel();
        }
    }

    void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(0));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}