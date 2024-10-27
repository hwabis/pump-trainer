﻿using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModHorizontalTwists : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> HorizontalTwistFrequency { get; } = new BindableDouble(0.5)
        {
            MinValue = 0.1,
            MaxValue = 1.0,
            Default = 0.5,
            Precision = 0.1,
        };

        [SettingSource("[Half-Dbl+] Large Twists")]
        public Bindable<bool> IncludeLargeTwists { get; } = new BindableBool(false)
        {
            Default = false,
        };

        [SettingSource("[P1Single+] Diagonal Twists")]
        public Bindable<bool> AllowDiagonalTwists { get; } = new BindableBool(false)
        {
            Default = false,
        };

        public override string Name => "Horizontal Twists";
        public override string Acronym => "H";
        public override LocalisableString Description =>
            "Horizontal crossovers involving a center panel.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyIncrease;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.HorizontalTwistFrequency = HorizontalTwistFrequency.Value;

            if (IncludeLargeTwists.Value)
            {
                pumpBeatmapConverter.Settings.LargeTwistFrequency = HorizontalTwistFrequency.Value;
            }

            if (AllowDiagonalTwists.Value)
            {
                pumpBeatmapConverter.Settings.AllowDiagonalTwists = true;
            }
        }
    }
}
