Shader "Custom/baseToonShader" {
	Properties
	{
		_Color ("Main Color", Color) = (0,0,0,0)

	//	_depthCol ("Depth Color", Color) = (0,0,0,0)
	//	_maxDepth ("MAX distance of depth", float) = 0
	//	_minDepth ("MIN distance of depth", float) = 0
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

		//Depth fade
		float _camDist;
		float _depthFactor, _maxDepth, _minDepth;
		float4 _depthCol;

		struct vertInput
		{
			float4 position : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct vertOutput
		{
			float4 position : SV_POSITION;
			float3 worldPos : TEXCOORD1;
			float2 uv : TEXCOORD0;
		};

		vertOutput vert(vertInput v)
		{
			vertOutput o;
			UNITY_INITIALIZE_OUTPUT(vertOutput, o);
			o.position = UnityObjectToClipPos(v.position);
			o.worldPos = mul(unity_ObjectToWorld, v.position).xyz;
			return o;
		}

		float4 frag(vertOutput psIn) : SV_Target
		{
			//Depth Fade
			_maxDepth = 2028.71;
			_minDepth = 477.8;
			_depthCol = float4(0.27, 0.314, 0.349, 1);
			//_depthCol = float4(70/255, 80/255, 89/255, 1);
			_camDist = distance(psIn.worldPos, _WorldSpaceCameraPos);
			float depth = 1 - ( (clamp(_camDist, _minDepth, _maxDepth) - _minDepth) / (_maxDepth - _minDepth) );

			fixed4 col = _Color;
			col = lerp(_depthCol, col, depth);

			return col;
		}


		ENDCG
	}
	}
		Fallback "Diffuse"
}
