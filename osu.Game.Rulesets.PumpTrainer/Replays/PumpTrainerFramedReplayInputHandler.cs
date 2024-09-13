// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Input.StateChanges;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.PumpTrainer.Replays
{
    public class PumpTrainerFramedReplayInputHandler : FramedReplayInputHandler<PumpTrainerReplayFrame>
    {
        public PumpTrainerFramedReplayInputHandler(Replay replay)
            : base(replay)
        {
        }

        protected override bool IsImportant(PumpTrainerReplayFrame frame) => frame.Actions.Any();

        protected override void CollectReplayInputs(List<IInput> inputs)
        {
            inputs.Add(new ReplayState<PumpTrainerAction>
            {
                PressedActions = CurrentFrame?.Actions ?? new List<PumpTrainerAction>(),
            });
        }
    }
}
