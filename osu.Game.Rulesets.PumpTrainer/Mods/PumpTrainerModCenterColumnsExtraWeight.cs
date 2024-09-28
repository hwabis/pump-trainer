using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModCenterColumnsExtraWeight : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Extra weight")]
        public Bindable<int> CenterColumnsExtraWeight { get; } = new BindableInt(5)
        {
            MinValue = 1,
            MaxValue = 20,
            Default = 5,
        };

        public override string Name => "Prioritize center columns";
        public override string Acronym => "S";
        public override LocalisableString Description => "Recommended for singles. Not recommended for half-doubles.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyReduction;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.CenterColumnsExtraWeight = CenterColumnsExtraWeight.Value;
        }
    }
}
