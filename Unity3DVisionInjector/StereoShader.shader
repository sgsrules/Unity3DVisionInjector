Shader "StereoShader"
{
	Properties
	{
		rightTex("Texture", 2D) = "black" {}
		leftTex("Texture", 2D) = "red" {}
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
				o.vertex = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1.0));
				o.uv = v.uv;
				return o;
			}
			
			sampler2D rightTex;
			sampler2D leftTex;
			//Texture2D<float4> StereoParams : register(t125);

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;
		//	float4 stereo = StereoParams.Load(0);
			//	if(stereo.z>0) col = tex2D(leftTex, i.uv);
			//	else col =  tex2D(rightTex, i.uv);

			if (i.uv.x>.5) col = tex2D(leftTex, float2(i.uv.x*2-1,1-i.uv.y));
				else col =  tex2D(rightTex, float2(i.uv.x * 2 , 1-i.uv.y));
		
				return col;
			}
			ENDCG
		}
	}
}
