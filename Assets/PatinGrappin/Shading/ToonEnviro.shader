Shader "Custom/ToonEnviro" {
	Properties
	{
		_ToonTex ("Toon Sampler", 2D) = "White" {}
		_shadowColor ("Shadow toon Color", Color) = (0,0,0,0)
		_Color ("Main Color", Color) = (0,0,0,0)
		_OverColor ("Over Color", Color) = (0,0,0,0)
		_VolumeInfo ("Color of volume (Lambert)", Color) = (0,0,0,0)
		_shadowFactor ("Factor of toon shadow", float) = 0
		_depthFactor ("Factor of depth color", float) = 0
		_maxDepth ("MAX distance of depth", float) = 0
		_minDepth ("MIN distance of depth", float) = 0
		_FresnelToonTex ("Fresnel Toon Sampler", 2D) = "White" {}
		_FresnelValue ("Factor of Fresnel", float) = 0
		_FresnelIntensity ("Fresnel Intensity", Range(0,2)) = 0
		_BlueVector ("Blue Variation Vector", Vector) = (0,0,0,0)
		_BlueColor ("Color of Blue Variation", Color) = (0,0,0,0)
		_BobbyVector ("Bobby Variation Vector", Vector) = (0,0,0,0)
		_BobbyColor ("Color of Bobby Variation", Color) = (0,0,0,0)
		_ReflectIntensity ("Intensity of reflect", float) = 0
	}
	SubShader {

	Pass
	{

		Tags { "LightMode" = "ForwardBase" }

		CGPROGRAM
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"

		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fwdbase

		//Toon
		sampler2D _ToonTex;
		fixed4 _Color, _OverColor;
		fixed4 finalColor, _shadowColor;
		float _shadowFactor;

		//Depth fade
		float _camDist;
		float _depthFactor, _maxDepth, _minDepth;

		//Blue variation
		float3 _BlueVector;
		fixed4 _BlueColor;

		float3 _BobbyVector;
		fixed4 _BobbyColor;

		//PlayerReflect
		float _ReflectIntensity;
		float4 _PlayerPosition;

		// volume Information color
		fixed4 _VolumeInfo;


		//Fresnel
		float _FresnelValue, _FresnelIntensity;
		//float3 _camWorldDir;
		sampler2D _FresnelToonTex;

		struct vertInput
		{
			float4 position : POSITION;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};

		struct vertOutput
		{
			float4 position : SV_POSITION;
			float3 worldPos : TEXCOORD1;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};

		vertOutput vert(vertInput v)
		{
			vertOutput o;
			UNITY_INITIALIZE_OUTPUT(vertOutput, o);
			o.position = UnityObjectToClipPos(v.position);
			o.worldPos = mul(unity_ObjectToWorld, v.position).xyz;
			o.normal = normalize(mul(float4(v.normal.x, v.normal.y, v.normal.z, 0.0), unity_WorldToObject).xyz);
			return o;
		}

		float4 frag(vertOutput psIn) : SV_Target
		{

			//NdotL
			float3 diffDir = normalize (_WorldSpaceLightPos0.xyz);
			float diffIntensity = dot(diffDir, psIn.normal);

			fixed3 volumeInfo = fixed3(clamp(_VolumeInfo.r + diffIntensity, 0,1),
				clamp(_VolumeInfo.g + diffIntensity, 0,1),
				clamp(_VolumeInfo.b + diffIntensity, 0,1));

			//Fresnel
			float3 toCamVector = normalize(_WorldSpaceCameraPos - psIn.worldPos);
			//float3 camOrient = UNITY_MATRIX_V[2].xyz;
			float VdotN = clamp(dot(toCamVector, psIn.normal),0,1);
			float fresnel = pow(1 - VdotN, _FresnelValue);

			//ToonFresnel
			float4 toonFresnel = tex2D(_FresnelToonTex, float2(fresnel, 0)) * _FresnelIntensity;

			//Toon
			float toonUVX = (diffIntensity * 0.5) + 0.5;
			float4 toonShadow = tex2D(_ToonTex, float2(toonUVX, 0));
			toonShadow = pow(toonShadow, _shadowFactor);
			float4 toonCol = lerp(_shadowColor, fixed4(1,1,1,1), toonShadow);

			//Blue variation
			float BdotN = clamp(dot(psIn.normal, _BlueVector), 0, 1);
			finalColor.xyz = lerp(_Color, _BlueColor, BdotN);

			float AdotN = clamp(dot(psIn.normal, _BobbyVector), 0, 1);
			finalColor.xyz = lerp(finalColor, _BobbyColor, AdotN);


			//Depth Fade
			_camDist = distance(psIn.worldPos, _WorldSpaceCameraPos);
			float depth = 1 - ( (clamp(_camDist, _minDepth, _maxDepth) - _minDepth) / (_maxDepth - _minDepth) );

			//Reflet Perso
			float3 toPlayerVector = normalize(_PlayerPosition.xyz - psIn.worldPos);
			float3 halfVector = normalize(toPlayerVector + toCamVector);
			float NdotH = clamp(dot(psIn.normal, halfVector), -1, 1);
			float playerReflect = 1 - pow((NdotH / 2) + 0.5, _ReflectIntensity);

			//Ombre Perso
			float playerDist = distance(_PlayerPosition.xyz, psIn.worldPos);
			// float gpasltemps = clamp(playerDist,0, maxDist); ///////////////////////////////////////// A FINIR !!!!!!!!!!!!!!!!!!!!


			//Return
			fixed3 col = (toonCol.rgb * finalColor.rgb * depth) + toonFresnel;
			//col *= playerReflect;
			return float4(col * _OverColor.xyz * volumeInfo, 1.0);
		}


		ENDCG
	}
	}
		Fallback "Diffuse"
		Fallback "VertexLit"
}
