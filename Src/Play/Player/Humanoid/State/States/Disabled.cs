using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.Game.Domain;

namespace Jomolith.Play.Player.Humanoid.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public partial record Disabled : PlayerState, IGet<Input.Enable>
        {
            public Disabled()
            {
                this.OnEnter(() =>
                {
                    Output(new Output.SetFrozen(true));
                    Output(new Output.Animations.Disabled());
                });
                
                OnAttach(() => Get<IGameRepo>().TowerEntered += OnTowerEntered);
                OnDetach(() => Get<IGameRepo>().TowerEntered -= OnTowerEntered);
            }

            public Transition On(in Input.Enable input) => To<Idle>();
        }
        
        public void OnTowerEntered() => Input(new Input.Enable());
    }
}