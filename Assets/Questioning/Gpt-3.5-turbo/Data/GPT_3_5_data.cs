using System.Collections.Generic;
using System;

namespace OpenAI.GPT_3_5_TURBO
{
    #region GPT-3.5-turbo

    [Serializable]
    public class RequestData
    {
        // GPT-3.5-turbo�̎g�p���郂�f����
        public string model;

        // ���̓e�L�X�g�̋L������
        public List<MessageModel> messages;

        #region �ڍאݒ�A�ꍇ�ɂ���ăR�����g�O���Ă��������B

        // �I�v�V�����̃e�L�X�g�t�B�[���h
        // public string prompt;

        // ���͂̐������ɁA�����_��
        // public float temperature;

        // �������镶�͂̍ő�̃g�[�N�������w��
        // public int max_tokens;

        // ���͐������L�[���[�h���w�肵�Ē�~����
        // public string[] stop;

        #endregion
    }

    [Serializable]
    public class MessageModel
    {
        // user bot system��
        public string role;

        // ���b�Z�[�W�{���̕�����
        public string content;

        // ���b�Z�[�W�̈�ӓI�Ȏ��ʎq������������
        // public string id;

        // ���b�Z�[�W�̍쐬����������ISO 8601�`���̕�����
        // public string timestamp;
    }

    [Serializable]
    public class OpenAIAPI
    {
        public string id;
        public string @object;
        public int created;
        public Choice[] choices;
        public Usage usage;
    }

    [Serializable]
    public class Choice
    {
        public int index;
        public MessageModel message;
        public string finish_reason;
    }

    [Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
    #endregion
}
