using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LevelLoader : MonoBehaviour
{
    [Header("UI Settings")]
    public Animator transition;
    public float transitionTime = 1f;
    public GameObject promptUI;

    [Header("Player and Door Settings")]
    public Transform player;
    public Transform door;
    public float activationDistance = 3f;

    [Header("Input Settings")]
    private InputAction interactAction;

    private bool isNearDoor = false;

    private void Awake()
    {
        interactAction = new InputAction(binding: "<Keyboard>/e");
        interactAction.performed += ctx => TryLoadLevel();
        interactAction.Enable();
    }

    private void OnDestroy()
    {
        interactAction.Disable();
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.position, door.position);
        if (distance <= activationDistance)
        {
            if (!isNearDoor)
            {
                isNearDoor = true;
                promptUI.SetActive(true);
            }
        }
        else
        {
            if (isNearDoor)
            {
                isNearDoor = false;
                promptUI.SetActive(false);
            }
        }
    }

    private void TryLoadLevel()
    {
        if (isNearDoor)
        {
            promptUI.SetActive(false);
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
