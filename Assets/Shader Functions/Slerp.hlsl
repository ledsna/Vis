
float3 Slerp_float(float3 start, float3 end, float percent, out float result)
{
    float slerpDot = dot(start, end);
    slerpDot = clamp(slerpDot, -1.0, 1.0);
    float theta = acos(slerpDot) * percent;
    float3 RelativeVec = normalize(end - start * slerpDot);
    result = ((start * cos(theta)) + (RelativeVec * sin(theta)));
}