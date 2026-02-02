Shader "PostEffects/PostEffectShader"
{
	Properties
	{
		_TintScreen("Tint Screen", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
	}
		CGINCLUDE
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
		float4 screenSpace: TEXCOORD1;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		o.screenSpace = ComputeScreenPos(o.vertex);
		return o;
	}

	sampler2D _MainTex;
	fixed4 _TintScreen;
	ENDCG


		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass //0
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
			fixed4 col = tex2D(_MainTex, i.uv);

			return col * _TintScreen;
			}
		ENDCG
		}

		Pass //1
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 _MainTex_TexelSize;

			fixed4 frag(v2f i) : SV_Target
			{
				//fixed4 col = tex2D(_MainTex, i.uv);
				float3 blurSample =
				tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(-1, -1)) +
				tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(1, -1)) +
				tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(-1, 1)) +
				tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(-1, -1));
				float2 screenSpaceUV = i.screenSpace.xy / i.screenSpace.w;
				return fixed4(screenSpaceUV,0, 1);
			}
			ENDCG
		}
	}
}
