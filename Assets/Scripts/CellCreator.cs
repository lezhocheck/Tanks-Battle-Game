using UnityEngine;

public class CellCreator
{
    private GameField _gameField;
    private SpriteLoader _loader;
    
    private Vector2Int _cellPosition;
    private Quaternion _cellRotation;

    public CellCreator(GameField gameField, SpriteLoader loader, Vector2Int position, Quaternion rotation)
    {
        _gameField = gameField;
        _cellPosition = position;
        _cellRotation = rotation;
        _loader = loader;
    }

    public void CreateCell(Cell.CType type)
    {
        Cell cell = null;

        switch (type)
        {
            case Cell.CType.Boarder :
                cell = new Border(_gameField, _loader, _cellPosition, _cellRotation);
                break;
            case Cell.CType.Empty :
                cell = new Empty(_gameField, _loader, _cellPosition, _cellRotation);
                break;
            case Cell.CType.DestroyableFirst:
                cell = new DestroyableFirst(_gameField, _loader, _cellPosition, _cellRotation);
                break;
            case Cell.CType.DestroyableSecond:
                cell = new DestroyableSecond(_gameField, _loader, _cellPosition, _cellRotation);
                break;
            case Cell.CType.DestroyableThird:
                cell = new DestroyableThird(_gameField, _loader, _cellPosition, _cellRotation);
                break;
            case Cell.CType.DestroyableFourth:
                cell = new DestroyableFourth(_gameField, _loader, _cellPosition, _cellRotation);
                break;
            case Cell.CType.DestroyedTile:
                cell = new DestroyedTile(_gameField, _loader, _cellPosition, _cellRotation);
                break;
        }
        
        _gameField[_cellPosition.y, _cellPosition.x] = cell;
    }
}
