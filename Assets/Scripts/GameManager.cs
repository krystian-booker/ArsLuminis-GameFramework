using Characters;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Tooltip("If enabled all text will be localized")]
    public bool enableLocalization = true;

    [HideInInspector] public GameObject activePlayer;
    [HideInInspector] public CharacterManager activeCharacterManager;

    private void Awake()
    {
        Systems.Initialize();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        //TODO: Replace: Plan the character functionality. Once scene loads look at Party manager for primary character?
        activePlayer = GameObject.FindWithTag("Player");
        if (activePlayer != null)
        {
            ChangeCharacter(activePlayer);
        }
    }

    private void ChangeCharacter(GameObject newCharacter)
    {
        activePlayer = newCharacter;
        activeCharacterManager = activePlayer.GetComponent<CharacterManager>();
    }
}