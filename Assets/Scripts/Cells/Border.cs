using UnityEngine;

public class Border : Cell
{
    public sealed override bool IsOccupied { get; set; }
    
    public override CType Type => CType.Boarder;

    public Border(GameField field, SpriteLoader loader, Vector2Int position, Quaternion rotation) : base(field, loader, position, rotation)
    {
        CellObject.name = CType.Boarder.ToString();
        CellObject.AddComponent<BoxCollider2D>();
        IsOccupied = true;
    }
}