// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UnityTutoToonPG" {
	Properties
	{
		_ToonTex ("Toon Sampler", 2D) = "White" {}
		_Color ("Main Color", Color) = (0,0,0,0)
	}
	SubShader {

	Pass
	{

		Tags { "LightMode" = "ForwardBase" }

		CGPROGRAM
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

		sampler2D _ToonTex;
		fixed4 _Color;

		struct vertInput
		{
			float4 position : POSITION;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};

		struct vertOutput
		{
			float4 position : SV_POSITION;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};

		vertOutput vert(vertInput v)
		{
			vertOutput o;
			UNITY_INITIALIZE_OUTPUT(vertOutput, o);
			o.position = UnityObjectToClipPos(v.position);
			o.normal = normalize(mul(float4(v.normal.x, v.normal.y, v.normal.z, 0.0), unity_WorldToObject).xyz);
			return o;
		}

		float4 frag(vertOutput psIn) : SV_Target
		{
			//float4 ambientLight = UNITY_LIGHTMODEL_AMBIENT;
			float3 diffDir = normalize (_WorldSpaceLightPos0.xyz);
			float diffIntensity = clamp(dot(diffDir, psIn.normal), -1, 1);
			float toonUVX = (diffIntensity * 0.5) + 0.5;
			float4 toonCol = tex2D(_ToonTex, toonUVX);

			return float4(toonCol.rgb * _Color.rgb, 1.0);
		}


		ENDCG
	}
	}
		Fallback "Diffuse"
}
