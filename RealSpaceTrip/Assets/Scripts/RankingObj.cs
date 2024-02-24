using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingObj : MonoBehaviour
{
    // 順位
    Text textRank;
    // 名前
    public Text textName;
    // スコア
    public Text textScore;

    // 実際の値
    public string pName;
    public float score;


    public void Init()
    {
        var tmp = GetComponentsInChildren<Text>();
        textRank = tmp[0];
        textName = tmp[1];
        textScore = tmp[2];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_no"></param>
    /// <param name="_name"></param>
    /// <param name="_score"></param>
    public void DispRankInfo(int _no, string _name, float _score)
    {
        pName = _name;
        score = _score;

        textRank.text = _no + ".";
        textName.text = _name;
        textScore.text = _score.ToString("F2");
    }

    /// <summary>
    /// 引数なしオーバーロード
    /// </summary>
    public void DispRankInfo()
    {
        if(score < 0) gameObject.SetActive(false);
        Debug.Log("名前：" + pName + "　　スコア：" + score.ToString("F2"));
        textName.text = pName;
        textScore.text = score.ToString("F2");
    }
}
