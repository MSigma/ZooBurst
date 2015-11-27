using System;
using ZooBurst.Core;

namespace ZooBurst.View
{
    public interface IInputHandler
    {
        Action<Swap> Swap { get; set; }
    }
}