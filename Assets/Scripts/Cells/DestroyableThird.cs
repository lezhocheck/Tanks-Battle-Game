using UnityEngine;

public class DestroyableThird : Cell
{
    public override CType Type => CType.DestroyableThird;
    
    public sealed override bool IsOccupied { get; set; }

    public DestroyableThird(GameField field, SpriteLoader loader, Vector2Int position, Quaternion rotation) : base(field, loader, position, rotation)
    {
        CellObject.name = CType.DestroyableThird.ToString();
        CellObject.AddComponent<BoxCollider2D>();
        IsOccupied = true;
    }
}