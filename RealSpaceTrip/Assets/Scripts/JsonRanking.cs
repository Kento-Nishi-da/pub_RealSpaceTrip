using System;
using System.Text;

/// <summary>
/// �����L���O���
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
        ret.AppendLine("  datas�F[");
        foreach (var row in datas)
        {
            ret.AppendLine("  {");
            ret.AppendLine("    name�F" + row.name);
            ret.AppendLine("    score�F" + row.score);
            ret.AppendLine("  },");
        }

        ret.AppendLine("  ]");
        ret.AppendLine("}");
        return ret.ToString();
    }


}

/// <summary>
/// �����L���O����̃f�[�^
/// </summary>
[Serializable]
public class JsonRankData
{
    public string name;
    public string score;
}