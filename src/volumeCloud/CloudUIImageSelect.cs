using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
namespace volumeCloud
{
    public class CloudUIImageSelect : CloudUIBaseMapper
    {

        // Use this for initialization
        public UnityEngine.UI.RawImage _image;
        public UnityEngine.UI.Button _button;
        public UnityEngine.UI.InputField _inputField;
        public UnityEngine.UI.Text _text;
        public Texture2D _tex2D;
        private string _resourcePath;
        public UnityEngine.Events.UnityAction<string> _onValueChange;
        public void Init(string title, string folderPath, string defaultValue, Texture2D defaultTex2D)
        {
            _title = title;
            _button = this.transform.GetChild(0).GetComponent<UnityEngine.UI.Button>();
            _inputField = this.transform.GetChild(1).GetComponent<UnityEngine.UI.InputField>();
            _text = this.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>();
            _image = this.transform.GetChild(3).GetComponent<UnityEngine.UI.RawImage>();
            this.gameObject.AddComponent<CloudUILangFix>().Init(_text, title);

            _resourcePath = folderPath;
            _inputField.text = defaultValue;
            _tex2D = Instantiate(defaultTex2D);
            _image.texture = _tex2D;
            _image.raycastTarget = false;
        }
        public override string SaveData()
        {
            return $"\"{_title}\":\"{_inputField.text}\",";
        }
        public override void LoadData(string _value)
        {
            _inputField.text = _value;
            _inputField.onEndEdit.Invoke(_value);
        }
        void Start()
        {
            _button.onClick.AddListener(() =>
            {
                Modding.ModIO.OpenFolderInFileBrowser(_resourcePath, true);
            });
            _inputField.onEndEdit.AddListener((string value) => {
                string _filename = _inputField.text;
                if(_filename.Length < 4)
                {
                    _filename += ".png";
                }
                else
                {
                    _filename = (_filename.Substring(_filename.Length - 4, 4) == ".png") ? _filename : $"{_filename}.png";
                }
                if (!Modding.ModIO.ExistsFile($"{_resourcePath}/{_filename}", true))
                    return;
                if(_tex2D.LoadImage(Modding.ModIO.ReadAllBytes($"{_resourcePath}/{_filename}", true)))
                {
                    _image.texture = _tex2D;
                }
            } +_onValueChange);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
