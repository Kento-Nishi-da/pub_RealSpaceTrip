using UnityEngine;
using CommonConst;

public class Player : MonoBehaviour
{

    const float MOVE_SPEED = 5;
    const float JUMP_SPEED = 6f;
    const float PLAYER_WIDTH = 1;
    const float PLAYER_HEIGHT = 1;

    /// <summary>
    /// �v���C���[�̐i�s����
    /// </summary>
    public enum Direction
    {
        UP,
        RIGHT,
        LEFT
    }

    // �i�s����
    Direction direction;
    // �^�b�v�J�n���̍��W
    Vector3 tapStPos;
    // �^�b�v���̍��W
    Vector3 mousePos;
    // �^�b�v���Ă��邩1
    bool isTap;

    // ��e���Ă��邩
    bool isBreak;
    // �u�[�X�g��Ԃ�
    bool isBoost;
    float boostTimer;
    const float BOOST_LIMIT = 1;
    public GameObject boostSprite;

    // �ړ����x
    float sp = MOVE_SPEED;

    // ���͂̑傫��
    [SerializeField, Range(0, MOVE_SPEED)]
    float gravity;

    // �^�b�v�J�n�ʒu��\������I�u�W�F�N�g
    [SerializeField]
    GameObject tapPosObj;

    // ��ʏ��
    WorldScreen screen;

    // �A�j���[�^�[
    Animator animator;
    // �Q�[�}�l
    GameManager gm;

    // �Q�[�����Ɏ擾��������
    public int gameMoney;
    // �Q�[���̃X�R�A
    public float gameScore;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        // �Q�[�����̂ݑ���\
        if (gm.state == GameState.GAMING)
        {
            // ����
            PlayerInputs();

            if (isTap)
            {
                // �ړ���������
                GetOperation();
                // �ړ�
                Move();
            }



            if(isBoost)
            {
                BoostTime();
            }


            // �n���̈��͂Ɉ���������
            Gravitation();
        }
        // �A�j���[�V����
        PlayAnimation();
    }

    /// <summary>
    /// ������
    /// </summary>
    void Init()
    {
        // �ϐ�������
        transform.position = new Vector3(0, -1, 0);
        isTap = false;
        isBreak = false;
        isBoost = false;
        boostTimer = 0;
        tapPosObj.SetActive(false);

        // �R���|�[�l���g�擾
        animator = GetComponent<Animator>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        boostSprite = transform.GetChild(1).gameObject;
        boostSprite.SetActive(false);

        // �J�����͈̔͂��擾���ĉ�ʒ[���擾
        screen = new WorldScreen();

        // todo:�R�X�`���[���ŐF�ύX
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
    /// �����A�C�e���擾��
    /// </summary>
    void BoostTime()
    {
        print("�o�W���X�N�^�C���I");
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
    /// �I������
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
    /// ���͏���
    /// </summary>
    void PlayerInputs()
    {
        // �^�b�v�J�n���W�L�^�ƃt���Otrue
        if (Input.GetMouseButtonDown(0))
        {
            isTap = true;
            tapStPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tapPosObj.SetActive(true);
            tapPosObj.transform.position = new Vector3(tapStPos.x, tapStPos.y, tapPosObj.transform.position.z);
            Debug.Log(tapStPos);
        }

        // �w��������t���Ofalse
        if (Input.GetMouseButtonUp(0))
        {
            isTap = false;
            tapPosObj.SetActive(false);
        }


        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }

    /// <summary>
    /// �v���C���[�̓��͂���ړ�����������
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
    /// �ړ�����
    /// �ړ��x�N�g�����w��A���K��
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

        // 覐΂̃_���[�W��H����Ă��Ȃ���Γ�����
        if (!isBreak)
        {
            transform.position += moveVec * sp * Time.deltaTime;
        }


        // ��ʒ[����o�Ȃ��悤�ɂ���
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
    /// �A�j���[�V����
    /// �^�b�v���͉΂��o��
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
    /// �n���Ɉ���������
    /// </summary>
    void Gravitation()
    {
        transform.position += new Vector3(0, -1) * gravity * Time.deltaTime;
    }

    /// <summary>
    /// ��e�I�������A�A�j���[�V�����C�x���g�ŌĂяo��
    /// </summary>
    public void EndBreak()
    {
        isBreak = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        // �R�C���ɓ���������
        if (obj.tag == "Coin")
        {
            // �R�C�����擾
            if(obj.TryGetComponent<Coin>(out var coin))
            {
                gameMoney += coin.amount;
                Destroy(obj);
            }
        }

        // ���G�A�C�e���ɓ���������
        if(obj.tag == "Boost")
        {
            if(obj.TryGetComponent<Boost>(out var boost))
            {
                // ���G��ԓ˓�
                isBoost = true;
                Destroy(obj);
            }

        }

        // 覐΂ɓ���������
        if (obj.tag == "Meteor")
        {
            if(obj.TryGetComponent<Meteor>(out var meteor))
            {
                // �u�[�X�g�o�Ȃ���΃_���[�W
                if(!isBoost)
                {
                    isBreak = true;
                }
                Destroy(obj);
            }

        }

        if (obj.tag == "Earth")
        {
            // �I������
            Exit();
            // �Q�[���I���A�t�F�[�h�A�E�g�J�n
            if (gm.state == GameState.GAMING) { gm.GameEnd(); }

        }
    }
}
