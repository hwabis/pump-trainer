// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.PumpTrainer
{
    public partial class PumpTrainerInputManager : RulesetInputManager<PumpTrainerAction>
    {
        public PumpTrainerInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }
    }

    public enum PumpTrainerAction
    {
        P1DL,
        P1UL,
        P1C,
        P1UR,
        P1DR,
        P2DL,
        P2UL,
        P2C,
        P2UR,
        P2DR,
    }
}
