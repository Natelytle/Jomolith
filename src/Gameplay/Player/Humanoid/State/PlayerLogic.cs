using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;
using Jomolith.Gameplay.Domain;
using Jomolith.Gameplay.Player.Domain;
using Jomolith.Gameplay.Player.Humanoid.State.States;

namespace Jomolith.Gameplay.Player.Humanoid.State;

public interface IPlayerLogic : ILogicBlock;

[Meta]
public partial class PlayerLogic : LogicBlock, IPlayerLogic
{
    public PlayerLogic()
    {
        Set(new PlayerState.Idle());
        Set(new PlayerState.Disabled());
        Set(new PlayerState.Dead());
        Set(new PlayerState.Noclip());
        Set(new PlayerState.Running());
        Set(new PlayerState.Jumping());
        Set(new PlayerState.Climbing());
        Set(new PlayerState.Falling());
        Set(new PlayerState.Landed());
        Set(new PlayerState.Coyote());
    }

    private AutoValue<float>.Binding? opacityBinding;

    public override void OnStart()
    {
        IPlayerRepo playerRepo = Get<IPlayerRepo>();

        opacityBinding = playerRepo.AvatarOpacity.Bind()
            .OnValue(opacity => State?.Output(new Outputs.Visual.SetTransparency(opacity)));
    }

    public override void OnStop()
    {
        opacityBinding?.Dispose();
    }

    public override IEnumerable<IDisposable> OnStartSubscriptions()
    {
        yield return Get<IGameplayRepo>().AutoChannel.Bind()
            .On((in IGameplayRepo.GameplayStarted _) => (State as PlayerState.Disabled)?.OnGameplayStarted());
    }
}
