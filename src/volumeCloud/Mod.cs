using System;
using Modding;
using UnityEngine;
using UnityEngine.UI;

namespace volumeCloud
{
	public class Mod : ModEntryPoint
	{
        public static GameObject mod;
        public override void OnLoad()
		{
            mod = new GameObject("Volume Cloud!");
            UnityEngine.Object.DontDestroyOnLoad(mod);
            mod.AddComponent<LanguageManager>().Init();
            mod.AddComponent<VolumeCloudController>();
            // Called when the mod is loaded.
        }
	}
}
