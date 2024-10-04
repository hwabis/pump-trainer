using System.Collections.Generic;
using System.Collections.Immutable;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer
{
    public static class PumpTrainerKeybindConversions
    {
        public static readonly ImmutableDictionary<PumpTrainerAction, Column> ACTION_TO_COLUMN = new Dictionary<PumpTrainerAction, Column>()
        {
            { PumpTrainerAction.P1DL, Column.P1DL },
            { PumpTrainerAction.P1UL, Column.P1UL },
            { PumpTrainerAction.P1C, Column.P1C },
            { PumpTrainerAction.P1UR, Column.P1UR },
            { PumpTrainerAction.P1DR, Column.P1DR },
            { PumpTrainerAction.P2DL, Column.P2DL },
            { PumpTrainerAction.P2UL, Column.P2UL },
            { PumpTrainerAction.P2C, Column.P2C },
            { PumpTrainerAction.P2UR, Column.P2UR },
            { PumpTrainerAction.P2DR, Column.P2DR },
        }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<Column, PumpTrainerAction> COLUMN_TO_ACTION;

        static PumpTrainerKeybindConversions()
        {
            COLUMN_TO_ACTION = ACTION_TO_COLUMN.ToImmutableDictionary(i => i.Value, i => i.Key);
        }
    }
}
