using System.Linq;
using Dialog;
using Saving;
using Saving.Models;
using UnityEngine;

[RequireComponent(typeof(DialogManager))]
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

    [SerializeField]
    public GameState gameState;
        
    private void Initialize()
    {
        //TODO: Remove. Loading the auto save file is just for testing
        var files = SaveManager.GetSaveFilesDetails();
        var autoFile = files.FirstOrDefault(x => x.fileName == "auto.el");
        if (autoFile != null)
        {
            gameState = SaveManager.LoadGame(autoFile.filePath);
        }
    }
}