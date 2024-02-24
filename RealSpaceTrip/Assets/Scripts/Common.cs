using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Text;

namespace CommonConst
{
    #region �񋓑̒�`

    /// <summary>
    /// �Q�[�����̏��
    /// </summary>
    public enum GameState
    {
        START_ANIM,     // �J�n���̃A�j���[�V����
        GAMING,         // �Q�[����
        FADE_OUT,       // �n���ɋA�Ғ�
        SHOW_RESULT,    // �b���ƃ����L���O�\���J�n
        HIGH_SCORE,     // �����L���O����n�C�X�R�A
        WAITING         // �����L���O�X�V�����A���v���C�ƃ^�C�g���{�^���\��
    }

    /// <summary>
    /// �I�u�W�F�N�g��ID
    /// </summary>
    public enum  ObjectID
    {
        METEOR
    }

    #endregion


    /// <summary>
    /// ���E���W�ŉ�ʃT�C�Y�����N���X.�R���X�g���N�^�Ŏ擾����
    /// </summary>
    public class WorldScreen
    {
        public Vector2 bottomLeft { get; }
        public Vector2 topRight { get; }

        /// <summary>
        /// �X�N���[�����W�n
        /// </summary>
        public WorldScreen()
        {
            // �J�����͈̔͂��擾���ĉ�ʒ[���擾
            bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
            topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        }
    }

    /// <summary>
    /// �ʐM�����N���X
    /// </summary>
    public class HTTPCommunication
    {
        /// <summary>
        /// �ʐM����
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
                        Debug.Log("�ʐM����");
                        //Debug.Log(req.downloadHandler.text);
                        yield return req.downloadHandler.text;
                        break;

                    default:
                        Debug.Log("�G���[");
                        Debug.Log(req.error);
                        break;
                }
            }
        }




        /// <summary>
        /// �摜�擾�ʐM����
        /// ref�ɂ��Q�Ɠn�����ł��Ȃ��̂Ŕz��������ɂ��ĎQ�Ɠn��
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IEnumerator TextureCommunication(string url, Texture2D[] _texture)
        {

            using (var req = UnityWebRequestTexture.GetTexture(url))
            {
                // �񓯊��ʐM
                yield return req.SendWebRequest();
                switch (req.result)
                {
                    case UnityWebRequest.Result.Success:
                        Debug.Log("�摜�擾�ʐM����");

                        _texture[0] = DownloadHandlerTexture.GetContent(req);
                        break;
                    default:
                        Debug.Log("�G���[");
                        break;
                }
            }
        }



        /// <summary>
        /// �|�X�g�ʐM�p����
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strPost"></param>
        /// <returns></returns>
        public static IEnumerator PostCommunication(string url, string strPost)
        {

            using (var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {

                // Json�`���������Byte�ɕϊ�
                var postData = Encoding.UTF8.GetBytes(strPost);

                // Post�ʐM�ݒ�
                req.uploadHandler = new UploadHandlerRaw(postData);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");

                // �񓯊��ʐM
                yield return req.SendWebRequest();
                switch (req.result)
                {
                    // �ʐM�ɐ��������Ƃ��ʐM���ʂ����^�[��
                    case UnityWebRequest.Result.Success:
                        
                        yield return req.downloadHandler.text;

                        break;
                    default:
                        Debug.Log("�G���[");
                        break;
                }
            }
        }
    }


    /// <summary>
    /// �萔��`�A�ėp�֐��N���X
    /// </summary>
    public class Common
    {
        /// <summary>�����̗�</summary>
        public const string MONEY_PREFS_KEY = "money";

        /// <summary>�R�X�`���[���J���[1�̊J���󋵂̃L�[</summary>
        public const string IS_COS_COLOR01_PREFS_KEY = "color_01";

        /// <summary>�R�X�`���[����ID�̃L�[</summary>
        public const string SELECT_COS_PREFS_KEY = "selected_costume";



        /// <summary>GET�p�����[�^�Fapi_key</summary>
        public const string NASA_BASE_URL = "https://api.nasa.gov/planetary/apod";
        public const string MY_KEY = "2l0uEEwPBHarvNaifkD1rsw9CsooCFFvI2S2z61c";

        /// <summary>GET�p�����[�^�Ftitle, explanation, target_language</summary>
        public const string TRANSLATE_BASE_URL = "https://script.google.com/macros/s/AKfycbwfSWIyZAEFBski8wAn8QfwGc8a2Bds8TL1Et78o7Gjr2a_ugKy6xdEP1b5Mrj1_xkbxg/exec";

        public const string GET_RANKING_URL = "https://script.google.com/macros/s/AKfycbz0lgpiyQ5I4Rymk73VH0dM6770a_4m71C5_15DFiNNj6yEQS7I1o8rGWvtUom6akLlzQ/exec";
        /// <summary>POST�p�����[�^�Fjson_ranking</summary>
        public const string SET_RANKING_URL = "https://script.google.com/macros/s/AKfycbweuUtKbR95ZgJ87eArrLNQ8sJ1Wsx3cBXqfG6HWQEMAh3T2BzZQY3q7Q0aVoDAKG79HQ/exec";

        public const string LANGUAGE_JA = "ja";


        // ���[�f�B���O��ʂ̒萔
        public const float LOADING_LIMIT = .5f;
        public const string LOADING_MSG = "�ʐM��";
        public const int LOADING_CNT = 2;

        /// <summary>
        /// ObjectID
        /// </summary>
        public static readonly string[] OBJECT_NAMES = {
            "Meteor"
        };

        /// <summary>
        /// �ʐM�����b�Z�[�W�\���֐�
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
        /// �ėp�V�[���`�F���W�֐�
        /// </summary>
        /// <param name="_scene"></param>
        public static void LoadScene(string _scene)
        {
            SceneManager.LoadScene(_scene);
        }


    }
}