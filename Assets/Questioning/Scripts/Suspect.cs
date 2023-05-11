using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using OpenAI.GPT_3_5_TURBO;

/// <summary>
/// GPT-3.5-turboに通信して応答を受け取れる処理
/// </summary>
public class Suspect : MonoBehaviour
{

    // APIキー
    private string _apiKey;

    // ユーザ、被疑者、APIを入力するテキストフィールド
    [SerializeField] private InputField _playerInputField, _suspectInputField, _apiKeyInputField;

    // チャットログ機能
    [SerializeField] private Text _chatLog;

    // File情報系Text
    [SerializeField] private Text _fileName, _fileAge, _fileCrime, _fileInfo;

    // 結果系Text
    [SerializeField] private Text _resultTruth, _resultConfessionRate;

    [SerializeField] private Canvas _resultCanvas;

    [SerializeField] private Crimedata _crimeData;

    // 生成されたコード
    private string _result = default;

    //APIのURL
    private const string API_URL = "https://api.openai.com/v1/chat/completions";

    // モデル名
    private const string _modelName = "gpt-3.5-turbo";

    // 要求分
    private string _prompt = default;

    // 記憶
    private List<MessageModel> _history = new List<MessageModel>();

    // 質問内容 名前 年齢 性別 職業 IQ 背景 疑われている罪 有罪か無罪か 動機
    private string _question, _name, _age, _gender, _job,  _background, _crime, _truth, _motive;

    // API通信中か否かを判断するbool型変数
    private bool _isRunning = false;

    // 回答が準備完了か判断するbool型変数
    private bool _isReady = false;

    // 会話中か判断するbool型変数
    private bool _isTalking = false;

    // 何回やり取りしたか
    private int _count = 0;

    // 尋問進捗
    private enum Progress_Status
    {
        // 質問状態
        Question,

        // 尋問終了状態
        End
    }

    [SerializeField] private Progress_Status _progressStatus = Progress_Status.Question;


    // 難易度
    private enum Difficult_Status
    {
        Easy = 1,
        Normal = 2,
        Hard = 3,
    }

    [SerializeField] private Difficult_Status _difficult = Difficult_Status.Easy;

    // メソッド部 #####################################################


    private void Start()
    {
        // APIキーのテキストを入れる。
        _apiKey = _apiKeyInputField.text;

        // 被疑者の情報をスクリプタブルオブジェクトからセット
        SetConfessionInfo();

        // ファイルの情報をセット
        SetFileInfo();

        // 準備完了状態にする
        _isReady = true;

        // 記憶を初期化
        ResetHistory();
    }


    private void Update()
    {
        // APIキーのテキストを入れる。
        _apiKey = _apiKeyInputField.text;

        // Textが空じゃない状態で、右クリックを押すとリクエストを送信。
        if (Input.GetMouseButtonDown(1) && _playerInputField.text != "")
        {
            _question = _playerInputField.text;

            Interact();
        }
    }

    // 記憶をリセット
    private void OnDestroy()
    {
        if (_history != null)
        {
            _history.Clear();
        }
    }


    private void SetConfessionInfo()
    {
        // ランダム値を与える。
        int l_index = UnityEngine.Random.Range(0, _crimeData._suspectInfo.Count);

        // 被疑者の情報を入力。
        SuspectInfo l_suspectInfo = _crimeData._suspectInfo[l_index];
        _name = l_suspectInfo._name;
        _age = l_suspectInfo._age;
        _gender = l_suspectInfo._gender;
        _job = l_suspectInfo._job;
        _crime = l_suspectInfo._crime;
        _motive = l_suspectInfo._motive;
        _background = l_suspectInfo._background;
        _truth = l_suspectInfo._truth;
    }



    private void SetFileInfo()
    {
        _fileName.text = _name;
        _fileAge.text = _age;
        _fileCrime.text = _crime;
        _fileInfo.text = "職業は"+_job+"。"+_background;
        _resultTruth.text = _truth;
    }



    /// <summary>
    /// 要求を送る
    /// </summary>
    public void Execute(string question,int timeOutMs)
    {

        // 通信しているときはログをだして、returnする。
        if (_isRunning)
        {
            Debug.Log("現在通信中");
            return;
        }

        // 入力不可にする。
        _playerInputField.interactable = false;

        // 通信中はテキストフィールドに...と表示する。
        _suspectInputField.text = "...";

        // 会話回数。
        _count++;

        // 通信中フラグをtrueにする
        _isRunning = true;

        // 準備状態にする
        _isReady = false;


        if (_progressStatus == Progress_Status.Question)
        {
            if (_count == 1)
            {
                _prompt = "こんにちは、ChatGPTさん。\r\n" +
                "私は尋問をテーマにしたゲームを制作しています。\r\n" +
                "ChatGPTさんには容疑者になってほしいです。\r\n" +
                "心身状態レベルは、1～3まであり、今回の被疑者はレベル" + (int)_difficult + "です。レベルが高ければ高いほど、心身状態が不安定な状態です。\r\n" +
                "どのレベルでも尋問自体は素直に受けて、なかなか自白しないようにしてください。" +
                "心身状態が異常の場合、容疑を否認、または黙秘してください。\r\n" +
                "心が落ち着かないと、自白しないようにしてください。\r\n" +
                "ChatGPTさんが容疑者を演じるためのリクエストを送信したいと思います。" +
                "容疑者の設定は以下の通りで、容疑者は尋問を受けています。" + _count + "回目の質問である、<question>の質問に対する返答をしてください。\r\n" +
                "\r\n" +
                "<data>\r\n" +
                "名前：" + _name + "\r\n" +
                "年齢：" + _age + "\r\n" +
                "性別：" + _gender + "\r\n" +
                "職業：" + _job + "\r\n" +
                "疑われている罪状：" + _crime + "\r\n" +
                "有罪か無罪か：" + _truth + "\r\n" +
                "動機：" + _motive + "\r\n" +
                "\r\n" +
                "状況に応じた被疑者の返答を、50文字以内かつセリフのみで、セリフは「」で囲ってください。" +
                "余計な説明等は省いてください。\r\n" +
                "容疑者は最近の動向や経歴はこちらです。「" + _background + "」" +
                "\r\n" +
                "<question>\r\n" +
                "質問：" + question;
            }
            else
            {
               _prompt = "<question>\r\n" +
                         "質問：" + question;
            }

        }

        // メッセージモデルを設定
        MessageModel l_messageModel = new MessageModel()
        {
            // 使用者
            role = "user",

            // 要求内容
            content = _prompt,
        };

        // 記憶に追加
        _history.Add(l_messageModel);

        // リクエストモデルを設定
        RequestData l_requestData = new RequestData()
        {
            // モデル名
            model = _modelName,

            // メッセージ全容に記憶を設定
            messages = _history,
        };

        // 要求分をJsonデータに変換
        string l_jsonData = JsonUtility.ToJson(l_requestData);

        // Jsonデータをバイトにエンコード
        byte[] l_postData = System.Text.Encoding.UTF8.GetBytes(l_jsonData);


        //###############################################

        // 指定URLにJson要求分をリクエスト
        UnityWebRequest l_request = UnityWebRequest.Post(API_URL, l_jsonData);

        l_request.timeout = timeOutMs;

        l_request.uploadHandler = new UploadHandlerRaw(l_postData);
        l_request.downloadHandler = new DownloadHandlerBuffer();

        //###############################################

        // リクエストボディーを指定
        l_request.SetRequestHeader("Content-Type", "application/json");

        // APIキーを認証
        l_request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

        // APIに対するリクエストを１回だけ再試行
        l_request.SetRequestHeader("X-Slack-Np-Retry", "1");


        // リクエストの送信結果
        UnityWebRequestAsyncOperation l_async = l_request.SendWebRequest();

        l_async.completed += (op) =>
        {
            // 通信結果が通信エラーまたはプロトコルエラーの場合
            if (l_request.result == UnityWebRequest.Result.ConnectionError ||
                l_request.result == UnityWebRequest.Result.ProtocolError)
            {
                // 通信結果エラーを出す。
                Debug.LogError(l_request.error);
            }
            else
            {

                try
                {
                    // GPT-3.5-turboの型に変換する。
                    OpenAIAPI l_responseData = JsonUtility.FromJson<OpenAIAPI>(l_request.downloadHandler.text);

                    // choice配列の最初のメッセージオブジェクト。
                    MessageModel l_generatedMessage = l_responseData.choices[0].message;

                    // 生成された文を格納。
                    string l_generateText = l_generatedMessage.content;

                    // 履歴に追加。
                    _history.Add(l_generatedMessage);

                    // 結果に格納。
                    _result = l_generateText;

                }
                catch
                {

                    // APIを取得できるサイトURL
                    const string l_getApiKeyURL = "https://platform.openai.com/account/api-keys";

                    // エラーログを出す。
                    Debug.LogError("リクエスト情報エラー：APIキーが期限切れ、もしくはAPIキー等が無効になっている可能性があります \n" +
                                    "APIを発行するには以下のURLから発行してください \n" +
                                    l_getApiKeyURL + "\n");
                }
            }


            // _resultを成形
            _result = ExtractString(_result, '「', '」');

            // 被疑者テキストに表示
            _suspectInputField.text = _result;


            _chatLog.text += "\r\n尋問官：「" + _playerInputField.text + "」\r\n" +
                            "容疑者：" + _suspectInputField.text + "\r\n";

            // プレイヤーの入力を初期化
            _playerInputField.text = "";

            // 通信中フラグをfalseにする
            _isRunning = false;

            // 準備完了状態にする
            _isReady = true;

            // 入力可能状態にする。
            _playerInputField.interactable = true;
        }; 
    }


    /// <summary>
    /// ボタンを押して、履歴をリセット
    /// </summary>
    public void ResetHistory()
    {
        // 準備がまだの場合、
        if (!_isReady) return;

        // 履歴をリセット
        _history.Clear();
        _count = 0;
        _progressStatus = Progress_Status.Question;
    }



    /// <summary>
    /// ChatGPTと対話することができるメソッド
    /// </summary>
    private void Interact()
    {
        // 準備完了している時。
        if (_isReady)
        {
            try
            {
                // 会話状態にする。
                _isTalking = true;

                // 準備状態にする。
                _isReady = false;

                // 次の回答を取得する
                Execute(_question, 1000);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        else
        {
            Debug.Log("通信中です！");
        }
    }



    /// <summary>
    /// 指定した文字からの文字列に変換する。
    /// </summary>
    /// <param name="inputString">変換前の文字列</param>
    /// <param name="startChar">最初の一文字</param>
    /// <param name="endChar">最後の一文字</param>
    /// <returns></returns>
    private string ExtractString(string inputString, char startChar, char endChar)
    {
        // 初期値
        string origin = inputString;

        // 最初の文字番号
        int startIndex = inputString.IndexOf(startChar);

        // 最後の文字番号
        int endIndex = inputString.IndexOf(endChar, startIndex + 1);

        if (startIndex != -1 && endIndex != -1)
        {
            try
            {
                return inputString.Substring(startIndex, endIndex - startIndex + 1);
            }
            catch
            {
                return origin;
            }

        }
        else return origin;
    }



    /// <summary>
    /// ボタンを押すことで、Statusが終了状態になり、自白率をだしてくれる。
    /// </summary>
    public void EndOfQuestioning(bool isGuilt)
    {

        if (!_isReady) return;

        // 終了状態にする
        _progressStatus = Progress_Status.End;

        // 結果が的中しているなら
        if ((_truth == "有罪" && isGuilt)||(_truth == "無罪" && !isGuilt)) _resultConfessionRate.text = "当然の判断だ";
        else                                                               _resultConfessionRate.text = "お前は懲戒だ";

        // 結果キャンバスを表示
        _resultCanvas.enabled = true;

    }


    /// <summary>
    /// シーンを再ロードする。
    /// </summary>
    public void BackTitle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}