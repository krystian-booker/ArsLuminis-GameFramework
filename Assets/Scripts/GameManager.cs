using System.Linq;
using Cinemachine;
using Dialog;
using Input;
using Saving;
using Saving.Models;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

[RequireComponent(typeof(DialogManager), typeof(InputManager))]
public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
    }

    #endregion

    #region MoveToParty/CharacterManager

    public GameObject ActiveCharacter { get; private set; }
    public NavMeshAgent ActiveCharacterNavMeshAgent { get; private set; }

    #endregion

    public Camera mainCamera;

    //TODO: Remove, this is for debugging until decide what to do
    public GameObject activePlayer;

    [SerializeField] public GameState gameState;

    [HideInInspector] public InputManager inputManager;
    [HideInInspector] public DialogManager dialogManager;
    [HideInInspector] public CinemachineBrain cinemachineBrain;

    private void Initialize() //Awake
    {
        //Validations
        Assert.IsNotNull(mainCamera);
        Assert.IsNotNull(activePlayer);

        //Get component
        inputManager = GetComponent<InputManager>();
        dialogManager = GetComponent<DialogManager>();
        cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();

        //Validate components
        Assert.IsNotNull(inputManager);
        Assert.IsNotNull(dialogManager);
        Assert.IsNotNull(cinemachineBrain);

        //TODO: Remove. Loading the auto save file is just for testing
        var files = SaveManager.GetSaveFilesDetails();
        var autoFile = files.FirstOrDefault(x => x.fileName == "auto.el");
        if (autoFile != null)
        {
            gameState = SaveManager.LoadGame(autoFile.filePath);
        }

        ChangeCharacter(activePlayer);
    }

    private void ChangeCharacter(GameObject newCharacter)
    {
        //Set past character to obstacle avoidance
        if (ActiveCharacterNavMeshAgent != null)
        {
            ActiveCharacterNavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }

        //Update character
        ActiveCharacter = newCharacter;
        ActiveCharacterNavMeshAgent = ActiveCharacter.GetComponent<NavMeshAgent>();
        ActiveCharacterNavMeshAgent.updateRotation = true;
        ActiveCharacterNavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }
}