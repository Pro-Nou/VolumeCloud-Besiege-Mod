using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace volumeCloud
{
    public class CloudUIDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        //public GameObject _UIBase;
        private bool dragging;
        private Vector3 lastMousePose;
        public RectTransform _rectTransform;
        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log("Button is pressed");
            dragging = true;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log("Button is Up");
            dragging = false;
        }
        // Use this for initialization
        void Start()
        {
            dragging = false;
            lastMousePose = Input.mousePosition;

            UnityEngine.UI.Text _text = this.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
            this.gameObject.AddComponent<CloudUILangFix>().Init(_text, "_drag_title");
            //_rectTransform = this.transform.parent.parent.GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            if (dragging)
            {
                _rectTransform.anchoredPosition3D += Input.mousePosition - lastMousePose;
            }
            lastMousePose = Input.mousePosition;
        }
    }
}
