using System.Collections.Generic;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    public struct PumpTrainerBeatmapConverterSettings
    {
        public List<Column> AllowedColumns =
            [Column.P1DL, Column.P1UL, Column.P1C, Column.P1UR, Column.P1DR, Column.P2DL, Column.P2UL, Column.P2C, Column.P2UR, Column.P2DR];

        /// <summary>
        /// 0 to 1 determining how frequently to generate crossovers.
        /// A cross over would be like: starting left foot: UL, C, UR
        /// </summary>
        public double SmallCrossOverFrequency = 0; // todo mod selectable

        public PumpTrainerBeatmapConverterSettings()
        {
        }
    }
}
