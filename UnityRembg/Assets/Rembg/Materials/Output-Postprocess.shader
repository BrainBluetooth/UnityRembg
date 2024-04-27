Shader "Hidden/Rembg/Output-Postprocess"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _Min ("Min", float) = 0
        _Max ("Max", float) = 1
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

            float4 frag (v2f i) : SV_Target
            {
                float mask = tex2D(_MainTex, i.uv).r;
                mask = (mask - _Min) / (_Max - _Min);
                return float4(mask, 0, 0, 1);
            }
            ENDCG
        }
    }
}
