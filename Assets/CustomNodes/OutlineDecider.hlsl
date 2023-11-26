#ifndef OUTLINE_DECIDER_INCLUDED
#define OUTLINE_DECIDER_INCLUDED

void OutlineDecider_float(float NormalEdge, float DepthEdge, float DepthDifference,
    float Rs, float Gs, float Bs,
    float HL, float SH,
    out float R, out float G, out float B, out float A) 
{
    R = Rs;
    G = Gs;
    B = Bs;
    if (DepthDifference < 0)
    {
        A = 0;
    }
    else
    {
        if (DepthEdge > 0.0)
        {
            R *= SH;
            G *= SH;
            B *= SH;
            A = DepthEdge;
        }
        else
        {
            R *= HL;
            G *= HL;
            B *= HL;
            A = NormalEdge;
        }
    }
}
#endif
