using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConst;

public class Generator : MonoBehaviour
{
    /// <summary>
    /// オブジェクトの生成
    /// </summary>
    public void Generate(GameObject _prefab)
    {
        // ランダムに生成座標決定
        var tmpX = Random.Range(-2.5f, 2.5f);
        var tmpY = Random.Range(6f, 10f);

        Vector3 vec = new Vector3(tmpX, tmpY, 0);

        Instantiate(_prefab, vec, Quaternion.identity);
    }
}
