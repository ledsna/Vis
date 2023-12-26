#ifndef FRESNEL_DECIDER_INCLUDED
#define FRESNEL_DECIDER_INCLUDED

void FresnelClamper_float(float ramps, float shade, out float result)
{
    for (int i = 0; i < ramps; i++)
    {
        if (shade >= (i + 1) / ramps)
        {
            if (i == ramps - 1)
            {
                result = 1;
                return;
            }
            continue;
        }
        result = i / ramps;
        return;
    }
	result = 1;
}
#endif
