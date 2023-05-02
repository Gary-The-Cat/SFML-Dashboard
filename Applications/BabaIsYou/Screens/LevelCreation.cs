using BabaIsYou.DataStructures;
using BabaIsYou.ECS.Components;
using BabaIsYou.Resources;
using BabaIsYou.UiElements;
using CSharpFunctionalExtensions;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Events.EventArgs;
using Shared.ExtensionMethods;
using Shared.Infrastructure;
using Shared.Interfaces.Services;
using Shared.Menus;
using Shared.Notifications;

namespace BabaIsYou.Screens
{
    public class LevelCreation : Screen
    {
        private WeakReference<INotificationService> notificationServiceReference;
        private WeakReference<IIoService> ioServiceReference;

        // Level Grid
        private GridVisual gridVisual;

        // Current node selection
        private Maybe<GridNodeVisual> mouseOverNode;
        private NodeSelectionVisual nodeSelectionVisual;
        private List<GridNodeVisual> nodes = new List<GridNodeVisual>();
        private List<Button> buttons;
        private Vector2f screenSize = new Vector2f(1920, 1080);
        private bool isLeftMouseButtonPressed;

        public LevelCreation(
            INotificationService notificationService,
            IIoService ioService,
            IEventService eventService,
            GridConfiguration gridConfiguration)
        {
            this.notificationServiceReference = new WeakReference<INotificationService>(notificationService);
            this.ioServiceReference = new WeakReference<IIoService>(ioService);

            SetGridSize(gridConfiguration.GridIndexSize);

            // Load the available nodes
            var nodeTypes = NodeTypes.GetAllNodes();
            var selectionVisualResult = NodeSelectionVisual.Create(nodeTypes, new Vector2f(1820, 100 - 64));

            if (selectionVisualResult.HasValue)
            {
                nodeSelectionVisual = selectionVisualResult.Value;
            }

            buttons = new List<Button>()
            {
                new Button("Export", screenSize - new Vector2f(70, 52), OnExportPress),
                new Button("Clear", screenSize - new Vector2f(200, 50), OnClearPress),
                new Button("Load", new Vector2f(70, screenSize.Y - 50), OnLoadLevelPress),
                new Button("[+X]", new Vector2f(140, 10), OnAddXRowPress),
                new Button("[-X]", new Vector2f(50, 10), OnRemoveXRowPress),
                new Button("[+Y]", new Vector2f(140, 80), OnAddYRowPress),
                new Button("[-Y]", new Vector2f(50, 80), OnRemoveYRowPress),
            };

            eventService.RegisterMouseMoveCallback(this.Id, OnMouseMove);
            eventService.RegisterKeyboardCallback(this.Id, Keyboard.Key.Left, OnKeyPress);
            eventService.RegisterKeyboardCallback(this.Id, Keyboard.Key.Right, OnKeyPress);
            eventService.RegisterMouseWheelScrollCallback(this.Id, OnMouseScroll);
            eventService.RegisterMouseClickCallback(this.Id, Mouse.Button.Left, OnLeftMousePress);
            eventService.RegisterMouseClickReleaseCallback(this.Id, Mouse.Button.Left, OnLeftMouseRelease);
            eventService.RegisterMouseClickCallback(this.Id, Mouse.Button.Right, OnRightMousePress);
        }

        public IIoService IoService => this.ioServiceReference.Value();

        public INotificationService NotificationService => this.notificationServiceReference.Value();

        private async Task OnLoadLevelPress()
        {
            var levelResult =
                await IoService.SelectFile()
                .Bind(levelFileResult => IoService.TryLoadCsvLines(levelFileResult)
                .Bind(levelFileData => LevelData.CreateFromCsv(levelFileData)));

            if (levelResult.IsFailure)
            {
                NotificationService.ShowToast(ToastType.Error, levelResult.Error);
                return;
            }

            foreach (var node in levelResult.Value.Nodes)
            {
                var screenPosition = gridVisual.GetCellTopLeftFromIndex(node.IndexPosition);

                if (!screenPosition.HasValue) continue;

                var nodeVisual = NodeTypes.NodeTypeToVisualMap[node.NodeType]();
                var gridNodeVisual = GridNodeVisual.FromNodeVisual(
                    nodeVisual,
                    node.IndexPosition);
                gridNodeVisual.SetPosition(screenPosition.Value);
 
                nodes.Add(gridNodeVisual);
            }
        }

        private async Task OnExportPress()
        {
            var levelNodes = new List<LevelNode>();
            foreach (var node in nodes)
            {
                levelNodes.Add(new LevelNode()
                {
                    Direction = Enums.Direction.Right,
                    NodeType = node.NodeType,
                    IndexPosition = node.GridIndex,
                });
            }
            var levelResult = LevelData.CreateFromLevelContents(gridVisual.GridConfiguration, levelNodes);

            if (levelResult.IsFailure)
            {
                this.NotificationService.ShowToast(
                    ToastType.Error,
                    levelResult.Error);

                return;
            }

            var levelFileText = LevelData.CreateLevelFile(levelResult.Value);

            var writeResult = await IoService.TryWriteLines(@"C:\dev\BabaLevels\level.csv", levelFileText);

            if (writeResult.IsSuccess)
            {
                this.NotificationService.ShowToast(
                    ToastType.Successful,
                    $"Successfully Exported to C:\\dev\\BabaLevels\\level.csv");
            }
            else
            {
                this.NotificationService.ShowToast(
                    ToastType.Error,
                    writeResult.Error);
            }
        }

        private void OnClearPress()
        {
            nodes.Clear();
        }

        private void OnRemoveXRowPress()
        {
            OnClearPress();

            var currentsize = gridVisual.GridConfiguration.GridIndexSize;
            if (currentsize.X > 1)
            {
                SetGridSize(new Vector2u(currentsize.X - 2, currentsize.Y));
            }
        }

        private void OnRemoveYRowPress()
        {
            OnClearPress();

            var currentsize = gridVisual.GridConfiguration.GridIndexSize;
            if (currentsize.Y > 1)
            {
                SetGridSize(new Vector2u(currentsize.X, currentsize.Y - 2));
            }
        }

        private void OnAddXRowPress()
        {
            OnClearPress();

            var currentsize = gridVisual.GridConfiguration.GridIndexSize;
            SetGridSize(new Vector2u(currentsize.X + 2, currentsize.Y));
        }

        private void OnAddYRowPress()
        {
            OnClearPress();

            var currentsize = gridVisual.GridConfiguration.GridIndexSize;
            SetGridSize(new Vector2u(currentsize.X, currentsize.Y + 2));
        }

        private void SetGridSize(Vector2u newGridSize)
        {
            var gridConfiguration = new GridConfiguration()
            {
                CellHeight = 64,
                CellWidth = 64,
                GridIndexSize = newGridSize
            };

            this.gridVisual = GridVisual.CreateFromGrid(new Grid(gridConfiguration));
            this.gridVisual.SetCentre(screenSize / 2);
            this.gridVisual.IsDebug = true;
        }

        private void OnLeftMousePress(MouseClickEventArgs obj)
        {
            this.isLeftMouseButtonPressed = true;

            buttons.ForEach(button => button.TryClick(obj));

            this.SetNodeInSelectedCell();
        }

        private void SetNodeInSelectedCell()
        {
            mouseOverNode.Match(
                some =>
                {
                    nodes.RemoveAll(x => x.GridIndex == some.GridIndex);
                    nodes.Add(some);
                },
                NoAction);
        }

        private void OnLeftMouseRelease(MouseClickEventArgs obj) => this.isLeftMouseButtonPressed = false;

        // If the mouse is over a node, remove it from the map
        private void OnRightMousePress(MouseClickEventArgs obj)=> mouseOverNode.Match(
            node => nodes.RemoveAll(x => x.GridIndex == node.GridIndex),
            NoAction);

        private void OnMouseScroll(MouseWheelScrolledEventArgs obj)
        {
            this.nodeSelectionVisual.OnKeyPress(obj.Args.Delta > 0 ? Keyboard.Key.Left : Keyboard.Key.Right);
            
            var cell = gridVisual.GetCell(new Vector2i(obj.Args.X, obj.Args.Y));

            cell.Match(
                some => mouseOverNode = GridNodeVisual.FromNodeVisual(nodeSelectionVisual.SelectedVisual, some),
                NoAction);
        }

        private void OnKeyPress(KeyboardEventArgs obj) => this.nodeSelectionVisual.OnKeyPress(obj.Args.Code);

        private void OnMouseMove(MoveMouseEventArgs obj)
        {
            var cell = gridVisual.GetCell(new Vector2i(obj.Args.X, obj.Args.Y));

            cell.Match(
                some =>
                {
                    mouseOverNode = GridNodeVisual.FromNodeVisual(nodeSelectionVisual.SelectedVisual, some);
                    if (this.isLeftMouseButtonPressed)
                    {
                        this.SetNodeInSelectedCell();
                    }
                },
                () => mouseOverNode = Maybe<GridNodeVisual>.None);
        }

        public override void OnUpdate(float deltaT)
        {
            if(mouseOverNode.HasValue)
            {
                var mouseOverNodeValue = mouseOverNode.Value;
                var positionResult = gridVisual.GetCellTopLeftFromIndex(mouseOverNodeValue.GridIndex);

                if (positionResult.HasValue)
                {
                    mouseOverNodeValue.SetPosition(positionResult.Value);
                }
            }
        }

        public override void OnRender(RenderTarget target)
        {
            target.Clear(new Color(0x12, 0x18, 0x21));

            target.Draw(gridVisual);
            target.Draw(nodeSelectionVisual);

            buttons.ForEach(b => target.Draw(b));

            foreach (var node in nodes)
            {
                target.Draw(node);
            }

            mouseOverNode.Match(some => target.Draw(some), NoAction);
        }
    }
}
