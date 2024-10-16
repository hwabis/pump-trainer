using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModIgnoreOsuSliderEnds : Mod, IApplicableToBeatmapConverter
    {
        public override string Name => "Ignore osu! slider ends";
        public override string Acronym => "SE";
        public override LocalisableString Description =>
            "Recommended for osu! \"tech\"-style maps which have difficult rhythms.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyReduction;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.IgnoreNormalSliderEnds = true;
        }
    }
}
