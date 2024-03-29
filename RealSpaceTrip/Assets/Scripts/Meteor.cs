using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : ObjectBase
{
    protected override void Init()
    {
        moveSpeed = 2;
        gameObject.tag = "Meteor";
    }
    protected override void Update()
    {
        base.Update();

        Rotation();
    }

    /// <summary>
    /// 回転させながら落下してくる
    /// </summary>
    void Rotation()
    {
        transform.Rotate(0, 0, 1);
    }


}
