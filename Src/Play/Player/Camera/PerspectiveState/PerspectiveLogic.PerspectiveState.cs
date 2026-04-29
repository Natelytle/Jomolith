using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Play.Player.Camera.PerspectiveState;

public partial class PerspectiveLogic
{
    [Meta]
    public abstract partial record PerspectiveState : StateLogic<PerspectiveState>
    {
        [Meta]
        public partial record FirstPerson : PerspectiveState, IGet<Input.ToThirdPerson>
        {
            public Transition On(in Input.ToThirdPerson input)
            {
                return To<ThirdPerson>();
            }
        }
        
        [Meta]
        public partial record ThirdPerson : PerspectiveState, IGet<Input.ToFirstPerson>
        {
            public Transition On(in Input.ToFirstPerson input)
            {
                return To<FirstPerson>();
            }
        }
    }
}