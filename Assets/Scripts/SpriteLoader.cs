using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour
{
    [SerializeField]
    private string tilesTexturesFolderName;
    
    public Dictionary<Cell.CType, Sprite> MapTiles;

    public GameObject PlayerPrefab { get; private set; }
    
    public GameObject EnemyPrefab { get; private set; }

    public GameObject BulletPrefab{ get; private set; }
    
    public GameObject ShootParticles { get; private set; }
    
    public Sprite DestroyedEnemy { get; private set; }
    
    public Sprite DestroyedPlayer { get; private set; }
    
    public void Awake()
    {
        MapTiles = new Dictionary<Cell.CType, Sprite>();
        
        foreach (Cell.CType type in Enum.GetValues(typeof(Cell.CType)))
        {
            Sprite sprite = Resources.Load<Sprite>($"{tilesTexturesFolderName}/{type.ToString()}");
            
            if (sprite == null) throw new NullReferenceException();

            MapTiles.Add(type, sprite);
        }
        
        PlayerPrefab = Resources.Load("Player") as GameObject;
        EnemyPrefab = Resources.Load("Enemy") as GameObject;
        BulletPrefab = Resources.Load("Bullet") as GameObject;
        ShootParticles = Resources.Load(("ShootParticles")) as GameObject;
        DestroyedEnemy  = Resources.Load<Sprite>($"{tilesTexturesFolderName}/DestroyedEnemy");
        DestroyedPlayer  = Resources.Load<Sprite>($"{tilesTexturesFolderName}/DestroyedPlayer");

        if (PlayerPrefab == null || EnemyPrefab == null || BulletPrefab == null || 
            ShootParticles == null || DestroyedEnemy == null || DestroyedPlayer == null)
        {
            throw new NullReferenceException();
        }
    }
}
