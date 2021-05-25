using UnityEngine;

public class DestroyedTile : Cell
{
    public override CType Type => CType.DestroyedTile;
    
    public sealed override bool IsOccupied { get; set; }

    public DestroyedTile(GameField field, SpriteLoader loader, Vector2Int position, Quaternion rotation) : base(field, loader, position, rotation)
    {
        CellObject.name = CType.DestroyedTile.ToString();
        IsOccupied = false;
    }
}