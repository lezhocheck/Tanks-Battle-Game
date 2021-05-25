using UnityEngine;
using System.Linq;

public abstract class Cell
{
    public enum CType
    {
        Empty,
        Boarder,
        DestroyableFirst,
        DestroyableSecond,
        DestroyableThird,
        DestroyableFourth,
        DestroyedTile,
    }

    private GameObject _cellObject;

    protected Cell(GameField field, SpriteLoader loader, Vector2Int position, Quaternion rotation)
    {
        Sprite sprite = loader.MapTiles.FirstOrDefault(pair => pair.Key == Type).Value;

        _cellObject = new GameObject();
        var sr = CellObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        Position = position;
        Rotation = rotation;

        _cellObject.transform.parent = field.FieldObject.transform;
    }

    public Vector2Int Position
    {
        get
        {
            Vector3 pos = _cellObject.transform.position;
            return new Vector2Int((int)pos.x, (int)pos.y);
        }
        set => _cellObject.transform.position = new Vector3(value.x, value.y, 0);
    }
    
    public Quaternion Rotation
    {
        get => _cellObject.transform.rotation;
        set => _cellObject.transform.rotation = value;
    }

    public abstract CType Type { get; }
    
    public abstract bool IsOccupied { get; set; }
    public GameObject CellObject => _cellObject;
}

