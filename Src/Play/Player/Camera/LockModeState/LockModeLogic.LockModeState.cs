using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Play.Player.Camera.LockModeState;

public partial class LockModeLogic
{
    [Meta]
    public abstract partial record LockModeState : StateLogic<LockModeState>
    {
        [Meta]
        public partial record FreeCam : LockModeState, IGet<Input.ToggleShiftLock>
        {
            public Transition On(in Input.ToggleShiftLock input)
            {
                return To<Locked>();
            }
        }
        
        [Meta]
        public partial record Locked : LockModeState, IGet<Input.ToggleShiftLock>
        {
            public Transition On(in Input.ToggleShiftLock input)
            {
                return To<FreeCam>();
            }
        }
    }
}