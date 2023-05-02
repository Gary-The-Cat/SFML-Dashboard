using SFML.Graphics;
using Shared.Interfaces.Services;

namespace Shared.Services
{
    public class RenderInstanceService : IRenderInstanceService
    {
        public RenderInstanceService(RenderTarget target)
        {
            this.RenderTarget = target;
        }

        public RenderTarget RenderTarget { get; }
    }
}
