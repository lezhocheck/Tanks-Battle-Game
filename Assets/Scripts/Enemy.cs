using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy 
{
    private GameObject _enemyObject;

    private EnemyController _enemyController;

    public Game Game { get; }
    public GameField GameField => Game.GameField;

    public uint Hp { get; set; }
    
    public Vector2Int Position
    {
        get
        {
            Vector3 pos = _enemyObject.transform.position;
            return new Vector2Int((int)pos.x, (int)pos.y);
        }
        set => _enemyObject.transform.position = new Vector3(value.x, value.y, 0);
    }

    public Quaternion Rotation
    {
        get => _enemyObject.transform.rotation;
        set => _enemyObject.transform.rotation = value;
    }

    public GameObject EnemyObject => _enemyObject;

    public Enemy(Game game, uint hp)
    {
        Game = game;
        Hp = hp;
        Instantiate();
    }

    private void Instantiate()
    {
        List<Cell> freeCells = new List<Cell>();
        
        for (int i = 0; i < GameField.Height; i++)
        {
            for (int j = 0; j < GameField.Width; j++)
            {
                Cell cell = GameField[i, j];
                
                if (!cell.IsOccupied)
                {
                    freeCells.Add(cell);
                }
            }
        }
        
        int random = Random.Range(0, freeCells.Count);
        Cell spawnIn = freeCells[random];
        
        _enemyObject = Object.Instantiate(Game.SpriteLoader.EnemyPrefab);
        Position = spawnIn.Position;
        
        _enemyController = _enemyObject.AddComponent<EnemyController>();
        
        _enemyController.Enemy = this;
        GameField[spawnIn.Position.y, spawnIn.Position.x].IsOccupied = true;
    }

    public void Destroy()
    {
        Vector2Int nextPos = Position + new Vector2Int(Mathf.RoundToInt(_enemyObject.transform.up.x), Mathf.RoundToInt(_enemyObject.transform.up.y));

        Object.Destroy(GameField[Position.y, Position.x].CellObject);
        
        CellCreator creator = new CellCreator(GameField, Game.SpriteLoader, Position, Rotation);
        creator.CreateCell(Cell.CType.DestroyedTile);
        
        Cell newCell = GameField[Position.y, Position.x];
        newCell.CellObject.GetComponent<SpriteRenderer>().sprite = Game.SpriteLoader.DestroyedEnemy;
        newCell.Position = GameField[Position.y, Position.x].Position;
        
        GameField[Position.y, Position.x].IsOccupied = false;

        if (_enemyController.Step > 0.0f && _enemyController.Step < 1.0f)
        {
            GameField[nextPos.y, nextPos.x].IsOccupied = false;
        }

        Object.Destroy(_enemyObject);
        
        Game.TotalEnemiesOnLevelCount--;
        Game.EnemiesInScene--;

        if (Game.EnemiesInScene <= 0)
        {
            Game.SpawnEnemies();   
        }

        if (Game.TotalEnemiesOnLevelCount <= 0)
        {
            Game.StartCoroutine(Game.UIController.Win());            
        }
    }
}
