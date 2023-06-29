using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace volumeCloud
{
    class CloudUIFixFog : MonoBehaviour
    {
        public UnityEngine.UI.Button _button;
        void Start()
        {
            UnityEngine.UI.Text _text = this.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
            this.gameObject.AddComponent<CloudUILangFix>().Init(_text, "_fog_skybox_fix");

            _button = this.GetComponent<UnityEngine.UI.Button>();
            _button.onClick.AddListener(() => {
                volumeCloud.VolumeCloudController.Instance.FixFog();
                volumeCloud.VolumeCloudController.Instance.FixSkyBox();
            });
        }
    }
}
