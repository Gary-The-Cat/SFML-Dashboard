using RoverSimulation.DataStructures;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Events.CallbackArgs;
using Shared.Events.EventArgs;
using Shared.ExtensionMethods;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using Shared.Menus;
using Shared.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RoverSimulation.Screens
{
    public class RoverSimulationHudScreen : Screen
    {
        private List<Button> buttons;
        private SharedRoverData roverData;
        private INotificationService notificationService;
        private Text goldCollectedText;
        private Text fuelCollectedText;

        public RoverSimulationHudScreen(
            SharedRoverData roverData,
            IApplicationManager applicationManager,
            IEventService eventService,
            INotificationService notificationService)
        {
            this.notificationService = notificationService;
            this.roverData = roverData;

            eventService.RegisterMouseClickCallback(
                this.Id,
                new MouseClickCallbackEventArgs(Mouse.Button.Left),
                OnMousePress);

            buttons = new List<Button>();

            buttons.Add(new Button("Load Result", new Vector2f(20, 20), () =>
            {
                LoadResult();

            }, HorizontalAlignment.Left));

            var windowSize = applicationManager.GetWindowSize();

            goldCollectedText = new Text($"Gold: {roverData.Gold}", new Font("Resources\\font.ttf"))
            {
                Position = new Vector2f(windowSize.X / 2, 50),
                FillColor = Color.White
            };

            fuelCollectedText = new Text($"Fuel: {roverData.Fuel}", new Font("Resources\\font.ttf"))
            {
                Position = new Vector2f(windowSize.X - 200, 50),
                FillColor = Color.White
            };
        }

        private void LoadResult()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var map = Path.Combine(currentDirectory, "Map.txt");
            var path = Path.Combine(currentDirectory, "Path.txt");

            if (!File.Exists(map))
            {
                notificationService.ShowToast(
                           ToastType.Error,
                           $"Could not find file: {map}");

                return;
            }

            if (!File.Exists(path))
            {
                notificationService.ShowToast(
                           ToastType.Error,
                           $"Could not find file: {path}");

                return;
            }

            roverData.Gold = 0;
            roverData.Fuel = 0;

            var mapText = File.ReadAllText(map);
            var pathText = File.ReadAllText(path);

            (float[,] fuel, float[,] gold) = LoadFuelGoldValues(mapText);
            List<Vector2u> pathValue = LoadPath(pathText);
            roverData.StartNewSimulation(fuel, gold, pathValue);

            notificationService.ShowToast(
                       ToastType.Successful,
                       "Result load successful");

            notificationService.ShowToast(
                ToastType.Info,
                "Beginning Simulation...");
        }

        private List<Vector2u> LoadPath(string pathText)
        {
            pathText = pathText.Replace("(", "").Replace(")", "");
            var positions = pathText.Split("\n").Select(c => c.Split(','));

            return positions.Select(p => new Vector2u(uint.Parse(p[0]), uint.Parse(p[1]))).ToList();
        }

        private (float[,] fuel, float[,] gold) LoadFuelGoldValues(string mapText)
        {
            var lines = mapText.Split("\n").ToArray();
            var first = lines.First().Trim();

            var yCount = lines.Count();
            var xCount = first.Split(')').Where(s => !string.IsNullOrEmpty(s)).Count();

            var fuel = new float[xCount, yCount];
            var gold = new float[xCount, yCount];

            for (int y = 0; y < yCount; y++)
            {
                var line = lines[y];
                var reducedString = line.Replace('(', ' ');
                var values = reducedString.Split(')').Where(s => !string.IsNullOrEmpty(s)).ToArray(); ;


                for (int x = 0; x < xCount; x++)
                {
                    var value = values[x];
                    var xy = value.Split(',');
                    var fuelValue = float.Parse(xy[0]);
                    var goldValue = float.Parse(xy[1]);

                    fuel[x, y] = fuelValue;
                    gold[x, y] = goldValue;
                }
            }

            return (fuel, gold);
        }

        public override void OnUpdate(float deltaT)
        {
            goldCollectedText.DisplayedString = $"Gold: {roverData.Gold.ToString("0.00")}";
            fuelCollectedText.DisplayedString = $"Fuel: {roverData.Fuel.ToString("0.00")}";
        }

        public override void OnRender(RenderTarget target)
        {
            buttons.ForEach(b => b.OnRender(target));

            target.DrawString(goldCollectedText);
            target.DrawString(fuelCollectedText);
        }

        private void OnMousePress(MouseClickEventArgs args)
        {
            buttons.ForEach(b => b.TryClick(args));
        }
    }
}
