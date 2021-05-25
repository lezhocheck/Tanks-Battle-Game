using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    private Coroutine _movement;
    private float _stepSpeed = 0.2f;
    private Collider2D _collider;
    private ParticleSystem _gunParticles;
    private AudioSource _gunAudio;

    public Player Player;
    
    public Game game;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        var systems = transform.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < systems.Length; i++)
        {
            if (systems[i].isStopped)
            {
                _gunParticles = systems[i];
                break;
            }
        }

        var audios = transform.GetComponentsInChildren<AudioSource>();
        
        foreach (var source in audios)
        {
            if (source.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                _gunAudio = source;
                break;
            }
        }
    }

    private void Update()
    {
        if (_movement == null && !game.IsGameFinished)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                _movement = StartCoroutine(Move(Vector2Int.up));
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                transform.eulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
                _movement = StartCoroutine(Move(Vector2Int.down));
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
                _movement = StartCoroutine(Move(Vector2Int.left));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {  
                transform.eulerAngles = new Vector3(0.0f, 0.0f, -90.0f);
                _movement = StartCoroutine(Move(Vector2Int.right));
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {  
                Shoot(false);
            }
            else if (Input.GetKeyDown(KeyCode.R) && game.HardBulletsCount > 0)
            {
                Shoot(true);
                game.HardBulletsCount--;
            }
        }
    }
    
    private IEnumerator Move(Vector2Int direction)
    {
        if (CheckNextCell())
        {
            Vector2 position = transform.position;
            Vector2 nextPosition = position + direction;

            Vector2Int pos = new Vector2Int((int)position.x, (int)position.y);
            Vector2Int nextPos = new Vector2Int((int)nextPosition.x, (int)nextPosition.y);
            game.GameField[pos.y, pos.x].IsOccupied = false;
            game.GameField[nextPos.y, nextPos.x].IsOccupied = true;
            float step = 0.0f;

            while (step < 1.0f)
            {
                transform.position = Vector2.Lerp(position, nextPosition, step);
                step += Time.deltaTime / _stepSpeed;
                yield return new WaitForEndOfFrame();
            }

            transform.position = nextPosition;
            _movement = null;   
        }
    }

    private bool CheckNextCell()
    {
        Vector2Int pos = Player.Position + new Vector2Int((int)transform.up.x, (int)transform.up.y);
        Cell nextCell = game.GameField[pos.y, pos.x];
        if (!nextCell.IsOccupied)
            return true; 

        return false;
    }

    private void Shoot(bool bigDamageable)
    {
        Bullet b = new Bullet(_collider, false, bigDamageable,
            game, transform.position, transform.rotation, transform.up);
        
        _gunParticles.Play();
        _gunAudio.Play();
    }
}
