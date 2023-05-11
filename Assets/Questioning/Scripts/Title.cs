using System.Collections;
using UnityEngine;

/// <summary>
/// �^�C�g������
/// </summary>
public class Title : MonoBehaviour
{
    [SerializeField] private Suspect _suspect;

    [SerializeField] private GameObject _ui;

    private Canvas _canvas;

    private Animator _titleAnimator;

    [SerializeField] private Animator _uiAnimator;

    [SerializeField] private float _time;



    /// <summary>
    /// �X�^�[�g�{�^�����������Ƃ��A�A�j���[�V�����J�n
    /// </summary>
    public void GameStartButton()
    {
        // �L�����o�X���擾
        _canvas = GetComponent<Canvas>();

        // Animator���擾
        _titleAnimator = GetComponent<Animator>();

        _titleAnimator.SetBool("UI", true);
    }


    /// <summary>
    /// �^�C�g���A�j���[�V�������I���������B�A�j���[�V�����C�x���g�ŌĂяo��
    /// </summary>
    private void TitleUIAnimationEnd()
    {
        // �L�����o�X�𖳌���
        _canvas.enabled = false;

        // Suspect��L����
        _suspect.enabled = true;

        StartCoroutine(ActiveUI());
    }

    IEnumerator ActiveUI()
    {
        _uiAnimator.SetBool("UI", true);

        yield return new WaitForSeconds(_time);
        // UI��L����
        _ui.SetActive(true);
    }
}
