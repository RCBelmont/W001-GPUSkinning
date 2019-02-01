Shader "Unlit/Test"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _MatrixTex ("Matrix", 2D) = "white" { }
        _Param ("Param", Vector) = (1, 1, 1, 1)
        _MainColor ("_Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex: POSITION;
                float2 uv: TEXCOORD0;
                float4 tan: TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv: TEXCOORD0;
                float4 tan: TANGENT;
                float4 vertex: SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _MatrixTex;
            float4 _MainTex_ST;
            float4 _Param;
            float4 _MainColor;
            float _FrameIdx;
            
            half4 GetUV(float startPix1)
            {
                float row1 = floor(startPix1 / _Param.x);
                float column1 = (startPix1 % _Param.x);
                return half4(column1 / _Param.x, row1 / _Param.y, 0, 5);
            }
            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
				
                float time = _FrameIdx;//floor(_Param.w)
                float startPix1 = _FrameIdx * _Param.z * 3 + v.tan.x * 3;
                float4 m11 = tex2Dlod(_MatrixTex, GetUV(startPix1));
                float4 m12 = tex2Dlod(_MatrixTex, GetUV(startPix1 + 1));
                float4 m13 = tex2Dlod(_MatrixTex, GetUV(startPix1 + 2));
                
                float4x4 m1 = float4x4
                (
                    m11, m12, m13, float4(0, 0, 0, 1)
                );

                float startPix2 = _FrameIdx * _Param.z * 3 + v.tan.z * 3;
                float4 m21 = tex2Dlod(_MatrixTex, GetUV(startPix2));
                float4 m22 = tex2Dlod(_MatrixTex, GetUV(startPix2 + 1));
                float4 m23 = tex2Dlod(_MatrixTex, GetUV(startPix2 + 2));
                float4x4 m2 = float4x4
                (
                    m21, m22, m23, float4(0, 0, 0, 1)
                );
                
                

                v.vertex = mul(m1, v.vertex) * v.tan.y + mul(m2, v.vertex) * v.tan.w;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.tan = v.tan;

                return o;
            }
            
            fixed4 frag(v2f i): SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;
                
                return col;
            }
            ENDCG
            
        }
    }
}
