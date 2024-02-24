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
    /// ‰ñ“]‚³‚¹‚È‚ª‚ç—Ž‰º‚µ‚Ä‚­‚é
    /// </summary>
    void Rotation()
    {
        transform.Rotate(0, 0, 1);
    }


}
