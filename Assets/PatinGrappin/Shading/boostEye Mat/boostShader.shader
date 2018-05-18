Shader "Custom/boostShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ToonTex ("Toon Sampler", 2D) = "White" {}
		_ColTex ("Color Sampler", 2D) = "White" {}
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
				float3 worldNormal : TEXCOORD2;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex, _ToonTex, _ColTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.position = UnityObjectToClipPos(v.position);
				o.worldPos = mul(unity_ObjectToWorld, v.position).xyz;
				o.normal = v.normal;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f o) : SV_Target
			{
				//NdotC
				float3 camDir = normalize(o.worldPos - _WorldSpaceCameraPos);
				float NdotC = dot(camDir, o.worldNormal)* 0.5 + 0.5;

				fixed4 toonValue = tex2D(_ToonTex, float2(NdotC, 0));

				fixed4 col = tex2D(_ColTex, float2(toonValue.r, 0));
				
				return col;
			}
			ENDCG
		}
	}
}
