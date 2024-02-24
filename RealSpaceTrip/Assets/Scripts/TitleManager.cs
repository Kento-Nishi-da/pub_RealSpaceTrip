using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using CommonConst;
using UnityEngine.UI;
using System.Security.Cryptography;

public class TitleManager : MonoBehaviour
{
    // todo:2024/1/9��APOD��������ۂ��Ă����������̂ŃT���v���ɂ��悤

    [Header("�ʐM���̕\��")]

    [Tooltip("�ʐM���̃p�l��")]
    [SerializeField] GameObject loadingPanel;
    [Tooltip("�ʐM���̕���")]
    [SerializeField] Text loadingText;
    // ������ύX����^�C�}�[
    float timer;
    // �����̕ύX��
    int cnt;
    // �ʐM�����ǂ���
    bool isLoading;


    [Header("�����L���O�@�\")]

    [Tooltip("�����L���O�I�u�W�F�N�g�̐e")]
    [SerializeField] Transform rankingParent;
    [Tooltip("�����L���O���Prefab")]
    [SerializeField] GameObject rankingPrefab;
    [Tooltip("�����L���O�p�l��")]
    [SerializeField] GameObject rankingPanel;


    [Header("�R�X�`���[��")]

    [Tooltip("�R�X�`���[���p�l��")]
    [SerializeField] GameObject costumePanel;
    [Tooltip("�f�t�H���g�{�^��")]
    [SerializeField] GameObject costumeDefault;
    [Tooltip("�J���[1�{�^��")]
    [SerializeField] GameObject costumeColor1;
    [Tooltip("�I�𒆂̋���")]
    [SerializeField] Image selectedObj;
    [Tooltip("�w���B��")]
    [SerializeField] GameObject buyPanel;
    [Tooltip("�����e�L�X�g")]
    [SerializeField] Text costumeMoneyText;
    [Tooltip("�R�X�`���[���v���C���[")]
    [SerializeField] SpriteRenderer playerSpriteRenderer;



    [Header("�M�������[�@�\")]

    [Tooltip("�M�������[�p�l��")]
    [SerializeField] GameObject galleryPanel;
    [Tooltip("APOD�\��RawImage")]
    [SerializeField] RawImage apodRawImage;
    [Tooltip("APOD�^�C�g���e�L�X�g")]
    [SerializeField] Text apodTitleText;
    [Tooltip("APOD�����e�L�X�g")]
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
            costumeMoneyText.text = "�R�C�������F" + ShareData.instance.Money;
        }
    }

    /// <summary>
    /// ����������
    /// </summary>
    IEnumerator Init()
    {
        // ������
        galleryPanel.SetActive(false);
        loadingPanel.SetActive(true);
        rankingPanel.SetActive(false);
        costumePanel.SetActive(false);
        timer = 0;
        cnt = 0;
        isLoading = true;

        // ��������ǂݍ��ށA�\������Ashare�ɓ˂�����
        if(PlayerPrefs.HasKey(Common.MONEY_PREFS_KEY))
        {
            ShareData.instance.Money = PlayerPrefs.GetInt(Common.MONEY_PREFS_KEY);
            print("������������" + ShareData.instance.Money);
        }
        else
        {
            PlayerPrefs.SetInt(Common.MONEY_PREFS_KEY, 0);
            ShareData.instance.Money = PlayerPrefs.GetInt(Common.MONEY_PREFS_KEY);
        }

        // �R�X�`���[���̐ݒ�
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

        // �R�X�`���[���̑I��ݒ�
        if (PlayerPrefs.HasKey(Common.SELECT_COS_PREFS_KEY))
        {
            var tmp = PlayerPrefs.GetInt(Common.SELECT_COS_PREFS_KEY);
            if (tmp == 1)
            {
                PushCostumeColor1();
                ShareData.instance.Color = 1;
            }

        }

        costumeMoneyText.text = "�R�C�������F" + ShareData.instance.Money;


        // APOD�̒ʐM�����AAPI�s��̂��ߕ���
#if true
        Debug.Log("�S�ʐM�����J�n");

        // �ʐM������A�f�[�^���L���b�V��
        if (ShareData.instance.APOD == null)
        {
            yield return StartCoroutine(GetAPOD());
        }

#else
        // API���˔@�����Ȃ��Ȃ����̂ł��̂܂ܑ��

        // �ʐM������A�f�[�^���L���b�V��
        if (ShareData.instance.APOD == null)
        {
            var jsonAPOD = new JsonAPOD();
            jsonAPOD.title = "Rosette Deep Field";
            jsonAPOD.url = "https://apod.nasa.gov/apod/image/2402/RosetteCone_Bernard_5398.jpg";
            jsonAPOD.explanation = "Can you find the Rosette Nebula? The large, red, and flowery-looking nebula on the upper left may seem the obvious choice, but that is actually just diffuse hydrogen emission surrounding the Cone and Fox Fur Nebulas. The famous Rosette Nebula is really located on the lower right and connected to the other nebulas by irregular filaments. Because the featured image of Rosetta's field is so wide and deep, it seems to contain other flowers. Designated NGC 2237, the center of the Rosette nebula is populated by the bright blue stars of open cluster NGC 2244, whose winds and energetic light are evacuating the nebula's center. The Rosette Nebula is about 5,000 light years distant and, just by itself, spans about three times the diameter of a full moon. This flowery field can be found toward the constellation of the Unicorn (Monoceros).";

            // �摜�擾�͉��̂��ł���̂ŒʐM���Ď擾
            // �R���[�`���ɂ͎Q�Ɠn�����ł��Ȃ��̂Ŕėp�����邽�߂ɔz��ŎQ�Ƃ�n��
            var tmpRef = new Texture2D[1];
            yield return StartCoroutine(HTTPCommunication.TextureCommunication(jsonAPOD.url, tmpRef));
            
            // �f�[�^���o��
            jsonAPOD.texture = tmpRef[0];

            // �p���|��̂��ߒʐM
            // URL�쐬
            var param = "?title=" + jsonAPOD.title + "&explanation=" + jsonAPOD.explanation + "&target_language=" + Common.LANGUAGE_JA;
            var coroutine = HTTPCommunication.Communication(Common.TRANSLATE_BASE_URL + param);
            // �񓯊��ʐM
            yield return StartCoroutine(coroutine);
            var res = coroutine.Current.ToString();
            // Json�f�[�^���V���A���C�Y
            var jsonTranslate = JsonUtility.FromJson<JsonTranslate>(res);
            print("�p���|�󊮗�");

            // �f�[�^�̃L���b�V��
            ShareData.instance.APOD = jsonAPOD;
            ShareData.instance.Translated = jsonTranslate;
        }
#endif
        // �L���b�V�����ꂽ�f�[�^�̓ǂݍ���
        apodTitleText.text = ShareData.instance.Translated.translated_title;
        apodExText.text = ShareData.instance.Translated.translated_ex;
        apodRawImage.texture = ShareData.instance.APOD.texture;

        Debug.Log("�S�ʐM�����I��");
        Debug.Log("�f�[�^�̓L���b�V���ɂ���܂�");


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
    /// �����L���O�I�u�W�F�N�g�̕\��
    /// </summary>
    void DispRanking()
    {
        // �����L���O�I�u�W�F�N�g����
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
    /// APOD�擾����
    /// </summary>
    /// <returns></returns>
    IEnumerator GetAPOD()
    {
        // �ʐMURL�쐬
        var param = "?api_key=" + Common.MY_KEY;
        var coroutine = HTTPCommunication.Communication(Common.NASA_BASE_URL + param);
        print(Common.NASA_BASE_URL + param);
        // �񓯊��ʐM
        yield return StartCoroutine(coroutine);
        var res = coroutine.Current.ToString();
        // Json�f�[�^���V���A���C�Y
        var jsonAPOD = JsonUtility.FromJson<JsonAPOD>(res);
        print("APOD�ʐM����");



        // �摜�擾�̂��ߍēx�ʐM
        // �R���[�`���ɂ͎Q�Ɠn�����ł��Ȃ��̂Ŕėp�����邽�߂ɔz��ŎQ�Ƃ�n��
        var tmpRef = new Texture2D[1];
        yield return StartCoroutine(HTTPCommunication.TextureCommunication(jsonAPOD.url, tmpRef));
        // �f�[�^���o��
        jsonAPOD.texture = tmpRef[0];
        print("�摜�擾����");



        // �p���|��̂��ߍēx�ʐM
        // URL�쐬
        param = "?title=" + jsonAPOD.title + "&explanation=" + jsonAPOD.explanation + "&target_language=" + Common.LANGUAGE_JA;
        coroutine = HTTPCommunication.Communication(Common.TRANSLATE_BASE_URL + param);
        // �񓯊��ʐM
        yield return StartCoroutine(coroutine);
        res = coroutine.Current.ToString();
        // Json�f�[�^���V���A���C�Y
        var jsonTranslate = JsonUtility.FromJson<JsonTranslate>(res);
        print("�p���|�󊮗�");


        // �f�[�^�̃L���b�V��
        ShareData.instance.APOD = jsonAPOD;
        ShareData.instance.Translated = jsonTranslate;


        //// �擾�e�L�X�g��\��
        //print(ShareData.instance.APOD.ToString());
    }


    /// <summary>
    /// �����L���O�擾����
    /// </summary>
    /// <returns></returns>
    IEnumerator GetRanking()
    {
        // �ʐMURL�쐬
        var coroutine = HTTPCommunication.Communication(Common.GET_RANKING_URL);
        // �񓯊��ʐM
        yield return StartCoroutine(coroutine);
        var res = coroutine.Current.ToString();
        print(res);

        // Json�f�[�^���V���A���C�Y
        var jsonRanking = JsonUtility.FromJson<JsonRanking>(res);

        // �f�[�^�̃L���b�V��
        ShareData.instance.Ranking = jsonRanking;

        //// �擾�e�L�X�g��\��
        //print(ShareData.instance.RANKING.ToString());
    }


    public void PushStartButton()
    {
        Debug.Log("�Q�[���X�^�[�g");
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// �����L���O�{�^��������
    /// </summary>
    public void PushRankingButton()
    {
        Debug.Log("�����L���O");
        rankingPanel.SetActive(true);

    }

    /// <summary>
    /// �����L���O�߂�{�^��������
    /// </summary>
    public void PushRankingBackButton()
    {
        rankingPanel.SetActive(false);
    }

    public void PushCostumeButton()
    {
        Debug.Log("�R�X�`���[��");
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
            costumeMoneyText.text = "�R�C�������F" + ShareData.instance.Money;
            PlayerPrefs.SetInt(Common.IS_COS_COLOR01_PREFS_KEY, 1);
        }

    }

    /// <summary>
    /// �M�������[�{�^��������
    /// </summary>
    public void PushGalleryButton()
    {
        Debug.Log("�M�������[");
        galleryPanel.SetActive(true);
    }

    /// <summary>
    /// �M�������[�߂�{�^��������
    /// </summary>
    public void PushGalleryBackButton()
    {
        galleryPanel.SetActive(false);
    }
}
