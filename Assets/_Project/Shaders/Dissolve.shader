Shader "Custom/DissolveSurface"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _EdgeWidth ("Edge Width", Range(0,0.2)) = 0.05
        _EdgeColor ("Edge Color", Color) = (1,0.5,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NoiseTex;
        half _Glossiness;
        half _Metallic;
        float _DissolveAmount;
        float _EdgeWidth;
        fixed4 _EdgeColor;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NoiseTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            float noise = tex2D(_NoiseTex, IN.uv_NoiseTex).r;
            float dissolve = noise - _DissolveAmount;
            clip(dissolve);

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            
            if (_DissolveAmount > 0.001 && dissolve < _EdgeWidth)
            {
                o.Emission = _EdgeColor.rgb * (1.0 - dissolve / _EdgeWidth);
            }
            else
            {
                o.Emission = 0;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}