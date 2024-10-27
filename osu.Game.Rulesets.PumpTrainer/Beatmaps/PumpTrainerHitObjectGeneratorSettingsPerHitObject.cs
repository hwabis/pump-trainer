namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    /// <summary>
    /// By default, all the fields in this class are set to maximize the frequency of each pattern.
    /// </summary>
    public class PumpTrainerHitObjectGeneratorSettingsPerHitObject
    {
        /// <summary>
        /// 0 to 1 determining how frequently to generate a corner pattern. Higher means more likely.
        /// Corner patterns include 90 degree patterns and V shape patterns (see examples below).
        /// (Corner patterns are usually banned on sixteenth rhythms.)
        /// Examples:
        /// Starting left: UL, UR, DR (and 7 other variations per singles panel) - 90 degrees across a single panel.
        /// Starting left: UL, DR, UR (and 7 other variations per singles panel) - V shape across a single panel.
        /// </summary>
        public double CornersFrequency = 1;

        /// <summary>
        /// 0 to 1 determining how frequently to generate 3 adjacent notes that span unique physical columns,
        /// go in one direction (i.e. only left, or only right), and are not all on the same single pad. Higher means more likely.
        /// Example: P1C --> P1DR --> P2UL
        /// Example: P2UL --> P1DR --> P1C
        /// NON-example: P1C --> P2UL --> P2DL (because the physical columns are not unique)
        /// NON-example: P1C --> P2UL --> P1DR (because the notes do not go in one direction)
        /// NON-example: P1UL --> P1C --> P1UR (because the notes only span a single pad)
        /// </summary>
        public double HorizontalTripleFrequency = 1;

        public PumpTrainerHitObjectGeneratorSettingsPerHitObject()
        {
        }
    }
}
