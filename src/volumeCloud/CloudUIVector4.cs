using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace volumeCloud
{
    public class CloudUIVector4 : CloudUIBaseMapper
    {

        public UnityEngine.UI.InputField[] _inputfield = new UnityEngine.UI.InputField[4];
        public UnityEngine.UI.Text _text;
        public Vector4 _vec4;
        public UnityEngine.Events.UnityAction<string> _onValueChange;
        // Use this for initialization
        public void Init(string title, Vector4 defaultValue)
        {
            _title = title;
            _vec4 = defaultValue;
            _text = this.transform.GetChild(4).GetComponent<UnityEngine.UI.Text>();
            this.gameObject.AddComponent<CloudUILangFix>().Init(_text, title);
            for (int i = 0; i < 4; i++)
            {
                _inputfield[i] = this.transform.GetChild(i).GetComponent<UnityEngine.UI.InputField>();
            }
            _inputfield[0].text = _vec4.x.ToString();
            _inputfield[1].text = _vec4.y.ToString();
            _inputfield[2].text = _vec4.z.ToString();
            _inputfield[3].text = _vec4.w.ToString();
        }
        public override string SaveData()
        {
            return $"\"{_title}\":{"{"}\"x\":{_vec4.x},\"y\":{_vec4.y},\"z\":{_vec4.z},\"w\":{_vec4.w}{"}"},";
        }
        public override void LoadData(Vector4 _vec4In)
        {
            _inputfield[0].text = _vec4In.x.ToString();
            _inputfield[1].text = _vec4In.y.ToString();
            _inputfield[2].text = _vec4In.z.ToString();
            _inputfield[3].text = _vec4In.w.ToString();
            _vec4 = _vec4In;
            _inputfield[0].onEndEdit.Invoke(_inputfield[0].text);
        }
        void Start()
        {
            _inputfield[0].onEndEdit.AddListener((string value) => {
                try
                {
                    _vec4.x = Convert.ToSingle(value);
                    //Debug.Log(_vec4);
                }
                catch { }
            } + _onValueChange);
            _inputfield[1].onEndEdit.AddListener((string value) => {
                try
                {
                    _vec4.y = Convert.ToSingle(value);
                    //Debug.Log(_vec4);
                }
                catch { }
            } + _onValueChange);
            _inputfield[2].onEndEdit.AddListener((string value) => {
                try
                {
                    _vec4.z = Convert.ToSingle(value);
                    //Debug.Log(_vec4);
                }
                catch { }
            } + _onValueChange);
            _inputfield[3].onEndEdit.AddListener((string value) => {
                try
                {
                    _vec4.w = Convert.ToSingle(value);
                    //Debug.Log(_vec4);
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
