// sampler2D _LightDir;

uniform float4 _WorldSpaceLightPos0;

// sampler2D _MaskedDepthNormalsTexture;
SamplerState point_clamp_sampler;
float _Zoom;

float3 getNormal(float2 uv)
{
    return _MaskedDepthNormalsTexture.Sample(point_clamp_sampler, uv);
    // return tex2D(_CameraNormalTexture, uv).rgb;
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

float getMaskedDepth(float2 uv)
{
    return _CameraDepthTexture.Sample(point_clamp_sampler, uv) *
        (_MaskedDepthNormalsTexture.Sample(point_clamp_sampler, uv).a != 0);
    // return tex2D(_CameraDepthTexture, uv);
    // return _CameraDepthTexture.Sample(point_clamp_sampler, uv);
}

float getCameraDepth(float2 uv)
{
    return _CameraDepthTexture.Sample(point_clamp_sampler, uv);
}


void get_neighbour_uvs(float2 uv, float distance, out float2 neighbours[4])
{
    float2 pixel_size = 1. / (_ScreenParams.xy);
    neighbours[0] = uv + float2(0, pixel_size.y) * distance;
    neighbours[1] = uv - float2(0, pixel_size.y) * distance;
    neighbours[2] = uv + float2(pixel_size.x, 0) * distance;
    neighbours[3] = uv - float2(pixel_size.x, 0) * distance;
}

// float3 outline_color(float2 uv, float3 base_color)
// {
//     float depth = getMaskedDepth(uv);
//     if (depth == 0) return base_color;
//     float3 normal = getNormal(uv);
//     
//     float2 neighbour_depths[4];
//     float2 neighbour_normals[4];
//     
//     get_neighbour_uvs(uv, _DepthOutlineScale, neighbour_depths);
//     get_neighbour_uvs(uv, _NormalOutlineScale, neighbour_normals);
//     
//     float depth_diff_sum = 0.;
//
//     [unroll]
//     for (int d = 0; d < 4; d++)
//     {
//         float neighbour_depth = getMaskedDepth(neighbour_depths[d]);
//         // If the current pixel's neighbour shouldn't be outlined (depth 0)
//         // and the current pixel is further (depth < neighbour_depth),
//         // then produce no outline
//         if (neighbour_depth == 0 && depth < getCameraDepth(neighbour_depths[d]))
//             continue;
//         depth_diff_sum += depth - neighbour_depth;
//     }
//
//     float3 normal_edge_bias = normalize(float3(1, 1, 1));
//     float dotSum = 0.0;
//     [unroll]
//     for (int n = 0; n < 4; n++)
//     {
//         float3 neighbour_normal = getNormal(neighbour_normals[n]);
//         if (getMaskedDepth(neighbour_normals[n]) == 0) // && depth < getCameraDepth(neighbour_normals[n]))
//             continue;
//         float3 normal_diff = -normal + neighbour_normal;
//         float normal_diff_weight = smoothstep(-.01, .01, dot(normal_diff, normal_edge_bias));
//
//         dotSum += dot(normal_diff, normal_diff) * normal_diff_weight;
//     }
//     
//     float normal_edge = step(_NormalThreshold, sqrt(dotSum));
//     float depth_edge = step(_DepthThreshold / 10000., depth_diff_sum);
//
//     base_color = float3(0, 0, 0);
//
//     if (depth_diff_sum < 0.0)
//     {
//         return base_color;
//     }
//     if (depth_edge > 0.0)
//     {
//         // return lerp(base_color, base_color * (_ShadowPower - 1), depth_edge);
//         return lerp(base_color, float3(0, 0, 1), depth_edge);
//
//     }
//     // return lerp(base_color, base_color * DiffuseForView(normal), normal_edge);
//     return lerp(base_color, float3(1, 0, 0), normal_edge);
// }

float3 outline_color(float2 uv, float3 base_color)
{
    float depth = getMaskedDepth(uv);
    if (depth == 0) return base_color;
    float3 normal = getNormal(uv);
    
    float2 neighbour_depths[4];
    float2 neighbour_normals[4];
    
    get_neighbour_uvs(uv, _DepthOutlineScale, neighbour_depths);
    get_neighbour_uvs(uv, _NormalOutlineScale, neighbour_normals);
    
    float depth_diff_sum = 0.;

    [unroll]
    for (int d = 0; d < 4; d++)
    {
        float neighbour_depth = getCameraDepth(neighbour_depths[d]);
        depth_diff_sum += depth - neighbour_depth;
    }

    float3 normal_edge_bias = normalize(float3(1, 1, 1));
    float dotSum = 0.0;
    [unroll]
    for (int n = 0; n < 4; n++)
    {
        float3 neighbour_normal = getNormal(neighbour_normals[n]);
        float3 normal_diff = normal - neighbour_normal;
        float normal_diff_weight = smoothstep(-.01, .01, dot(normal_diff, normal_edge_bias));

        dotSum += dot(normal_diff, normal_diff) * normal_diff_weight;
    }
    
    float normal_edge = step(_NormalThreshold, sqrt(dotSum));
    float depth_edge = step(_DepthThreshold / 10000., depth_diff_sum);

    base_color = float3(0, 0, 0);

    if (depth_diff_sum < 0.0)
    {
        return base_color;
    }
    if (depth_edge > 0.0)
    {
        // return lerp(base_color, base_color * (_ShadowPower - 1), depth_edge);
        return lerp(base_color, float3(0, 0, 1), depth_edge);

    }
    // return lerp(base_color, base_color * DiffuseForView(normal), normal_edge);
    return lerp(base_color, float3(1, 0, 0), normal_edge);
}


#ifndef OUTLINE_GRAPH_INCLUDED
#define OUTLINE_GRAPH_INCLUDED
void GetOutline_float(float2 uv, float3 base_color, out float3 color) {
    color = outline_color(uv, base_color);
}
#endif





