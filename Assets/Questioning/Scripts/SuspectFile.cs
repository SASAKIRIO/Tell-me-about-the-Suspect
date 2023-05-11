using UnityEngine;

/// <summary>
/// ��^�҂̃t�@�C���A�j���[�V����
/// </summary>
public class SuspectFile : MonoBehaviour
{
    // �t�@�C��Image
    [SerializeField] private RectTransform _fileRectTransform;

    // �t�@�C�����J���Ă��邩�ۂ�
    private bool _isOpenFile = false;

    [SerializeField] private Vector3 _closePosition;

    [SerializeField] private Vector3 _openPosition;

    // �t�@�C���̈ʒu�̕⊮�l
    private float _lerpIndex = 1;

    // �⊮���x
    [SerializeField] private float _lerpSpeed = 0.01f;
    public void MoveFile()
    {
        // �t�@�C�����J���Ă��鎞
        if (_isOpenFile)    _isOpenFile = false;
        // �t�@�C�������Ă��鎞
        else                _isOpenFile = true;
    }

    private void Start()
    {
        _fileRectTransform.position = _openPosition;
    }


    private void FixedUpdate()
    {
        // �t�@�C�����J���Ă��鎞
        if (_isOpenFile)
        {
            // 0�ȏ�Ȃ猸�Z
            if(_lerpIndex >= 0) _lerpIndex -= _lerpSpeed;
        }
        // �t�@�C�������Ă��鎞
        else
        {
            // 1�ȉ��Ȃ���Z
            if (_lerpIndex <= 1) _lerpIndex += _lerpSpeed;
        }

        // �⊮�l�ɍ��킹�ăI�u�W�F�N�g���ړ� 
        _fileRectTransform.position = Vector3.Lerp(_openPosition, _closePosition, _lerpIndex);
    }
}
