using UnityEngine;

public class DestroyableSecond : Cell
{
    public override CType Type => CType.DestroyableSecond;
    
    public sealed override bool IsOccupied { get; set; }

    public DestroyableSecond(GameField field, SpriteLoader loader, Vector2Int position, Quaternion rotation) : base(field, loader, position, rotation)
    {
        CellObject.name = CType.DestroyableSecond.ToString();
        CellObject.AddComponent<BoxCollider2D>();
        IsOccupied = true;
    }
}