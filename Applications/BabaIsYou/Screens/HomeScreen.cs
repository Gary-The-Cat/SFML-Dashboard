using BabaIsYou.DataStructures;
using BabaIsYou.Resources;
using BabaIsYou.Screens.Levels;
using BabaIsYou.Screens.LevelSelect;
using CSharpFunctionalExtensions;
using Ninject;
using Ninject.Parameters;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Events.EventArgs;
using Shared.ExtensionMethods;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using Shared.Notifications;
using static SFML.Window.Mouse;
using Button = Shared.Menus.Button;

namespace BabaIsYou.Screens
{
    public class HomeScreen : Screen
    {
        private WeakReference<IKernel> kernelReference;
        private WeakReference<IIoService> ioServiceReference;
        private WeakReference<IApplicationManager> appManagerReference;
        private WeakReference<INotificationService> notificationServiceReference;

        private List<Shared.Menus.Button> buttons;

        public HomeScreen(
            IKernel kernel,
            IIoService ioService,
            IApplicationManager appManager,
            INotificationService notificationService)
        {
            this.kernelReference = new WeakReference<IKernel>(kernel);
            this.ioServiceReference = new WeakReference<IIoService>(ioService);
            this.appManagerReference = new WeakReference<IApplicationManager>(appManager);
            this.notificationServiceReference =
                new WeakReference<INotificationService>(notificationService);
        }

        private IKernel Kernel => kernelReference.Value();
        private IIoService IoService => ioServiceReference.Value();
        private IApplicationManager AppManager => appManagerReference.Value();
        private INotificationService NotificationService => notificationServiceReference.Value();

        // The screen has been loaded & is about to show.
        public override void InitializeScreen()
        {
            base.InitializeScreen();

            this.buttons = new List<Shared.Menus.Button>();

            this.PopulateMenuButtons();

            var eventService = Kernel.Get<IEventService>();

            eventService.RegisterMouseClickCallback(
                this.Id,
                new Shared.Events.CallbackArgs.MouseClickCallbackEventArgs(SFML.Window.Mouse.Button.Left),
                eventArgs => this.OnMouseClick(eventArgs));
        }

        private void OnMouseClick(MouseClickEventArgs eventArgs)
        {
            foreach (var button in this.buttons)
            {
                button.TryClick(eventArgs);
            }
        }

        public override void OnRender(RenderTarget target)
        {
            target.Clear(new Color(0x1e, 0x1e, 0x1e));

            foreach (var button in this.buttons)
            {
                target.Draw(button);
            }
        }

        private void PopulateMenuButtons()
        {
            var appManager = this.Kernel.Get<IApplicationManager>();

            var windowSize = appManager.GetWindowSize();
            var centre = new Vector2f(windowSize.X / 2, windowSize.Y / 2);
            var playPosition = centre - new Vector2f(0, 80);
            var loadLevelPosition = centre + new Vector2f(0, 80);
            var exitPosition = centre + new Vector2f(0, 160);

            buttons.Add(new Button("Let's Play", centre, LevelSelect));
            buttons.Add(new Button("Load Level", loadLevelPosition, LoadLevel));
            buttons.Add(new Button("Baba Maker", playPosition, LevelCreate));
            buttons.Add(new Button("Quit", exitPosition, ExitGame));
        }

        private void LevelSelect()
        {
            var appManager = this.Kernel.Get<IApplicationManager>();
            var worldScreen = this.Kernel.Get<WorldScreen>();
            appManager.AddChildScreen(worldScreen);
        }

        private async Task LoadLevel()
        {
            var levelScreenResult =
                await IoService.SelectFile()
                .Bind(levelFileResult => IoService.TryLoadCsvLines(levelFileResult)
                .Bind(levelFileData => LevelData.CreateFromCsv(levelFileData))
                .Bind(levelResult => LevelScreen.TryCreateLevelFromData(levelResult)));

            if (levelScreenResult.IsFailure)
            {
                NotificationService.ShowToast(ToastType.Error, levelScreenResult.Error);
                return;
            }

            this.AppManager.AddChildScreen(levelScreenResult.Value);
        }

        private void LevelCreate()
        {
            var appManager = this.Kernel.Get<IApplicationManager>();
            var gridConfiguration = new GridConfiguration()
            {
                GridIndexSize = new Vector2u(11, 11),
                CellHeight = 64,
                CellWidth = 64,
            };

            var levelCreationScreen = this.Kernel.Get<LevelCreation>(
                new ConstructorArgument("gridConfiguration", gridConfiguration));
            appManager.AddChildScreen(levelCreationScreen);
        }

        private void ExitGame()
        {
            Environment.Exit(0);
        }
    }
}
