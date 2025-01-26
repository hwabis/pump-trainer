using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModSeeded : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Seed")]
        public Bindable<int> Seed { get; } = new BindableInt(0)
        {
            MinValue = 0,
            MaxValue = 100,
        };

        public override string Name => "Seeded";
        public override string Acronym => "SD";
        public override LocalisableString Description => "Play consistently generated charts.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyReduction;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.HitObjectGenerator.Seed = Seed.Value;
        }
    }
}
