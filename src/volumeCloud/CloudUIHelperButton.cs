using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace volumeCloud
{
    class CloudUIHelperButton : MonoBehaviour
    {
        public UnityEngine.UI.Button _button;
        public void Start()
        {
            _button = this.GetComponent<UnityEngine.UI.Button>();
            _button.onClick.AddListener(()=>
            {
                Modding.ModIO.OpenFolderInFileBrowser("Resources/Readme", false);
            });
        }
    }
}
