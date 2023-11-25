using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlinePass : ScriptableRenderPass
{
    private PixelizeFeature.CustomPassSettings settings;

    private RenderTargetIdentifier colorBuffer, pixelBuffer;
    private Material material;

    public OutlinePass()
    {
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler("Outline Pass")))
        { 
            Blit(cmd, colorBuffer, pixelBuffer, material);
            Blit(cmd, pixelBuffer, colorBuffer);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

}