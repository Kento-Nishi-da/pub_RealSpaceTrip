using UnityEngine;
using CommonConst;

public class Player : MonoBehaviour
{

    const float MOVE_SPEED = 5;
    const float JUMP_SPEED = 6f;
    const float PLAYER_WIDTH = 1;
    const float PLAYER_HEIGHT = 1;

    /// <summary>
    /// プレイヤーの進行方向
    /// </summary>
    public enum Direction
    {
        UP,
        RIGHT,
        LEFT
    }

    // 進行方向
    Direction direction;
    // タップ開始時の座標
    Vector3 tapStPos;
    // タップ中の座標
    Vector3 mousePos;
    // タップしているか1
    bool isTap;

    // 被弾しているか
    bool isBreak;
    // ブースト状態か
    bool isBoost;
    float boostTimer;
    const float BOOST_LIMIT = 1;
    public GameObject boostSprite;

    // 移動速度
    float sp = MOVE_SPEED;

    // 引力の大きさ
    [SerializeField, Range(0, MOVE_SPEED)]
    float gravity;

    // タップ開始位置を表示するオブジェクト
    [SerializeField]
    GameObject tapPosObj;

    // 画面情報
    WorldScreen screen;

    // アニメーター
    Animator animator;
    // ゲーマネ
    GameManager gm;

    // ゲーム中に取得したお金
    public int gameMoney;
    // ゲームのスコア
    public float gameScore;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        // ゲーム中のみ操作可能
        if (gm.state == GameState.GAMING)
        {
            // 入力
            PlayerInputs();

            if (isTap)
            {
                // 移動方向決定
                GetOperation();
                // 移動
                Move();
            }



            if(isBoost)
            {
                BoostTime();
            }


            // 地球の引力に引っ張られる
            Gravitation();
        }
        // アニメーション
        PlayAnimation();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    void Init()
    {
        // 変数初期化
        transform.position = new Vector3(0, -1, 0);
        isTap = false;
        isBreak = false;
        isBoost = false;
        boostTimer = 0;
        tapPosObj.SetActive(false);

        // コンポーネント取得
        animator = GetComponent<Animator>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        boostSprite = transform.GetChild(1).gameObject;
        boostSprite.SetActive(false);

        // カメラの範囲を取得して画面端を取得
        screen = new WorldScreen();

        // todo:コスチュームで色変更
        if(ShareData.instance.Color == 0)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    /// <summary>
    /// 強化アイテム取得時
    /// </summary>
    void BoostTime()
    {
        print("バジリスクタイム！");
        boostTimer += Time.deltaTime;
        boostSprite.SetActive(true);
        if(boostTimer > BOOST_LIMIT)
        {
            boostTimer = 0;
            isBoost = false;
            boostSprite.SetActive(false);
        }
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    void Exit()
    {
        tapPosObj.SetActive(false);
        isBreak = false;
        isBoost = false;
        transform.GetChild(0).gameObject.SetActive(false);
        animator.SetTrigger("Idle");
    }

    /// <summary>
    /// 入力処理
    /// </summary>
    void PlayerInputs()
    {
        // タップ開始座標記録とフラグtrue
        if (Input.GetMouseButtonDown(0))
        {
            isTap = true;
            tapStPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tapPosObj.SetActive(true);
            tapPosObj.transform.position = new Vector3(tapStPos.x, tapStPos.y, tapPosObj.transform.position.z);
            Debug.Log(tapStPos);
        }

        // 指離したらフラグfalse
        if (Input.GetMouseButtonUp(0))
        {
            isTap = false;
            tapPosObj.SetActive(false);
        }


        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }

    /// <summary>
    /// プレイヤーの入力から移動方向を決定
    /// </summary>
    void GetOperation()
    {
        const float threshold = 0.5f;
        var dis = tapStPos - mousePos;

        if (dis.x < -threshold)
        {
            direction = Direction.RIGHT;
        }
        else if (dis.x > threshold)
        {
            direction = Direction.LEFT;
        }
        else
        {
            direction = Direction.UP;
        }
    }

    /// <summary>
    /// 移動処理
    /// 移動ベクトルを指定、正規化
    /// </summary>
    void Move()
    {
        if (isBoost)
        {
            sp = JUMP_SPEED;
        }
        else
        {
            sp = MOVE_SPEED;
        }
        Vector3 moveVec = Vector3.zero;
        switch (direction)
        {
            case Direction.LEFT:
                moveVec = new Vector3(-0.6f, 1).normalized;
                break;
            case Direction.RIGHT:
                moveVec = new Vector3(0.6f, 1).normalized;
                break;
            case Direction.UP:
                moveVec = new Vector3(0, 1).normalized;
                break;
        }

        // 隕石のダメージを食らっていなければ動ける
        if (!isBreak)
        {
            transform.position += moveVec * sp * Time.deltaTime;
        }


        // 画面端から出ないようにする
        if (transform.position.x > screen.topRight.x - (PLAYER_WIDTH / 2))
        {
            transform.position = new Vector3(
                screen.topRight.x - (PLAYER_WIDTH / 2),
                transform.position.y,
                transform.position.z
                );
        }

        if (transform.position.x < screen.bottomLeft.x + (PLAYER_WIDTH / 2))
        {
            transform.position = new Vector3(
                screen.bottomLeft.x + (PLAYER_WIDTH / 2),
                transform.position.y,
                transform.position.z
                );
        }

        if (transform.position.y > screen.topRight.y - (PLAYER_HEIGHT / 2))
        {
            transform.position = new Vector3(
                transform.position.x,
                screen.topRight.y - (PLAYER_HEIGHT / 2),
                transform.position.z
                );
        }
    }

    /// <summary>
    /// アニメーション
    /// タップ中は火が出る
    /// </summary>
    void PlayAnimation()
    {
        var triggerName = "";
        if (isBreak)
        {
            triggerName = "Break";
        }
        else
        {
            if (isTap)
            {
                triggerName = "Move";
            }
            else
            {
                triggerName = "Idle";
            }
        }


        animator.SetTrigger(triggerName);
    }

    /// <summary>
    /// 地球に引っ張られる
    /// </summary>
    void Gravitation()
    {
        transform.position += new Vector3(0, -1) * gravity * Time.deltaTime;
    }

    /// <summary>
    /// 被弾終了処理、アニメーションイベントで呼び出し
    /// </summary>
    public void EndBreak()
    {
        isBreak = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        // コインに当たったら
        if (obj.tag == "Coin")
        {
            // コインを取得
            if(obj.TryGetComponent<Coin>(out var coin))
            {
                gameMoney += coin.amount;
                Destroy(obj);
            }
        }

        // 無敵アイテムに当たったら
        if(obj.tag == "Boost")
        {
            if(obj.TryGetComponent<Boost>(out var boost))
            {
                // 無敵状態突入
                isBoost = true;
                Destroy(obj);
            }

        }

        // 隕石に当たったら
        if (obj.tag == "Meteor")
        {
            if(obj.TryGetComponent<Meteor>(out var meteor))
            {
                // ブースト出なければダメージ
                if(!isBoost)
                {
                    isBreak = true;
                }
                Destroy(obj);
            }

        }

        if (obj.tag == "Earth")
        {
            // 終了処理
            Exit();
            // ゲーム終了、フェードアウト開始
            if (gm.state == GameState.GAMING) { gm.GameEnd(); }

        }
    }
}
