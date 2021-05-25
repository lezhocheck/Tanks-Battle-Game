using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteLoader))]
public class Game : MonoBehaviour
{
    [Header("UI")] 
    public AudioSource winSource;
    public AudioSource gameOverSource;
    
    public Text playerHpText;
    public Image playerHpBar;
    public Text enemyLastText;
    public Text levelNameText;
    public Text hardBulletsText;
    public GameObject gameOverPanel;

    [Header("Camera input")] 
    public float sensitivity;
    public float minSize;
    public float maxSize;
    
    public SpriteLoader SpriteLoader { get; private set; }
    
    public GameField GameField { get; private set; }
    
    public int TotalEnemiesOnLevelCount { get; set; }

    public uint HardBulletsCount { get; set; }
    
    public int EnemiesInScene { get; set; }

    private int _enemiesPortionCount;
    
    
    public Player Player { get; private set; }

    private Camera _sceneCam;

    public UIController UIController { get; private set; }

    public bool IsGameFinished { get; set; }
    
    public LevelData TempLevel { get; private set; }
    private int[,] _grid2D;

    public void Start()
    {
        IsGameFinished = false;

        MenuManager manager = FindObjectOfType<MenuManager>();

        TempLevel = manager.LevelToLoad;
        Destroy(manager.gameObject);
        _sceneCam = FindObjectOfType<Camera>();
        SpriteLoader = GetComponent<SpriteLoader>();
        
        GameField = new GameField(SpriteLoader, GameField.ConvertTo2DGrid(TempLevel.gridArray, 
            TempLevel.gridSize.y, TempLevel.gridSize.x), _sceneCam);
        GameField.Initialize();

        TotalEnemiesOnLevelCount = TempLevel.totalEnemies;
        _enemiesPortionCount = TempLevel.enemiesPortionCount;
        HardBulletsCount = TempLevel.bigBullets;
        SpawnPlayer();
        SpawnEnemies();

        UIController = new UIController(this, winSource, gameOverSource, playerHpText, playerHpBar, 
            enemyLastText, hardBulletsText, levelNameText, gameOverPanel);
    }

    private void SpawnPlayer()
    {
        Player = new Player(this, TempLevel.startPlayerPosition, 
            Quaternion.identity, SpriteLoader, TempLevel.playerHp);
    }
    
    public void SpawnEnemies()
    {
        int spawnCount = TotalEnemiesOnLevelCount > _enemiesPortionCount
            ? _enemiesPortionCount
            : TotalEnemiesOnLevelCount;
        for (int i = 0; i < spawnCount; i++)
        {
            Enemy en = new Enemy(this, TempLevel.enemyHp);
            EnemiesInScene++;
        }
    }

    private void Update()
    {
        CameraZoom();
        UIController.UpdateUI();
    }

    private void CameraZoom()
    {
        float size = _sceneCam.orthographicSize;
        size -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        size = Mathf.Clamp(size, minSize, maxSize);
        _sceneCam.orthographicSize = size;
    }

    public void CoroutineStart(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}