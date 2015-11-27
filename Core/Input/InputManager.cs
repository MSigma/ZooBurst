using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XMouse = Microsoft.Xna.Framework.Input.Mouse;
using XKeyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace ZooBurst.Core.Input
{
    public static class InputManager
    {
        public static bool UserInputEnabled { get; set; } = true;

        public static bool KeyboardEnabled
        {
            get { return (UserInputEnabled && _keyboardEnabled); }
            set { _keyboardEnabled = value; }
        }
        private static bool _keyboardEnabled = true;

        public static bool MouseEnabled
        {
            get { return (UserInputEnabled && _mouseEnabled); }
            set { _mouseEnabled = value; }
        }
        private static bool _mouseEnabled = true;

        public static EventsContainer Events => _events;
        internal static EventsContainer _events;

        private static readonly InputListenerManager _input;
        private static Vector2 _previousMousePosition;

        private static readonly Dictionary<Keys, bool> _handledKeys;
        private static readonly Dictionary<MouseButton, bool> _handledMouseButtons;

        static InputManager()
        {
            _input = new InputListenerManager();
            _events = new EventsContainer(_input);

            _handledKeys = new Dictionary<Keys, bool>();
            _handledMouseButtons = new Dictionary<MouseButton, bool>();
        }

        public static void Update(GameTime gameTime)
        {
            if (KeyboardEnabled)
                _input.Update(gameTime);

            _previousMousePosition = new Vector2(Mouse.X, Mouse.Y);
            MousePrevious = Mouse;
            KeyboardPrevious = Keyboard;

            Mouse = XMouse.GetState();
            Keyboard = XKeyboard.GetState();

            _handledKeys.Clear();
            _handledMouseButtons.Clear();
        }

        public static bool IsHandled(Keys key)
        {
            return _handledKeys.ContainsKey(key) && _handledKeys[key];
        }

        public static void SetHandled(Keys key, bool handled = true)
        {
            _handledKeys[key] = handled;
        }

        public static bool IsHandled(MouseButton button)
        {
            return _handledMouseButtons.ContainsKey(button) && _handledMouseButtons[button];
        }

        public static void SetHandled(MouseButton button, bool handled = true)
        {
            _handledMouseButtons[button] = handled;
        }

        public static Vector2 MousePosition => new Vector2(Mouse.X, Mouse.Y);
        public static Vector2 MouseDelta => new Vector2(_previousMousePosition.X - Mouse.X, _previousMousePosition.Y - Mouse.Y);
        public static bool MouseHasMoved => (MouseDelta != Vector2.Zero);
        public static Vector2 MouseVector => new Vector2((float)Math.Cos(MouseAngle), -(float)Math.Sin(MouseAngle));
        public static float MouseAngle => (float)Math.Atan2(Mouse.X - MousePrevious.X, Mouse.Y - MousePrevious.Y);
        public static Rectangle MouseRectangle => new Rectangle(Mouse.X, Mouse.Y, 1, 1);

        public static MouseState Mouse
        {
            get;
            private set;
        }

        public static MouseState MousePrevious
        {
            get;
            private set;
        }

        public static KeyboardState Keyboard
        {
            get;
            private set;
        }

        public static KeyboardState KeyboardPrevious
        {
            get;
            private set;
        }

        public static bool CapsLock => Console.CapsLock;

        public static bool MouseReleased(MouseButton mb)
        {
            return MouseEnabled && ((mb == MouseButton.Left && (Mouse.LeftButton == ButtonState.Released)) ||
                (mb == MouseButton.Middle && (Mouse.MiddleButton == ButtonState.Released)) ||
                (mb == MouseButton.Right && (Mouse.RightButton == ButtonState.Released)));
        }

        public static bool MouseOver(int x, int y, int width, int height) => (MouseRectangle.Intersects(new Rectangle(x, y, width, height)));
        public static bool MouseOver(Rectangle rectangle) => (MouseRectangle.Intersects(rectangle));

        public static bool HasScrolled(ScrollDirection dir)
        {
            return (Mouse.ScrollWheelValue < MousePrevious.ScrollWheelValue && dir == ScrollDirection.Up
                || Mouse.ScrollWheelValue > MousePrevious.ScrollWheelValue && dir == ScrollDirection.Down);
        }

        public static int GetScrollDelta() => (Mouse.ScrollWheelValue - MousePrevious.ScrollWheelValue);

        public static int GetNumberPressed()
        {
            for (var i = 0; i < 9; i++)
                if (KeyPressed((Keys)Enum.Parse(typeof(Keys), "D" + i)) || KeyPressed((Keys)Enum.Parse(typeof(Keys), "NumPad" + i)))
                    return i;

            return -1;
        }

        public static bool KeyHeld(Keys key) => KeyboardEnabled && !_handledKeys.ContainsKey(key) && Keyboard.IsKeyDown(key);
        public static bool KeyUp(Keys key) => (KeyboardEnabled && Keyboard.IsKeyUp(key));
        public static bool KeyPressed(Keys key) => KeyboardEnabled && !_handledKeys.ContainsKey(key) && (Keyboard.IsKeyDown(key) && KeyboardPrevious.IsKeyUp(key));
        public static bool KeyReleased(Keys key) => (KeyboardEnabled && (KeyboardPrevious.IsKeyDown(key) && Keyboard.IsKeyUp(key)));

        public static bool MouseHeld(MouseButton mb)
        {
            if (!MouseEnabled || IsHandled(mb))
                return false;

            if ((mb != MouseButton.Left || (Mouse.LeftButton != ButtonState.Pressed || MousePrevious.LeftButton != ButtonState.Pressed)) &&
                (mb != MouseButton.Middle || (Mouse.MiddleButton != ButtonState.Pressed || MousePrevious.MiddleButton != ButtonState.Pressed)) &&
                (mb != MouseButton.Right || (Mouse.RightButton != ButtonState.Pressed || MousePrevious.RightButton != ButtonState.Pressed)))
                return false;

            SetHandled(mb);
            return true;
        }

        public static bool AreaHeld(MouseButton mb, Rectangle rectangle) => MouseEnabled && !IsHandled(mb) && (MouseOver(rectangle) && MouseHeld(mb));

        /// <summary>
        /// Returns true if the mouse has clicked inside the defined rectangle.
        /// </summary>
        public static bool AreaClicked(MouseButton mb, Rectangle rectangle) => MouseEnabled && !IsHandled(mb) && (MouseOver(rectangle) && MouseClicked(mb));

        public static bool MouseClicked(MouseButton mb)
        {
            if (!MouseEnabled || IsHandled(mb) || (mb != MouseButton.Left || (Mouse.LeftButton != ButtonState.Pressed || MousePrevious.LeftButton != ButtonState.Released)) &&
                (mb != MouseButton.Middle || (Mouse.MiddleButton != ButtonState.Pressed || MousePrevious.MiddleButton != ButtonState.Released)) &&
                (mb != MouseButton.Right || (Mouse.RightButton != ButtonState.Pressed || MousePrevious.RightButton != ButtonState.Released)))
                return false;

            SetHandled(mb);
            return true;
        }

        public class EventsContainer
        {
            private readonly KeyboardListener _keyboard;
            private readonly MouseListener _mouse;

            internal EventsContainer(InputListenerManager input)
            {
                _keyboard = input.AddListener<KeyboardListener>();
                _mouse = input.AddListener<MouseListener>();
            }

            public event EventHandler<KeyboardEventArgs> KeyPressed
            {
                add { _keyboard.KeyPressed += value; }
                remove { _keyboard.KeyPressed -= value; }
            }

            public event EventHandler<KeyboardEventArgs> KeyReleased
            {
                add { _keyboard.KeyReleased += value; }
                remove { _keyboard.KeyReleased -= value; }
            }

            public event EventHandler<KeyboardEventArgs> KeyTyped
            {
                add { _keyboard.KeyTyped += value; }
                remove { _keyboard.KeyTyped -= value; }
            }

            public event EventHandler<MouseEventArgs> MouseClicked
            {
                add { _mouse.MouseClicked += value; }
                remove { _mouse.MouseClicked -= value; }
            }

            public event EventHandler<MouseEventArgs> MouseDoubleClicked
            {
                add { _mouse.MouseDoubleClicked += value; }
                remove { _mouse.MouseDoubleClicked -= value; }
            }

            public event EventHandler<MouseEventArgs> MouseDown
            {
                add { _mouse.MouseDown += value; }
                remove { _mouse.MouseDown -= value; }
            }

            public event EventHandler<MouseEventArgs> MouseDragged
            {
                add { _mouse.MouseDragged += value; }
                remove { _mouse.MouseDragged -= value; }
            }

            public event EventHandler<MouseEventArgs> MouseMoved
            {
                add { _mouse.MouseMoved += value; }
                remove { _mouse.MouseMoved -= value; }
            }

            public event EventHandler<MouseEventArgs> MouseUp
            {
                add { _mouse.MouseUp += value; }
                remove { _mouse.MouseUp -= value; }
            }

            public event EventHandler<MouseEventArgs> MouseWheelMoved
            {
                add { _mouse.MouseWheelMoved += value; }
                remove { _mouse.MouseWheelMoved -= value; }
            }
        }
    }
}
