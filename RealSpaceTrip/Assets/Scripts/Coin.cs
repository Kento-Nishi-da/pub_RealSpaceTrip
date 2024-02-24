using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : ObjectBase
{
    public int amount { get; protected set; }

    protected override void Init()
    {
        moveSpeed = 1.7f;
        gameObject.tag = "Coin";
        // �l���ł�����z�������_����
        amount = (Random.Range(0, 10) * 5) + 5;
    }
}
