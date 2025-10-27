Shader "Custom/GradientSkybox"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.1, 0.3, 0.7, 1)
        _BottomColor ("Bottom Color", Color) = (0.8, 0.9, 1, 1)
        _Exponent ("Gradient Exponent", Range(0.1, 8)) = 1.5
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" }
        Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            float4x4 unity_ObjectToWorld;
            float4x4 unity_MatrixVP;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, v.vertex));
                o.dir = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
                return o;
            }

            float4 _TopColor;
            float4 _BottomColor;
            float _Exponent;

            float4 frag (v2f i) : SV_Target
            {
                float t = saturate(pow(i.dir.y * 0.5 + 0.5, _Exponent));
                return lerp(_BottomColor, _TopColor, t);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
