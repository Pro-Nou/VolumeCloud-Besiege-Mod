using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;

using Modding.Common;
using UnityEngine;
using UnityEngine.Networking;
namespace volumeCloud
{
    public class JsonSaveData
    {
        public float _total_scale;
        public float _cloud_height;
        public float _Density;
        public float _StepSize;
        public float _StepScaleDis;
        public float _max_count;
        public float _heightCullThreshold;
        public float _NoiseCullThreshold;
        public float _w;

        public Vector2 _floating_velocity;
        public float _noise3d_mul;

        public float _light_damper;
        public float _LightStepSize;
        public float _light_max_count;
        public Vector3 _dir_light_dir;
        public string _dir_light;
        public string _ambient_light;

        public string _fog_color;
        public float _fog_density;
        public float _fog_start_dis;

        public int _ColorMappingMode;
        public string _baseColor;
        public string _backColor;
        public string _ColorMap;

        public string _RayMaskTex;
        public float _BlueNoiseScale;
        public string _Noise2DA;
        public Vector4 _Noise2DATile;
        public string _Noise2DB;
        public Vector4 _Noise2DBTile;
        public Vector4 _Noise3DATile;
        public Vector4 _Noise3DBTile;
    }
    class VolumeCloudController : SingleInstance<VolumeCloudController>
    {
        public override string Name { get; } = "Volume Cloud Controller";

        public Shader volumeCloudShader;
        public Material volumeCloudMat;
        public GameObject volumeCloudObject;
        public MeshRenderer volumeCloudRenderer;
        public Texture2D tex2DNoiseA;
        public Texture2D tex2DNoiseB;
        public Texture2D blueNoise;
        public Texture2D sampleLightMap;
        public Texture3D tex3DNoiseA;
        public Texture3D tex3DNoiseB;
        public Camera _mainCamera;
        public GameObject canvas;

        public Transform _cloudScrollView;
        public Transform _UIBase;
        public Transform _cloudMenu;
        public Dictionary<string, GameObject> UIPool;

        private float currentYPose;
        private Vector4 cloudOffsetVec4;
        private Vector2 cloudFloatingVelocity;
        private float totalScale;
        private bool allInited;

        private string ResourcePath = "Resource";
        private string texturePath = "Resource";
        private string savePath = "Resource";

        private Dictionary<string, CloudUIBaseMapper> _UIMapperList;

        private CloudUILabel _colorLabel;
        private CloudUIDropDown _colorModeDropDown;
        private CloudUIColorSlider _baseColorSlider;
        private CloudUIColorSlider _backColorSlider;
        private CloudUIImageSelect colorMapSelect;

        private CloudUILabel _lightLabel;
        private CloudUISlider _lightStepSizeSlider;
        private CloudUISlider _lightDamperSlider;
        private CloudUISlider _lightMaxCount;

        private CloudUILabel _floatingLabel;
        private CloudUIVector2 _addCloudUIVector2;
        private CloudUISlider noise3DVelocityMulSlider;
        private CloudUISlider cloudHeightSlider;
        private CloudUIVector3 _dirLightDir;
        private CloudUIColorSlider _DirLightColorSlider;
        private CloudUIColorSlider _AmbientLightColorSlider;

        private CloudUILabel _FogLabel;
        private CloudUIColorSlider _FogColor;
        private CloudUISlider _FogDensity;
        private CloudUISlider _FogStartDistance;

        private CloudUILabel _basicLabel;
        private CloudUILabel _samplingLabel;
        private CloudUISlider _scaleSlider;
        private CloudUISlider _densitySlider;
        private CloudUISlider _stepSizeSlider;
        private CloudUISlider _stepScaleDisSlider;
        private CloudUISlider _maxCountSlider;
        private CloudUISlider _heightCullSlider;
        private CloudUISlider _noiseCullSlider;
        private CloudUISlider _wSlider;

        private CloudUILabel _noiseLabel;
        private CloudUIImageSelect _blueNoiseSelect;
        private CloudUISlider _blueNoiseScaleSlider;
        private CloudUIImageSelect _noise2DASelect;
        private CloudUIVector4 _noise2DATitleVec4;
        private CloudUIImageSelect _noise2DBSelect;
        private CloudUIVector4 _noise2DBTitleVec4;
        private CloudUIVector4 _noise3DATitleVec4;
        private CloudUIVector4 _noise3DBTitleVec4;

        private void Start()
        {
            UIPool = new Dictionary<string, GameObject>();
            allInited = false;
            totalScale = 2000f;
            _UIMapperList = new Dictionary<string, CloudUIBaseMapper>();
            StartCoroutine(InitCloud());
        }
        public void FixFog()
        {
            try
            {
                _mainCamera = Camera.main;
                for (int i = 0; i < _mainCamera.gameObject.transform.childCount; i++)
                {
                    if (_mainCamera.gameObject.transform.GetChild(i).gameObject.name == "FOG SPHERE")
                    {
                        _mainCamera.gameObject.transform.GetChild(i).localScale = 0.9f * _mainCamera.farClipPlane * Vector3.one;
                    }
                }
            }
            catch { }
        }
        public void FixSkyBox()
        {
            if (StatMaster.isMP)
            {
                try
                {
                    GameObject skybox = GameObject.Find("MULTIPLAYER LEVEL").transform.FindChild("Environments").FindChild("Barren").FindChild("AviamisAtmosphere").FindChild("STAR SPHERE").gameObject;
                    skybox.transform.localScale = 0.9f * _mainCamera.farClipPlane * Vector3.one;
                    skybox.GetComponent<MeshRenderer>().sortingOrder = -3000;
                    skybox.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -3000;
                    skybox.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().sortingOrder = -3000;
                    skybox.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sortingOrder = -3000;
                    skybox.transform.GetChild(1).GetChild(0).GetComponent<ParticleSystemRenderer>().sortingOrder = -3000;
                }
                catch { }
            }
            else
            {
                try
                {
                    GameObject skybox = GameObject.Find("LEVEL BARREN EXPANSE").transform.FindChild("AviamisAtmosphere").FindChild("STAR SPHERE").gameObject;
                    skybox.transform.localScale = 0.9f * _mainCamera.farClipPlane * Vector3.one;
                    skybox.GetComponent<MeshRenderer>().sortingOrder = -3000;
                    skybox.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().sortingOrder = -3000;
                    skybox.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().sortingOrder = -3000;
                    skybox.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sortingOrder = -3000;
                    skybox.transform.GetChild(1).GetChild(0).GetComponent<ParticleSystemRenderer>().sortingOrder = -3000;
                }
                catch { }
            }
        }
        public void SetDirLightDir(Vector3 _vec3)
        {
            UnityEngine.Light mainLight = null;
            if (StatMaster.isMP)
            {
                try
                {
                    mainLight = GameObject.Find("Environments").transform.FindChild("Directional light").gameObject.GetComponent<UnityEngine.Light>();
                }
                catch { }
            }
            else
            {
                try
                {
                    mainLight = GameObject.Find("AviamisAtmosphere").transform.FindChild("Directional light").gameObject.GetComponent<UnityEngine.Light>();
                }
                catch { }
                try
                {
                    mainLight = GameObject.Find("ATMOSPHERE").transform.FindChild("Directional light").gameObject.GetComponent<UnityEngine.Light>();
                }
                catch { }

            }
            if (mainLight != null)
            {
                mainLight.transform.rotation = Quaternion.Euler(_vec3);
            }
        }
        public Vector3 GetDirLightDir()
        {
            UnityEngine.Light mainLight = null;
            if (StatMaster.isMP)
            {
                try
                {
                    mainLight = GameObject.Find("Environments").transform.FindChild("Directional light").gameObject.GetComponent<UnityEngine.Light>();
                }
                catch { }
            }
            else
            {
                try
                {
                    mainLight = GameObject.Find("AviamisAtmosphere").transform.FindChild("Directional light").gameObject.GetComponent<UnityEngine.Light>();
                }
                catch { }
                try
                {
                    mainLight = GameObject.Find("ATMOSPHERE").transform.FindChild("Directional light").gameObject.GetComponent<UnityEngine.Light>();
                }
                catch { }

            }
            if (mainLight != null)
            {
                return mainLight.transform.rotation.eulerAngles;
            }
            return Vector3.zero;
        }
        public void SetDirLight(Color _color)
        {
            UnityEngine.Light mainLight = null;
            if (StatMaster.isMP)
            {
                try
                {
                    mainLight = GameObject.Find("Environments").transform.FindChild("Directional light").gameObject.GetComponent<UnityEngine.Light>();
                }
                catch { }
            }
            else
            {
                try
                {
                    mainLight = GameObject.Find("AviamisAtmosphere").transform.FindChild("Directional light").gameObject.GetComponent<UnityEngine.Light>();
                }
                catch { }
                try
                {
                    mainLight = GameObject.Find("ATMOSPHERE").transform.FindChild("Directional light").gameObject.GetComponent<UnityEngine.Light>();
                }
                catch { }

            }
            if (mainLight != null)
            {
                mainLight.color = _color;
            }
        }
        public Color GetDirLight()
        {
            UnityEngine.Light mainLight = null;
            if (StatMaster.isMP)
            {
                try
                {
                    mainLight = GameObject.Find("Environments").transform.FindChild("Directional light").gameObject.GetComponent<UnityEngine.Light>();
                }
                catch { }
            }
            else
            {
                try
                {
                    mainLight = GameObject.Find("AviamisAtmosphere").transform.FindChild("Directional light").gameObject.GetComponent<UnityEngine.Light>();
                }
                catch { }
                try
                {
                    mainLight = GameObject.Find("ATMOSPHERE").transform.FindChild("Directional light").gameObject.GetComponent<UnityEngine.Light>();
                }
                catch { }

            }
            if (mainLight != null)
            {
                return mainLight.color;
            }
            return Color.white;
        }
        public void SetAmbientLight(Color _color)
        {
            RenderSettings.ambientLight = _color;
        }
        public Color GetAmbientLight()
        {
            return RenderSettings.ambientLight;
        }
        public void SetColorFog(Color fogColor, float fogDensity, float fogStartDis)
        {
            foreach (var a in Camera.main.GetComponents<ColorfulFog>())
            {
                a.coloringMode = ColorfulFog.ColoringMode.Solid;
                a.solidColor = fogColor;
                a.fogDensity = fogDensity;
                a.startDistance = fogStartDis;
            }
        }
        public void GetColorFog(out Color fogColor, out float fogDensity, out float fogStartDis)
        {
            fogColor = Color.white;
            fogDensity = 0.004f;
            fogStartDis = 140f;
            foreach (var a in Camera.main.GetComponents<ColorfulFog>())
            {
                if(a.enabled)
                {
                    fogColor = a.solidColor;
                    fogDensity = a.fogDensity;
                    fogStartDis = a.startDistance;
                    break;
                }
            }
        }
        private System.Collections.IEnumerator InitCloud()
        {
            texturePath = $"{ResourcePath}/textures";
            savePath = $"{ResourcePath}/saves";

            currentYPose = -20f;
            cloudOffsetVec4 = new Vector4(-0.24f, 600f / totalScale, 0f, 0f);
            cloudFloatingVelocity = Vector2.zero;
            
            _mainCamera = Camera.main;

            LoadAssets();
            InitCloudMat();
            InitCloudUIBanner();
            InitUIPool();

            InitSampling();
            InitFloating();
            InitLight();
            InitFog();
            InitColor();
            InitNoiseMaps();
            _cloudScrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, -currentYPose);
            
            Modding.Events.OnActiveSceneChanged += (UnityEngine.SceneManagement.Scene e1, UnityEngine.SceneManagement.Scene e2) =>
            {
                volumeCloudObject.SetActive(!StatMaster.isMainMenu);
            };
            allInited = true;
            yield break;
        }
        public void SaveConfig(string _filename)
        {
            string _saveFilename = _filename;
            if(_saveFilename.Length < 5)
            {
                _saveFilename += ".json";
            }
            else
            {
                _saveFilename = (_saveFilename.Substring(_saveFilename.Length - 5, 5) == ".json") ? _saveFilename : $"{_saveFilename}.json";
            }
            List<string> saveData = new List<string>();
            saveData.Add("{");
            foreach(var mapper in _UIMapperList)
            {
                saveData.Add(mapper.Value.SaveData());
            }
            saveData[saveData.Count - 1] = saveData[saveData.Count - 1].Substring(0, saveData[saveData.Count - 1].Length - 1);
            saveData.Add("}");
            Modding.ModIO.WriteAllLines($"{savePath}/{_saveFilename}", saveData.ToArray(), true);
        }
        public void LoadConfig(string _filename)
        {
            string _saveFilename = _filename;
            if (_saveFilename.Length < 5)
            {
                _saveFilename += ".json";
            }
            else
            {
                _saveFilename = (_saveFilename.Substring(_saveFilename.Length - 5, 5) == ".json") ? _saveFilename : $"{_saveFilename}.json";
            }
            string savedata = Modding.ModIO.ReadAllText($"{savePath}/{_saveFilename}",true);
            JsonSaveData _jsonSaveData = JsonUtility.FromJson<JsonSaveData>(savedata);

            _UIMapperList[nameof(_jsonSaveData._total_scale)].LoadData(_jsonSaveData._total_scale);
            _UIMapperList[nameof(_jsonSaveData._Density)].LoadData(_jsonSaveData._Density);
            _UIMapperList[nameof(_jsonSaveData._StepSize)].LoadData(_jsonSaveData._StepSize);
            _UIMapperList[nameof(_jsonSaveData._StepScaleDis)].LoadData(_jsonSaveData._StepScaleDis);
            _UIMapperList[nameof(_jsonSaveData._max_count)].LoadData(_jsonSaveData._max_count);
            _UIMapperList[nameof(_jsonSaveData._heightCullThreshold)].LoadData(_jsonSaveData._heightCullThreshold);
            _UIMapperList[nameof(_jsonSaveData._NoiseCullThreshold)].LoadData(_jsonSaveData._NoiseCullThreshold);
            _UIMapperList[nameof(_jsonSaveData._w)].LoadData(_jsonSaveData._w);

            _UIMapperList[nameof(_jsonSaveData._floating_velocity)].LoadData(_jsonSaveData._floating_velocity);
            _UIMapperList[nameof(_jsonSaveData._noise3d_mul)].LoadData(_jsonSaveData._noise3d_mul);
            _UIMapperList[nameof(_jsonSaveData._cloud_height)].LoadData(_jsonSaveData._cloud_height);
            _UIMapperList[nameof(_jsonSaveData._dir_light_dir)].LoadData(_jsonSaveData._dir_light_dir);
            _UIMapperList[nameof(_jsonSaveData._dir_light)].LoadData(_jsonSaveData._dir_light);
            _UIMapperList[nameof(_jsonSaveData._ambient_light)].LoadData(_jsonSaveData._ambient_light);

            _UIMapperList[nameof(_jsonSaveData._fog_color)].LoadData(_jsonSaveData._fog_color);
            _UIMapperList[nameof(_jsonSaveData._fog_density)].LoadData(_jsonSaveData._fog_density);
            _UIMapperList[nameof(_jsonSaveData._fog_start_dis)].LoadData(_jsonSaveData._fog_start_dis);

            _UIMapperList[nameof(_jsonSaveData._light_damper)].LoadData(_jsonSaveData._light_damper);
            _UIMapperList[nameof(_jsonSaveData._LightStepSize)].LoadData(_jsonSaveData._LightStepSize);
            _UIMapperList[nameof(_jsonSaveData._light_max_count)].LoadData(_jsonSaveData._light_max_count);

            _UIMapperList[nameof(_jsonSaveData._ColorMappingMode)].LoadData(_jsonSaveData._ColorMappingMode);
            _UIMapperList[nameof(_jsonSaveData._baseColor)].LoadData(_jsonSaveData._baseColor);
            _UIMapperList[nameof(_jsonSaveData._backColor)].LoadData(_jsonSaveData._backColor);
            _UIMapperList[nameof(_jsonSaveData._ColorMap)].LoadData(_jsonSaveData._ColorMap);

            _UIMapperList[nameof(_jsonSaveData._RayMaskTex)].LoadData(_jsonSaveData._RayMaskTex);
            _UIMapperList[nameof(_jsonSaveData._BlueNoiseScale)].LoadData(_jsonSaveData._BlueNoiseScale);
            _UIMapperList[nameof(_jsonSaveData._Noise2DA)].LoadData(_jsonSaveData._Noise2DA);
            _UIMapperList[nameof(_jsonSaveData._Noise2DATile)].LoadData(_jsonSaveData._Noise2DATile);
            _UIMapperList[nameof(_jsonSaveData._Noise2DB)].LoadData(_jsonSaveData._Noise2DB);
            _UIMapperList[nameof(_jsonSaveData._Noise2DBTile)].LoadData(_jsonSaveData._Noise2DBTile);
            _UIMapperList[nameof(_jsonSaveData._Noise3DATile)].LoadData(_jsonSaveData._Noise3DATile);
            _UIMapperList[nameof(_jsonSaveData._Noise3DBTile)].LoadData(_jsonSaveData._Noise3DBTile);
        }
        public void LoadAssets()
        {
            volumeCloudShader = ModResource.GetAssetBundle("VolumeCloudAsset").LoadAsset<Shader>("assets/raymarchingtest/raymarchingcloudtest.shader");
            tex2DNoiseA = ModResource.GetAssetBundle("VolumeCloudAsset").LoadAsset<Texture2D>("assets/raymarchingtest/2dnoisea.png");
            tex2DNoiseB = ModResource.GetAssetBundle("VolumeCloudAsset").LoadAsset<Texture2D>("assets/raymarchingtest/2dnoiseb.png");
            blueNoise = ModResource.GetAssetBundle("VolumeCloudAsset").LoadAsset<Texture2D>("assets/raymarchingtest/bluenoise.png");
            sampleLightMap = ModResource.GetAssetBundle("VolumeCloudAsset").LoadAsset<Texture2D>("assets/raymarchingtest/samplelightmap.png");
            tex3DNoiseA = ModResource.GetAssetBundle("VolumeCloudAsset").LoadAsset<Texture3D>("assets/raymarchingtest/3DNoiseA_A8.asset");
            tex3DNoiseB = ModResource.GetAssetBundle("VolumeCloudAsset").LoadAsset<Texture3D>("assets/raymarchingtest/3DNoiseB_A8.asset");


            if (!Modding.ModIO.ExistsDirectory(texturePath, true))
            {
                Modding.ModIO.CreateDirectory(texturePath, true);
            }
            if (!Modding.ModIO.ExistsDirectory(savePath, true))
            {
                Modding.ModIO.CreateDirectory(savePath, true);
            }
        }
        public void InitCloudMat()
        {
            volumeCloudMat = new Material(volumeCloudShader);
            volumeCloudMat.SetInt("_ColorMappingMode", 0);
            volumeCloudMat.SetColor("_baseColor", new Color(0.56f, 0.56f, 0.56f, 1f));
            volumeCloudMat.SetColor("_backColor", new Color(0f, 0f, 0f, 1f));
            volumeCloudMat.SetTexture("_ColorMap", sampleLightMap);

            volumeCloudMat.SetFloat("_light_damper", 0.81f);
            volumeCloudMat.SetFloat("_LightStepSize", 0.02f);
            volumeCloudMat.SetFloat("_light_max_count", 4f);

            volumeCloudMat.SetTexture("_RayMaskTex", blueNoise);
            volumeCloudMat.SetFloat("_BlueNoiseScale", 0.02f);

            volumeCloudMat.SetFloat("_Density", 0.5f);
            volumeCloudMat.SetFloat("_StepSize", 0.01f);
            volumeCloudMat.SetFloat("_StepScaleDis", 0.001f);
            volumeCloudMat.SetFloat("_max_count", 50f);
            volumeCloudMat.SetFloat("_heightCullThreshold", 0.05f);

            volumeCloudMat.SetFloat("_NoiseCullThreshold", 0.21f);
            volumeCloudMat.SetFloat("_w", 0.01f);


            volumeCloudMat.SetVector("_positionOffset", cloudOffsetVec4);
            volumeCloudMat.SetTexture("_Noise3DA", tex3DNoiseA);
            volumeCloudMat.SetVector("_Noise3DATile", new Vector4(5f, 5f, 5f, 0.04f));
            volumeCloudMat.SetTexture("_Noise3DB", tex3DNoiseB);
            volumeCloudMat.SetVector("_Noise3DBTile", new Vector4(8f, 8f, 8f, 0.02f));
            volumeCloudMat.SetTexture("_Noise2DA", tex2DNoiseA);
            volumeCloudMat.SetVector("_Noise2DATile", new Vector4(1f, 0.1f, 1f, 0.12f));
            volumeCloudMat.SetTexture("_Noise2DB", tex2DNoiseB);
            volumeCloudMat.SetVector("_Noise2DBTile", new Vector4(1.3f, 0.05f, 0.7f, 0f));

            volumeCloudObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            volumeCloudObject.name = "Volume Cloud Cube";
            Destroy(volumeCloudObject.GetComponent<BoxCollider>());
            volumeCloudObject.transform.position = Vector3.zero;
            volumeCloudObject.transform.localScale = totalScale * Vector3.one;
            volumeCloudRenderer = volumeCloudObject.GetComponent<MeshRenderer>();
            volumeCloudRenderer.material = volumeCloudMat;

            Bounds _bounds = new Bounds();
            _bounds.center = Vector3.zero;
            _bounds.size = Vector3.one * float.PositiveInfinity;
            volumeCloudObject.GetComponent<MeshFilter>().mesh.bounds = _bounds;

            UnityEngine.Object.DontDestroyOnLoad(volumeCloudObject);
        }
        public void InitCloudUIBanner()
        {
            canvas = Instantiate(ModResource.GetAssetBundle("VolumeCloudAsset").LoadAsset<GameObject>("CloudCanvas"));

            _UIBase = canvas.transform.GetChild(0);
            _cloudMenu = _UIBase.GetChild(0);
            _UIBase.GetChild(1).gameObject.AddComponent<CloudUIOpen>();
            _UIBase.GetChild(1).gameObject.AddComponent<CloudUIDrag>()._rectTransform = _UIBase.GetComponent<RectTransform>();
            _UIBase.GetChild(1).gameObject.SetActive(false);

            _cloudMenu.GetChild(1).gameObject.AddComponent<CloudUIDrag>()._rectTransform = _UIBase.GetComponent<RectTransform>();
            _cloudMenu.GetChild(2).gameObject.AddComponent<CloudUIFixFog>();
            CloudUIConfigSave _cloudUIConfigSave = _cloudMenu.GetChild(3).gameObject.AddComponent<CloudUIConfigSave>();
            _cloudUIConfigSave._onSaveClicked += () => 
            {
                SaveConfig(_cloudUIConfigSave._filenameInput.text);
            };
            _cloudUIConfigSave._onLoadClicked += () =>
            {
                LoadConfig(_cloudUIConfigSave._filenameInput.text);
            };
            _cloudUIConfigSave._onFolderClicked += () =>
            {
                Modding.ModIO.OpenFolderInFileBrowser(savePath, true);
            };
            _cloudMenu.GetChild(4).gameObject.AddComponent<CloudUIClose>();
            _cloudMenu.GetChild(5).gameObject.AddComponent<CloudUIHelperButton>();
            _cloudScrollView = _cloudMenu.GetChild(6).GetChild(0).GetChild(0);

            UnityEngine.Object.DontDestroyOnLoad(canvas);
        }
        public void InitUIPool()
        {
            UIPool.Add("CloudUIDropDown", _cloudScrollView.GetChild(0).gameObject);
            UIPool.Add("CloudUISlider", _cloudScrollView.GetChild(1).gameObject);
            UIPool.Add("CloudUIColorSlider", _cloudScrollView.GetChild(2).gameObject);
            UIPool.Add("CloudUIVector4", _cloudScrollView.GetChild(3).gameObject);
            UIPool.Add("CloudUIVector3", _cloudScrollView.GetChild(4).gameObject);
            UIPool.Add("CloudUIVector2", _cloudScrollView.GetChild(5).gameObject);
            UIPool.Add("CloudUIImageSelect", _cloudScrollView.GetChild(6).gameObject);
            UIPool.Add("CloudUILabel", _cloudScrollView.GetChild(7).gameObject);

            _cloudScrollView.GetChild(0).gameObject.AddComponent<CloudUIDropDown>();
            _cloudScrollView.GetChild(1).gameObject.AddComponent<CloudUISlider>();
            _cloudScrollView.GetChild(2).gameObject.AddComponent<CloudUIColorSlider>();
            _cloudScrollView.GetChild(3).gameObject.AddComponent<CloudUIVector4>();
            _cloudScrollView.GetChild(4).gameObject.AddComponent<CloudUIVector3>();
            _cloudScrollView.GetChild(5).gameObject.AddComponent<CloudUIVector2>();
            _cloudScrollView.GetChild(6).gameObject.AddComponent<CloudUIImageSelect>();//height = 240
            _cloudScrollView.GetChild(7).gameObject.AddComponent<CloudUILabel>();
        }
        public void InitColor()
        {
            _colorLabel = AddCloudUILabel("_ColorLabel");
            
            _colorModeDropDown = AddCloudUIDropDown("_ColorMappingMode", new List<string> { "Colors", "Texture" });
            _colorModeDropDown._onValueChange += (int value) =>
            {
                volumeCloudMat.SetInt("_ColorMappingMode", value);
            };
            _baseColorSlider = AddCloudUIColorSlider("_baseColor", new Color(0.56f, 0.56f, 0.56f, 1f));
            _baseColorSlider._onValueChange += (float value) =>
            {
                volumeCloudMat.SetColor("_baseColor", _baseColorSlider._color);
            };
            _baseColorSlider._onValueChangeString += (string value) =>
            {
                volumeCloudMat.SetColor("_baseColor", _baseColorSlider._color);
            };
            _backColorSlider = AddCloudUIColorSlider("_backColor", new Color(0f, 0f, 0f, 1f));
            _backColorSlider._onValueChange += (float value) =>
            {
                volumeCloudMat.SetColor("_backColor", _backColorSlider._color);
            };
            _backColorSlider._onValueChangeString += (string value) =>
            {
                volumeCloudMat.SetColor("_backColor", _backColorSlider._color);
            };
            colorMapSelect = AddCloudUIImageSelect("_ColorMap", texturePath, "SampleLightMap.png", sampleLightMap);
            colorMapSelect._onValueChange += (string value) =>
            {
                colorMapSelect._tex2D.wrapMode = TextureWrapMode.Clamp;
                volumeCloudMat.SetTexture("_ColorMap", colorMapSelect._tex2D);
            };
        }
        public void InitLight()
        {
            _lightLabel = AddCloudUILabel("_lightLabel");

            _lightDamperSlider = AddCloudUISlider("_light_damper", 0.81f, 0f, 5f);
            _lightDamperSlider._onValueChange += (float value) =>
            {
                volumeCloudMat.SetFloat("_light_damper", value);
            };
            _lightStepSizeSlider = AddCloudUISlider("_LightStepSize", 0.02f, 0.001f, 1f);
            _lightStepSizeSlider._onValueChange += (float value) =>
            {
                volumeCloudMat.SetFloat("_LightStepSize", value);
            };
            _lightMaxCount = AddCloudUISlider("_light_max_count", 4f, 1f, 64f);
            _lightMaxCount._onValueChange += (float value) =>
            {
                volumeCloudMat.SetFloat("_light_max_count", value);
            };
            _dirLightDir = AddCloudUIVector3("_dir_light_dir", GetDirLightDir());
            _dirLightDir._onValueChange += (string value) =>
            {
                SetDirLightDir(_dirLightDir._vec3);
            };
            _DirLightColorSlider = AddCloudUIColorSlider("_dir_light", GetDirLight());
            _DirLightColorSlider._onValueChange += (float value) =>
            {
                SetDirLight(_DirLightColorSlider._color);
            };
            _DirLightColorSlider._onValueChangeString += (string value) =>
            {
                SetDirLight(_DirLightColorSlider._color);
            };
            _AmbientLightColorSlider = AddCloudUIColorSlider("_ambient_light", GetAmbientLight());
            _AmbientLightColorSlider._onValueChange += (float value) =>
            {
                SetAmbientLight(_AmbientLightColorSlider._color);
            };
            _AmbientLightColorSlider._onValueChangeString += (string value) =>
            {
                SetAmbientLight(_AmbientLightColorSlider._color);
            };
        }
        public void InitFloating()
        {
            _floatingLabel = AddCloudUILabel("_floatingLabel");

            _addCloudUIVector2 = AddCloudUIVector2("_floating_velocity", new Vector2(0f, 0f));
            _addCloudUIVector2._onValueChange += (string value) =>
            {
                cloudFloatingVelocity = _addCloudUIVector2._vec2;
            };
            noise3DVelocityMulSlider = AddCloudUISlider("_noise3d_mul", 0f, 0f, 2f);
            noise3DVelocityMulSlider._onValueChange += (float value) =>
            {
                cloudOffsetVec4.w = value;
            };
        }
        public void InitSampling()
        {
            _basicLabel = AddCloudUILabel("_BasicLabel");
            _scaleSlider = AddCloudUISlider("_total_scale", 1f, 0.1f, 10f);
            _scaleSlider._onValueChange += (float value) =>
            {
                totalScale = 2000f * value;
                volumeCloudObject.transform.localScale = Vector3.one * totalScale;
            };
            cloudHeightSlider = AddCloudUISlider("_cloud_height", 600f, 0f, 2000f);
            cloudHeightSlider._onValueChange += (float value) =>
            {
                cloudOffsetVec4.y = value / 2000f;
            };
            _densitySlider = AddCloudUISlider("_Density", 0.5f, 0f, 1f);
            _densitySlider._onValueChange += (float value) =>
            {
                volumeCloudMat.SetFloat("_Density", value);
            };
            _noiseCullSlider = AddCloudUISlider("_NoiseCullThreshold", 0.21f, 0.001f, 1f);
            _noiseCullSlider._onValueChange += (float value) =>
            {
                volumeCloudMat.SetFloat("_NoiseCullThreshold", value);
            };
            _wSlider = AddCloudUISlider("_w", 0.01f, 0.001f, 1f);
            _wSlider._onValueChange += (float value) =>
            {
                volumeCloudMat.SetFloat("_w", value);
            };

            _samplingLabel = AddCloudUILabel("_samplingLabel");
            _stepSizeSlider = AddCloudUISlider("_StepSize", 0.01f, 0.001f, 0.1f);
            _stepSizeSlider._onValueChange += (float value) =>
            {
                volumeCloudMat.SetFloat("_StepSize", value);
            };
            _stepScaleDisSlider = AddCloudUISlider("_StepScaleDis", 0.001f, 0f, 0.1f);
            _stepScaleDisSlider._onValueChange += (float value) =>
            {
                volumeCloudMat.SetFloat("_StepScaleDis", value);
            };
            _maxCountSlider = AddCloudUISlider("_max_count", 50f, 1f, 256f);
            _maxCountSlider._onValueChange += (float value) =>
            {
                volumeCloudMat.SetFloat("_max_count", value);
            };
            _heightCullSlider = AddCloudUISlider("_heightCullThreshold", 0.05f, 0f, 0.5f);
            _heightCullSlider._onValueChange += (float value) =>
            {
                volumeCloudMat.SetFloat("_heightCullThreshold", value);
            };
        }
        public void InitNoiseMaps()
        {
            _noiseLabel = AddCloudUILabel("_noiseLabel");

            _blueNoiseSelect = AddCloudUIImageSelect("_RayMaskTex", texturePath, "BlueNoise.png", blueNoise);
            _blueNoiseSelect._onValueChange += (string value) =>
            {
                volumeCloudMat.SetTexture("_RayMaskTex", _blueNoiseSelect._tex2D);
            };
            _blueNoiseScaleSlider = AddCloudUISlider("_BlueNoiseScale", 0.02f, 0f, 1f);
            _blueNoiseScaleSlider._onValueChange += (float value) =>
            {
                volumeCloudMat.SetFloat("_BlueNoiseScale", value);
            };
            _noise2DASelect = AddCloudUIImageSelect("_Noise2DA", texturePath, "2DNoiseA.png", tex2DNoiseA);
            _noise2DASelect._onValueChange += (string value) =>
            {
                volumeCloudMat.SetTexture("_Noise2DA", _noise2DASelect._tex2D);
            };
            _noise2DATitleVec4 = AddCloudUIVector4("_Noise2DATile", new Vector4(1f, 0.1f, 1f, 0.12f));
            _noise2DATitleVec4._onValueChange += (string value) =>
            {
                volumeCloudMat.SetVector("_Noise2DATile", _noise2DATitleVec4._vec4);
            };
            _noise2DBSelect = AddCloudUIImageSelect("_Noise2DB", texturePath, "2DNoiseB.png", tex2DNoiseB);
            _noise2DBSelect._onValueChange += (string value) =>
            {
                volumeCloudMat.SetTexture("_Noise2DB", _noise2DBSelect._tex2D);
            };
            _noise2DBTitleVec4 = AddCloudUIVector4("_Noise2DBTile", new Vector4(1.3f, 0.05f, 0.7f, 0f));
            _noise2DBTitleVec4._onValueChange += (string value) =>
            {
                volumeCloudMat.SetVector("_Noise2DBTile", _noise2DBTitleVec4._vec4);
            };
            _noise3DATitleVec4 = AddCloudUIVector4("_Noise3DATile", new Vector4(5f, 5f, 5f, 0.04f));
            _noise3DATitleVec4._onValueChange += (string value) =>
            {
                volumeCloudMat.SetVector("_Noise3DATile", _noise3DATitleVec4._vec4);
            };
            _noise3DBTitleVec4 = AddCloudUIVector4("_Noise3DBTile", new Vector4(8f, 8f, 8f, 0.02f));
            _noise3DBTitleVec4._onValueChange += (string value) =>
            {
                volumeCloudMat.SetVector("_Noise3DBTile", _noise3DBTitleVec4._vec4);
            };
        }
        public void InitFog()
        {
            
            Color _fogColor = Color.white;
            float _fogDensity = 0.004f;
            float _fogStartDis = 140f;
            GetColorFog(out _fogColor, out _fogDensity, out _fogStartDis);
            
            _FogLabel = AddCloudUILabel("_fog_label");
            _FogColor = AddCloudUIColorSlider("_fog_color", _fogColor);
            _FogColor._onValueChange += (float value) =>
            {
                SetColorFog(_FogColor._color, _FogDensity._slider.value, _FogStartDistance._slider.value);
            };
            _FogColor._onValueChangeString += (string value) =>
            {
                SetColorFog(_FogColor._color, _FogDensity._slider.value, _FogStartDistance._slider.value);
            };
            _FogDensity = AddCloudUISlider("_fog_density", _fogDensity, 0f, 1f);
            _FogDensity._onValueChange += (float value) =>
            {
                SetColorFog(_FogColor._color, _FogDensity._slider.value, _FogStartDistance._slider.value);
            };
            _FogStartDistance = AddCloudUISlider("_fog_start_dis", _fogStartDis, 0f, 4500f);
            _FogStartDistance._onValueChange += (float value) =>
            {
                SetColorFog(_FogColor._color, _FogDensity._slider.value, _FogStartDistance._slider.value);
            };
        }
        public CloudUIDropDown AddCloudUIDropDown(string title, List<string> values)
        {
            GameObject _dropDownAdd = Instantiate(UIPool["CloudUIDropDown"]);
            _dropDownAdd.transform.SetParent(_cloudScrollView);
            _dropDownAdd.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, currentYPose);
            currentYPose -= 30f;
            _dropDownAdd.SetActive(true);
            CloudUIDropDown _cloudUIDropDown = _dropDownAdd.GetComponent<CloudUIDropDown>();
            _cloudUIDropDown.Init(title, values);
            _UIMapperList.Add(title, _cloudUIDropDown);
            return _cloudUIDropDown;
        }
        public CloudUISlider AddCloudUISlider(string title, float defaultValue, float min, float max)
        {
            GameObject _sliderAdd = Instantiate(UIPool["CloudUISlider"]);
            _sliderAdd.transform.SetParent(_cloudScrollView);
            _sliderAdd.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, currentYPose);
            currentYPose -= 30f;
            _sliderAdd.SetActive(true);
            CloudUISlider _cloudUISlider = _sliderAdd.GetComponent<CloudUISlider>();
            _cloudUISlider.Init(title, defaultValue, min, max);
            _UIMapperList.Add(title, _cloudUISlider);
            return _cloudUISlider;
        }
        public CloudUIColorSlider AddCloudUIColorSlider(string title, Color defaultValue)
        {
            GameObject _colorSliderAdd = Instantiate(UIPool["CloudUIColorSlider"]);
            _colorSliderAdd.transform.SetParent(_cloudScrollView);
            _colorSliderAdd.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, currentYPose);
            currentYPose -= 30f;
            _colorSliderAdd.SetActive(true);
            CloudUIColorSlider _cloudUIColorSlider = _colorSliderAdd.GetComponent<CloudUIColorSlider>();
            _cloudUIColorSlider.Init(title, defaultValue);
            _UIMapperList.Add(title, _cloudUIColorSlider);
            return _cloudUIColorSlider;
        }
        public CloudUIVector4 AddCloudUIVector4(string title, Vector4 defaultValue)
        {
            GameObject _vec4Add = Instantiate(UIPool["CloudUIVector4"]);
            _vec4Add.transform.SetParent(_cloudScrollView);
            _vec4Add.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, currentYPose);
            currentYPose -= 30f;
            _vec4Add.SetActive(true);
            CloudUIVector4 _cloudUIVector4 = _vec4Add.GetComponent<CloudUIVector4>();
            _cloudUIVector4.Init(title, defaultValue);
            _UIMapperList.Add(title, _cloudUIVector4);
            return _cloudUIVector4;
        }
        public CloudUIVector3 AddCloudUIVector3(string title, Vector3 defaultValue)
        {
            GameObject _vec3Add = Instantiate(UIPool["CloudUIVector3"]);
            _vec3Add.transform.SetParent(_cloudScrollView);
            _vec3Add.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, currentYPose);
            currentYPose -= 30f;
            _vec3Add.SetActive(true);
            CloudUIVector3 _cloudUIVector3 = _vec3Add.GetComponent<CloudUIVector3>();
            _cloudUIVector3.Init(title, defaultValue);
            _UIMapperList.Add(title, _cloudUIVector3);
            return _cloudUIVector3;
        }
        public CloudUIVector2 AddCloudUIVector2(string title, Vector2 defaultValue)
        {
            GameObject _vec2Add = Instantiate(UIPool["CloudUIVector2"]);
            _vec2Add.transform.SetParent(_cloudScrollView);
            _vec2Add.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, currentYPose);
            currentYPose -= 30f;
            _vec2Add.SetActive(true);
            CloudUIVector2 _cloudUIVector2 = _vec2Add.GetComponent<CloudUIVector2>();
            _cloudUIVector2.Init(title, defaultValue);
            _UIMapperList.Add(title, _cloudUIVector2);
            return _cloudUIVector2;
        }
        public CloudUIImageSelect AddCloudUIImageSelect(string title, string resourcePath, string defaultValue, Texture2D defaultTex2D)
        {
            GameObject _imgSelectAdd = Instantiate(UIPool["CloudUIImageSelect"]);
            _imgSelectAdd.transform.SetParent(_cloudScrollView);
            _imgSelectAdd.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, currentYPose);
            currentYPose -= 240f;
            _imgSelectAdd.SetActive(true);
            CloudUIImageSelect _cloudUIImageSelect = _imgSelectAdd.GetComponent<CloudUIImageSelect>();
            _cloudUIImageSelect.Init(title, resourcePath, defaultValue, defaultTex2D);
            _UIMapperList.Add(title, _cloudUIImageSelect);
            return _cloudUIImageSelect;
        }
        public CloudUILabel AddCloudUILabel(string title)
        {
            GameObject _lableAdd = Instantiate(UIPool["CloudUILabel"]);
            _lableAdd.transform.SetParent(_cloudScrollView);
            _lableAdd.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, currentYPose);
            currentYPose -= 30f;
            _lableAdd.SetActive(true);
            CloudUILabel _cloudUILabel = _lableAdd.GetComponent<CloudUILabel>();
            _cloudUILabel.Init(title);
            return _cloudUILabel;
        }
        public void Update()
        {
            if (allInited)
            {
                if (!StatMaster.isMainMenu)
                {
                    cloudOffsetVec4.x += cloudFloatingVelocity.x * Time.timeScale * Time.deltaTime;
                    cloudOffsetVec4.z += cloudFloatingVelocity.y * Time.timeScale * Time.deltaTime;
                    volumeCloudMat.SetVector("_positionOffset", cloudOffsetVec4);
                }
            }
            if((StatMaster.hudHidden || StatMaster.isMainMenu) == _UIBase.gameObject.activeSelf)
            {
                _UIBase.gameObject.SetActive(!_UIBase.gameObject.activeSelf);
            }
            //volumeCloudObject.transform.position = new Vector3(_mainCamera.transform.position.x, volumeCloudObject.transform.position.y, _mainCamera.transform.position.z);
            //volumeCloudMat.SetVector("_XYOffsetAndScale", new Vector4(volumeCloudObject.transform.position.x / 2000f, volumeCloudObject.transform.position.z / 2000f, 1f, 1f));
        }
    }
}
