using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ZooBurst.Utils;
using ZooBurst.Core;
using ZooBurst.Core.Input;

namespace ZooBurst.View
{
    public class MouseInputHandler : IInputHandler
    {
        public Controller Controller { get; }
        public GameView View { get; }
        public Action<Swap> Swap { get; set;  }

        private Point? _dragStart;

        public MouseInputHandler(Controller controller, GameView view)
        {
            Controller = controller;
            View = view;

            InputManager.Events.MouseDown += Events_MouseDown;
            InputManager.Events.MouseMoved += Events_MouseMoved;
            InputManager.Events.MouseUp += Events_MouseUp;
            InputManager.Events.KeyPressed += Events_KeyPressed;
        }

        private void Events_KeyPressed(object sender, KeyboardEventArgs e)
        {
            if (e.Key != Keys.Enter || Controller.State == PlayState.Playing)
                return;

            Controller.Restart();
        }

        private void Events_MouseUp(object sender, MouseEventArgs e)
        {
            if (!InputManager.MouseEnabled)
                return;

            if (View.SelectedSprite.Parent != null && _dragStart.HasValue)
                View.HideSelection();

            _dragStart = null;
        }

        private void Events_MouseDown(object sender, MouseEventArgs e)
        {
            if (!InputManager.MouseEnabled || e.Button != MouseButton.Left || _dragStart != null)
                return;

            _dragStart = null;
            var tile = new Point((e.Position.X - (int)View.AnimalLayer.Position.X) / View.TileSize.X, (e.Position.Y - (int)View.AnimalLayer.Position.Y) / View.TileSize.Y);

            if (!Controller.Level.Contains(tile))
                return;

            var animal = Controller.Level.GetAnimalAt(tile.X, tile.Y);
            if (animal == null)
                return;

            _dragStart = new Point(tile.X, tile.Y);
            View.ShowSelection(animal);
        }

        private void Events_MouseMoved(object sender, MouseEventArgs e)
        {
            if (!InputManager.MouseEnabled || !_dragStart.HasValue)
                return;

            var tile = new Point((e.Position.X - (int)View.AnimalLayer.Position.X) / View.TileSize.X, (e.Position.Y - (int)View.AnimalLayer.Position.Y) / View.TileSize.Y);

            if (!Controller.Level.Contains(tile))
                return;

            var delta = tile.GetDirectionVector(_dragStart.Value);
            if (delta == Point.Zero)
                return;

            var toPoint = _dragStart.Value + delta;
            if (!Controller.Level.Contains(toPoint))
                return;

            var to = Controller.Level.GetAnimalAt(toPoint.X, toPoint.Y);
            if (to == null)
                return;

            var from = Controller.Level.GetAnimalAt(_dragStart.Value.X, _dragStart.Value.Y);
            if (from == null)
                return;

            Swap?.Invoke(new Swap(from, to));
            View.HideSelection();
            _dragStart = null;
        }
    }
}
