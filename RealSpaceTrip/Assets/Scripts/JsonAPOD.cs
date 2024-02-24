using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// APODの翻訳前データ
/// </summary>
[Serializable]
public class JsonAPOD
{
    public string date;         // "2024-02-11",
    public string explanation;  // "Why would the shadow of a rocket's launch plume point toward the Moon?  In early 2001 during a launch of the space shuttle Atlantis, the Sun, Earth, Moon, and rocket were all properly aligned for this photogenic coincidence.  First, for the space shuttle's plume to cast a long shadow, the time of day must be either near sunrise or sunset.  Only then will the shadow be its longest and extend all the way to the horizon.  Finally, during a Full Moon, the Sun and Moon are on opposite sides of the sky.  Just after sunset, for example, the Sun is slightly below the horizon, and, in the other direction, the Moon is slightly above the horizon.  Therefore, as Atlantis blasted off, just after sunset, its shadow projected away from the Sun toward the opposite horizon, where the Full Moon happened to be.    Almost Hyperspace: Random APOD Generator",
    public string title;        // "Rocket Plume Shadow Points to the Moon",
    public string url;          // "https://apod.nasa.gov/apod/image/2402/sts98plume_nasa_960.jpg"
    public Texture2D texture;   // urlから取得した画像



    public override string ToString()
    {
        var ret = new StringBuilder();

        ret.AppendLine("{");
        ret.AppendLine("  date(日付)：" + date);
        ret.AppendLine("  explanation(説明文)：" + explanation);
        ret.AppendLine("  title(タイトル)：" + title);
        ret.AppendLine("  url：" + url);
        ret.AppendLine("}");


        return ret.ToString();
    }

}


/// <summary>
/// 翻訳後の文章
/// </summary>
[Serializable]
public class JsonTranslate
{
    public string response;
    public string translated_title;
    public string translated_ex;

    public override string ToString()
    {
        var ret = new StringBuilder();

        ret.AppendLine("{");
        ret.AppendLine("  response：" + response);
        ret.AppendLine("  trans_title：" + translated_title);
        ret.AppendLine("  trans_ex：" + translated_ex);
        ret.AppendLine("}");


        return ret.ToString();
    }
}
