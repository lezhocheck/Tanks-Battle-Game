using UnityEngine;

public class DestroyableFirst : Cell
{
    public sealed override bool IsOccupied { get; set; }
    
    public override CType Type => CType.DestroyableFirst;

    public DestroyableFirst(GameField field, SpriteLoader loader, Vector2Int position, Quaternion rotation) : base(field, loader, position, rotation)
    {
        CellObject.name = CType.DestroyableFirst.ToString();
        CellObject.AddComponent<BoxCollider2D>();
        IsOccupied = true;
    }
}