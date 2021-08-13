using System.Linq;
using Audio;
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

[RequireComponent(typeof(InputManager), typeof(DialogManager), typeof(EventSystemManager))]
[RequireComponent(typeof(SceneControlManager), typeof(AudioManager))]
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

    public GameObject ActiveCharacter { get; set; }
    public NavMeshAgent ActiveCharacterNavMeshAgent { get; set; }

    #endregion

    public Camera mainCamera;

    //TODO: Remove, this is for debugging until decide what to do
    public GameObject activePlayer;

    [Tooltip("If enabled all text will be localized")]
    public bool enableLocalization = true;

    [SerializeField] public GameState gameState;

    [HideInInspector] public InputManager inputManager;
    [HideInInspector] public DialogManager dialogManager;
    [HideInInspector] public EventSystemManager eventSystemManager;
    [HideInInspector] public SceneControlManager sceneControlManager;
    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public CinemachineBrain cinemachineBrain;

    private UnityEngine.EventSystems.EventSystem _eventSystem;

    public UnityEngine.EventSystems.EventSystem EventSystem
    {
        get
        {
            if (_eventSystem == null)
            {
                _eventSystem = UnityEngine.EventSystems.EventSystem.current;
            }

            return _eventSystem;
        }
    }

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
        audioManager = GetComponent<AudioManager>();

        //Validate components
        Assert.IsNotNull(inputManager);
        Assert.IsNotNull(dialogManager);
        Assert.IsNotNull(eventSystemManager);
        Assert.IsNotNull(cinemachineBrain);
        Assert.IsNotNull(sceneControlManager);
        Assert.IsNotNull(audioManager);

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