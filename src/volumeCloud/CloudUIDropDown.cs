using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
namespace volumeCloud
{
    public class CloudUIDropDown : CloudUIBaseMapper
    {
        public UnityEngine.UI.Dropdown _dropDown;
        public UnityEngine.UI.Text _text;
        public UnityEngine.Events.UnityAction<int> _onValueChange;
        // Use this for initialization
        public void Init(string title, System.Collections.Generic.List<string> values)
        {
            _title = title;
            _dropDown = this.transform.GetChild(0).GetComponent<UnityEngine.UI.Dropdown>();
            _text = this.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
            this.gameObject.AddComponent<CloudUILangFix>().Init(_text, title);
            _dropDown.ClearOptions();
            _dropDown.AddOptions(values);
        }
        public override string SaveData()
        {
            return $"\"{_title}\":{_dropDown.value},";
        }
        public override void LoadData(int _value)
        {
            _dropDown.value = _value;
            _dropDown.onValueChanged.Invoke(_value);
        }
        void Start()
        {
            _dropDown.onValueChanged.AddListener(_onValueChange);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
