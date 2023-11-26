#ifndef MAX_ALPHA_COLOR_INCLUDED
#define MAX_ALPHA_COLOR_INCLUDED

void MaxAlphaColor_float(float R1, float G1, float B1, float A1,
                         float R2, float G2, float B2, float A2,
                         out float R, out float G, out float B, out float A) 
{
    if (A1 > A2)
    {
        if (A1 != 0)
        {
        	R = R1;
        	G = G1;
        	B = B1;
            A = 1;
        }
        else
        {
            A = 0;
        }
    }
    else
    {
        if (A2 != 0)
        {
        	R = R2;
        	G = G2;
        	B = B2;
            A = 1;
        }
        else
        {
            A = 0;
        }
    }
}
#endif
