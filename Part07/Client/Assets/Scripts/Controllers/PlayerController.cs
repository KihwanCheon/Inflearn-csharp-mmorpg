using System;
using UnityEngine;
using static Define;
using static Define.MoveDir;

public class PlayerController : MonoBehaviour
{
    public float _speed = 5.0f;

    Vector3Int _cellPos = Vector3Int.zero; // destination position.
    bool _isMoving = false;
    Animator _animator;

    MoveDir _dir = Down; 
    // MoveDir _dir = Left; // If Animator default value is WALK_RIGHT.

    public MoveDir Dir
    {
        get => _dir;
        set
        {
            if (_dir == value)
                return;

            switch (value)
            {
                case Up:
                    _animator.Play("WALK_BACK");
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); break;
                case Down:
                    _animator.Play("WALK_FRONT");
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                case Left:
                    _animator.Play("WALK_RIGHT");
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    break;
                case Right:
                    _animator.Play("WALK_RIGHT");
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                case None:
                {
                    switch (_dir)
                    {
                        case Up:
                            _animator.Play("IDLE_BACK");
                            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                            break;
                        case Down:
                            _animator.Play("IDLE_FRONT");
                            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                            break;
                        case Left:
                            _animator.Play("IDLE_RIGHT");
                            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                            break;
                        case Right:
                        default:
                            _animator.Play("IDLE_RIGHT");
                            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                            break;
                    }
                }
                break;
            }

            _dir = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        Vector3 pos = Managers.Map.CurreGrid.CellToWorld(_cellPos) + /*player init position */ new Vector3(0.5f, 0.5f, 0);
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        GetDirInput();

        UpdatePosition();

        UpdateIfIsMoving();
    }

    /// <summary>
    /// 이동 연출.
    /// </summary>
    void UpdatePosition()
    {
        if (!_isMoving)
            return;

        Vector3 destPos = Managers.Map.CurreGrid.CellToWorld(_cellPos) + /*player init position */ new Vector3(0.5f, 0.5f, 0);
        Vector3 moveDir = destPos - transform.position;

        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            _isMoving = false;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            //_isMoving = true;
        }
    }

    /// <summary>
    /// 로직 내부적으로 이동(연출 목적지 설정)
    /// </summary>
    void UpdateIfIsMoving()
    {
        if (_isMoving || Dir == None)
            return;

        var destPos = _cellPos;

        switch (Dir)
        {
            case Up:    destPos += Vector3Int.up;    break;
            case Down:  destPos += Vector3Int.down;  break;
            case Left:  destPos += Vector3Int.left;  break;
            case Right: destPos += Vector3Int.right; break;
        }

        if (Managers.Map.CanGo(destPos))
        {
            _cellPos = destPos;
            _isMoving = true;
        }
    }

    /// <summary>
    /// 키보드 입력 처리. 
    /// </summary>
    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }
}