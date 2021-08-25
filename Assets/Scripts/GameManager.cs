using Characters;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Tooltip("If enabled all text will be localized")]
    public bool enableLocalization = true;

    [HideInInspector] public GameObject activePlayer;
    [HideInInspector] public CharacterManager activePlayerCharacterManager;
    [HideInInspector] public NavMeshAgent activeCharacterNavMeshAgent;

    void OnEnable()
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
        //Set past character to obstacle avoidance
        if (activeCharacterNavMeshAgent != null)
        {
            activeCharacterNavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }

        //Update character
        activePlayer = newCharacter;
        activeCharacterNavMeshAgent = activePlayer.GetComponent<NavMeshAgent>();
        activeCharacterNavMeshAgent.updateRotation = true;
        activeCharacterNavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        activePlayerCharacterManager = activePlayer.GetComponent<CharacterManager>();
    }
}