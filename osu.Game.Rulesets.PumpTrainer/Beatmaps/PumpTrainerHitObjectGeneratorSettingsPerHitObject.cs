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

        public PumpTrainerHitObjectGeneratorSettingsPerHitObject()
        {
        }
    }
}
