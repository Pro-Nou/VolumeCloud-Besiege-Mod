using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace volumeCloud
{
    public class CloudUISlider : CloudUIBaseMapper
    {
        public UnityEngine.UI.Slider _slider;
        public UnityEngine.UI.InputField _inputfield;
        public UnityEngine.UI.Text _text;
        
        public float _defaultValue;
        public float _min;
        public float _max;
        public UnityEngine.Events.UnityAction<float> _onValueChange;
        // Use this for initialization
        public void Init(string title, float defaultValue, float min, float max)
        {
            _title = title;
            _text = this.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
            _inputfield = this.transform.GetChild(1).GetComponent<UnityEngine.UI.InputField>();
            _slider = this.transform.GetChild(2).GetComponent<UnityEngine.UI.Slider>();
            this.gameObject.AddComponent<CloudUILangFix>().Init(_text, title);

            _slider.maxValue = max;
            _slider.minValue = min;
            _slider.value = defaultValue;
            _inputfield.text = defaultValue.ToString();
            //_onValueChange = new UnityEngine.Events.UnityAction<float>((float value) => { });

            _onValueChange += (float value) =>
            {
                try
                {
                    _inputfield.text = value.ToString();
                }
                catch { }
            };
        }
        public override string SaveData()
        {
            return $"\"{_title}\":{_slider.value},";
        }
        public override void LoadData(float _value)
        {
            _slider.value = _value;
            _slider.onValueChanged.Invoke(_value);
        }
        void Start()
        {

            _slider.onValueChanged.AddListener(_onValueChange);
            _inputfield.onEndEdit.AddListener((string value) => {
                try
                {
                    _slider.value = Convert.ToSingle(value);
                }
                catch { }
            });
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
