using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
namespace volumeCloud
{
    public class CloudUILangFix : MonoBehaviour
    {
        public UnityEngine.UI.Text _text;
        public string _langID;
        public void Init(UnityEngine.UI.Text text, string langID)
        {
            this._text = text;
            this._langID = langID;
            this._text.text = LanguageManager.Instance.outLang[this._langID];
        }
        public void FixLang(object sender, StringEventArgs e)
        {
            this._text.text = LanguageManager.Instance.outLang[this._langID];
        }
        public void Start()
        {
            LanguageManager.Instance.OnLanguageChange += FixLang;
        }
    }
}
