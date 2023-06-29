using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Modding;
using InternalModding;
using Modding.Common;
using UnityEngine;

namespace volumeCloud
{
    public class StringEventArgs : EventArgs
    {
        public StringEventArgs()
        {
            this._Value = "";
        }
        public StringEventArgs(string _value)
        {
            this._Value = _value;
        }
        public string _Value;
    }
    class LanguageManager : SingleInstance<LanguageManager>
    {
        public override string Name { get; } = "Language Manager";
        public string thisLang = "";
        public Dictionary<string, string> outLang;
        public Dictionary<string, Dictionary<string, string>> myLangs = new Dictionary<string, Dictionary<string, string>>();
        public event EventHandler<StringEventArgs> OnLanguageChange;
        public void Init()
        {
            thisLang = Localisation.LocalisationManager.Instance.currLangName;
            Debug.Log($"[volume cloud language manager]: {thisLang}");
            myLangs.Add("English", new Dictionary<string, string>());
            myLangs.Add("简体中文", new Dictionary<string, string>());
            InitEnglish();
            InitChinese();
            try
            {
                outLang = myLangs[thisLang];
            }
            catch
            {
                outLang = myLangs["English"];
            }
            OnLanguageChange += EventRecieve;
            //OnLanguageChanged += ChangLanguage;
        }
        public void EventRecieve(object sender, StringEventArgs e)
        {
            Debug.Log($"language changed:{e._Value}");
        }
        public void InitEnglish()
        {
            myLangs["English"].Add("_drag_title", "Volum Cloud UI");
            myLangs["English"].Add("_fog_skybox_fix", "Fog & Skybox Fix");
            myLangs["English"].Add("_load_config", "Load Config");
            myLangs["English"].Add("_save_config", "Save Config");

            myLangs["English"].Add("_BasicLabel", "Basic Settings");
            myLangs["English"].Add("_ColorLabel", "Color Settings");
            myLangs["English"].Add("_lightLabel", "Light Settings");
            myLangs["English"].Add("_fog_label", "Fog Settings");
            myLangs["English"].Add("_floatingLabel", "Floating Settings");
            myLangs["English"].Add("_samplingLabel", "Sampling Settings");
            myLangs["English"].Add("_noiseLabel", "Noise Detail Settings");

            myLangs["English"].Add("_ColorMappingMode","Color Mapping Mod");
            myLangs["English"].Add("_baseColor", "Base Color");
            myLangs["English"].Add("_backColor", "Shadow Color");
            myLangs["English"].Add("_ColorMap", "Color Map Texture");

            myLangs["English"].Add("_light_damper", "Shadow Strength");
            myLangs["English"].Add("_LightStepSize", "Light Step Size");
            myLangs["English"].Add("_light_max_count", "Light Step Count");
            myLangs["English"].Add("_dir_light_dir", "Main Light Direction");
            myLangs["English"].Add("_dir_light", "Main Light");
            myLangs["English"].Add("_ambient_light", "Ambient Light");

            myLangs["English"].Add("_fog_color", "Fog Color");
            myLangs["English"].Add("_fog_density", "Fog Density");
            myLangs["English"].Add("_fog_start_dis", "Fog Start Distance");

            myLangs["English"].Add("_floating_velocity", "Floating Velocity");
            myLangs["English"].Add("_noise3d_mul", "Detail Floating Offset");

            myLangs["English"].Add("_total_scale", "Total Scale");
            myLangs["English"].Add("_cloud_height", "Cloud Height");
            myLangs["English"].Add("_Density", "Cloud Density");
            myLangs["English"].Add("_StepSize", "Cloud Step Size");
            myLangs["English"].Add("_StepScaleDis", "Step Scale By Depth");
            myLangs["English"].Add("_max_count", "Cloud Step Count");
            myLangs["English"].Add("_heightCullThreshold", "Height Clamp");
            myLangs["English"].Add("_NoiseCullThreshold", "Cloud Sparsity");
            myLangs["English"].Add("_w", "Border Smoothness"); 

            myLangs["English"].Add("_RayMaskTex", "Anti Banding Map");
            myLangs["English"].Add("_BlueNoiseScale", "Anti Banding Scale");
            myLangs["English"].Add("_Noise2DA", "Base Height Map");
            myLangs["English"].Add("_Noise2DATile", "Base Height Scale");
            myLangs["English"].Add("_Noise2DB", "Sub Height Map");
            myLangs["English"].Add("_Noise2DBTile", "Sub Height Scale");
            myLangs["English"].Add("_Noise3DATile", "Primary Detail Scale");
            myLangs["English"].Add("_Noise3DBTile", "Sub Detail Scale");
        }
        public void InitChinese()
        {
            myLangs["简体中文"].Add("_drag_title", "体积云UI");
            myLangs["简体中文"].Add("_fog_skybox_fix", "雾效 & 天空盒修正");
            myLangs["简体中文"].Add("_load_config", "加载配置");
            myLangs["简体中文"].Add("_save_config", "保存配置");

            myLangs["简体中文"].Add("_BasicLabel", "基本设置");
            myLangs["简体中文"].Add("_ColorLabel", "色彩设置");
            myLangs["简体中文"].Add("_lightLabel", "光照设置");
            myLangs["简体中文"].Add("_fog_label", "雾效设置");
            myLangs["简体中文"].Add("_floatingLabel", "流动设置");
            myLangs["简体中文"].Add("_samplingLabel", "采样设置");
            myLangs["简体中文"].Add("_noiseLabel", "噪声细节设置");

            myLangs["简体中文"].Add("_ColorMappingMode", "颜色采样模式");
            myLangs["简体中文"].Add("_baseColor", "基本颜色");
            myLangs["简体中文"].Add("_backColor", "阴影颜色");
            myLangs["简体中文"].Add("_ColorMap", "颜色采样图");

            myLangs["简体中文"].Add("_light_damper", "阴影强度");
            myLangs["简体中文"].Add("_LightStepSize", "光照采样步进距离");
            myLangs["简体中文"].Add("_light_max_count", "光照采样次数");
            myLangs["简体中文"].Add("_dir_light_dir", "主光源方向");
            myLangs["简体中文"].Add("_dir_light", "主光源颜色");
            myLangs["简体中文"].Add("_ambient_light", "环境光颜色");

            myLangs["简体中文"].Add("_fog_color", "雾效颜色");
            myLangs["简体中文"].Add("_fog_density", "雾效密度");
            myLangs["简体中文"].Add("_fog_start_dis", "雾效起始距离");

            myLangs["简体中文"].Add("_floating_velocity", "流动速度");
            myLangs["简体中文"].Add("_noise3d_mul", "噪声流动偏移");

            myLangs["简体中文"].Add("_total_scale", "整体缩放");
            myLangs["简体中文"].Add("_cloud_height", "云层高度");
            myLangs["简体中文"].Add("_Density", "云层密度");
            myLangs["简体中文"].Add("_StepSize", "采样步进距离");
            myLangs["简体中文"].Add("_StepScaleDis", "步进深度缩放");
            myLangs["简体中文"].Add("_max_count", "采样次数");
            myLangs["简体中文"].Add("_heightCullThreshold", "高度钳制");
            myLangs["简体中文"].Add("_NoiseCullThreshold", "云层稀疏度");
            myLangs["简体中文"].Add("_w", "边缘平滑度");

            myLangs["简体中文"].Add("_RayMaskTex", "反分层采样图");
            myLangs["简体中文"].Add("_BlueNoiseScale", "反分层强度");
            myLangs["简体中文"].Add("_Noise2DA", "基本高度图");
            myLangs["简体中文"].Add("_Noise2DATile", "基本高度图缩放");
            myLangs["简体中文"].Add("_Noise2DB", "次级高度图");
            myLangs["简体中文"].Add("_Noise2DBTile", "次级高度图缩放");
            myLangs["简体中文"].Add("_Noise3DATile", "主要细节缩放");
            myLangs["简体中文"].Add("_Noise3DBTile", "次级细节缩放");
        }
        public void Start()
        {
        }
        public void Update()
        {
            if (thisLang != Localisation.LocalisationManager.Instance.currLangName)
            {
                thisLang = Localisation.LocalisationManager.Instance.currLangName;
                try
                {
                    outLang = myLangs[thisLang];
                }
                catch
                {
                    outLang = myLangs["English"];
                }
                OnLanguageChange.Invoke(this, new StringEventArgs(thisLang));
            }
        }
    }
}
