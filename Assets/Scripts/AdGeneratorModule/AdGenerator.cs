using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Math = System.Math;
using Random = UnityEngine.Random;

namespace AdGeneratorModule
{
    public class AdGenerator : MonoBehaviour
    {
        private GameObject _cameraGameObject;
        private Camera _cameraComponent;
        private GameObject _canvas;

        private Text _message;
        private Text _header;
        private Image _bg;

        private string[] _politePhrases = new string[]
        {
            "it is very kind of you to",
            "may we take this opportunity of thanking you for",
            "please accept our sincere deep appreciation for",
            "we apologize for",
            "we very much regret to say that",
            "please accept our apologies for",
            "we wish to offer our sincere apologies for",
            "we were extremely sorry that",
            "we sincerely regret that",
            "we appreciate your attitude",
            "we shall be pleased if",
            "we should be glad if you would",
            "please take notice that",
            "we have to inform you that",
            "may we remind you that",
            "we intend to",
            "we have to inform you",
            "please take due note that",
            "we are sure you will understand that our actions will be in the best interests of",
            "we shall do our best to",
            "please rest assured that we will do our best to bring this matter to a satisfactory conclusion",
            "we trust your kind attention on the matter",
            "we regret to inform you that",
            "to our great regret we must inform you that"
        };

        private string[] _politeHeaders = new string[]
        {
            "dear tenants",
            "dear everyone",
            "pay little attention",
            "hello",
            "hello everyone",
            "announcement",
            "important message"
        };

        private static readonly int MessageTextMaxLineCount = 3;
        private static readonly float NewLineProbability = 0.1f;
        private static readonly float UpperCaseProbability = 0.02f;

        private static readonly float NumbersProbability = 0.4f;
        private static readonly int NumbersMaxCount = 5;

        private static readonly int NewLineSpacesMaxCount = 2;
        private static readonly float BgColorMinValue = .9f;

        private void Start()
        {
            _cameraGameObject = transform.Find("Camera").gameObject;
            _cameraComponent = _cameraGameObject.GetComponent<Camera>();
            _canvas = transform.Find("Canvas").gameObject;

            _message = _canvas.transform.Find("message").GetComponent<Text>();
            _header = _canvas.transform.Find("header").GetComponent<Text>();
            _bg = _canvas.transform.Find("bg").GetComponent<Image>();
        }

        public Texture2D GetRandomAdTexture()
        {
            GenerateRandomAd();
            return CameraUtils.GetCameraTexture(_cameraComponent, 1024, 1024);
        }

        private string Mul(string str, int times)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < times; i++) sb.Append(str);

            return sb.ToString();
        }

        private void AppendRandomNumbersToStringBuilder(ref StringBuilder sb)
        {
            if (Random.Range(0, 1f) < NumbersProbability)
            {
                var numbersStr = Random.Range(0, 1f).ToString();
                sb.Append(numbersStr.Substring(2, Math.Min(NumbersMaxCount, numbersStr.Length - 3)));
                sb.Append('\n');
            }
        }

        private void GenerateRandomAd()
        {
            string GetRandomAmountOfSpaces()
            {
                return Mul(" ", Random.Range(0, NewLineSpacesMaxCount));
            }

            _bg.color = new Color(
                Random.Range(BgColorMinValue, 1f),
                Random.Range(BgColorMinValue, 1f),
                Random.Range(BgColorMinValue, 1f)
            );

            _header.text = _politeHeaders[Random.Range(0, _politeHeaders.Length)].ToUpper();

            var messageTextSb = new StringBuilder();
            var messageTextCount = Random.Range(1, MessageTextMaxLineCount + 1);
            var messageTextValue = _politePhrases[Random.Range(0, _politePhrases.Length)];

            AppendRandomNumbersToStringBuilder(ref messageTextSb);

            for (var i = 0; i < messageTextCount; i++)
            {
                var messageTextLineSb = new StringBuilder(messageTextValue);

                for (var j = 0; j < messageTextLineSb.Length; j++)
                    if (messageTextLineSb[j].Equals(' '))
                    {
                        if (Random.Range(0, 1f) < NewLineProbability) messageTextLineSb[j] = '\n';
                    }
                    else
                    {
                        if (Random.Range(0, 1f) < UpperCaseProbability)
                            messageTextLineSb[j] = messageTextLineSb[j].ToString().ToUpper()[0];
                    }

                for (var j = 0; j < messageTextLineSb.Length; j++)
                    if (messageTextLineSb[j].Equals('\n'))
                        messageTextLineSb.Insert(j + 1, GetRandomAmountOfSpaces());

                messageTextSb.Append(messageTextLineSb);
                messageTextSb.Append('\n');
                messageTextSb.Append(GetRandomAmountOfSpaces());
            }

            AppendRandomNumbersToStringBuilder(ref messageTextSb);
            _message.text = messageTextSb.ToString();
        }
    }
}