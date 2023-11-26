#ifndef OUTLINE_DECIDER_INCLUDED
#define OUTLINE_DECIDER_INCLUDED

void OutlineDecider_float(float NormalEdge, float DepthEdge, float DepthDifference,
    out float R, out float G, out float B, out float A) 
{
    R = 0;
    G = 0;
    B = 0;
    if (DepthDifference < 0)
    {
        A = 0;
    }
    else
    {
        if (DepthEdge > 0.0)
        {
            A = DepthEdge;
        }
        else
        {
            R = 1;
            G = 1;
            B = 1;
            A = NormalEdge;
        }
    }
}
#endif
