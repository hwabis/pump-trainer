// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.PumpTrainer.Replays
{
    public class PumpTrainerReplayFrame : ReplayFrame
    {
        public List<PumpTrainerAction> Actions = new List<PumpTrainerAction>();

        public PumpTrainerReplayFrame(PumpTrainerAction? button = null)
        {
            if (button.HasValue)
                Actions.Add(button.Value);
        }
    }
}
