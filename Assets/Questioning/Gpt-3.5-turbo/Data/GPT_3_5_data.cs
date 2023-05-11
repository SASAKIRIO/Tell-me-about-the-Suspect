using System.Collections.Generic;
using System;

namespace OpenAI.GPT_3_5_TURBO
{
    #region GPT-3.5-turbo

    [Serializable]
    public class RequestData
    {
        // GPT-3.5-turboの使用するモデル名
        public string model;

        // 入力テキストの記憶履歴
        public List<MessageModel> messages;

        #region 詳細設定、場合によってコメント外してください。

        // オプションのテキストフィールド
        // public string prompt;

        // 文章の生成時に、ランダム
        // public float temperature;

        // 生成する文章の最大のトークン数を指定
        // public int max_tokens;

        // 文章生成をキーワードを指定して停止する
        // public string[] stop;

        #endregion
    }

    [Serializable]
    public class MessageModel
    {
        // user bot system等
        public string role;

        // メッセージ本文の文字列
        public string content;

        // メッセージの一意的な識別子を示す文字列
        // public string id;

        // メッセージの作成日時を示すISO 8601形式の文字列
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
