using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace volumeCloud
{
    public class CloudUIColorSlider : CloudUIBaseMapper
    {
        public UnityEngine.UI.Slider _slider;
        public UnityEngine.UI.InputField _inputfield;
        public UnityEngine.UI.Text _text;
        public float _Hue = 1f;
        public float _Saturation = 1f;
        public float _Volum = 1f;
        public UnityEngine.Events.UnityAction<float> _onValueChange;//both need init
        public UnityEngine.Events.UnityAction<string> _onValueChangeString;//both need init
        public Color _color;
        // Use this for initialization
        public void Init(string title, Color defaultValue)
        {
            _title = title;
            _text = this.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
            _inputfield = this.transform.GetChild(1).GetComponent<UnityEngine.UI.InputField>();
            _slider = this.transform.GetChild(2).GetComponent<UnityEngine.UI.Slider>();
            this.gameObject.AddComponent<CloudUILangFix>().Init(_text, title);

            _color = defaultValue;
            Color.RGBToHSV(defaultValue, out _Hue, out _Saturation, out _Volum);
            _inputfield.text = ColorUtility.ToHtmlStringRGB(defaultValue);
            _slider.value = _Hue;

            _onValueChange += (float value) =>
            {
                try
                {
                    _color = Color.HSVToRGB(value, _Saturation, _Volum);
                    _inputfield.text = ColorUtility.ToHtmlStringRGB(_color);
                }
                catch { }
            };
            _onValueChangeString += (string value) =>
            {
                try
                {
                    _color = Color.white;
                    ColorUtility.TryParseHtmlString("#" + value, out _color);
                    Color.RGBToHSV(_color, out _Hue, out _Saturation, out _Volum);
                    _slider.value = _Hue;
                }
                catch { }
            };
        }
        public override string SaveData()
        {
            return $"\"{_title}\":\"{_inputfield.text}\",";
        }
        public override void LoadData(string _value)
        {
            _inputfield.text = _value;
            _color = Color.white;
            ColorUtility.TryParseHtmlString("#" + _value, out _color);
            Color.RGBToHSV(_color, out _Hue, out _Saturation, out _Volum);
            _inputfield.onEndEdit.Invoke(_value);
        }
        void Start()
        {
            _slider.onValueChanged.AddListener(_onValueChange);
            _inputfield.onEndEdit.AddListener(_onValueChangeString);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
