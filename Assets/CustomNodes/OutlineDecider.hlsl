#ifndef OUTLINE_DECIDER_INCLUDED
#define OUTLINE_DECIDER_INCLUDED

void OutlineDecider_float(float NormalEdge, float DepthEdge, float DepthDifference,
    float Rs, float Gs, float Bs, 
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
            R *= 0.5;
            G *= 0.5;
            B *= 0.5;
            A = DepthEdge;
        }
        else
        {
            R *= 1.5;
            G *= 1.5;
            B *= 1.5;
            A = NormalEdge;
        }
    }
}
#endif
