Shader "LanguageIndex/LinesShader"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1 ,0, 1, 1)
        _BrightnessController ("BrightnessController", float) = 21.55
        _FrontOfGraph ("FrontOfGraph", Vector) = (0, 0, -16.63553, 1)
    }
    SubShader
    {
//        Tags { "RenderType"="Opaque" }
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _Color;
            float4 _MainTex_ST;
            float4 _FrontOfGraph;
            float _BrightnessController;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // get color
                float4 col = _Color;
                // get distance from front of graph
                const float dist_from_origin = sqrt(pow(i.worldPos.x - _FrontOfGraph.x, 2) + pow(i.worldPos.z - _FrontOfGraph.z, 2));
                col.xyz *= 1 - smoothstep(0, _BrightnessController, dist_from_origin);
                // col.a *= 1- smoothstep(0, _BrightnessController, dist_from_origin);
                return col;
            }
            ENDCG
        }
    }
//    Fallback "Diffuse"
}
