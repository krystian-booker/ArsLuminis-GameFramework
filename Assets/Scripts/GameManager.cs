using System.Linq;
using Cinemachine;
using Dialog;
using EventSystem;
using Input;
using Saving;
using Saving.Models;
using Scene;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

[RequireComponent(typeof(InputManager),typeof(DialogManager), typeof(EventSystemManager))]
[RequireComponent(typeof(SceneControlManager))]
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

    [Tooltip("If enabled all text will be localized")]
    public bool enableLocalization = true;
    
    [SerializeField] public GameState gameState;

    [HideInInspector] public InputManager inputManager;
    [HideInInspector] public DialogManager dialogManager;
    [HideInInspector] public CinemachineBrain cinemachineBrain;
    [HideInInspector] public EventSystemManager eventSystemManager;
    [HideInInspector] public SceneControlManager sceneControlManager;

    private void Initialize() //Awake
    {
        //Validations
        Assert.IsNotNull(mainCamera);

        //Get component
        inputManager = GetComponent<InputManager>();
        dialogManager = GetComponent<DialogManager>();
        eventSystemManager = GetComponent<EventSystemManager>();
        cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
        sceneControlManager = GetComponent<SceneControlManager>();
        
        //Validate components
        Assert.IsNotNull(inputManager);
        Assert.IsNotNull(dialogManager);
        Assert.IsNotNull(eventSystemManager);
        Assert.IsNotNull(cinemachineBrain);
        Assert.IsNotNull(sceneControlManager);
        
        //TODO: Remove. Loading the auto save file is just for testing
        var files = SaveManager.GetSaveFilesDetails();
        var autoFile = files.FirstOrDefault(x => x.fileName == "auto.el");
        if (autoFile != null)
        {
            gameState = SaveManager.LoadGame(autoFile.filePath);
        }

        //TODO: Remove
        Assert.IsNotNull(activePlayer);
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