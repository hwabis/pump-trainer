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
        public Bindable<int> CenterColumnsExtraWeight { get; } = new BindableInt(10)
        {
            MinValue = 0,
            MaxValue = 30,
            Default = 10,
        };

        public override string Name => "Center columns weight";

        public override string Acronym => "CCW";

        public override LocalisableString Description => "Increases the frequency of P1C and P2C, creating more realistic singles charts.";

        public override double ScoreMultiplier => 1;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.CenterColumnsExtraWeight = CenterColumnsExtraWeight.Value;
        }
    }
}
