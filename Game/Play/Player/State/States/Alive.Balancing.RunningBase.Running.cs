using Chickensoft.Introspection;

namespace Jomolith.Game.Play.Player.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public partial record Running : RunningBase, 
            IGet<Input.FacingLadder>,
            IGet<Input.Jump>,
            IGet<Input.OffFloor>,
            IGet<Input.IsIdle>
        {
            public Transition On(in Input.FacingLadder input)
            {
                throw new System.NotImplementedException();
            }

            public Transition On(in Input.Jump input)
            {
                throw new System.NotImplementedException();
            }

            public Transition On(in Input.OffFloor input)
            {
                throw new System.NotImplementedException();
            }

            public Transition On(in Input.IsIdle input)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}