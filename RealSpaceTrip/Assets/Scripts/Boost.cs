using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : ObjectBase
{
    protected override void Init()
    {
        moveSpeed = 1.7f;
        gameObject.tag = "Boost";
    }
    protected override void Update()
    {
        base.Update();
    }
}
