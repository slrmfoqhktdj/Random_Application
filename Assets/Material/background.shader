Shader "Custom/background"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.2, 0.5, 1, 1)
        _BottomColor ("Bottom Color", Color) = (1, 0.8, 0.6, 1)
        _Exponent ("Exponent", Range(0.1, 8.0)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
        Cull Off ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 uv : TEXCOORD0;
            };
            
            half4 _TopColor;
            half4 _BottomColor;
            half _Exponent;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 viewDir = normalize(i.uv);
                float lerpValue = pow(max(0, viewDir.y + 0.5), _Exponent);
                return lerp(_BottomColor, _TopColor, lerpValue);
            }
            ENDCG
        }
    }
}