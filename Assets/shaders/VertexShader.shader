Shader "JHDev/VertexShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_RedTex("R Texture", 2D) = "white" {}
		_RedNormal("R (Normal)", 2D) = "white" {}
		_GreenRex("G Texture", 2D) = "white" {}
		_GreenNormal("G (Normal)", 2D) = "white" {}
		_BlueTex("B Texture", 2D) = "white" {}
		_BlueNormal("B (Normal)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _RedTex;
		sampler2D _RedNormal;
		sampler2D _GreenTex;
		sampler2D _GreenNormal;
		sampler2D _BlueTex;
		sampler2D _BlueNormal;

		struct Input {
			float2 uv_RedTex;
			float2 uv_RedNormal;
			float2 uv_GreenTex;
			float2 uv_GreenNormal;
			float2 uv_BlueTex;
			float2 uv_BlueNormal;
			float4 color: Color;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half4 r = tex2D(_RedTex, IN.uv_RedTex);
			half4 g = tex2D(_GreenTex, IN.uv_GreenTex);
			half4 b = tex2D(_BlueTex, IN.uv_BlueTex);
			half4 rn = tex2D(_RedNormal, IN.uv_RedNormal);
			half4 gn = tex2D(_GreenNormal, IN.uv_GreenNormal);
			half4 bn = tex2D(_BlueNormal, IN.uv_BlueNormal);
			o.Albedo = r.rgb*IN.color.r + g.rgb*IN.color.g + b.rgb*IN.color.b;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Normal = UnpackNormal(rn*IN.color.r + gn*IN.color.g + bn*IN.color.b);
			//o.Emission = r.rgb*IN.color.r+g.rgb*IN.color.g+b.rgb*IN.color.b;
			o.Alpha = r.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
