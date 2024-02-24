using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全アイテムとお邪魔の基底クラス
/// 移動速度を持ち、基本的に落下してくる
/// </summary>
public class ObjectBase : MonoBehaviour
{
    // 派生クラスでアイテムか障害物かを記述
    // 上からまっすぐ降ってくる丸型の岩、コイン、無敵アイテム


    protected float moveSpeed;

    void Start()
    {
        Init();
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        Move();
    }

    /// <summary>
    /// 初期化処理、オーバーライドして使う
    /// </summary>
    virtual protected void Init() { }

    /// <summary>
    /// 移動処理
    /// </summary>
    virtual protected void Move()
    {
        transform.position += new Vector3(0, -moveSpeed, 0) * Time.deltaTime;
    }

    /// <summary>
    /// 画面外に消える処理、地球に当たったら自壊
    /// </summary>
    /// <param name="collision"></param>
    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Earth")
        {
            Destroy(gameObject);
        }
    }
}
