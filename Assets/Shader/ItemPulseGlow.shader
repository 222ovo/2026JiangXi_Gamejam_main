Shader "Custom/ItemPulseGlow"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)

        _GlowColor ("Glow Color", Color) = (1, 0.8, 0.3, 1)
        _GlowStrength ("Glow Strength", Float) = 2

        _PulseInterval ("Pulse Interval", Float) = 2
        _FlashDuration ("Flash Duration", Float) = 0.4
        _PulsePower ("Pulse Power", Float) = 3

        _MinGlow ("Min Glow", Range(0, 1)) = 0
        _MaxGlow ("Max Glow", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _GlowColor;
                float _GlowStrength;
                float _PulseInterval;
                float _FlashDuration;
                float _PulsePower;
                float _MinGlow;
                float _MaxGlow;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);

                output.positionHCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 baseTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half3 baseColor = baseTex.rgb * _BaseColor.rgb;

                Light mainLight = GetMainLight();
                half3 normalWS = normalize(input.normalWS);
                half lightAmount = saturate(dot(normalWS, mainLight.direction));
                half3 litColor = baseColor * (mainLight.color * lightAmount + 0.25);

                float interval = max(_PulseInterval, 0.01);
                float duration = max(_FlashDuration, 0.01);

                float cycleTime = fmod(_Time.y, interval);
                float flashMask = 1.0 - smoothstep(0.0, duration, cycleTime);

                flashMask = pow(flashMask, _PulsePower);
                flashMask = lerp(_MinGlow, _MaxGlow, flashMask);

                half3 glow = _GlowColor.rgb * flashMask * _GlowStrength;

                return half4(litColor + glow, baseTex.a * _BaseColor.a);
            }
            ENDHLSL
        }
    }
}