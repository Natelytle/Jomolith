using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks.Auto;
using Chickensoft.Sync.Primitives;
using Jomolith.App.Domain;
using Jomolith.Play.Player.Domain;
using Jomolith.Play.Player.Humanoid.State.States;

namespace Jomolith.Play.Player.Humanoid.State;

public interface IPlayerLogic : IAutoLogicBlock;

[Meta]
public partial class PlayerLogic : AutoBlock, IPlayerLogic
{
    public PlayerLogic()
    {
        Preallocate<PlayerState>();
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
        // TODO: Add enable logic once I get tower loading working again.
        // yield return Get<IAppRepo>().AutoChannel.Bind()
        //     .On((in IAppRepo.TowerEntered _) => (State as PlayerState.Disabled)?.OnTowerEntered());

        yield break;
    }
}
