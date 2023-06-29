using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace volumeCloud
{
    public class CloudUIVector3 : CloudUIBaseMapper
    {
        public UnityEngine.UI.InputField[] _inputfield = new UnityEngine.UI.InputField[3];
        public UnityEngine.UI.Text _text;
        public Vector3 _vec3;
        public UnityEngine.Events.UnityAction<string> _onValueChange;
        public void Init(string title, Vector3 defaultValue)
        {
            _title = title;
            _vec3 = defaultValue;
            _text = this.transform.GetChild(3).GetComponent<UnityEngine.UI.Text>();
            this.gameObject.AddComponent<CloudUILangFix>().Init(_text, title);
            for (int i = 0; i < 3; i++)
            {
                _inputfield[i] = this.transform.GetChild(i).GetComponent<UnityEngine.UI.InputField>();
            }
            _inputfield[0].text = _vec3.x.ToString();
            _inputfield[1].text = _vec3.y.ToString();
            _inputfield[2].text = _vec3.z.ToString();
        }
        public override string SaveData()
        {
            return $"\"{_title}\":{"{"}\"x\":{_vec3.x},\"y\":{_vec3.y},\"z\":{_vec3.z}{"}"},";
        }
        public override void LoadData(Vector3 _vec3In)
        {
            _inputfield[0].text = _vec3In.x.ToString();
            _inputfield[1].text = _vec3In.y.ToString();
            _inputfield[2].text = _vec3In.z.ToString();
            _vec3 = _vec3In;
            _inputfield[0].onEndEdit.Invoke(_inputfield[0].text);
        }
        // Use this for initialization
        void Start()
        {
            _inputfield[0].onEndEdit.AddListener((string value) => {
                try
                {
                    _vec3.x = Convert.ToSingle(value);
                   //Debug.Log(_vec3);
                }
                catch { }
            } + _onValueChange);
            _inputfield[1].onEndEdit.AddListener((string value) => {
                try
                {
                    _vec3.y = Convert.ToSingle(value);
                    //Debug.Log(_vec3);
                }
                catch { }
            } + _onValueChange);
            _inputfield[2].onEndEdit.AddListener((string value) => {
                try
                {
                    _vec3.z = Convert.ToSingle(value);
                    //Debug.Log(_vec3);
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
