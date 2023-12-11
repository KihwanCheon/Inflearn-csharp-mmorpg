using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define.CreatureState;
using static Define.MoveDir;

public class MonsterController : CreatureController
{
    protected override void Init()
    {
        base.Init(); // call first.

        State = Idle;
        Dir = None;
    }

    protected override void UpdateController()
    {
        // GetDirInput();
        base.UpdateController();
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
            Dir = Define.MoveDir.None;
        }
    }
}
