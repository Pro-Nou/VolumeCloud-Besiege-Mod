using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace volumeCloud
{
    class CloudUIConfigSave : MonoBehaviour
    {
        public UnityEngine.UI.Button _loadButton;
        public UnityEngine.UI.Button _saveButton;
        public UnityEngine.UI.InputField _filenameInput;
        public UnityEngine.UI.Button _folderButton;

        public UnityEngine.Events.UnityAction _onFolderClicked;
        public UnityEngine.Events.UnityAction _onSaveClicked;
        public UnityEngine.Events.UnityAction _onLoadClicked;
        public void Start()
        {
            _loadButton = this.transform.GetChild(0).GetComponent<UnityEngine.UI.Button>();
            _saveButton = this.transform.GetChild(1).GetComponent<UnityEngine.UI.Button>();
            _filenameInput = this.transform.GetChild(2).GetComponent<UnityEngine.UI.InputField>();
            _folderButton = this.transform.GetChild(3).GetComponent<UnityEngine.UI.Button>();

            _loadButton.gameObject.AddComponent<CloudUILangFix>().Init(_loadButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>(), "_load_config");
            _saveButton.gameObject.AddComponent<CloudUILangFix>().Init(_saveButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>(), "_save_config");

            _loadButton.onClick.AddListener(_onLoadClicked);
            _saveButton.onClick.AddListener(_onSaveClicked);
            _folderButton.onClick.AddListener(_onFolderClicked);
        }
    }
}
