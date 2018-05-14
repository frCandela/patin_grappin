Shader "Custom/speedEffectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SpeedTex ("Speed Effect Texture", 2D) = "white" {}
		_Offset ("Deformation Offset", Range(0, 0.6)) = 0
		_DistPowFactor ("factor pow", float) = 2
		_TexFactor ("SpeedTex Factor", Range(-1,1)) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex, _SpeedTex;
			float2 _CenterPos;
			float _Offset, _DistPowFactor, _TexFactor;

			fixed4 frag (v2f i) : SV_Target
			{
				_CenterPos = float2(0.5, 0.5);

				//fixed4 deform = tex2D(_SpeedTex, i.uv);
				float2 dirVec = i.uv - _CenterPos;
				float dist = distance(i.uv, _CenterPos) / distance(_CenterPos, float2(1,1));
				dirVec *= pow(dist, _DistPowFactor);

				float texOffset = 1 + (tex2D(_SpeedTex, i.uv) * _TexFactor);

				float2 finalOffset = dirVec * _Offset * texOffset;

				fixed4 col = tex2D(_MainTex, i.uv - finalOffset);
				return col;
			}
			ENDCG
		}
	}
}
