using System;
using UnityEngine;
using static Define;
using static Define.MoveDir;

public class PlayerController : CreatureController
{
    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {
        GetDirInput();
        base.UpdateController();
    }

    void LateUpdate()
    {
        // var z = Camera.main.transform.position.z;
        // Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, z);
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10); // not change camera z.
    }

    /// <summary>
    /// 키보드 입력 처리. 
    /// </summary>
    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Dir = Up;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Dir = Down;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            Dir = Left;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Dir = Right;
        }
        else
        {
            Dir = None;
        }
    }
}