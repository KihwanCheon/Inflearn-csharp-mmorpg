using UnityEngine;
using static Define;
using static Define.MoveDir;

public class PlayerController : MonoBehaviour
{
    public Grid _grid;
    public float _speed = 5.0f;


    Vector3Int _cellPos = Vector3Int.zero;
    MoveDir _dir = None;
    bool _isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = _grid.CellToWorld(_cellPos);
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        GetDirInput();

        if (_isMoving == false)
        {
            switch (_dir)
            {
                
            }
        }
    }

    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            // transform.position += Vector3.up * Time.deltaTime * _speed;
            _dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // transform.position += Vector3.down * Time.deltaTime * _speed;
            _dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            // transform.position += Vector3.left * Time.deltaTime * _speed;
            _dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // transform.position += Vector3.right * Time.deltaTime * _speed;
            _dir = MoveDir.Right;
        }
        else
        {
            _dir = MoveDir.None;
        }
    }
}