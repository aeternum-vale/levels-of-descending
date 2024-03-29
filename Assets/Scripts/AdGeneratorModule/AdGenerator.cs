﻿using System;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

namespace AdGeneratorModule
{
    public class AdGenerator : MonoBehaviour
    {
        private static readonly int MessageTextMaxLineCount = 3;
        private static readonly float NewLineProbability = 0.1f;
        private static readonly float UpperCaseProbability = 0.02f;

        private static readonly float NumbersProbability = 0.4f;
        private static readonly int NumbersMaxCount = 5;

        private static readonly int NewLineSpacesMaxCount = 2;
        private static readonly float BgColorMinValue = .9f;

        private readonly string[] _politeHeaders =
        {
            "dear tenants",
            "dear everyone",
            "pay little attention",
            "hello",
            "hello everyone",
            "announcement",
            "important message"
        };

        private readonly string[] _politePhrases =
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

        private Image _bg;
        private Camera _cameraComponent;
        private GameObject _cameraGameObject;
        private GameObject _canvas;
        private Text _header;

        private Text _message;

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

        private static string Mul(string str, int times)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < times; i++) sb.Append(str);

            return sb.ToString();
        }

        private static void AppendRandomNumbersToStringBuilder(ref StringBuilder sb)
        {
            if (!(Random.Range(0, 1f) < NumbersProbability)) return;

            string numbersStr = Random.Range(0, 1f).ToString(CultureInfo.InvariantCulture);
            sb.Append(numbersStr.Substring(2, Math.Min(NumbersMaxCount, numbersStr.Length - 3)));
            sb.Append('\n');
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

            StringBuilder messageTextSb = new StringBuilder();
            int messageTextCount = Random.Range(1, MessageTextMaxLineCount + 1);
            string messageTextValue = _politePhrases[Random.Range(0, _politePhrases.Length)];

            AppendRandomNumbersToStringBuilder(ref messageTextSb);

            for (int i = 0; i < messageTextCount; i++)
            {
                StringBuilder messageTextLineSb = new StringBuilder(messageTextValue);

                for (int j = 0; j < messageTextLineSb.Length; j++)
                    if (messageTextLineSb[j].Equals(' '))
                    {
                        if (Random.Range(0, 1f) < NewLineProbability) messageTextLineSb[j] = '\n';
                    }
                    else
                    {
                        if (Random.Range(0, 1f) < UpperCaseProbability)
                            messageTextLineSb[j] = messageTextLineSb[j].ToString().ToUpper()[0];
                    }

                for (int j = 0; j < messageTextLineSb.Length; j++)
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