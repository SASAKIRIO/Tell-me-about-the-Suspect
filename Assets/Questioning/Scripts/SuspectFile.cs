using UnityEngine;

/// <summary>
/// 被疑者のファイルアニメーション
/// </summary>
public class SuspectFile : MonoBehaviour
{
    // ファイルImage
    [SerializeField] private RectTransform _fileRectTransform;

    // ファイルを開いているか否か
    private bool _isOpenFile = false;

    [SerializeField] private Vector3 _closePosition;

    [SerializeField] private Vector3 _openPosition;

    // ファイルの位置の補完値
    private float _lerpIndex = 1;

    // 補完速度
    [SerializeField] private float _lerpSpeed = 0.01f;
    public void MoveFile()
    {
        // ファイルが開いている時
        if (_isOpenFile)    _isOpenFile = false;
        // ファイルが閉じている時
        else                _isOpenFile = true;
    }

    private void Start()
    {
        _fileRectTransform.position = _openPosition;
    }


    private void FixedUpdate()
    {
        // ファイルが開いている時
        if (_isOpenFile)
        {
            // 0以上なら減算
            if(_lerpIndex >= 0) _lerpIndex -= _lerpSpeed;
        }
        // ファイルが閉じている時
        else
        {
            // 1以下なら加算
            if (_lerpIndex <= 1) _lerpIndex += _lerpSpeed;
        }

        // 補完値に合わせてオブジェクトを移動 
        _fileRectTransform.position = Vector3.Lerp(_openPosition, _closePosition, _lerpIndex);
    }
}
