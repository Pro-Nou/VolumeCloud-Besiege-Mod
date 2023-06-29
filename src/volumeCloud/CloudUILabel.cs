using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace volumeCloud
{
    public class CloudUILabel : MonoBehaviour
    {
        public UnityEngine.UI.Text _text;
        // Use this for initialization
        public void Init(string title)
        {
            _text = this.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
            this.gameObject.AddComponent<CloudUILangFix>().Init(_text, title);
        }
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
