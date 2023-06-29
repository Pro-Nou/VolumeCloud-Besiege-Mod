using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace volumeCloud
{
    public class CloudUIBaseMapper : MonoBehaviour
    {
        public string _title;
        public virtual string SaveData() { return ""; }
        public virtual void LoadData(Vector4 _vec4In) { }
        public virtual void LoadData(Vector3 _vec3In) { }
        public virtual void LoadData(Vector2 _vec2In) { }
        public virtual void LoadData(string _strIn) { }
        public virtual void LoadData(float _floatIn) { }
        public virtual void LoadData(int _intIn) { }
    }
}
