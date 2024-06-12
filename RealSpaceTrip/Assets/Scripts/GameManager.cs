using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CommonConst;
using System;

public class GameManager : MonoBehaviour
{
    [Header("�Q�[���J�n�O")]
    [Tooltip("�J�E���g�_�E���e�L�X�g")]
    [SerializeField] Text countText;

    [Header("�Q�[�����̃I�u�W�F�N�g")]

    [Tooltip("�w�i")]
    [SerializeField] RawImage backRawImage;



    [Header("�t�F�[�h�A�E�g")]

    [Tooltip("�Q�[���I�����̃t�F�[�h�A�E�g�p�l��")]
    [SerializeField] GameObject fadeOutPanel;



    [Header("���U���g")]

    [Tooltip("���U���g�\���p�p�l��")]
    [SerializeField] GameObject resultPanel;
    [Tooltip("����̃X�R�A�̃e�L�X�g")]
    [SerializeField] Text resultScoreText;
    [Tooltip("�����L���O���̐����̐e�I�u�W�F�N�g")]
    [SerializeField] Transform rankingParent;



    [Header("�n�C�X�R�A")]

    [Tooltip("���O���̓p�l��")]
    [SerializeField] GameObject nameInputPanel;
    [Tooltip("���O���͗�")]
    [SerializeField] InputField nameInputField;
    [Tooltip("���O���͂𑣂��e�L�X�g")]
    [SerializeField] Text nameInfoText;



    [Header("���v���C")]
    [Tooltip("�R���e�B�j���[�����v���C�𑣂��p�l��")]
    [SerializeField] GameObject continuePanel;

    [Header("UI")]
    [Tooltip("�X�R�A�\��")]
    [SerializeField] Text scoreText;
    [Tooltip("�����\��")]
    [SerializeField] Text moneyText;



    Player pl;
    Generator gr;

    GameObject meteorPrefab;
    GameObject coinPrefab;
    GameObject boostPrefab;

    GameObject rankObjPrefab;
    GameObject[] rankingObjs;

    // �Q�[���̐i�s��
    [Header("�Q�[���̏��")]

    [Tooltip("�Q�[���̏��")]
    public GameState state;

    // �Q�[���J�n�O�̃J�E���g�_�E����
    float startCount = 3;


    string playerName;
    // �t�F�[�h�A�E�g�ɂ����鎞��
    const float FADE_OUT_TIME = 2.0f;

    // �I�u�W�F�N�g��������Ԋu
    const float GENERATE_TIME = 2f;
    // �I�u�W�F�N�g�����̃^�C�}�[
    float objectTimer;

    // �n�C�X�R�A���̃A�j���[�V�����p
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
        // �Q�[���̏�Ԃɍ��킹�ĕ���
        switch (state)
        {
            case GameState.START_ANIM:
                // todo:�J�n���A�j���[�V����
                StartAnimation();
                break;
            case GameState.GAMING:
                // �Q�[��������
                Gaming();
                break;
            case GameState.FADE_OUT:
                // �I�����t�F�[�h�A�E�g
                FadeOut();
                break;
            case GameState.SHOW_RESULT:
                // ���U���g�p�l���̕\���A�n�C�X�R�A�Ȃ疼�O�̓��͂������ăX�R�A�̍X�V
                ShowResult();
                break;
            case GameState.HIGH_SCORE:
            case GameState.WAITING:
                // �v���C���[�̓��͑҂��̊Ԃ��_�ł͂�����
                RankingAnimation();
                break;
        }
    }

    /// <summary>
    /// ������
    /// </summary>
    void Init()
    {
        // �R���|�[�l���g���擾
        pl = GameObject.Find("Player").GetComponent<Player>();
        gr = GameObject.Find("Generator").GetComponent<Generator>();

        // �e�v���n�u�̓ǂݍ���
        rankObjPrefab = (GameObject)Resources.Load("Prefabs/RankingObj");
        meteorPrefab = (GameObject)Resources.Load("Prefabs/Meteor");
        coinPrefab = (GameObject)Resources.Load("Prefabs/Coin");
        boostPrefab = (GameObject)Resources.Load("Prefabs/Boost");


        if (ShareData.instance.APOD.texture != null)
        {
            backRawImage.texture = ShareData.instance.APOD.texture;
            backRawImage.color = Color.white;
        }

        // ���炩���߃����L���O��ǂݍ���ŕ\�����Ă���
        DispRanking();

        // �ϐ�������
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
    /// �����L���O�I�u�W�F�N�g�̕\��
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
                        i + "�ʂ̐l�ł�",
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
            // �����L���O�I�u�W�F�N�g����
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
    /// �J�n���̃A�j���[�V����
    /// </summary>
    void StartAnimation()
    {
        // todo:�n�����o�����ĉF���֔�ї��悤�ȃA�j���[�V����
        startCount -= Time.deltaTime;
        countText.text = startCount.ToString("F0");
        if(startCount < 0)
        {
            if (state == GameState.START_ANIM) state++;
            countText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// �Q�[�����̏���
    /// </summary>
    void Gaming()
    {
        // �v���C���[�̂����A�X�R�A�\��
        pl.gameScore += Time.deltaTime;
        scoreText.text = "���s���ԁF" + pl.gameScore.ToString("F2");
        moneyText.text = "�l���R�C���F+" + pl.gameMoney;


        // ��莞�Ԃ��o�߂�����I�u�W�F�N�g����
        objectTimer += Time.deltaTime;
        if (objectTimer > GENERATE_TIME)
        {
            objectTimer = 0;
            gr.Generate(meteorPrefab);

            // 覐Δ������Ɋm���ŃA�C�e��������
            var rand = UnityEngine.Random.Range(0, 10);
            // �m���F1/10
            if(rand < 5)
            {
                gr.Generate(boostPrefab);
            }
            // �m���F2/10
            else if (rand < 10)
            {
                gr.Generate(coinPrefab);
            }
        }
    }


    /// <summary>
    /// �v���C���[���n���ɐG�ꂽ�Ƃ��ɁAstate���t�F�[�h�A�E�g�ɕύX
    /// </summary>
    public void GameEnd()
    {
        fadeOutPanel.SetActive(true);
        if (state == GameState.GAMING) { state++; }
    }

    /// <summary>
    /// ���X�ɖ��]����
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
    /// ���U���g�̕\��
    /// �����L���O��\���A�X�R�A�ƃ����L���O���r���ă^�b�v���̏����𕪊�
    /// </summary>
    void ShowResult()
    {
        resultPanel.SetActive(true);
        var tmpScore = pl.gameScore;

        // �X�R�A�̒l���ۂ�����
        tmpScore = ((float)Math.Floor(tmpScore * 100)) / 100;
        resultScoreText.text = tmpScore.ToString("F2");


        // �^�b�v������i��
        if (Input.GetMouseButtonDown(0))
        {
            ShareData.instance.Money += pl.gameMoney;
            // share�̂�����Prefs�ɕۑ�
            PlayerPrefs.SetInt(Common.MONEY_PREFS_KEY, ShareData.instance.Money);

            // �����L���O��ԉ����X�R�A��������΃����L���O�X�V
            if (rankingObjs[rankingObjs.Length - 1].GetComponent<RankingObj>().score 
                < tmpScore)
            {
                nameInputPanel.SetActive(true);
            }
            // �n�C�X�R�A�ł͂Ȃ��̂Ń��v���C���^�C�g���𑣂�
            else
            {
                if (state == GameState.SHOW_RESULT) state += 2;
                playerRankIndex = -1;
                continuePanel.SetActive(true);
            }
        }
    }

    /// <summary>
    /// ���O����͂����ă����L���O�̍X�V
    /// </summary>
    IEnumerator RankingUpdate()
    {
        var tmpScore = pl.gameScore;

        // �b��Ń����L���O���肳����
        if (rankingObjs[rankingObjs.Length - 1].TryGetComponent<RankingObj>(out var tmpRank))
        {
            tmpRank.score = tmpScore;
            tmpRank.pName = playerName;
            playerRankIndex = rankingObjs.Length - 1;
        }

        // ���Ԗڂɓ���̂��𒲂ׂ�&�z��̍X�V
        for (int i = rankingObjs.Length - 2; i >= 0; i--)
        {
            var rank = rankingObjs[i].GetComponent<RankingObj>();
            // ���ʂ��オ���Ă���������
            if (rank.score < tmpScore)
            {
                print("�X�R�A���傫������");
                // i+1�Ԗڂ̃X�R�A�̏��ʂ�����ւ��炷
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
                // ������i�ɓ���
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

        // �����L���O�̕\���ƃL���b�V���f�[�^�̍X�V
        for (int i = 0; i < rankingObjs.Length; i++)
        {
            var tmp = rankingObjs[i].GetComponent<RankingObj>();
            ShareData.instance.Ranking.datas[i].name = tmp.pName;
            ShareData.instance.Ranking.datas[i].score = tmp.score.ToString("F2"); ;
            rankingObjs[i].GetComponent<RankingObj>().DispRankInfo();
        }
        Debug.Log("�����L���O�z��X�V����");

        // �����L���O���m�肵���̂�GAS�̍X�V
        yield return StartCoroutine(UpdateGas());
    }

    /// <summary>
    /// GAS�̍X�V
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateGas()
    {
        // json�f�[�^�쐬
        var jsonPost = JsonUtility.ToJson(ShareData.instance.Ranking);

        // �ʐM���������s
        var coroutine = HTTPCommunication.PostCommunication(Common.SET_RANKING_URL, jsonPost);
        yield return StartCoroutine(coroutine);
        var res = coroutine.Current.ToString();

    }


    /// <summary>
    /// �����̃X�R�A��_�ł�����
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

        // �����L���O���X�V�͓_�łȂ�
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
    /// ���v���C���߂邩��I��������
    /// </summary>
    void ContinueOrTitle()
    {
        // �p�l���\��
        continuePanel.SetActive(true);
        Debug.Log("�R���e�B�j���[�����v���C��I��");
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
    /// �����L���O�̖��O����{�^���������̏���
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
