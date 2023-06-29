Shader "Unlit/VolumeCloudShader" {
  Properties{
  	[Enum(color, 0, texture, 1)] _ColorMappingMode ("ColorMappingMode", int) = 0
    _baseColor ("front color", Color) = (1.0,1.0,1.0,1.0)
    _backColor ("back color", Color) = (1.0,1.0,1.0,1.0)
    [NoScaleOffset]_ColorMap ("color map", 2D) = "white" {}

    _light_damper ("Light Damper", range(0,5)) = 1
    _LightStepSize ("Light Step Size", range(0.001,1)) = 0.01
    _light_max_count ("Light Step Count", range(1,64)) = 16

    [NoScaleOffset]_RayMaskTex ("ray mask", 2D) = "white" {}
    _BlueNoiseScale ("BlueNoiseScale", Range(0, 1)) = 1

    _Density ("Density", range(0.0,1.0)) = 0.4
    _StepSize ("Step Size", range(0.001,0.1)) = 0.01
    _StepScaleDis ("Step Depth Offset", range(0,0.1)) = 0.001
    _max_count ("Step Count", range(1,256)) = 12
    _heightCullThreshold ("HeightCullThreshold ", range(0,0.5)) = 0.05

    _NoiseCullThreshold ("NoiseCullThreshold", Range(0.001, 1)) = 0.1
    _w ("_w",range(0,1)) = 0.001

    _positionOffset ("Position offset", Vector) = (0, 0, 0, 0)
    [NoScaleOffset]_Noise3DA ("Noise3D A", 3D) = "white" {}
	_Noise3DATile ("Noise3D Tile A", Vector) = (1, 1, 1, 1)
    [NoScaleOffset]_Noise3DB ("Noise3D B", 3D) = "white" {}
	_Noise3DBTile ("Noise3D Tile B", Vector) = (1, 1, 1, 1)
   	[NoScaleOffset]_Noise2DA ("Noise mask A", 2D) = "white" {}
    _Noise2DATile ("Noise Tile A", Vector) = (0, 0, 0, 0)
    [NoScaleOffset]_Noise2DB ("Noise mask B", 2D) = "white" {}
    _Noise2DBTile ("Noise Tile B", Vector) = (0, 0, 0, 0)
    //[NoScaleOffset]_CloudLightMap ("CloudLightMap", 2D) = "white" {}
  }
  SubShader{
    Tags { "Queue" = "Transparent" "RenderType" = "volum" "LightMode" = "ForwardBase" "PerformanceChecks"="False" }
    Blend SrcAlpha OneMinusSrcAlpha
    ZWrite off
	  ZTest less
	  Cull off
    LOD 100

    Pass{
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_fwdbase
      #include "UnityCG.cginc"
	    #include "AutoLight.cginc"
	    #include "Lighting.cginc"

      #define EPSILON 0.500001f
      #define poseOffset3D float3(_positionOffset.x,0,_positionOffset.z) * _positionOffset.w
      sampler2D _CameraDepthTexture;

      struct appdata{
        float4 vertex : POSITION;
        float3 normal : NORMAL;
      };

      struct v2f{
        float4 vertex : SV_POSITION;
        float3 objectVertex : TEXCOORD0;
        float4 scrPos: TEXCOORD1;
		float3 worldPos : TEXCOORD2;
		float4 cameraOffset : TEXCOORD3;
		float3 normal: TEXCOORD4;
		float3 localLightDir: TEXCOORD5;
		float3 localCameraPos: TEXCOORD6;
		float3 localScale: TEXCOORD7;
        float depth: DEPTH;
      };

      fixed4 _baseColor;
      fixed4 _backColor;
      int _ColorMappingMode;
      sampler2D _ColorMap;

      float4 _positionOffset;

      sampler3D _Noise3DA;
      float4 _Noise3DATile;

      sampler3D _Noise3DB;
      float4 _Noise3DBTile;

      float _NoiseCullThreshold;
      float _w;

      sampler2D _RayMaskTex;
      float4 _RayMaskTex_ST;
      float _BlueNoiseScale;

      sampler2D _Noise2DA;
      float4 _Noise2DATile;

      sampler2D _Noise2DB;
      float4 _Noise2DBTile;

      //sampler2D _CloudLightMap;

      float _Density;
      float _StepSize;
      float _StepScaleDis;
      int _max_count;
      float _heightCullThreshold;

      float _LightStepSize;
      int _light_max_count;
      float _light_damper;

      v2f vert (appdata v){
        v2f o;
        o.localScale = float3(length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)),
        					  length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)),
        					  length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z)));
        o.cameraOffset = fixed4(_WorldSpaceCameraPos / o.localScale.xyz, 0);
        float4 vertexOffseted = v.vertex + o.cameraOffset;
        o.objectVertex = vertexOffseted.xyz;
        o.vertex = UnityObjectToClipPos(vertexOffseted);
		o.worldPos = mul(unity_ObjectToWorld, vertexOffseted).xyz;
        o.scrPos = ComputeScreenPos(o.vertex);
        o.depth = -mul(UNITY_MATRIX_MV, vertexOffseted).z * _ProjectionParams.w;
        o.vertex.z = 0;
        o.normal = normalize(v.normal);
        o.localLightDir = normalize(mul((float3x3)unity_WorldToObject, normalize(UnityWorldSpaceLightDir(o.worldPos))));
        o.localCameraPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos,1));
        return o;
      }

      float getNoise(float3 samplePosition){
            float heightA = tex2Dlod(_Noise2DA, float4((samplePosition.xz + _positionOffset.xz) * _Noise2DATile.xz, 0, 0)).r * _Noise2DATile.y + _Noise2DATile.w;
            float heightB = (tex2Dlod(_Noise2DB, float4((samplePosition.xz + _positionOffset.xz) * _Noise2DBTile.xz, 0, 0)).r) * _Noise2DBTile.y + _Noise2DBTile.w;
            float noiseBase = heightA - abs(samplePosition.y + heightB);
            float noise3D = tex3Dlod(_Noise3DA, float4((samplePosition.xyz + poseOffset3D) * _Noise3DATile.xyz,1)).a * _Noise3DATile.w + 
            				tex3Dlod(_Noise3DB, float4((samplePosition.xyz + poseOffset3D) * _Noise3DBTile.xyz,1)).a * _Noise3DBTile.w;

          	return smoothstep(_NoiseCullThreshold - _w, _NoiseCullThreshold + _w, saturate((noiseBase + noise3D)));
      }

      fixed4 frag(v2f i) : SV_Target0{
        float3 rayOrigin = i.objectVertex;
        float3 rayDirection = normalize(rayOrigin - i.localCameraPos);
        rayOrigin = i.localCameraPos;

      	float blueNoiseOffset = tex2Dlod(_RayMaskTex, float4((_ScreenParams.xy / 256) * (i.scrPos.xy / i.scrPos.w),0,0)).r
      							+tex2Dlod(_RayMaskTex, float4((_ScreenParams.xy / 128) * (i.scrPos.xy / i.scrPos.w),0,0)).r;
        float rayLength = blueNoiseOffset * _BlueNoiseScale;


        float screenDepth = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r);


        float diff1 = screenDepth + mul(UNITY_MATRIX_MV, fixed4(rayOrigin, 1)).z * _ProjectionParams.w;
        float diff2 = screenDepth + mul(UNITY_MATRIX_MV, fixed4(rayOrigin + rayDirection, 1)).z * _ProjectionParams.w;
        float maxRayLength = min(_ProjectionParams.z / i.localScale.x ,diff1 / (diff1 - diff2));

        rayOrigin.y -= _positionOffset.y;
        //rayOrigin.xz +=  fixed2(0.5, 0.5);
      	if(abs(rayOrigin.y) > _heightCullThreshold)
      	{
      		rayLength += (abs(rayOrigin.y) - _heightCullThreshold) / abs(rayDirection.y);
      	}

        float cloudDensity = 0;
        float lightDepth = 0;
        float lightDepthOnePose = 0;
        float depthCount = 0;
        float3 samplePosition = float3(0,0,0);
        float3 org_light = float3(0,0,0);
        int iteration = 0;
        float noise = 0;
        float depthSizeParam = 1;
        float stepSizeParam = _StepSize;
        //[unroll(64)]
        while(iteration < _max_count)
        {
        	
        	samplePosition = rayLength * rayDirection + rayOrigin;

        	if(abs(samplePosition.y) > _heightCullThreshold || rayLength > maxRayLength || cloudDensity > 1)
        	{
        		break;
        	}
        	noise = getNoise(samplePosition);

        	if(noise > 0)
        	{
        		cloudDensity += depthSizeParam * noise * _Density;
        		/*
        		float yOrg = (samplePosition.y + 0.5) * 128 * (_heightCullThreshold * 2);
        		float yStep = floor(yOrg);
        		float2 pose2DA = float2(((samplePosition.x + _positionOffset.x) % 1) / 128 +
        								yStep / 128, 
        								(samplePosition.z + _positionOffset.z));
        		float2 pose2DB = pose2DA;
        		pose2DB.x += 1 / 128;
        		lightDepth += lerp(tex2Dlod(_CloudLightMap, float4(pose2DA,0,0)).r,tex2Dlod(_CloudLightMap, float4(pose2DB,0,0)).r,yOrg%1);
        		*/
        		//lightDepth += tex2Dlod(_CloudLightMap, float4(pose2DB,0,0)).r;

        		org_light = samplePosition;
        		for (int j = 0; j < _light_max_count; j++)
        		{
        			org_light += _LightStepSize * i.localLightDir;
        			if(abs(org_light.y) > _heightCullThreshold)
        			{
        				break;
        			}
        			lightDepth += getNoise(org_light);
        		}
        		depthCount += depthSizeParam;
        		//rayLength -= 0.875 * _StepSize;
        	}

        	else
        	{
        		iteration++;
        		depthSizeParam += iteration * _StepScaleDis;
        		stepSizeParam = _StepSize * depthSizeParam;
        		rayLength += stepSizeParam;
        		while(iteration < _max_count)
        		{
        			samplePosition = rayLength * rayDirection + rayOrigin;
        			if(abs(samplePosition.y) > _heightCullThreshold || rayLength > maxRayLength)
        			{
        				break;
        			}
        			float noise = getNoise(samplePosition);


          			if(noise > 0)
          			{
          					rayLength -= stepSizeParam / 2;
          					samplePosition = rayLength * rayDirection + rayOrigin;
          					noise = getNoise(samplePosition);

		          			rayLength += ((noise == 0) - 0.5) * stepSizeParam / 2;
        			  		samplePosition = rayLength * rayDirection + rayOrigin;
          					noise = getNoise(samplePosition);

          					rayLength += ((noise == 0) - 0.5) * stepSizeParam / 4;
          					samplePosition = rayLength * rayDirection + rayOrigin;
          					noise = getNoise(samplePosition);

			          		rayLength += (noise == 0) * stepSizeParam / 8;
			          		rayLength -= stepSizeParam;
          					break;
          			}
        			iteration++;
        			depthSizeParam += iteration * _StepScaleDis;
        			stepSizeParam = _StepSize * depthSizeParam;
          			rayLength += stepSizeParam;
        		}
        	}

        	iteration++;
        	depthSizeParam += iteration * _StepScaleDis;
        	stepSizeParam = _StepSize * depthSizeParam;
        	rayLength += stepSizeParam;
        }

        if((abs(samplePosition.y) > _heightCullThreshold || rayLength > maxRayLength) && noise > 0)
        {
        	rayLength -= 0.875 * stepSizeParam;
        	int jteration = 0;
        	while (jteration < 7)
        	{
        		samplePosition = rayLength * rayDirection + rayOrigin;
        		if(abs(samplePosition.y) > _heightCullThreshold || rayLength > maxRayLength || cloudDensity > 1)
        		{
        			break;
        		}
        		noise = getNoise(samplePosition);

        		if(noise > 0)
        		{
        			cloudDensity += depthSizeParam * noise * _Density / 8;

        			/*
        			float yOrg = (samplePosition.y + 0.5) * 128 * (_heightCullThreshold * 2);
        			float yStep = floor(yOrg);
        			float2 pose2DA = float2(((samplePosition.x + _positionOffset.x) % 1) / 128 +
        									yStep / 128, 
        									samplePosition.z + _positionOffset.z);
        			float2 pose2DB = pose2DA;
        			pose2DB.x += 1 / 128;
        			lightDepth += lerp(tex2Dlod(_CloudLightMap, float4(pose2DA,0,0)).r,tex2Dlod(_CloudLightMap, float4(pose2DB,0,0)).r,yOrg - yStep) / 8;
        			*/

        			org_light = samplePosition;
        			for (int j = 0; j < _light_max_count; j++)
        			{
        				org_light += _LightStepSize * i.localLightDir;
        				if(abs(org_light.y) > _heightCullThreshold)
        				{
        					break;
        				}
        				lightDepth += getNoise(org_light) / 8;
        			}
        			depthCount += 0.125 * depthSizeParam;
        		}

        		rayLength += stepSizeParam / 8;
        		jteration++;	
        	}
        }

        float lightCalculate = saturate(lightDepth / (_light_max_count * max(1, depthCount)) * _light_damper);
        if(_ColorMappingMode == 0)
      		return fixed4(lerp(_baseColor.rgb,_backColor.rgb, lightCalculate) * _LightColor0.rgb.rgb + UNITY_LIGHTMODEL_AMBIENT, saturate(cloudDensity));
      	else
      		return fixed4(tex2Dlod(_ColorMap, float4(lightCalculate,0.5,0,0)).rgb * _LightColor0.rgb.rgb + UNITY_LIGHTMODEL_AMBIENT, saturate(cloudDensity));
      	//return color;
      }
      ENDCG
    }
  }
} 
