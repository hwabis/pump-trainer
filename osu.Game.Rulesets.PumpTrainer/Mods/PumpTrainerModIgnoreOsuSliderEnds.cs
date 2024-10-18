using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModIgnoreOsuSliderEnds : Mod, IApplicableToBeatmapConverter
    {
        public override string Name => "Ignore osu! Slider Ends";
        public override string Acronym => "SE";
        public override LocalisableString Description =>
            "Recommended for certain osu! \"tech\"-style maps.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyReduction;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.CountNormalSliderEnds = false;
        }
    }
}
