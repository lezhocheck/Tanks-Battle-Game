using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    private const float ShootTime = 1f;

    private Collider2D _collider;
    private ParticleSystem _gunParticles;
    private AudioSource _gunAudio;
    private Image _progressBar;
    private float _startHp;
    
    private Coroutine _movement;
    private float _stepSpeed = 0.2f;
    
    public float Step { get; private set; }
    public Enemy Enemy { get; set; }

    private Game _game;
    private GameField _gameField;
    
    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _startHp = Enemy.Hp;
        _game = Enemy.Game;
        _gameField = Enemy.GameField;
        
        var systems = transform.GetComponentsInChildren<ParticleSystem>();
        foreach (var system in systems)
        {
            if (system.isStopped)
            {
                _gunParticles = system;
                break;
            }
        }

        _gunAudio = transform.GetComponentInChildren<AudioSource>();
        
        var images = transform.GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            if (image.type == Image.Type.Filled)
            {
                _progressBar = image;
                break;
            }
        }

        StartCoroutine(Move());
        StartCoroutine(Shoot());
    }
    
    void Update()
    {
        _progressBar.fillAmount = Enemy.Hp / _startHp;
    }

    private IEnumerator Shoot()
    {
        while (!_game.IsGameFinished)
        {
            Bullet b = new Bullet(_collider, true, false, _game, transform.position, transform.rotation, 
                transform.up);
            _gunParticles.Play();
            _gunAudio.Play();
            yield return new WaitForSeconds(ShootTime);   
        }
    }
    
    public IEnumerator Move()
    {
        while (!_game.IsGameFinished)
        {
            if (!Attack())
            {
                Vector2Int forward = Enemy.Position + new Vector2Int(Mathf.RoundToInt(transform.up.x), Mathf.RoundToInt(transform.up.y));
                Vector2Int left = Enemy.Position - new Vector2Int(Mathf.RoundToInt(transform.right.x), Mathf.RoundToInt(transform.right.y));
                Vector2Int right = Enemy.Position + new Vector2Int(Mathf.RoundToInt(transform.right.x),Mathf.RoundToInt(transform.right.y));
                Vector2Int back = Enemy.Position - new Vector2Int(Mathf.RoundToInt(transform.up.x), Mathf.RoundToInt(transform.up.y));

                if (!_gameField[forward.y, forward.x].IsOccupied)
                {
                    _movement = StartCoroutine(MoveNext(forward));
                }
                else if (!_gameField[left.y, left.x].IsOccupied)
                {
                    _movement = StartCoroutine(MoveNext(left));
                    transform.eulerAngles = transform.eulerAngles + new Vector3(0.0f, 0.0f, 90.0f);
                }
                else if (!_gameField[back.y, back.x].IsOccupied)
                {
                    _movement = StartCoroutine(MoveNext(back));
                    transform.eulerAngles = transform.eulerAngles +  new Vector3(0.0f, 0.0f, 180.0f);
                }
                else if (!_gameField[right.y, right.x].IsOccupied)
                {
                    _movement = StartCoroutine(MoveNext(right));
                    transform.eulerAngles = transform.eulerAngles + new Vector3(0.0f, 0.0f, -90.0f);
                }
            }
                 
            yield return new WaitForSeconds(1f);   
        }
    }

    private IEnumerator MoveNext(Vector2Int nextPos)
    {
        Vector2 position = transform.position;
        Vector2Int pos = new Vector2Int((int)position.x, (int)position.y);
        Vector2Int nextPosition = nextPos;
        _gameField[pos.y, pos.x].IsOccupied = false;
        _gameField[nextPos.y, nextPos.x].IsOccupied = true;

        while (Step < 1.0f)
        {
            transform.position = Vector2.Lerp(position, nextPosition, Step);
            Step += Time.deltaTime / _stepSpeed;
            yield return new WaitForEndOfFrame();
        }

        transform.position = new Vector3(nextPosition.x, nextPosition.y, 0.0f);
        Step = 0.0f;
        _movement = null;    
    }

    private bool Attack()
    {
        Vector2 position = transform.position;
        Vector2Int pos = new Vector2Int((int)position.x, (int)position.y);
        Vector2Int playerPos = _game.Player.Position;
        
        if (playerPos.x < pos.x && playerPos.y == pos.y)
        {
            for (int i = pos.x; i >= playerPos.x; i--)
            {
                if (_gameField[pos.y, i].Type != Cell.CType.Empty && _gameField[pos.y, i].Type != Cell.CType.DestroyedTile)
                {
                    return false;   
                }
            }
            
            transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
            return true;
        }
        else if (playerPos.x > pos.x && playerPos.y == pos.y)
        {
            for (int i = pos.x; i <= playerPos.x; i++)
            {
                if (_gameField[pos.y, i].Type != Cell.CType.Empty && _gameField[pos.y, i].Type != Cell.CType.DestroyedTile)
                {
                    return false;   
                }
            }
            
            transform.eulerAngles =  new Vector3(0.0f, 0.0f, -90.0f);
            return true; 
        }
        else if (playerPos.y < pos.y && playerPos.x == pos.x)
        {
            for (int i = pos.y; i >= playerPos.y; i--)
            {
                if (_gameField[i, pos.x].Type != Cell.CType.Empty && _gameField[i, pos.x].Type != Cell.CType.DestroyedTile)
                {
                    return false;   
                }
            }
            
            transform.eulerAngles =  new Vector3(0.0f, 0.0f, 180.0f);
            return true;
        }
        else if (playerPos.y > pos.y && playerPos.x == pos.x)
        {
            for (int i = pos.y; i <= playerPos.x; i++)
            {
                if (_gameField[i, pos.x].Type != Cell.CType.Empty && _gameField[i, pos.x].Type != Cell.CType.DestroyedTile)
                {
                    return false;   
                }
            }
            
            transform.eulerAngles =  new Vector3(0.0f, 0.0f, 0.0f);
            return true;
        }
        
        return false;
    }
}
