Shader "Custom/baseToonShader" {
	Properties
	{
		_Color ("Main Color", Color) = (0,0,0,0)
	}
	SubShader {

	Pass
	{

		Tags { "LightMode" = "ForwardBase" }

		CGPROGRAM
		#include "UnityCG.cginc"

		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

		fixed4 _Color;

		struct vertInput
		{
			float4 position : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct vertOutput
		{
			float4 position : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		vertOutput vert(vertInput v)
		{
			vertOutput o;
			UNITY_INITIALIZE_OUTPUT(vertOutput, o);
			o.position = UnityObjectToClipPos(v.position);
			return o;
		}

		float4 frag(vertOutput psIn) : SV_Target
		{
			fixed4 col = _Color;
			return col;
		}


		ENDCG
	}
	}
		Fallback "Diffuse"
}
