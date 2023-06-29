using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace volumeCloud
{
    public class CloudUIVector2 : CloudUIBaseMapper
    {
        public UnityEngine.UI.InputField[] _inputfield = new UnityEngine.UI.InputField[2];
        public UnityEngine.UI.Text _text;
        public Vector2 _vec2;
        public UnityEngine.Events.UnityAction<string> _onValueChange;
        // Use this for initialization
        public void Init(string title, Vector2 defaultValue)
        {
            _title = title;
            _vec2 = defaultValue;
            _text = this.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>();
            this.gameObject.AddComponent<CloudUILangFix>().Init(_text, title);
            for (int i = 0; i < 2; i++)
            {
                _inputfield[i] = this.transform.GetChild(i).GetComponent<UnityEngine.UI.InputField>();
            }
            _inputfield[0].text = _vec2.x.ToString();
            _inputfield[1].text = _vec2.y.ToString();
        }
        public override string SaveData()
        {
            return $"\"{_title}\":{"{"}\"x\":{_vec2.x},\"y\":{_vec2.y}{"}"},";
        }
        public override void LoadData(Vector2 _vec2In)
        {
            _inputfield[0].text = _vec2In.x.ToString();
            _inputfield[1].text = _vec2In.y.ToString();
            _vec2 = _vec2In;
            _inputfield[0].onEndEdit.Invoke(_inputfield[0].text);
        }
        void Start()
        {
            _inputfield[0].onEndEdit.AddListener((string value) => {
                try
                {
                    _vec2.x = Convert.ToSingle(value);
                    //Debug.Log(_vec2);
                }
                catch { }
            } + _onValueChange);
            _inputfield[1].onEndEdit.AddListener((string value) => {
                try
                {
                    _vec2.y = Convert.ToSingle(value);
                    //Debug.Log(_vec2);
                }
                catch { }
            } + _onValueChange);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
