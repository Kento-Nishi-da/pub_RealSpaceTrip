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
    /// ��]�����Ȃ��痎�����Ă���
    /// </summary>
    void Rotation()
    {
        transform.Rotate(0, 0, 1);
    }


}
