// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UnityTutoToonGround" {
	Properties
	{
		_ToonTex ("Toon Sampler", 2D) = "White" {}
		_NormalMap ("NormalMap", 2D) = "White" {}
		_normalTileX ("Normal Tile X", float) = 0
		_normalTileY ("Normal Tile Y", float) = 0
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

		sampler2D _ToonTex, _NormalMap;
		fixed4 _Color;
		float _normalTileX, _normalTileY;

		struct vertInput
		{
			float4 position : POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
			  half3 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z
			float2 uv : TEXCOORD0;
		};

		struct vertOutput
		{
			float4 position : SV_POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
			  half3 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z
			float2 uv : TEXCOORD0;
		};

		vertOutput vert(vertInput v)
		{
			vertOutput o;
			UNITY_INITIALIZE_OUTPUT(vertOutput, o);
			o.position = UnityObjectToClipPos(v.position);
			o.normal = normalize(mul(float4(v.normal.x, v.normal.y, v.normal.z, 0.0), unity_WorldToObject).xyz);
			half3 wNormal = UnityObjectToWorldNormal(v.normal);
                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
                // output the tangent space matrix
                o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
                o.uv = v.uv;
                return o;
		}

		float4 frag(vertOutput psIn) : SV_Target
		{

			// sample the normal map, and decode from the Unity encoding
                half3 tnormal = UnpackNormal(tex2D(_NormalMap, float2(psIn.normal.x * _normalTileX, psIn.normal.y * _normalTileY)));
                // transform normal from tangent to world space
                half3 worldNormal;
                worldNormal.x = dot(psIn.tspace0, tnormal);
                worldNormal.y = dot(psIn.tspace1, tnormal);
                worldNormal.z = dot(psIn.tspace2, tnormal);

			//float4 ambientLight = UNITY_LIGHTMODEL_AMBIENT;
			float3 diffDir = normalize (_WorldSpaceLightPos0.xyz);
			float diffIntensity = clamp(dot(diffDir, worldNormal), -1, 1);
			float toonUVX = (diffIntensity * 0.5) + 0.5;
			float4 toonCol = tex2D(_ToonTex, toonUVX);

			return float4(toonCol.rgb * _Color.rgb, 1.0);
		}


		ENDCG
	}
	}
		Fallback "Diffuse"
}
