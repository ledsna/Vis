Shader "Hidden/OutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
    }
    SubShader
    {
        // No culling or depth
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
                float4 posWorld : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4x4 _CameraViewToWorld;
            sampler2D _MainTex;
            sampler2D _LightDir;
            sampler2D _CameraDepthTexture;
            sampler2D _CameraNormalTexture;
            float _DepthOutlineScale;
            float _NormalOutlineScale;
            float _DepthThreshold;
            float _NormalThreshold;
            float _HighlightPower;
            float _ShadowPower;
            
            float3 getNormal(float2 uv)
            {
                return tex2D(_CameraNormalTexture, uv).rgb;
            }

            float3 ViewNormalToWorld(float3 viewNormal) {
                return normalize(mul((float3x3)_CameraViewToWorld, float4(normalize(viewNormal * 2 - 1), 0)));
            }

            float DiffuseComponent(float3 worldNormal) {
                return dot(normalize(worldNormal), normalize(_WorldSpaceLightPos0.xyz));
            }

            float DiffuseForView(float3 viewNormal)
            {
                return DiffuseComponent(ViewNormalToWorld(viewNormal)) * (_HighlightPower - _ShadowPower) + _ShadowPower;
            }

            float getDepth(float2 uv)
            {
                return tex2D(_CameraDepthTexture, uv);
            }

           void get_neighbour_uvs(float2 uv, float2 pixel_size, float distance, out float2 neighbours[4])
            {
                // distance == outline scale
                neighbours[0] = uv + float2(0, pixel_size.y) * distance;
                neighbours[1] = uv - float2(0, pixel_size.y) * distance;
                neighbours[2] = uv + float2(pixel_size.x, 0) * distance;
                neighbours[3] = uv - float2(pixel_size.x, 0) * distance;
            }

            float4 debug_normal(float2 uv)
            {
                float2 uv_pixel_size = 1. / (_ScreenParams.xy - 2);
                float2 nduvs[4];
                get_neighbour_uvs(uv, uv_pixel_size, 1, nduvs);

                float3 normal_edge_bias = float3(1, 1, 1);
                float3 normal_diff = getNormal(uv) - getNormal(nduvs[3]);
                return float4(normal_diff, 1);
                float normal_bias_diff = dot(normal_diff, normal_edge_bias);
                float normal_indicator = smoothstep(-.01, .01, normal_bias_diff);

                float dotSum = dot(normal_diff, normal_diff) * normal_indicator;
                return float4(dotSum, dotSum, dotSum, 1);
            }
            
            #ifndef OUTLINE_INCLUDED
            #define OUTLINE_INCLUDED
            float4 outline_color(float2 uv, float4 color, v2f i)
            {
                // return float4(ViewNormalToWorld(getNormal(i.uv)), 1);
                float2 uv_pixel_size = 1. / (_ScreenParams.xy - 2);
                
                float depth = getDepth(uv);
                float3 normal = getNormal(uv);
                
                float depth_diff_sum = 0.;
                
                float2 nduvs[4];
                get_neighbour_uvs(uv, uv_pixel_size, _DepthOutlineScale, nduvs);
                float depths[4];

                [unroll]
                for (int d = 0; d < 4; d++)
                {
                    depths[d] = getDepth(nduvs[d]);
                    depth_diff_sum += depth - getDepth(nduvs[d]);
                }

                float depth_edge = step(_DepthThreshold / 10000., depth_diff_sum);
                

                float2 nnuvs[4];
                get_neighbour_uvs(uv, uv_pixel_size, _NormalOutlineScale, nnuvs);
                float3 normals[4];
                float3 normal_edge_bias = float3(1, 1, 1);
                float dotSum = 0.0;
                [unroll]
                for (int n = 0; n < 4; n++)
                {
                    normals[n] = getNormal(nnuvs[n]);
                    float3 normal_diff = normal - normals[n];
                    float normal_bias_diff = dot(normal_diff, normal_edge_bias);
                    float normal_indicator = smoothstep(-.01, .01, normal_bias_diff);

                    dotSum += dot(normal_diff, normal_diff) * normal_indicator;
                }
                float indicator = sqrt(dotSum);
                float normal_edge = step(_NormalThreshold, indicator);
                
                if (depth_diff_sum < 0.0)
                {
                    return float4(color.rgb, 1);
                }
                if (depth_edge > 0.0)
                {
                    return float4(lerp(color, color.rgb * (_ShadowPower - 1), depth_edge), 1);
                }
                return  float4(lerp(color, color.rgb * DiffuseForView(normal), normal_edge), 1);
            }
            #endif

            

            fixed4 frag (v2f i) : SV_Target
            {
                float lighting_level = DiffuseComponent(getNormal(i.uv));
                fixed4 color = tex2D(_MainTex, i.uv);
                color = outline_color(i.uv, color, i);
                return color;
                // fixed4 out_col = 
                // return fixed4(out_col.xyz, 1);
                // return debug_normal(i.uv);
                // // float4 outl = outline_color(i.uv) * lighting_level;
                // float4 outl = lighting_level;
                // // return lerp(col, outl, outl.a);
                // return color;
                float4 outl_col = outline_color(i.uv, color, i);
                
                // return float4((tex * ndotl).xyz, 1); // Modulating texture color by light intensity
                return outl_col;
            }
            ENDCG
        }
    }
}
