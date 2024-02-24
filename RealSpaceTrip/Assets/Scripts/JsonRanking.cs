using System;
using System.Text;

/// <summary>
/// ランキング情報
/// </summary>
[Serializable]
public class JsonRanking
{
    public JsonRankData[] datas;


    public JsonRanking() { datas = new JsonRankData[10]; }


    public override string ToString()
    {
        var ret = new StringBuilder();
        ret.AppendLine("{");
        ret.AppendLine("  datas：[");
        foreach (var row in datas)
        {
            ret.AppendLine("  {");
            ret.AppendLine("    name：" + row.name);
            ret.AppendLine("    score：" + row.score);
            ret.AppendLine("  },");
        }

        ret.AppendLine("  ]");
        ret.AppendLine("}");
        return ret.ToString();
    }


}

/// <summary>
/// ランキング一個分のデータ
/// </summary>
[Serializable]
public class JsonRankData
{
    public string name;
    public string score;
}