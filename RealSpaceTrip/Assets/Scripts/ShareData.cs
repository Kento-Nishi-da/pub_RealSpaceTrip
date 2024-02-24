using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareData : MonoBehaviour
{
    public static ShareData instance;

    JsonRanking data;

    JsonAPOD apod;

    JsonTranslate translated;

    int money = 0;

    int playerColor;

    private void Awake()
    {
        // ƒVƒ“ƒOƒ‹ƒgƒ“
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
            return;
        }
    }


    public JsonRanking Ranking
    {
        get => data;
        set => data = value;

    }

    public JsonAPOD APOD
    {
        get => apod;
        set => apod = value;
    }

    public JsonTranslate Translated
    {
        get => translated;
        set => translated = value;
    }

    public int Money
    {
        get => money;
        set => money = value;
    }

    public int Color
    {
        get => playerColor;
        set => playerColor = value;
    }
}
