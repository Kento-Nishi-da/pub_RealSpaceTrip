using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �S�A�C�e���Ƃ��ז��̊��N���X
/// �ړ����x�������A��{�I�ɗ������Ă���
/// </summary>
public class ObjectBase : MonoBehaviour
{
    // �h���N���X�ŃA�C�e������Q�������L�q
    // �ォ��܂������~���Ă���ی^�̊�A�R�C���A���G�A�C�e��


    protected float moveSpeed;

    void Start()
    {
        Init();
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        Move();
    }

    /// <summary>
    /// �����������A�I�[�o�[���C�h���Ďg��
    /// </summary>
    virtual protected void Init() { }

    /// <summary>
    /// �ړ�����
    /// </summary>
    virtual protected void Move()
    {
        transform.position += new Vector3(0, -moveSpeed, 0) * Time.deltaTime;
    }

    /// <summary>
    /// ��ʊO�ɏ����鏈���A�n���ɓ��������玩��
    /// </summary>
    /// <param name="collision"></param>
    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Earth")
        {
            Destroy(gameObject);
        }
    }
}
