using UnityEngine;

public class Empty : Cell
{
    public sealed override bool IsOccupied { get; set; }
    
    public override CType Type => CType.Empty;
    
    public Empty(GameField field, SpriteLoader loader, Vector2Int position, Quaternion rotation) : base(field, loader, position, rotation)
    {
        CellObject.name = CType.Empty.ToString();
        IsOccupied = false;
    }
}
