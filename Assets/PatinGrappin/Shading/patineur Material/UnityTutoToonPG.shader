// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UnityTutoToonPG" {
	Properties
	{
		_ToonTex ("Toon Sampler", 2D) = "White" {}
		_Color ("Main Color", Color) = (0,0,0,0)
		_PointerColor ("Pointer Color", Color) = (1,0,0,1)
		_PointerRadius ("Pointer Radius", float) = 3
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

		// pointeur
		float3 _AimTargetPos;
		fixed4 _PointerColor;
		float _PointerRadius;

		struct vertInput
		{
			float4 position : POSITION;
			float3 worldPos : TEXCOORD01;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};

		struct vertOutput
		{
			float4 position : SV_POSITION;
			float3 worldPos : TEXCOORD01;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};

		vertOutput vert(vertInput v)
		{
			vertOutput o;
			UNITY_INITIALIZE_OUTPUT(vertOutput, o);
			o.position = UnityObjectToClipPos(v.position);
			o.normal = normalize(mul(float4(v.normal.x, v.normal.y, v.normal.z, 0.0), unity_WorldToObject).xyz);
			o.worldPos = mul(unity_ObjectToWorld, v.position).xyz;
			return o;
		}

		float4 frag(vertOutput psIn) : SV_Target
		{
			//float4 ambientLight = UNITY_LIGHTMODEL_AMBIENT;
			float3 diffDir = normalize (_WorldSpaceLightPos0.xyz);
			float diffIntensity = clamp(dot(diffDir, psIn.normal), -1, 1);
			float toonUVX = (diffIntensity * 0.5) + 0.5;
			float4 toonCol = tex2D(_ToonTex, toonUVX);

			// pointeur shader
			float distToAim = distance(psIn.worldPos, _AimTargetPos);
			// step(a,x) = 0 if x < a   or   1 if x >= a
			// isInRadius = 1 si dans le ridus, sinon 0
			float isInRadius = 1 - step(_PointerRadius, distToAim);


			fixed4 col = fixed4(toonCol.rgb * _Color.rgb, 1.0);
			col = ((1 - isInRadius) * col) + (isInRadius * _PointerColor);
			return col;
		}


		ENDCG
	}
	}
		Fallback "Diffuse"
}
