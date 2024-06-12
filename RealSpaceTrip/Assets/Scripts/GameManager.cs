using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CommonConst;
using System;

public class GameManager : MonoBehaviour
{
    [Header("ゲーム開始前")]
    [Tooltip("カウントダウンテキスト")]
    [SerializeField] Text countText;

    [Header("ゲーム中のオブジェクト")]

    [Tooltip("背景")]
    [SerializeField] RawImage backRawImage;



    [Header("フェードアウト")]

    [Tooltip("ゲーム終了時のフェードアウトパネル")]
    [SerializeField] GameObject fadeOutPanel;



    [Header("リザルト")]

    [Tooltip("リザルト表示用パネル")]
    [SerializeField] GameObject resultPanel;
    [Tooltip("今回のスコアのテキスト")]
    [SerializeField] Text resultScoreText;
    [Tooltip("ランキング情報の生成の親オブジェクト")]
    [SerializeField] Transform rankingParent;



    [Header("ハイスコア")]

    [Tooltip("名前入力パネル")]
    [SerializeField] GameObject nameInputPanel;
    [Tooltip("名前入力欄")]
    [SerializeField] InputField nameInputField;
    [Tooltip("名前入力を促すテキスト")]
    [SerializeField] Text nameInfoText;



    [Header("リプレイ")]
    [Tooltip("コンティニューかリプレイを促すパネル")]
    [SerializeField] GameObject continuePanel;

    [Header("UI")]
    [Tooltip("スコア表示")]
    [SerializeField] Text scoreText;
    [Tooltip("お金表示")]
    [SerializeField] Text moneyText;



    Player pl;
    Generator gr;

    GameObject meteorPrefab;
    GameObject coinPrefab;
    GameObject boostPrefab;

    GameObject rankObjPrefab;
    GameObject[] rankingObjs;

    // ゲームの進行状況
    [Header("ゲームの状態")]

    [Tooltip("ゲームの状態")]
    public GameState state;

    // ゲーム開始前のカウントダウン量
    float startCount = 3;


    string playerName;
    // フェードアウトにかかる時間
    const float FADE_OUT_TIME = 2.0f;

    // オブジェクト生成する間隔
    const float GENERATE_TIME = 2f;
    // オブジェクト生成のタイマー
    float objectTimer;

    // ハイスコア時のアニメーション用
    int playerRankIndex;
    bool blinkFlg;
    float blinkTimer;
    const float BLINK_ON_TIME = 1f;
    const float BLINK_OUT_TIME = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        // ゲームの状態に合わせて分岐
        switch (state)
        {
            case GameState.START_ANIM:
                // todo:開始時アニメーション
                StartAnimation();
                break;
            case GameState.GAMING:
                // ゲーム中処理
                Gaming();
                break;
            case GameState.FADE_OUT:
                // 終了時フェードアウト
                FadeOut();
                break;
            case GameState.SHOW_RESULT:
                // リザルトパネルの表示、ハイスコアなら名前の入力をさせてスコアの更新
                ShowResult();
                break;
            case GameState.HIGH_SCORE:
            case GameState.WAITING:
                // プレイヤーの入力待ちの間も点滅はさせる
                RankingAnimation();
                break;
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    void Init()
    {
        // コンポーネントを取得
        pl = GameObject.Find("Player").GetComponent<Player>();
        gr = GameObject.Find("Generator").GetComponent<Generator>();

        // 各プレハブの読み込み
        rankObjPrefab = (GameObject)Resources.Load("Prefabs/RankingObj");
        meteorPrefab = (GameObject)Resources.Load("Prefabs/Meteor");
        coinPrefab = (GameObject)Resources.Load("Prefabs/Coin");
        boostPrefab = (GameObject)Resources.Load("Prefabs/Boost");


        if (ShareData.instance.APOD.texture != null)
        {
            backRawImage.texture = ShareData.instance.APOD.texture;
            backRawImage.color = Color.white;
        }

        // あらかじめランキングを読み込んで表示しておく
        DispRanking();

        // 変数初期化
        state = GameState.START_ANIM;
        //state = GameState.GAMING;
        objectTimer = 0;
        fadeOutPanel.SetActive(false);
        resultPanel.SetActive(false);
        nameInputPanel.SetActive(false);
        continuePanel.SetActive(false);
        countText.gameObject.SetActive(true);
        blinkTimer = 0;
        blinkFlg = true;


    }

    /// <summary>
    /// ランキングオブジェクトの表示
    /// </summary>
    void DispRanking()
    {
        if (ShareData.instance == null)
        {
            rankingObjs = new GameObject[10];

            for (int i = 0; i < rankingObjs.Length; i++)
            {
                rankingObjs[i] = Instantiate(rankObjPrefab, rankingParent);

                if (rankingObjs[i].TryGetComponent<RankingObj>(out var obj))
                {
                    obj.Init();
                    Debug.Log("true");

                    obj.DispRankInfo(
                        i + 1,
                        i + "位の人です",
                        (10 - i) * 50
                    );
                    if (obj.score < 0)
                    {
                        rankingObjs[i].SetActive(false);
                    }
                }
            }
        }
        else
        {
            // ランキングオブジェクト生成
            rankingObjs = new GameObject[ShareData.instance.Ranking.datas.Length];
            for (int i = 0; i < rankingObjs.Length; i++)
            {
                rankingObjs[i] = Instantiate(rankObjPrefab, rankingParent);

                if (rankingObjs[i].TryGetComponent<RankingObj>(out var obj))
                {
                    obj.Init();
                    Debug.Log("true");

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



    }

    /// <summary>
    /// 開始時のアニメーション
    /// </summary>
    void StartAnimation()
    {
        // todo:地球を出発して宇宙へ飛び立つようなアニメーション
        startCount -= Time.deltaTime;
        countText.text = startCount.ToString("F0");
        if(startCount < 0)
        {
            if (state == GameState.START_ANIM) state++;
            countText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ゲーム中の処理
    /// </summary>
    void Gaming()
    {
        // プレイヤーのお金、スコア表示
        pl.gameScore += Time.deltaTime;
        scoreText.text = "旅行時間：" + pl.gameScore.ToString("F2");
        moneyText.text = "獲得コイン：+" + pl.gameMoney;


        // 一定時間が経過したらオブジェクト生成
        objectTimer += Time.deltaTime;
        if (objectTimer > GENERATE_TIME)
        {
            objectTimer = 0;
            gr.Generate(meteorPrefab);

            // 隕石発生時に確率でアイテムが発生
            var rand = UnityEngine.Random.Range(0, 10);
            // 確率：1/10
            if(rand < 5)
            {
                gr.Generate(boostPrefab);
            }
            // 確率：2/10
            else if (rand < 10)
            {
                gr.Generate(coinPrefab);
            }
        }
    }


    /// <summary>
    /// プレイヤーが地球に触れたときに、stateをフェードアウトに変更
    /// </summary>
    public void GameEnd()
    {
        fadeOutPanel.SetActive(true);
        if (state == GameState.GAMING) { state++; }
    }

    /// <summary>
    /// 徐々に明転する
    /// </summary>
    void FadeOut()
    {
        var tmp = fadeOutPanel.GetComponent<Image>().color;
        if (tmp.a >= 1)
        {
            fadeOutPanel.SetActive(false);
            if (state == GameState.FADE_OUT) { state++; }
        }
        else
        {
            tmp.a += (1 / FADE_OUT_TIME) * Time.deltaTime;
        }
        fadeOutPanel.GetComponent<Image>().color = tmp;
    }


    /// <summary>
    /// リザルトの表示
    /// ランキングを表示、スコアとランキングを比較してタップ時の処理を分岐
    /// </summary>
    void ShowResult()
    {
        resultPanel.SetActive(true);
        var tmpScore = pl.gameScore;

        // スコアの値を丸くする
        tmpScore = ((float)Math.Floor(tmpScore * 100)) / 100;
        resultScoreText.text = tmpScore.ToString("F2");


        // タップしたら進む
        if (Input.GetMouseButtonDown(0))
        {
            ShareData.instance.Money += pl.gameMoney;
            // shareのお金をPrefsに保存
            PlayerPrefs.SetInt(Common.MONEY_PREFS_KEY, ShareData.instance.Money);

            // ランキング一番下よりスコアが高ければランキング更新
            if (rankingObjs[rankingObjs.Length - 1].GetComponent<RankingObj>().score 
                < tmpScore)
            {
                nameInputPanel.SetActive(true);
            }
            // ハイスコアではないのでリプレイかタイトルを促す
            else
            {
                if (state == GameState.SHOW_RESULT) state += 2;
                playerRankIndex = -1;
                continuePanel.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 名前を入力させてランキングの更新
    /// </summary>
    IEnumerator RankingUpdate()
    {
        var tmpScore = pl.gameScore;

        // 暫定でランキング入りさせる
        if (rankingObjs[rankingObjs.Length - 1].TryGetComponent<RankingObj>(out var tmpRank))
        {
            tmpRank.score = tmpScore;
            tmpRank.pName = playerName;
            playerRankIndex = rankingObjs.Length - 1;
        }

        // 何番目に入るのかを調べる&配列の更新
        for (int i = rankingObjs.Length - 2; i >= 0; i--)
        {
            var rank = rankingObjs[i].GetComponent<RankingObj>();
            // 順位が上がっていくか判定
            if (rank.score < tmpScore)
            {
                print("スコアが大きかった");
                // i+1番目のスコアの順位を一つ下へずらす
                if (rankingObjs[i + 1].TryGetComponent<RankingObj>(out var lowRank))
                {
                    print("oooo");
                    lowRank.score = rankingObjs[i].GetComponent<RankingObj>().score;
                    lowRank.pName = rankingObjs[i].GetComponent<RankingObj>().pName;
                    if (rankingObjs[i].GetComponent<RankingObj>().score >= 0)
                    {
                        rankingObjs[i + 1].SetActive(true);
                    }
                }
                // 自分がiに入る
                if (rankingObjs[i].TryGetComponent<RankingObj>(out var myRank))
                {
                    myRank.score = tmpScore;
                    myRank.pName = playerName;
                    playerRankIndex = i;
                    rankingObjs[i].SetActive(true);
                }
                continue;
            }
            else
            {
                break; 
            }
        }

        // ランキングの表示とキャッシュデータの更新
        for (int i = 0; i < rankingObjs.Length; i++)
        {
            var tmp = rankingObjs[i].GetComponent<RankingObj>();
            ShareData.instance.Ranking.datas[i].name = tmp.pName;
            ShareData.instance.Ranking.datas[i].score = tmp.score.ToString("F2"); ;
            rankingObjs[i].GetComponent<RankingObj>().DispRankInfo();
        }
        Debug.Log("ランキング配列更新完了");

        // ランキングが確定したのでGASの更新
        yield return StartCoroutine(UpdateGas());
    }

    /// <summary>
    /// GASの更新
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateGas()
    {
        // jsonデータ作成
        var jsonPost = JsonUtility.ToJson(ShareData.instance.Ranking);

        // 通信処理を実行
        var coroutine = HTTPCommunication.PostCommunication(Common.SET_RANKING_URL, jsonPost);
        yield return StartCoroutine(coroutine);
        var res = coroutine.Current.ToString();

    }


    /// <summary>
    /// 自分のスコアを点滅させる
    /// </summary>
    void RankingAnimation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (state == GameState.HIGH_SCORE)
            {
                state++;
                ContinueOrTitle();
            }

        }

        // ランキング未更新は点滅なし
        if (playerRankIndex == -1) return;


        blinkTimer += Time.deltaTime;
        if (blinkFlg)
        {
            rankingObjs[playerRankIndex].GetComponent<RankingObj>().textName.gameObject.SetActive(true);
            rankingObjs[playerRankIndex].GetComponent<RankingObj>().textScore.gameObject.SetActive(true);
            if (blinkTimer > BLINK_ON_TIME)
            {
                blinkFlg = !blinkFlg;
                blinkTimer = 0;
            }
        }
        else
        {
            rankingObjs[playerRankIndex].GetComponent<RankingObj>().textName.gameObject.SetActive(false);
            rankingObjs[playerRankIndex].GetComponent<RankingObj>().textScore.gameObject.SetActive(false);
            if (blinkTimer > BLINK_OUT_TIME)
            {
                blinkFlg = !blinkFlg;
                blinkTimer = 0;
            }
        }




    }


    /// <summary>
    /// リプレイか戻るかを選択させる
    /// </summary>
    void ContinueOrTitle()
    {
        // パネル表示
        continuePanel.SetActive(true);
        Debug.Log("コンティニューかリプレイを選ぶ");
    }

    public void PushYesButton()
    {
        Common.LoadScene("GameScene");
    }

    public void PushNoButton()
    {
        Common.LoadScene("Title");
    }





    /// <summary>
    /// ランキングの名前決定ボタン押下時の処理
    /// </summary>
    public void PushNameButton()
    {
        if (nameInputField.text == "")
        {
            nameInfoText.color = Color.red;
        }
        else
        {
            playerName = nameInputField.text;
            nameInputPanel.SetActive(false);
            StartCoroutine(RankingUpdate());
            if (state == GameState.SHOW_RESULT) state += 1;
        }
    }
}
