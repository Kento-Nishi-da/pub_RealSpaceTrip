using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using CommonConst;
using UnityEngine.UI;
using System.Security.Cryptography;

public class TitleManager : MonoBehaviour
{
    // todo:2024/1/9のAPODがそれっぽくてかっこいいのでサンプルにしよう

    [Header("通信中の表示")]

    [Tooltip("通信中のパネル")]
    [SerializeField] GameObject loadingPanel;
    [Tooltip("通信中の文字")]
    [SerializeField] Text loadingText;
    // 文字を変更するタイマー
    float timer;
    // 文字の変更量
    int cnt;
    // 通信中かどうか
    bool isLoading;


    [Header("ランキング機能")]

    [Tooltip("ランキングオブジェクトの親")]
    [SerializeField] Transform rankingParent;
    [Tooltip("ランキング情報Prefab")]
    [SerializeField] GameObject rankingPrefab;
    [Tooltip("ランキングパネル")]
    [SerializeField] GameObject rankingPanel;


    [Header("コスチューム")]

    [Tooltip("コスチュームパネル")]
    [SerializeField] GameObject costumePanel;
    [Tooltip("デフォルトボタン")]
    [SerializeField] GameObject costumeDefault;
    [Tooltip("カラー1ボタン")]
    [SerializeField] GameObject costumeColor1;
    [Tooltip("選択中の強調")]
    [SerializeField] Image selectedObj;
    [Tooltip("購入隠し")]
    [SerializeField] GameObject buyPanel;
    [Tooltip("お金テキスト")]
    [SerializeField] Text costumeMoneyText;
    [Tooltip("コスチュームプレイヤー")]
    [SerializeField] SpriteRenderer playerSpriteRenderer;



    [Header("ギャラリー機能")]

    [Tooltip("ギャラリーパネル")]
    [SerializeField] GameObject galleryPanel;
    [Tooltip("APOD表示RawImage")]
    [SerializeField] RawImage apodRawImage;
    [Tooltip("APODタイトルテキスト")]
    [SerializeField] Text apodTitleText;
    [Tooltip("APOD説明テキスト")]
    [SerializeField] Text apodExText;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        StartCoroutine(Init());

        yield return null;
    }

    private void Update()
    {
        if (isLoading) loadingText.text = Common.LoadingMsgDisp(ref timer, ref cnt);

        if(Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteKey(Common.IS_COS_COLOR01_PREFS_KEY);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayerPrefs.DeleteKey(Common.MONEY_PREFS_KEY);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShareData.instance.Money += 500;
            PlayerPrefs.SetInt(Common.MONEY_PREFS_KEY, ShareData.instance.Money);
            costumeMoneyText.text = "コイン枚数：" + ShareData.instance.Money;
        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    IEnumerator Init()
    {
        // 初期化
        galleryPanel.SetActive(false);
        loadingPanel.SetActive(true);
        rankingPanel.SetActive(false);
        costumePanel.SetActive(false);
        timer = 0;
        cnt = 0;
        isLoading = true;

        // お金情報を読み込む、表示する、shareに突っ込む
        if(PlayerPrefs.HasKey(Common.MONEY_PREFS_KEY))
        {
            ShareData.instance.Money = PlayerPrefs.GetInt(Common.MONEY_PREFS_KEY);
            print("お金あったよ" + ShareData.instance.Money);
        }
        else
        {
            PlayerPrefs.SetInt(Common.MONEY_PREFS_KEY, 0);
            ShareData.instance.Money = PlayerPrefs.GetInt(Common.MONEY_PREFS_KEY);
        }

        // コスチュームの設定
        if (PlayerPrefs.HasKey(Common.IS_COS_COLOR01_PREFS_KEY))
        {
            var tmp = PlayerPrefs.GetInt(Common.IS_COS_COLOR01_PREFS_KEY);
            if(tmp == 0)
            {
                buyPanel.SetActive(true);
            }
            else
            {
                buyPanel.SetActive(false);
            }

        }
        else
        {
            PlayerPrefs.SetInt(Common.IS_COS_COLOR01_PREFS_KEY, 0);
        }

        // コスチュームの選択設定
        if (PlayerPrefs.HasKey(Common.SELECT_COS_PREFS_KEY))
        {
            var tmp = PlayerPrefs.GetInt(Common.SELECT_COS_PREFS_KEY);
            if (tmp == 1)
            {
                PushCostumeColor1();
                ShareData.instance.Color = 1;
            }

        }

        costumeMoneyText.text = "コイン枚数：" + ShareData.instance.Money;


        // APODの通信処理、API不具合のため分岐
#if true
        Debug.Log("全通信処理開始");

        // 通信処理後、データをキャッシュ
        if (ShareData.instance.APOD == null)
        {
            yield return StartCoroutine(GetAPOD());
        }

#else
        // APIが突如動かなくなったのでそのまま代入

        // 通信処理後、データをキャッシュ
        if (ShareData.instance.APOD == null)
        {
            var jsonAPOD = new JsonAPOD();
            jsonAPOD.title = "Rosette Deep Field";
            jsonAPOD.url = "https://apod.nasa.gov/apod/image/2402/RosetteCone_Bernard_5398.jpg";
            jsonAPOD.explanation = "Can you find the Rosette Nebula? The large, red, and flowery-looking nebula on the upper left may seem the obvious choice, but that is actually just diffuse hydrogen emission surrounding the Cone and Fox Fur Nebulas. The famous Rosette Nebula is really located on the lower right and connected to the other nebulas by irregular filaments. Because the featured image of Rosetta's field is so wide and deep, it seems to contain other flowers. Designated NGC 2237, the center of the Rosette nebula is populated by the bright blue stars of open cluster NGC 2244, whose winds and energetic light are evacuating the nebula's center. The Rosette Nebula is about 5,000 light years distant and, just by itself, spans about three times the diameter of a full moon. This flowery field can be found toward the constellation of the Unicorn (Monoceros).";

            // 画像取得は何故かできるので通信して取得
            // コルーチンには参照渡しができないので汎用化するために配列で参照を渡す
            var tmpRef = new Texture2D[1];
            yield return StartCoroutine(HTTPCommunication.TextureCommunication(jsonAPOD.url, tmpRef));
            
            // データ取り出し
            jsonAPOD.texture = tmpRef[0];

            // 英文翻訳のため通信
            // URL作成
            var param = "?title=" + jsonAPOD.title + "&explanation=" + jsonAPOD.explanation + "&target_language=" + Common.LANGUAGE_JA;
            var coroutine = HTTPCommunication.Communication(Common.TRANSLATE_BASE_URL + param);
            // 非同期通信
            yield return StartCoroutine(coroutine);
            var res = coroutine.Current.ToString();
            // Jsonデータをシリアライズ
            var jsonTranslate = JsonUtility.FromJson<JsonTranslate>(res);
            print("英文翻訳完了");

            // データのキャッシュ
            ShareData.instance.APOD = jsonAPOD;
            ShareData.instance.Translated = jsonTranslate;
        }
#endif
        // キャッシュされたデータの読み込み
        apodTitleText.text = ShareData.instance.Translated.translated_title;
        apodExText.text = ShareData.instance.Translated.translated_ex;
        apodRawImage.texture = ShareData.instance.APOD.texture;

        Debug.Log("全通信処理終了");
        Debug.Log("データはキャッシュにあります");


        if (ShareData.instance.Ranking == null)
        {
            yield return StartCoroutine(GetRanking());
        }


        isLoading = false;
        loadingPanel.SetActive(false);

        //print(ShareData.instance.Translated.ToString());



        DispRanking();
    }

    /// <summary>
    /// ランキングオブジェクトの表示
    /// </summary>
    void DispRanking()
    {
        // ランキングオブジェクト生成
        var rankingObjs = new GameObject[ShareData.instance.Ranking.datas.Length];

        for (int i = 0; i < rankingObjs.Length; i++)
        {
            rankingObjs[i] = Instantiate(rankingPrefab, rankingParent);

            if (rankingObjs[i].TryGetComponent<RankingObj>(out var obj))
            {
                obj.Init();
                obj.DispRankInfo(
                    i + 1,
                    ShareData.instance.Ranking.datas[i].name,
                    float.Parse(ShareData.instance.Ranking.datas[i].score)
                );
                if (obj.score < 0)
                {
                    rankingObjs[i].SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// APOD取得処理
    /// </summary>
    /// <returns></returns>
    IEnumerator GetAPOD()
    {
        // 通信URL作成
        var param = "?api_key=" + Common.MY_KEY;
        var coroutine = HTTPCommunication.Communication(Common.NASA_BASE_URL + param);
        print(Common.NASA_BASE_URL + param);
        // 非同期通信
        yield return StartCoroutine(coroutine);
        var res = coroutine.Current.ToString();
        // Jsonデータをシリアライズ
        var jsonAPOD = JsonUtility.FromJson<JsonAPOD>(res);
        print("APOD通信完了");



        // 画像取得のため再度通信
        // コルーチンには参照渡しができないので汎用化するために配列で参照を渡す
        var tmpRef = new Texture2D[1];
        yield return StartCoroutine(HTTPCommunication.TextureCommunication(jsonAPOD.url, tmpRef));
        // データ取り出し
        jsonAPOD.texture = tmpRef[0];
        print("画像取得完了");



        // 英文翻訳のため再度通信
        // URL作成
        param = "?title=" + jsonAPOD.title + "&explanation=" + jsonAPOD.explanation + "&target_language=" + Common.LANGUAGE_JA;
        coroutine = HTTPCommunication.Communication(Common.TRANSLATE_BASE_URL + param);
        // 非同期通信
        yield return StartCoroutine(coroutine);
        res = coroutine.Current.ToString();
        // Jsonデータをシリアライズ
        var jsonTranslate = JsonUtility.FromJson<JsonTranslate>(res);
        print("英文翻訳完了");


        // データのキャッシュ
        ShareData.instance.APOD = jsonAPOD;
        ShareData.instance.Translated = jsonTranslate;


        //// 取得テキストを表示
        //print(ShareData.instance.APOD.ToString());
    }


    /// <summary>
    /// ランキング取得処理
    /// </summary>
    /// <returns></returns>
    IEnumerator GetRanking()
    {
        // 通信URL作成
        var coroutine = HTTPCommunication.Communication(Common.GET_RANKING_URL);
        // 非同期通信
        yield return StartCoroutine(coroutine);
        var res = coroutine.Current.ToString();
        print(res);

        // Jsonデータをシリアライズ
        var jsonRanking = JsonUtility.FromJson<JsonRanking>(res);

        // データのキャッシュ
        ShareData.instance.Ranking = jsonRanking;

        //// 取得テキストを表示
        //print(ShareData.instance.RANKING.ToString());
    }


    public void PushStartButton()
    {
        Debug.Log("ゲームスタート");
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// ランキングボタン押下時
    /// </summary>
    public void PushRankingButton()
    {
        Debug.Log("ランキング");
        rankingPanel.SetActive(true);

    }

    /// <summary>
    /// ランキング戻るボタン押下時
    /// </summary>
    public void PushRankingBackButton()
    {
        rankingPanel.SetActive(false);
    }

    public void PushCostumeButton()
    {
        Debug.Log("コスチューム");
        costumePanel.SetActive(true);
    }

    public void PushCostumeBackButton()
    {
        costumePanel.SetActive(false);
    }

    public void PushCostumeDefault()
    {
        selectedObj.transform.position = costumeDefault.transform.position;
        ShareData.instance.Color = 0;
        playerSpriteRenderer.color = Color.white;
        PlayerPrefs.SetInt(Common.SELECT_COS_PREFS_KEY, 0);
    }

    public void PushCostumeColor1()
    {
        selectedObj.transform.position = costumeColor1.transform.position;
        ShareData.instance.Color = 1;
        playerSpriteRenderer.color = Color.green;
        PlayerPrefs.SetInt(Common.SELECT_COS_PREFS_KEY, 1);
    }

    public void PushBuy()
    {
        if(ShareData.instance.Money >= 500)
        {
            ShareData.instance.Money -= 500;
            PlayerPrefs.SetInt(Common.MONEY_PREFS_KEY, ShareData.instance.Money);
            buyPanel.SetActive(false);
            costumeMoneyText.text = "コイン枚数：" + ShareData.instance.Money;
            PlayerPrefs.SetInt(Common.IS_COS_COLOR01_PREFS_KEY, 1);
        }

    }

    /// <summary>
    /// ギャラリーボタン押下時
    /// </summary>
    public void PushGalleryButton()
    {
        Debug.Log("ギャラリー");
        galleryPanel.SetActive(true);
    }

    /// <summary>
    /// ギャラリー戻るボタン押下時
    /// </summary>
    public void PushGalleryBackButton()
    {
        galleryPanel.SetActive(false);
    }
}
