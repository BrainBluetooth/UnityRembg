Shader "Hidden/Rembg/Normalize"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		_Max ("Max", float) = 1
		_Mean ("Mean", Vector) = (0, 0, 0, 0)
		_Std ("Std", Vector) = (1, 1, 1, 1)
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

            sampler2D _MainTex;
			float _Max;
			float3 _Mean;
			float3 _Std;

            float4 frag (v2f i) : SV_Target
            {
                float3 col = tex2D(_MainTex, i.uv).rgb;
				col /= _Max;
				col = (col - _Mean) / _Std;
                return float4(col, 1);
            }
            ENDCG
        }
    }
}
