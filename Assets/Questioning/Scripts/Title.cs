using System.Collections;
using UnityEngine;

/// <summary>
/// タイトル処理
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
    /// スタートボタンを押したとき、アニメーション開始
    /// </summary>
    public void GameStartButton()
    {
        // キャンバスを取得
        _canvas = GetComponent<Canvas>();

        // Animatorを取得
        _titleAnimator = GetComponent<Animator>();

        _titleAnimator.SetBool("UI", true);
    }


    /// <summary>
    /// タイトルアニメーションが終了した時。アニメーションイベントで呼び出し
    /// </summary>
    private void TitleUIAnimationEnd()
    {
        // キャンバスを無効化
        _canvas.enabled = false;

        // Suspectを有効化
        _suspect.enabled = true;

        StartCoroutine(ActiveUI());
    }

    IEnumerator ActiveUI()
    {
        _uiAnimator.SetBool("UI", true);

        yield return new WaitForSeconds(_time);
        // UIを有効化
        _ui.SetActive(true);
    }
}
