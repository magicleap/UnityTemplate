// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/YUV_Shader"
{
	Properties
	{
		_MainTex("Y", 2D) = "black" {}
		_UTex("U", 2D) = "gray" {}
		_VTex("V", 2D) = "gray" {}
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
			#pragma target 2.0

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			sampler2D _UTex;
			sampler2D _VTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 ut = tex2D(_UTex, i.uv);
				fixed4 vt = tex2D(_VTex, i.uv);
				float y = 1.1643 * (col.a - 0.0625);
				float u = (1.1643 * (ut.a - 0.0625)) - 0.5;
				float v = (1.1643 * (vt.a - 0.0625)) - 0.5;

				//float r = y + 1.596 * v;
				//float g = y - 0.391  * u - 0.813 * v;
				//float b = y + 2.018  * u;
				float r = y + 1.403 * v;
				float g = y - 0.344  * u - 0.714 * v;
				float b = y + 1.770  * u;
				col.rgba = float4(r, g, b, 1.f);
				float3 result = max(6.10352e-5, col.rgb);
				result = result > 0.04045 ? pow(result * (1.0 / 1.055) + 0.0521327, 2.4) : result * (1.0 / 12.92);
				return float4(result, 1.f);
			}
			ENDCG
		}
	}
}

