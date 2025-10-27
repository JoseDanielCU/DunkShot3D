Shader "Unlit/ScoreRingGlow"
{
    Properties
    {
        _Color ("Ring Color", Color) = (1,1,1,1)
        _Thickness ("Ring Thickness", Range(0.0, 1.0)) = 0.3
        _Softness ("Edge Softness", Range(0.0, 1.0)) = 0.2
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        Lighting Off

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;
            float _Thickness;
            float _Softness;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);

                // Borde del anillo
                float outer = 0.5;
                float inner = outer - _Thickness;

                // Borde difuminado
                float ring = smoothstep(outer, outer - _Softness, dist) * 
                             (1.0 - smoothstep(inner, inner + _Softness, dist));

                return fixed4(_Color.rgb, _Color.a * ring);
            }
            ENDCG
        }
    }
}
