using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class KillTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public float delayBeforeReload = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered Kill Trigger! Reloading level...");
            ReloadLevel();
        }
    }

    private void ReloadLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        yield return new WaitForSeconds(delayBeforeReload);
        SceneManager.LoadScene(levelIndex);
    }
}
