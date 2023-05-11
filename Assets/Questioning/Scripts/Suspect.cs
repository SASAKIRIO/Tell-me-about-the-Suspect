using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using OpenAI.GPT_3_5_TURBO;

/// <summary>
/// GPT-3.5-turbo�ɒʐM���ĉ������󂯎��鏈��
/// </summary>
public class Suspect : MonoBehaviour
{

    // API�L�[
    private string _apiKey;

    // ���[�U�A��^�ҁAAPI����͂���e�L�X�g�t�B�[���h
    [SerializeField] private InputField _playerInputField, _suspectInputField, _apiKeyInputField;

    // �`���b�g���O�@�\
    [SerializeField] private Text _chatLog;

    // File���nText
    [SerializeField] private Text _fileName, _fileAge, _fileCrime, _fileInfo;

    // ���ʌnText
    [SerializeField] private Text _resultTruth, _resultConfessionRate;

    [SerializeField] private Canvas _resultCanvas;

    [SerializeField] private Crimedata _crimeData;

    // �������ꂽ�R�[�h
    private string _result = default;

    //API��URL
    private const string API_URL = "https://api.openai.com/v1/chat/completions";

    // ���f����
    private const string _modelName = "gpt-3.5-turbo";

    // �v����
    private string _prompt = default;

    // �L��
    private List<MessageModel> _history = new List<MessageModel>();

    // ������e ���O �N�� ���� �E�� IQ �w�i �^���Ă���� �L�߂����߂� ���@
    private string _question, _name, _age, _gender, _job,  _background, _crime, _truth, _motive;

    // API�ʐM�����ۂ��𔻒f����bool�^�ϐ�
    private bool _isRunning = false;

    // �񓚂��������������f����bool�^�ϐ�
    private bool _isReady = false;

    // ��b�������f����bool�^�ϐ�
    private bool _isTalking = false;

    // �������肵����
    private int _count = 0;

    // �q��i��
    private enum Progress_Status
    {
        // ������
        Question,

        // �q��I�����
        End
    }

    [SerializeField] private Progress_Status _progressStatus = Progress_Status.Question;


    // ��Փx
    private enum Difficult_Status
    {
        Easy = 1,
        Normal = 2,
        Hard = 3,
    }

    [SerializeField] private Difficult_Status _difficult = Difficult_Status.Easy;

    // ���\�b�h�� #####################################################


    private void Start()
    {
        // API�L�[�̃e�L�X�g������B
        _apiKey = _apiKeyInputField.text;

        // ��^�҂̏����X�N���v�^�u���I�u�W�F�N�g����Z�b�g
        SetConfessionInfo();

        // �t�@�C���̏����Z�b�g
        SetFileInfo();

        // ����������Ԃɂ���
        _isReady = true;

        // �L����������
        ResetHistory();
    }


    private void Update()
    {
        // API�L�[�̃e�L�X�g������B
        _apiKey = _apiKeyInputField.text;

        // Text���󂶂�Ȃ���ԂŁA�E�N���b�N�������ƃ��N�G�X�g�𑗐M�B
        if (Input.GetMouseButtonDown(1) && _playerInputField.text != "")
        {
            _question = _playerInputField.text;

            Interact();
        }
    }

    // �L�������Z�b�g
    private void OnDestroy()
    {
        if (_history != null)
        {
            _history.Clear();
        }
    }


    private void SetConfessionInfo()
    {
        // �����_���l��^����B
        int l_index = UnityEngine.Random.Range(0, _crimeData._suspectInfo.Count);

        // ��^�҂̏�����́B
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
        _fileInfo.text = "�E�Ƃ�"+_job+"�B"+_background;
        _resultTruth.text = _truth;
    }



    /// <summary>
    /// �v���𑗂�
    /// </summary>
    public void Execute(string question,int timeOutMs)
    {

        // �ʐM���Ă���Ƃ��̓��O�������āAreturn����B
        if (_isRunning)
        {
            Debug.Log("���ݒʐM��");
            return;
        }

        // ���͕s�ɂ���B
        _playerInputField.interactable = false;

        // �ʐM���̓e�L�X�g�t�B�[���h��...�ƕ\������B
        _suspectInputField.text = "...";

        // ��b�񐔁B
        _count++;

        // �ʐM���t���O��true�ɂ���
        _isRunning = true;

        // ������Ԃɂ���
        _isReady = false;


        if (_progressStatus == Progress_Status.Question)
        {
            if (_count == 1)
            {
                _prompt = "����ɂ��́AChatGPT����B\r\n" +
                "���͐q����e�[�}�ɂ����Q�[���𐧍삵�Ă��܂��B\r\n" +
                "ChatGPT����ɂ͗e�^�҂ɂȂ��Ăق����ł��B\r\n" +
                "�S�g��ԃ��x���́A1�`3�܂ł���A����̔�^�҂̓��x��" + (int)_difficult + "�ł��B���x����������΍����قǁA�S�g��Ԃ��s����ȏ�Ԃł��B\r\n" +
                "�ǂ̃��x���ł��q�⎩�̂͑f���Ɏ󂯂āA�Ȃ��Ȃ��������Ȃ��悤�ɂ��Ă��������B" +
                "�S�g��Ԃ��ُ�̏ꍇ�A�e�^��۔F�A�܂��͖ٔ邵�Ă��������B\r\n" +
                "�S�����������Ȃ��ƁA�������Ȃ��悤�ɂ��Ă��������B\r\n" +
                "ChatGPT���񂪗e�^�҂������邽�߂̃��N�G�X�g�𑗐M�������Ǝv���܂��B" +
                "�e�^�҂̐ݒ�͈ȉ��̒ʂ�ŁA�e�^�҂͐q����󂯂Ă��܂��B" + _count + "��ڂ̎���ł���A<question>�̎���ɑ΂���ԓ������Ă��������B\r\n" +
                "\r\n" +
                "<data>\r\n" +
                "���O�F" + _name + "\r\n" +
                "�N��F" + _age + "\r\n" +
                "���ʁF" + _gender + "\r\n" +
                "�E�ƁF" + _job + "\r\n" +
                "�^���Ă���ߏ�F" + _crime + "\r\n" +
                "�L�߂����߂��F" + _truth + "\r\n" +
                "���@�F" + _motive + "\r\n" +
                "\r\n" +
                "�󋵂ɉ�������^�҂̕ԓ����A50�����ȓ����Z���t�݂̂ŁA�Z���t�́u�v�ň͂��Ă��������B" +
                "�]�v�Ȑ������͏Ȃ��Ă��������B\r\n" +
                "�e�^�҂͍ŋ߂̓�����o���͂�����ł��B�u" + _background + "�v" +
                "\r\n" +
                "<question>\r\n" +
                "����F" + question;
            }
            else
            {
               _prompt = "<question>\r\n" +
                         "����F" + question;
            }

        }

        // ���b�Z�[�W���f����ݒ�
        MessageModel l_messageModel = new MessageModel()
        {
            // �g�p��
            role = "user",

            // �v�����e
            content = _prompt,
        };

        // �L���ɒǉ�
        _history.Add(l_messageModel);

        // ���N�G�X�g���f����ݒ�
        RequestData l_requestData = new RequestData()
        {
            // ���f����
            model = _modelName,

            // ���b�Z�[�W�S�e�ɋL����ݒ�
            messages = _history,
        };

        // �v������Json�f�[�^�ɕϊ�
        string l_jsonData = JsonUtility.ToJson(l_requestData);

        // Json�f�[�^���o�C�g�ɃG���R�[�h
        byte[] l_postData = System.Text.Encoding.UTF8.GetBytes(l_jsonData);


        //###############################################

        // �w��URL��Json�v���������N�G�X�g
        UnityWebRequest l_request = UnityWebRequest.Post(API_URL, l_jsonData);

        l_request.timeout = timeOutMs;

        l_request.uploadHandler = new UploadHandlerRaw(l_postData);
        l_request.downloadHandler = new DownloadHandlerBuffer();

        //###############################################

        // ���N�G�X�g�{�f�B�[���w��
        l_request.SetRequestHeader("Content-Type", "application/json");

        // API�L�[��F��
        l_request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

        // API�ɑ΂��郊�N�G�X�g���P�񂾂��Ď��s
        l_request.SetRequestHeader("X-Slack-Np-Retry", "1");


        // ���N�G�X�g�̑��M����
        UnityWebRequestAsyncOperation l_async = l_request.SendWebRequest();

        l_async.completed += (op) =>
        {
            // �ʐM���ʂ��ʐM�G���[�܂��̓v���g�R���G���[�̏ꍇ
            if (l_request.result == UnityWebRequest.Result.ConnectionError ||
                l_request.result == UnityWebRequest.Result.ProtocolError)
            {
                // �ʐM���ʃG���[���o���B
                Debug.LogError(l_request.error);
            }
            else
            {

                try
                {
                    // GPT-3.5-turbo�̌^�ɕϊ�����B
                    OpenAIAPI l_responseData = JsonUtility.FromJson<OpenAIAPI>(l_request.downloadHandler.text);

                    // choice�z��̍ŏ��̃��b�Z�[�W�I�u�W�F�N�g�B
                    MessageModel l_generatedMessage = l_responseData.choices[0].message;

                    // �������ꂽ�����i�[�B
                    string l_generateText = l_generatedMessage.content;

                    // �����ɒǉ��B
                    _history.Add(l_generatedMessage);

                    // ���ʂɊi�[�B
                    _result = l_generateText;

                }
                catch
                {

                    // API���擾�ł���T�C�gURL
                    const string l_getApiKeyURL = "https://platform.openai.com/account/api-keys";

                    // �G���[���O���o���B
                    Debug.LogError("���N�G�X�g���G���[�FAPI�L�[�������؂�A��������API�L�[���������ɂȂ��Ă���\��������܂� \n" +
                                    "API�𔭍s����ɂ͈ȉ���URL���甭�s���Ă������� \n" +
                                    l_getApiKeyURL + "\n");
                }
            }


            // _result�𐬌`
            _result = ExtractString(_result, '�u', '�v');

            // ��^�҃e�L�X�g�ɕ\��
            _suspectInputField.text = _result;


            _chatLog.text += "\r\n�q�⊯�F�u" + _playerInputField.text + "�v\r\n" +
                            "�e�^�ҁF" + _suspectInputField.text + "\r\n";

            // �v���C���[�̓��͂�������
            _playerInputField.text = "";

            // �ʐM���t���O��false�ɂ���
            _isRunning = false;

            // ����������Ԃɂ���
            _isReady = true;

            // ���͉\��Ԃɂ���B
            _playerInputField.interactable = true;
        }; 
    }


    /// <summary>
    /// �{�^���������āA���������Z�b�g
    /// </summary>
    public void ResetHistory()
    {
        // �������܂��̏ꍇ�A
        if (!_isReady) return;

        // ���������Z�b�g
        _history.Clear();
        _count = 0;
        _progressStatus = Progress_Status.Question;
    }



    /// <summary>
    /// ChatGPT�ƑΘb���邱�Ƃ��ł��郁�\�b�h
    /// </summary>
    private void Interact()
    {
        // �����������Ă��鎞�B
        if (_isReady)
        {
            try
            {
                // ��b��Ԃɂ���B
                _isTalking = true;

                // ������Ԃɂ���B
                _isReady = false;

                // ���̉񓚂��擾����
                Execute(_question, 1000);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        else
        {
            Debug.Log("�ʐM���ł��I");
        }
    }



    /// <summary>
    /// �w�肵����������̕�����ɕϊ�����B
    /// </summary>
    /// <param name="inputString">�ϊ��O�̕�����</param>
    /// <param name="startChar">�ŏ��̈ꕶ��</param>
    /// <param name="endChar">�Ō�̈ꕶ��</param>
    /// <returns></returns>
    private string ExtractString(string inputString, char startChar, char endChar)
    {
        // �����l
        string origin = inputString;

        // �ŏ��̕����ԍ�
        int startIndex = inputString.IndexOf(startChar);

        // �Ō�̕����ԍ�
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
    /// �{�^�����������ƂŁAStatus���I����ԂɂȂ�A�������������Ă����B
    /// </summary>
    public void EndOfQuestioning(bool isGuilt)
    {

        if (!_isReady) return;

        // �I����Ԃɂ���
        _progressStatus = Progress_Status.End;

        // ���ʂ��I�����Ă���Ȃ�
        if ((_truth == "�L��" && isGuilt)||(_truth == "����" && !isGuilt)) _resultConfessionRate.text = "���R�̔��f��";
        else                                                               _resultConfessionRate.text = "���O�͒�����";

        // ���ʃL�����o�X��\��
        _resultCanvas.enabled = true;

    }


    /// <summary>
    /// �V�[�����ă��[�h����B
    /// </summary>
    public void BackTitle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}