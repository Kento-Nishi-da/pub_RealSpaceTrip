using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Text;

namespace CommonConst
{
    #region 列挙体定義

    /// <summary>
    /// ゲーム中の状態
    /// </summary>
    public enum GameState
    {
        START_ANIM,     // 開始時のアニメーション
        GAMING,         // ゲーム中
        FADE_OUT,       // 地球に帰還中
        SHOW_RESULT,    // 秒数とランキング表示開始
        HIGH_SCORE,     // ランキング入りハイスコア
        WAITING         // ランキング更新完了、リプレイとタイトルボタン表示
    }

    /// <summary>
    /// オブジェクトのID
    /// </summary>
    public enum  ObjectID
    {
        METEOR
    }

    #endregion


    /// <summary>
    /// 世界座標で画面サイズを持つクラス.コンストラクタで取得する
    /// </summary>
    public class WorldScreen
    {
        public Vector2 bottomLeft { get; }
        public Vector2 topRight { get; }

        /// <summary>
        /// スクリーン座標系
        /// </summary>
        public WorldScreen()
        {
            // カメラの範囲を取得して画面端を取得
            bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
            topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        }
    }

    /// <summary>
    /// 通信処理クラス
    /// </summary>
    public class HTTPCommunication
    {
        /// <summary>
        /// 通信処理
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IEnumerator Communication(string url)
        {
            using (var req = UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();
                switch (req.result)
                {
                    case UnityWebRequest.Result.Success:
                        Debug.Log("通信完了");
                        //Debug.Log(req.downloadHandler.text);
                        yield return req.downloadHandler.text;
                        break;

                    default:
                        Debug.Log("エラー");
                        Debug.Log(req.error);
                        break;
                }
            }
        }




        /// <summary>
        /// 画像取得通信処理
        /// refによる参照渡しができないので配列を引数にして参照渡し
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IEnumerator TextureCommunication(string url, Texture2D[] _texture)
        {

            using (var req = UnityWebRequestTexture.GetTexture(url))
            {
                // 非同期通信
                yield return req.SendWebRequest();
                switch (req.result)
                {
                    case UnityWebRequest.Result.Success:
                        Debug.Log("画像取得通信成功");

                        _texture[0] = DownloadHandlerTexture.GetContent(req);
                        break;
                    default:
                        Debug.Log("エラー");
                        break;
                }
            }
        }



        /// <summary>
        /// ポスト通信用処理
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strPost"></param>
        /// <returns></returns>
        public static IEnumerator PostCommunication(string url, string strPost)
        {

            using (var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {

                // Json形式文字列をByteに変換
                var postData = Encoding.UTF8.GetBytes(strPost);

                // Post通信設定
                req.uploadHandler = new UploadHandlerRaw(postData);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");

                // 非同期通信
                yield return req.SendWebRequest();
                switch (req.result)
                {
                    // 通信に成功したとき通信結果をリターン
                    case UnityWebRequest.Result.Success:
                        
                        yield return req.downloadHandler.text;

                        break;
                    default:
                        Debug.Log("エラー");
                        break;
                }
            }
        }
    }


    /// <summary>
    /// 定数定義、汎用関数クラス
    /// </summary>
    public class Common
    {
        /// <summary>お金の量</summary>
        public const string MONEY_PREFS_KEY = "money";

        /// <summary>コスチュームカラー1の開放状況のキー</summary>
        public const string IS_COS_COLOR01_PREFS_KEY = "color_01";

        /// <summary>コスチュームのIDのキー</summary>
        public const string SELECT_COS_PREFS_KEY = "selected_costume";



        /// <summary>GETパラメータ：api_key</summary>
        public const string NASA_BASE_URL = "https://api.nasa.gov/planetary/apod";
        public const string MY_KEY = "2l0uEEwPBHarvNaifkD1rsw9CsooCFFvI2S2z61c";

        /// <summary>GETパラメータ：title, explanation, target_language</summary>
        public const string TRANSLATE_BASE_URL = "https://script.google.com/macros/s/AKfycbwfSWIyZAEFBski8wAn8QfwGc8a2Bds8TL1Et78o7Gjr2a_ugKy6xdEP1b5Mrj1_xkbxg/exec";

        public const string GET_RANKING_URL = "https://script.google.com/macros/s/AKfycbz0lgpiyQ5I4Rymk73VH0dM6770a_4m71C5_15DFiNNj6yEQS7I1o8rGWvtUom6akLlzQ/exec";
        /// <summary>POSTパラメータ：json_ranking</summary>
        public const string SET_RANKING_URL = "https://script.google.com/macros/s/AKfycbweuUtKbR95ZgJ87eArrLNQ8sJ1Wsx3cBXqfG6HWQEMAh3T2BzZQY3q7Q0aVoDAKG79HQ/exec";

        public const string LANGUAGE_JA = "ja";


        // ローディング画面の定数
        public const float LOADING_LIMIT = .5f;
        public const string LOADING_MSG = "通信中";
        public const int LOADING_CNT = 2;

        /// <summary>
        /// ObjectID
        /// </summary>
        public static readonly string[] OBJECT_NAMES = {
            "Meteor"
        };

        /// <summary>
        /// 通信中メッセージ表示関数
        /// </summary>
        /// <param name="_timer"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public static string LoadingMsgDisp(ref float _timer, ref int cnt)
        {
            _timer += Time.deltaTime;
            if (_timer > LOADING_LIMIT)
            {
                _timer = 0;


                if (cnt < LOADING_CNT)
                {
                    cnt++;
                }
                else
                {
                    cnt = 0;
                }
            }
            var txt = LOADING_MSG;
            for (int i = 0; i <= cnt; i++)
            {
                txt += ".";
            }
            return txt;
        }

        /// <summary>
        /// 汎用シーンチェンジ関数
        /// </summary>
        /// <param name="_scene"></param>
        public static void LoadScene(string _scene)
        {
            SceneManager.LoadScene(_scene);
        }


    }
}