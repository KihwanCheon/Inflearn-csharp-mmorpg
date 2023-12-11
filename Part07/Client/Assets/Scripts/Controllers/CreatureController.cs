using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Define;
using static Define.CreatureState;
using static Define.MoveDir;


public class CreatureController : MonoBehaviour
{
    public float _speed = 5.0f;

    protected Vector3Int _cellPos = Vector3Int.zero; // destination position.
    protected Animator _animator;
    protected CreatureState _state = Idle;

    public CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;
             
            _state = value;
        }
    }

    protected MoveDir _dir = Down;
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
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateController();
    }

    protected virtual void Init()
    {
        _animator = GetComponent<Animator>();
        Vector3 pos = Managers.Map.CurreGrid.CellToWorld(_cellPos) + /*player init position */ new Vector3(0.5f, 0.5f, 0);
        transform.position = pos;
    }

    protected virtual void UpdateController()
    {
        UpdatePosition();

        UpdateIfIsMoving();
    }


    /// <summary>
    /// 이동 연출.
    /// </summary>
    void UpdatePosition()
    {
        if (State != Moving)
            return;

        Vector3 destPos = Managers.Map.CurreGrid.CellToWorld(_cellPos) + /*player init position */ new Vector3(0.5f, 0.5f, 0);
        Vector3 moveDir = destPos - transform.position;

        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            State = Idle;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = Moving;
        }
    }

    /// <summary>
    /// 로직 내부적으로 이동(연출 목적지 설정)
    /// </summary>
    void UpdateIfIsMoving()
    {
        if (State == Moving || Dir == None)
            return;

        var destPos = _cellPos;

        switch (Dir)
        {
            case Up: destPos += Vector3Int.up; break;
            case Down: destPos += Vector3Int.down; break;
            case Left: destPos += Vector3Int.left; break;
            case Right: destPos += Vector3Int.right; break;
        }

        if (Managers.Map.CanGo(destPos))
        {
            _cellPos = destPos;
            State = Moving;
        }
    }
}
