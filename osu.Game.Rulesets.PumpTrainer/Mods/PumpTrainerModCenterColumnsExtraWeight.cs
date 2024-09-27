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
        [SettingSource("Weight")]
        public Bindable<int> CenterColumnsExtraWeight { get; } = new BindableInt(5)
        {
            MinValue = 0,
            MaxValue = 20,
            Default = 5,
        };

        public override string Name => "Prioritize center columns";

        public override string Acronym => "CC";

        public override LocalisableString Description => "Recommended for singles. Increases the frequency of P1C and P2C.";

        public override double ScoreMultiplier => 1;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.CenterColumnsExtraWeight = CenterColumnsExtraWeight.Value;
        }
    }
}
