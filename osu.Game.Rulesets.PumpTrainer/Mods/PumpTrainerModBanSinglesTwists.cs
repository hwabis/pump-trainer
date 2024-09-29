using System;
using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModBanSinglesTwists : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> SinglesTwistFrequency { get; } = new BindableDouble(0.3)
        {
            MinValue = 0,
            MaxValue = 0.9,
            Default = 0.3,
            Precision = 0.1,
        };

        public override string Name => "[S ONLY] Ban singles twists";
        public override string Acronym => "S";
        public override LocalisableString Description =>
            "Reduces the frequency of the right foot hitting the left pads, and vice versa, in the context of singles.\n" +
            "Do not use this mod for doubles.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyReduction;
        public override Type[] IncompatibleMods => new Type[]
        {
            typeof(PumpTrainerModHorizontalTwists),
            typeof(PumpTrainerModDiagonalTwists),
            typeof(PumpTrainerModHorizontalTriples),
        };

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.SinglesTwistFrequency = SinglesTwistFrequency.Value;
        }
    }
}
