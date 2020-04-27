using Math = System.Math;
using Random = UnityEngine.Random;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AdGenerator : MonoBehaviour
{
    GameObject cameraGameObject;
    Camera cameraComponent;
    GameObject canvas;

    Text message;
    Text header;
    Image bg;

    string[] politePhrases = new string[] {
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

    string[] politeHeaders = new string[] {
        "dear tenants",
        "dear everyone",
        "pay little attention",
        "hello",
        "hello everyone",
        "announcement",
        "important message"
    };

    static readonly int messageTextMaxLineCount = 3;
    static readonly float newLineProbability = 0.1f;
    static readonly float upperCaseProbability = 0.02f;

    static readonly float numbersProbability = 0.4f;
    static readonly int numbersMaxCount = 5;

    static readonly int newLineSpacesMaxCount = 2;
    static readonly float bgColorMinValue = .8f;

    void Start()
    {
        cameraGameObject = transform.Find("Camera").gameObject;
        cameraComponent = cameraGameObject.GetComponent<Camera>();
        canvas = transform.Find("Canvas").gameObject;

        message = canvas.transform.Find("message").gameObject.GetComponent<Text>();
        header = canvas.transform.Find("header").gameObject.GetComponent<Text>();
        bg = canvas.transform.Find("bg").gameObject.GetComponent<Image>();
    }

    public Texture2D GetRandomAdTexture()
    {
        GenerateRandomAd();
        return CameraUtils.GetCameraTexture(cameraComponent, 1024, 1024);
    }

    string Mul(string str, int times)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < times; i++)
        {
            sb.Append(str);
        }

        return sb.ToString();
    }

    void AppendRandomNumbersToStringBuilder(ref StringBuilder sb)
    {
        if (Random.Range(0, 1f) < numbersProbability)
        {
            string numbersStr = Random.Range(0, 1f).ToString();
            sb.Append(numbersStr.Substring(2, Math.Min(numbersMaxCount, numbersStr.Length - 3)));
            sb.Append('\n');
        }
    }

    void GenerateRandomAd()
    {
        string getRandomAmountOfSpaces() => Mul(" ", Random.Range(0, newLineSpacesMaxCount));

        bg.color = new Color(
            Random.Range(bgColorMinValue, 1f),
            Random.Range(bgColorMinValue, 1f),
            Random.Range(bgColorMinValue, 1f)
        );

        header.text = politeHeaders[Random.Range(0, politeHeaders.Length)].ToUpper();

        StringBuilder messageTextSb = new StringBuilder();
        int messageTextCount = Random.Range(1, messageTextMaxLineCount + 1);
        string messageTextValue = politePhrases[Random.Range(0, politePhrases.Length)];

        AppendRandomNumbersToStringBuilder(ref messageTextSb);

        for (int i = 0; i < messageTextCount; i++)
        {
            StringBuilder messageTextLineSb = new StringBuilder(messageTextValue);

            for (int j = 0; j < messageTextLineSb.Length; j++)
            {
                if (messageTextLineSb[j].Equals(' '))
                {
                    if (Random.Range(0, 1f) < newLineProbability)
                    {
                        messageTextLineSb[j] = '\n';
                    }
                }
                else
                {
                    if (Random.Range(0, 1f) < upperCaseProbability)
                    {
                        messageTextLineSb[j] = messageTextLineSb[j].ToString().ToUpper()[0];
                    }
                }
            }

            for (int j = 0; j < messageTextLineSb.Length; j++)
            {
                if (messageTextLineSb[j].Equals('\n'))
                {
                    messageTextLineSb.Insert(j + 1, getRandomAmountOfSpaces());
                }

            }

            messageTextSb.Append(messageTextLineSb);
            messageTextSb.Append('\n');
            messageTextSb.Append(getRandomAmountOfSpaces());
        }

        AppendRandomNumbersToStringBuilder(ref messageTextSb);
        message.text = messageTextSb.ToString();
    }
}
