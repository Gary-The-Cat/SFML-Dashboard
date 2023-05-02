using Ninject;
using SFML.System;
using Shared.Core;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using SplineDemo.Screens;
using System.Collections.Generic;

namespace SplineDemo
{
    public class SplineDemoInstance : ApplicationInstanceBase, IApplicationInstance
    {
        private IApplicationService appService;

        public SplineDemoInstance(IApplicationService appService)
        {
            this.appService = appService;
        }

        public string DisplayName => "Spline Demo";

        public new void Initialize()
        {
            var orToolsDemoScreen = appService.Kernel.Get<SplineDemoScreen>();
            AddChildScreen(orToolsDemoScreen);

            base.Initialize();
        }
    }
}
