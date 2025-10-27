Shader "Custom/BallTrail"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 0.5, 0, 1)
        _EmissionColor ("Emission Color", Color) = (1, 0.5, 0, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 2
        _MainTex ("Texture", 2D) = "white" {}
        _Softness ("Edge Softness", Range(0, 2)) = 0.5
        _Fade ("Alpha Fade", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend One OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _BaseColor;
            float4 _EmissionColor;
            float _EmissionIntensity;
            float _Softness;
            float _Fade;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Textura base
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Gradiente suave en los bordes
                float edge = smoothstep(0.0, _Softness, i.uv.x) * (1.0 - smoothstep(1.0 - _Softness, 1.0, i.uv.x));

                // Color base con transparencia
                fixed4 baseCol = _BaseColor * texColor * edge * _Fade;

                // Emisión
                fixed3 emission = _EmissionColor.rgb * _EmissionIntensity * edge;

                baseCol.rgb += emission;

                return baseCol;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
