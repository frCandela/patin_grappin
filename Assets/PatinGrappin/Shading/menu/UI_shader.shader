Shader "Custom/UI_shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Radius ("Pointer Radius (px)", float) = 0
	}
	SubShader
	{
		 Tags {"Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 screenPos : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			//Pointer
			float2 _MouseScreenPos, _ScreenSize;
			float _Radius;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// pointeur shader

				//get mousePos in pixels (not UVs) for a circle pointer
				float2 pixMouseScreenPos = _MouseScreenPos * _ScreenSize;
				float2 pixScreenPos = i.screenPos * _ScreenSize;

				float pixDistToAim = distance(pixScreenPos, pixMouseScreenPos);

				// isInRadius = 1 si dans le ridus, sinon 0    |||   step(a,x) = 0 if x < a   or   1 if x >= a
				float isInRadius = 1 - step(_Radius, pixDistToAim);

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				//Grey of current color
				float medianCol = (col.r + col.g + col.b) / 3.0;

				//Apply pointer
				col.rgb = ((1 - isInRadius) * col) + (isInRadius * float3(medianCol, medianCol, medianCol));

				return col;
			}
			ENDCG
		}
	}
}
