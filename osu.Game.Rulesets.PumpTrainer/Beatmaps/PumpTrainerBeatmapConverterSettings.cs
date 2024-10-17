using System.Collections.Generic;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    public class PumpTrainerBeatmapConverterSettings
    {
        /// <summary>
        /// For osu! sliders with no repeats, this represents whether to convert the end of the slider to a hitobject.
        /// (For repeating sliders, each slider end is always converted to hitobjects whether this is true or false.)
        /// </summary>
        public bool IgnoreNormalSliderEnds = false;

        public List<Column> AllowedColumns =
            [Column.P1DL, Column.P1UL, Column.P1C, Column.P1UR, Column.P1DR, Column.P2DL, Column.P2UL, Column.P2C, Column.P2UR, Column.P2DR];

        /// <summary>
        /// 0 to 1 determining how frequently to generate a singles "twist" that's not a horizontal twist. Higher means more likely.
        /// Example starting left foot: C --> UR --> DR
        /// Example starting left foot: C --> DR --> UR
        /// </summary>
        public double SinglesTwistFrequency = 1;

        /// <summary>
        /// 0 to 1 determining how frequently to generate two notes that are in adjacent columns on the physical dance pad.
        /// One of the notes must be a center panel (aka they do not span across a center panel).
        /// Higher means more likely.
        /// Example starting left foot: P1C --> P2UL
        /// Example starting right foot: P2DL --> P1C
        /// NON-example starting left foot: P1DL --> P1DR
        /// </summary>
        public double FarColumnsFrequency = 1;

        /// <summary>
        /// 0 to 1 determining how frequently to generate a horizontal twist. Higher means more likely.
        /// Example starting left foot: UL --> C --> UR
        /// </summary>
        public double HorizontalTwistFrequency = 0;

        /// <summary>
        /// 0 to 1 determining how frequently to generate a diagonal twist. Higher means more likely.
        /// Example starting left foot: DL --> C --> UR
        /// </summary>
        public double DiagonalTwistFrequency = 0;

        /// <summary>
        /// 0 to 1 determining how frequently to generate a diagonal skip. Higher means more likely.
        /// Example starting left foot: DR --> UL
        /// </summary>
        public double DiagonalSkipFrequency = 0;

        /// <summary>
        /// 0 to 1 determining how frequently to generate 3 adjacent notes that span unique columns on the physical dance pad,
        /// and go in one direction (i.e. only left, or only right).
        /// The only exception to this are triples that are all within 1 singles pad.
        /// Higher means more likely. For example, P1UR and P1DR are on the same unique column on the physical dance pad.
        /// Example: P1C --> P1DR --> P2UL
        /// Example: P2UL --> P1DR --> P1C
        /// NOT a horizontal triple: P1C --> P2UL --> P2DL (because the physical columns are not unique)
        /// NOT a horizontal triple: P1C --> P2UL --> P1DR (because the notes do not go in one direction)
        /// NOT a horizontal triple: P1UL --> P1C --> P1UR (because the notes only span a singles pad)
        /// </summary>
        public double HorizontalTripleFrequency = 0;

        public PumpTrainerBeatmapConverterSettings()
        {
        }
    }
}
