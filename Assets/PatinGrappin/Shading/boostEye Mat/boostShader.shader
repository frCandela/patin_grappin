Shader "Custom/boostShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ToonTex ("Toon Sampler", 2D) = "White" {}
		_ColTex ("Color Sampler", 2D) = "White" {}
		_ToShpere ("Spherical factor", Range(0,1)) = 0
		_RadiusFactor ("Radius of sphere form", Range(0.5,2)) = 0
		_ToWhite ("Illumination Factor", Range(0,2)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 position : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				float3 worldPos : TEXCOORD1;
				float lumiFactor : TEXCOORD3;
				float3 worldNormal : TEXCOORD2;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex, _ToonTex, _ColTex;
			float4 _MainTex_ST;
			float _ToWhite, _ToShpere, _RadiusFactor;

			v2f vert (appdata v)
			{
				v2f o;

				float4 spherePos = float4(normalize(v.position.xyz) * 0.05 * _RadiusFactor, 1); // HARD CODED SPHERE RADIUS = 0.05
				float4 newPos = lerp(v.position, spherePos, _ToShpere);

				o.lumiFactor = 1 - (length(v.position.xyz) - 0.0133) / (0.0335 - 0.0133) - 1; // MIN = 0.0133 // MAX = 0.0335
				o.lumiFactor = clamp(o.lumiFactor  + _ToWhite, 0,1);


				o.position = UnityObjectToClipPos(newPos);
				o.worldPos = mul(unity_ObjectToWorld, newPos).xyz;
				o.normal = v.normal;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//NdotC
				float3 camDir = normalize(i.worldPos - _WorldSpaceCameraPos);
				float NdotC = dot(camDir, i.worldNormal)* 0.5 + 0.5;

				fixed4 toonValue = tex2D(_ToonTex, float2(NdotC, 0));

				fixed4 col = tex2D(_ColTex, float2(toonValue.r, 0));

				col += i.lumiFactor;
				
				return col;
			}
			ENDCG
		}
	}
}
