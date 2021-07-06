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
    public CinemachineBrain cinemachineBrain;

    public GameObject activePlayer;

    [SerializeField] public GameState gameState;

    private void Initialize() //Awake
    {
        Assert.IsNotNull(mainCamera);
        Assert.IsNotNull(activePlayer);
        
        cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
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

    public void ChangeCharacter(GameObject newCharacter)
    {
        ActiveCharacter = newCharacter;
        ActiveCharacterNavMeshAgent = ActiveCharacter.GetComponent<NavMeshAgent>();
        ActiveCharacterNavMeshAgent.updateRotation = true;
    }
}