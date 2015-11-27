﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ZooBurst.Core.Input
{
    public class MouseEventArgs : EventArgs
    {
        internal MouseEventArgs(TimeSpan time, MouseState previousState, MouseState currentState, MouseButton button = MouseButton.None)
        {
            PreviousState = previousState;
            CurrentState = currentState;
            Position = new Point(currentState.X, currentState.Y);
            Button = button;
            ScrollWheelValue = currentState.ScrollWheelValue;
            ScrollWheelDelta = currentState.ScrollWheelValue - previousState.ScrollWheelValue;
            Time = time;
        }

        internal TimeSpan Time { get; private set; }

        public MouseState PreviousState { get; private set; }
        public MouseState CurrentState { get; private set; }
        public Point Position { get; private set; }
        public MouseButton Button { get; private set; }
        public int ScrollWheelValue { get; private set; }
        public int ScrollWheelDelta { get; private set; }
    }
}
