using System.Collections.Generic;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    public class PumpTrainerBeatmapConverterSettings
    {
        public List<Column> AllowedColumns =
            [Column.P1DL, Column.P1UL, Column.P1C, Column.P1UR, Column.P1DR, Column.P2DL, Column.P2UL, Column.P2C, Column.P2UR, Column.P2DR];

        /// <summary>
        /// 0 to 1 determining how frequently to generate a horizontal twist. Higher means more likely.
        /// Example starting left foot: UL --> C --> UR
        /// </summary>
        public double HorizontalTwistFrequency = 0;

        /// <summary>
        /// The higher the value, the higher the frequency of P1C's and P2C's being generated (over other columns).
        /// </summary>
        public int CenterColumnsExtraWeight = 0;

        /// <summary>
        /// 0 to 1 determining how frequently to generate a diagonal twist. Higher means more likely.
        /// Example starting left foot: DL --> C --> UR
        /// </summary>
        public double DiagonalTwistFrequency = 0;

        /// <summary>
        /// 0 to 1 determining how frequently to generate a diagonal twist. Higher means more likely.
        /// Example starting left foot: DL --> C --> UR
        /// </summary>
        //public double DiagonalTwistFrequency = 0;

        public PumpTrainerBeatmapConverterSettings()
        {
        }
    }
}
