#ifndef OUTLINE_DECIDER_INCLUDED
#define OUTLINE_DECIDER_INCLUDED

void OutlineDecider_float(float NormalEdge, float DepthEdge, float DepthDifference, float3 rgb, float HL, float SH,
    out float4 rgba)
{
    if (DepthDifference < 0)
    {
        rgba = float4(rgb, 0);
        return;
    }
    if (DepthEdge > 0.0)
    {
        rgba = float4(rgb * SH, DepthEdge);
        return;
    }
    rgba = float4(rgb * HL, NormalEdge);
}
#endif
