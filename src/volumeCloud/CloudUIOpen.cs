using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace volumeCloud
{
    public class CloudUIOpen : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        public UnityEngine.UI.Button _button;
        private float lastTime = 0f;
        // Use this for initialization
        public void OnPointerDown(PointerEventData eventData)
        {
            lastTime = Time.realtimeSinceStartup;
            //Debug.Log("Button is pressed");
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if(Time.realtimeSinceStartup - lastTime < 0.15f)
            {
                this.transform.parent.GetChild(0).gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            }
            //Debug.Log("Button is Up");
        }
        void Start()
        {
            /*
            _button = this.transform.GetComponent<UnityEngine.UI.Button>();
            _button.onClick.AddListener(() => {
                this.transform.parent.GetChild(0).gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            });
            */
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
