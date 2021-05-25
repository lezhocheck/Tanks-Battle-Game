using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public class Bullet
{
    private GameObject _bullet;
    public bool BigDamageable { get; private set; }
    private float _speed = 8.0f;
    private Vector2 _direction;
    private Game _game;
    private GameObject _shootParticles;
    
    public bool IsDamageableToPlayer { get; }

    public GameObject BulletObject => _bullet;

    public Bullet(Collider2D ignoreCollider, bool isDamageable, bool bigDamageable, Game game, Vector2 position, Quaternion rotation, Vector2 direction)
    {
        _game = game;
        IsDamageableToPlayer = isDamageable;
        BigDamageable = bigDamageable;
        _shootParticles = _game.SpriteLoader.ShootParticles;
        _bullet = Object.Instantiate(game.SpriteLoader.BulletPrefab, position, rotation);

        if (BigDamageable)
        {
            Vector3 scale = _bullet.transform.localScale;
            _bullet.transform.localScale = scale * 1.3f;
            _bullet.GetComponent<SpriteRenderer>().color = Color.red;
        }

        _direction = direction;
        AddForce();
        
        BulletController controller = _bullet.AddComponent<BulletController>();
        controller.Bullet = this;
  
        Collider2D bulletCollider = _bullet.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(bulletCollider, ignoreCollider);
    }

    public void AddForce()
    {
        Rigidbody2D rb = _bullet.GetComponent<Rigidbody2D>();
        rb.velocity = _direction * _speed;
    }

    public void UpdateCollision(Collision2D col)
    {
        var colPos = col.transform.position;
        Vector2Int pos= new Vector2Int((int)colPos.x, (int)colPos.y);
        
        Cell cell = _game.GameField[pos.y, pos.x];
        GenerateParticles(cell.Position);

        EnemyController ec = col.gameObject.GetComponent<EnemyController>(); 
        PlayerController pc = col.gameObject.GetComponent<PlayerController>();
        if (ec)
        {
            UpdateEnemyCollision(ec.Enemy);   
        }
        else if (pc)
        {
            UpdatePlayerCollision(pc.Player);   
        }
        else
        {
            UpdateCellCollision(cell, pos);   
        }
    }

    private void UpdateCellCollision(Cell cell, Vector2Int pos)
    {
        GameObject toDestroy = cell.CellObject;
        CellCreator creator = new CellCreator(_game.GameField, _game.SpriteLoader, pos, cell.Rotation);

        if (BigDamageable)
        {
            if (cell.Type != Cell.CType.Boarder)
            {
                creator.CreateCell(Cell.CType.DestroyedTile);
                Object.Destroy(toDestroy);
            }
        }
        else
        {
            if (cell.Type == Cell.CType.DestroyableFirst)
            {
                creator.CreateCell(Cell.CType.DestroyableSecond);
                Object.Destroy(toDestroy);
            }
            else if (cell.Type == Cell.CType.DestroyableSecond)
            {
                creator.CreateCell(Cell.CType.DestroyableThird);
                Object.Destroy(toDestroy);
            }
            else if (cell.Type == Cell.CType.DestroyableThird)
            {
                creator.CreateCell(Cell.CType.DestroyableFourth);
                Object.Destroy(toDestroy);
            }
            else if (cell.Type == Cell.CType.DestroyableFourth)
            {
                creator.CreateCell(Cell.CType.DestroyedTile);
                Object.Destroy(toDestroy);
            }   
        }
    }

    private void UpdatePlayerCollision(Player player)
    {
        if (player.Hp > 1)
        {
            player.Hp--;
        }
        else
        {
            player.Hp--;
            player.Destroy();
            _game.StartCoroutine(_game.UIController.GameOver());
        }
    }

    private void UpdateEnemyCollision(Enemy enemy)
    {
        uint bigDam = 10;
        
        if (!IsDamageableToPlayer)
        {
            if (!BigDamageable && enemy.Hp > 1)
            {
                enemy.Hp--;   
            }
            else if (BigDamageable && (int)enemy.Hp - bigDam > 0)
            {
                enemy.Hp -= bigDam; 
            }
            else
            {
                enemy.Destroy();      
            }
        }
    }

    private void GenerateParticles(Vector2Int position)
    {
        ParticleSystem system = Object.Instantiate(_shootParticles).GetComponent<ParticleSystem>();
        system.gameObject.transform.position = new Vector3(position.x, position.y, -0.1f);
        system.Play();
        _game.CoroutineStart(DestroyShootParticles(system));
    }

    private IEnumerator DestroyShootParticles(ParticleSystem system)
    {
        yield return new WaitForSeconds(system.main.duration);
        Object.Destroy(system.gameObject);
    }
}
