using BabaIsYou.Screens;
using Ninject;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;
using Shared.Interfaces.Services;

namespace BabaIsYou
{
    public class BabaIsYouInstance : ApplicationInstanceBase, IApplicationInstance
    {
        public string DisplayName => "Baba Is You";

        private IApplicationService appService;

        public BabaIsYouInstance(IApplicationService appService)
        {
            this.appService = appService;

            var texture = new Texture(new Image("Resources//BabaIsYouIcon.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public new void Initialize()
        {
            var homeScreen = appService.Kernel.Get<HomeScreen>();
            AddChildScreen(homeScreen);

            base.Initialize();
        }
    }
}