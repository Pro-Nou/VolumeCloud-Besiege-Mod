using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace volumeCloud
{
    public class CloudUIClose : MonoBehaviour
    {

        public UnityEngine.UI.Button _button;
        // Use this for initialization
        void Start()
        {
            _button = this.transform.GetComponent<UnityEngine.UI.Button>();
            _button.onClick.AddListener(() => {
                this.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                this.transform.parent.gameObject.SetActive(false);
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
