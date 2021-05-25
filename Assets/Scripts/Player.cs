using UnityEngine;

public class Player
{
    private GameObject _playerObject;

    private Game _game;

    private GameField _gameField;

    public uint Hp { get; set; }
    public Vector2Int Position
    {
        get
        {
            Vector3 pos = _playerObject.transform.position;
            return new Vector2Int((int)pos.x, (int)pos.y);
        }
        set => _playerObject.transform.position = new Vector3(value.x, value.y, 0);
    }

    public Quaternion Rotation
    {
        get => _playerObject.transform.rotation;
        set => _playerObject.transform.rotation = value;
    }

    public GameObject PlayerObject => _playerObject;

    public Player(Game game, Vector2Int position, Quaternion rotation, SpriteLoader loader, uint hp)
    {
        _game = game;
        _gameField = game.GameField;
        _playerObject = Object.Instantiate(loader.PlayerPrefab);
        PlayerController controller = _playerObject.AddComponent<PlayerController>();
        controller.Player = this;
        controller.game = game;
        Position = position;
        Rotation = rotation;

        Hp = hp;
        Vector2Int pos = position;
        _gameField[pos.y, pos.x].IsOccupied = true;
    }
    
    public void Destroy()
    {
        Object.Destroy(_gameField[Position.y, Position.x].CellObject);
        
        CellCreator creator = new CellCreator(_gameField, _game.SpriteLoader, Position, Rotation);
        creator.CreateCell(Cell.CType.DestroyedTile);
        
        Cell newCell = _gameField[Position.y, Position.x];
        newCell.CellObject.GetComponent<SpriteRenderer>().sprite = _game.SpriteLoader.DestroyedPlayer;
        newCell.Position = _gameField[Position.y, Position.x].Position;
        
        _gameField[Position.y, Position.x].IsOccupied = false;
        
        Object.Destroy(PlayerObject);
    }
}
