using UnityEngine;

public class DestroyableFourth : Cell
{
    public override CType Type => CType.DestroyableFourth;
    
    public sealed override bool IsOccupied { get; set; }

    public DestroyableFourth(GameField field, SpriteLoader loader, Vector2Int position, Quaternion rotation) : base(field, loader, position, rotation)
    {
        CellObject.name = CType.DestroyableFourth.ToString();
        CellObject.AddComponent<BoxCollider2D>();
        IsOccupied = true;
    }
}