using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(SpriteLoader))]
public class MenuManager : MonoBehaviour
{
    [Header("Camera")] 
    public Camera menuCamera;
    public float sensitivity;
    public float minSize;
    public float maxSize;
    
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject fieldCreatorPanel;
    public GameObject editorText;
    
    [Header("Inputs")] 
    public InputField levelName;
    public InputField levelLoadName;
    public InputField fieldWidth;
    public InputField fieldHeight;
    public InputField playerHp;
    public InputField enemyHp;
    public InputField enemiesCount;
    public InputField enemiesPortionCount;
    public InputField bigBulletsCount;
    public InputField startPlayerPosX;
    public InputField startPlayerPosY;
    public InputField levelNameDelete;

    [Header("Sound")] 
    public GameObject soundOnButton;
    public GameObject soundOffButton;

    [Header("Other")] 
    public GameObject creatorUI;
    public Text massage;
    public Image blocksImage;
    public GameObject levelProto;
    public GameObject listPanel;
    public Text customInfoText;
    public Transform mainPlotContentObj;
    public Transform customPlotContentObj;
    
    private SpriteLoader _loader;
    private int[,] _grid2D;
    private GameField _field;

    private int _selectedX;
    private int _selectedY;
    private Cell _lastSelected;

    private int _tHeight;
    private int _tWidth;

    private Cell.CType _selectedType = Cell.CType.Empty;

    private List<LevelData> _levelDataCustom;
    private List<LevelData> _levelDataMain;

    public LevelData LevelToLoad { get; private set; }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("LevelPassed"))
        {
            PlayerPrefs.SetInt("LevelPassed", 0);   
        }

        if (!PlayerPrefs.HasKey("Sound"))
        {
            PlayerPrefs.SetInt("Sound", 1);   
        }

        ChangeSoundUI();
            
        DontDestroyOnLoad(this);
        
        _loader = GetComponent<SpriteLoader>();
        _levelDataMain = SaveLoadSystem.LoadMain();

        CreateLevelIndicator(_levelDataMain, mainPlotContentObj, true);
    }
    
    public void PlayClick()
    {
        _levelDataCustom = SaveLoadSystem.LoadCustom();
        
        if (_levelDataCustom.Count > 0)
        {
            customInfoText.gameObject.SetActive(false);
            CreateLevelIndicator(_levelDataCustom, customPlotContentObj, false);
        }
        else
        {
            customInfoText.gameObject.SetActive(true);
        } 
        
        listPanel.SetActive(true);
    }

    private void CreateLevelIndicator(List<LevelData> data, Transform obj, bool checkForLock)
    {
        for (int i = 0; i < data.Count; i++)
        {
            GameObject go = Instantiate(levelProto, Vector3.zero, Quaternion.identity, obj);
            Text textObj = go.GetComponentInChildren<Text>();
            Image imageObj = go.transform.GetChild(1).GetComponent<Image>();
            textObj.text = $"Level {i + 1}: {data[i].levelName}";
            Button button = go.GetComponent<Button>();
            var a = i;

            if (!checkForLock || i <= PlayerPrefs.GetInt("LevelPassed"))
            {
                button.onClick.AddListener(delegate { LevelChooseClick(data, data[a].levelNumber - 1); });
                Destroy(imageObj);
            }
        }
    }

    private void LevelChooseClick(List<LevelData> list, int value)
    {
        LevelToLoad = list[value];
        SceneManager.LoadScene(1);
    }
    public void CustomClick()
    {
        mainPanel.SetActive(false);
        fieldCreatorPanel.SetActive(true);
        editorText.SetActive(true);
    }

    public void NextClick()
    {
        _levelDataCustom = SaveLoadSystem.LoadCustom();
        
        Int32.TryParse(fieldHeight.text.Trim(), out _tHeight);
        Int32.TryParse(fieldWidth.text.Trim(), out _tWidth);
        string lName = levelName.text.Trim();

        if (lName.Length < 4)
        {
            massage.text = "Invalid level name. Level name must contain at least 4 symbols.";   
        }
        else if (_levelDataCustom.Contains(_levelDataCustom.Find(x => x.levelName == lName)))
        {
            massage.text = $"The level with name '{lName}' already exists. Go to load level or choose another name.";   
        }
        else if (_tWidth < 5 || _tHeight < 5 || _tWidth > 25 || _tHeight > 25)
        {
            massage.text = "Invalid width or height. Parameters must be more than 5 and less or equals 25.";   
        }
        else
        {
            massage.text = String.Empty;   
            fieldCreatorPanel.SetActive(false);
            creatorUI.SetActive(true);
            BuildField(_tWidth, _tHeight);
        }
    }

    public void LoadLevelClick()
    {
        _levelDataCustom = SaveLoadSystem.LoadCustom();
        
        string lName = levelLoadName.text.Trim();
        LevelData data = _levelDataCustom.Find(x => x.levelName == lName);
        if (!_levelDataCustom.Contains(data))
        {
            massage.text = $"Level with name '{lName}' does not exists. Please check the spelling or create a new one.";   
        }
        else
        {
            massage.text = String.Empty;   
            fieldCreatorPanel.SetActive(false);
            creatorUI.SetActive(true);
            LoadField(data);
        }
    }

    private void LoadField(LevelData data)
    {
        levelName.text = data.levelName;
        fieldWidth.text = data.gridSize.x.ToString();
        fieldHeight.text = data.gridSize.y.ToString();
        playerHp.text = data.playerHp.ToString();
        enemyHp.text = data.enemyHp.ToString();
        enemiesCount.text = data.totalEnemies.ToString();
        enemiesPortionCount.text = data.enemiesPortionCount.ToString();
        bigBulletsCount.text = data.bigBullets.ToString();
        startPlayerPosX.text = data.startPlayerPosition.x.ToString();
        startPlayerPosY.text = data.startPlayerPosition.y.ToString();
        
        _grid2D = GameField.ConvertTo2DGrid(data.gridArray, data.gridSize.y, data.gridSize.x);
        
        _field = new GameField(_loader, _grid2D, menuCamera);
        _field.Initialize();
        
        _selectedX = 1;
        _selectedY = data.gridSize.y - 2;
        _lastSelected = null;
        Select(_lastSelected, _selectedX, _selectedY);
    }
    
    private void BuildField(int width, int height)
    {
        _grid2D = new int[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (i == 0 || j == 0 || i == height - 1 || j == width - 1)
                {
                    _grid2D[i, j] = (int)Cell.CType.Boarder;   
                }
                else
                {
                    _grid2D[i, j] = (int)Cell.CType.Empty;   
                }
            }
        }
        _field = new GameField(_loader, _grid2D, menuCamera);
        _field.Initialize();
        
        _selectedX = 1;
        _selectedY = height - 2;
        _lastSelected = null;
        Select(_lastSelected, _selectedX, _selectedY);
    }

    private void Select(Cell lastSelected, int x, int y)
    {
        if (lastSelected != null)
        {
            lastSelected.CellObject.GetComponent<SpriteRenderer>().color = Color.white;   
        }

        Cell selected = _field[y, x];
        var spriteRenderer = selected.CellObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.green;
        blocksImage.sprite = spriteRenderer.sprite;
    }
    
    private void CameraZoom()
    {
        float size = menuCamera.orthographicSize;
        size -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        size = Mathf.Clamp(size, minSize, maxSize);
        menuCamera.orthographicSize = size;
    }

    public void BackButtonClick()
    {
        fieldCreatorPanel.SetActive(true);
        creatorUI.SetActive(false);
        Destroy(_field.FieldObject);
        massage.text = String.Empty;
    }

    private void UpdateSelection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_selectedY < _field.Height - 2)
            {
                _lastSelected = _field[_selectedY, _selectedX];
                _selectedY++;
                Select(_lastSelected, _selectedX, _selectedY);   
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_selectedY > 1)
            {
                _lastSelected = _field[_selectedY, _selectedX];
                _selectedY--;
                Select(_lastSelected, _selectedX, _selectedY);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_selectedX > 1)
            {
                _lastSelected = _field[_selectedY, _selectedX];
                _selectedX--;
                Select(_lastSelected, _selectedX, _selectedY);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_selectedX < _field.Width - 2)
            {
                _lastSelected = _field[_selectedY, _selectedX];
                _selectedX++;
                Select(_lastSelected, _selectedX, _selectedY);
            }
        }
    }

    public void LeftButtonSelectClick()
    {
        Cell toDestroy = _field[_selectedY, _selectedX];
        _selectedType = toDestroy.Type;
        
        int t = (int)_selectedType;

        if (t > 0)
        {
            t--;
            _selectedType = (Cell.CType) t;
            _grid2D[_selectedY, _selectedX] = t;
            
            Destroy(toDestroy.CellObject);
            
            CellCreator creator = new CellCreator(_field, _loader, toDestroy.Position, Quaternion.identity);
            creator.CreateCell(_selectedType);
            
            Cell selected = _field[_selectedY, _selectedX];
            blocksImage.sprite = _loader.MapTiles.FirstOrDefault(x => x.Key == selected.Type).Value;
            selected.CellObject.GetComponent<SpriteRenderer>().color = Color.green;   
        }
    }
    
    public void RightButtonSelectClick()
    {
        Cell toDestroy = _field[_selectedY, _selectedX];
        _selectedType = toDestroy.Type;
        
        int t = (int)_selectedType;
        
        if (t < Enum.GetNames(typeof(Cell.CType)).Length - 1)
        {
            t++;
            _selectedType = (Cell.CType) t;
            _grid2D[_selectedY, _selectedX] = t;
            
            Destroy(toDestroy.CellObject);
            
            CellCreator creator = new CellCreator(_field, _loader, toDestroy.Position, Quaternion.identity);
            creator.CreateCell(_selectedType);
            
            Cell selected = _field[_selectedY, _selectedX];
            blocksImage.sprite = _loader.MapTiles.FirstOrDefault(x => x.Key == selected.Type).Value;
            selected.CellObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    public void Save()
    { 
        _levelDataCustom = SaveLoadSystem.LoadCustom();
        
        Int32.TryParse(fieldHeight.text.Trim(), out _tHeight);
        Int32.TryParse(fieldWidth.text.Trim(), out _tWidth);
        Int32.TryParse(playerHp.text.Trim(), out int tPlayerHp);
        Int32.TryParse(enemyHp.text.Trim(), out int tEnemyHp);
        Int32.TryParse(enemiesCount.text.Trim(), out int tEnemiesCount);
        Int32.TryParse(enemiesPortionCount.text.Trim(), out int tEnemiesPortionCount);
        Int32.TryParse(bigBulletsCount.text.Trim(), out int tbigBulletsCount);
        Int32.TryParse(startPlayerPosX.text.Trim(), out int tStartPlayerPosX);
        Int32.TryParse(startPlayerPosY.text.Trim(), out int tStartPlayerPosY);

        int emptyCellCount = _field.GetEmptyCount();

        if (tPlayerHp < 1 || tEnemyHp < 1)
        {
            massage.text = "Invalid player hp or enemy hp size. Parameters must be more than 0 and less then 1000.";   
        }
        else if (tEnemiesCount < 1)
        {
            massage.text = "Invalid enemies count. Parameter must be more than 0 and less then 100.";   
        }
        else if (tEnemiesPortionCount < 1 || tEnemiesPortionCount > 10 || tEnemiesPortionCount > tEnemiesCount)
        {
            massage.text = "Invalid enemy wave count. Parameter must be more than 0 and less then 10. Wave count cannot be greater then enemies count.";
        }
        else if (tbigBulletsCount < 0 || tbigBulletsCount > 100)
        {
            massage.text = "Invalid big bullets count. Parameter must be more or equals 0 and less or equals 100.";
        }
        else if (tStartPlayerPosX <= 0 || tStartPlayerPosX >= _tHeight - 1)
        {
            massage.text = $"Invalid player x position. Parameter must be more than 0 and less then {_tHeight - 1}.";   
        }
        else if (tStartPlayerPosY <= 0 || tStartPlayerPosX >= _tWidth - 1)
        {
            massage.text = $"Invalid player y position. Parameter must be more than 0 and less then {_tWidth - 1}.";   
        }
        else if (_field[tStartPlayerPosY, tStartPlayerPosX].IsOccupied)
        {
            massage.text = $"Player must be spawned in empty cell. Please choose another cell as spawn position.";   
        }
        else if (tEnemiesPortionCount + 1 > emptyCellCount)
        {
            massage.text = $"Not enough empty cells ({emptyCellCount}) to spawn player and enemies ({tEnemiesPortionCount + 1}). Please increase the number of empty cells.";   
        }
        else
        {
            massage.text = String.Empty;

            LevelData data = _levelDataCustom.Find(x => x.levelName == levelName.text.Trim());
            int fileCount = _levelDataCustom.Contains(data) ? data.levelNumber - 1 : SaveLoadSystem.GetCustomLevelsCount;
            
            LevelData ld = new LevelData()
            {
                levelNumber = fileCount + 1,
                levelName = levelName.text.Trim(),
                enemiesPortionCount =  tEnemiesPortionCount,
                enemyHp = (uint)tEnemyHp,
                gridArray = GameField.ConvertTo1DGrid(_grid2D),
                gridSize = new Vector2Int(_tWidth, _tHeight),
                playerHp = (uint)tPlayerHp,
                startPlayerPosition = new Vector2Int(tStartPlayerPosX, tStartPlayerPosY),
                totalEnemies = tEnemiesCount,
                bigBullets =  (uint)tbigBulletsCount
            };
            
            SaveLoadSystem.Save(ld);

            for (int i = 0; i < customPlotContentObj.childCount; i++)
            {
                Destroy(customPlotContentObj.GetChild(i).gameObject);   
            }

            LeaveFieldEditor();
        }
    }

    public void Delete()
    {
        _levelDataCustom = SaveLoadSystem.LoadCustom();
        
        string lName = levelNameDelete.text.Trim();
        LevelData data = _levelDataCustom.Find(x => x.levelName == lName);
        if (!_levelDataCustom.Contains(data))
        {
            massage.text = $"Level with name '{lName}' does not exists. Please check the spelling.";   
        }
        else
        {
            massage.text = String.Empty;   
            LeaveFieldEditor();

            for (int i = 0; i < customPlotContentObj.childCount; i++)
            {
                Destroy(customPlotContentObj.GetChild(i).gameObject);   
            }

            SaveLoadSystem.DeleteLevel(lName);
            _levelDataCustom.RemoveAll(x => x.levelName == lName);
        }
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (editorText.activeSelf)
            {
               LeaveFieldEditor();
            }

            if (listPanel.activeSelf)
            {
                listPanel.SetActive(false);

                for (int i = 0; i < customPlotContentObj.childCount; i++)
                {
                    Destroy(customPlotContentObj.GetChild(i).gameObject);   
                }
            }
        }

        if (creatorUI.activeSelf)
        {
            CameraZoom();
            UpdateSelection();
        }
    }

    private void LeaveFieldEditor()
    {
        mainPanel.SetActive(true);
        fieldCreatorPanel.SetActive(false);
        creatorUI.SetActive(false);
        editorText.SetActive(false);
        if (_field != null)
        {
            Destroy(_field.FieldObject);      
        }

        for (int i = 0; i < customPlotContentObj.childCount; i++)
        {
            Destroy(customPlotContentObj.GetChild(i).gameObject);   
        }
    }

    private void ChangeSoundUI()
    {
        int temp = PlayerPrefs.GetInt("Sound");
        
        soundOnButton.SetActive(temp == 1);
        soundOffButton.SetActive(temp == 0);
        AudioListener.pause = temp == 0;
    }
    
    public void ChangeSoundState()
    {
        int temp = PlayerPrefs.GetInt("Sound") == 0 ? 1 : 0;

        AudioListener.pause = temp == 0;
        soundOnButton.SetActive(!soundOnButton.activeSelf);
        soundOffButton.SetActive(!soundOffButton.activeSelf);
        
        PlayerPrefs.SetInt("Sound", temp);
    }
}
