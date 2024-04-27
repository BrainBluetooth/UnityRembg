Shader "Hidden/Rembg/Mask-Postprocess"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _Min ("Min", Range(0, 1)) = 0.1
        _Max ("Max", Range(0, 1)) = 0.9
		_Check1("Check Color 1", Color) = (0.4, 0.4, 0.4, 1)
		_Check2("Check Color 2", Color) = (0.8, 0.8, 0.8, 1)
    }
    SubShader
    {
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
            float _Min, _Max;

			float4 _MainTex_TexelSize;
			float4 _Check1;
			float4 _Check2;

            float sigmoid_reverse(float x)
            {
                return -log(1 / x - 1);
            }

            float4 frag (v2f i) : SV_Target
            {
                float mask = tex2D(_MainTex, i.uv).r;
                float x = sigmoid_reverse(mask);
                float min = sigmoid_reverse(_Min);
                float max = sigmoid_reverse(_Max);
                x = (x - min) / (max - min);
				if (mask < 1.0 / 255)
				{
					float2 xy = i.uv * _MainTex_TexelSize.zw;
					return (uint)((xy.x / 32) % 2) ^ (uint)((xy.y / 32) % 2) == 0 ? _Check1 : _Check2;
					// return 0;
				}
                return float4(x, x, mask > 0.5, 1);
            }
            ENDCG
        }
    }
}