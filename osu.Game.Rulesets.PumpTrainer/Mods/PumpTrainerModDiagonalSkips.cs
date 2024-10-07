﻿using System;
using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;
using osu.Game.Rulesets.PumpTrainer.Mods.ExcludeColumns;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModDiagonalSkips : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> DiagonalSkipFrequency { get; } = new BindableDouble(0.53)
        {
            MinValue = 0.1,
            MaxValue = 1.0,
            Default = 0.3,
            Precision = 0.1,
        };

        public override string Name => "[P1Single+] Diagonal Skips";
        public override string Acronym => "DD";
        public override LocalisableString Description => "Crossovers across a single panel, skipping over the center panel.\n";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyIncrease;
        public override Type[] IncompatibleMods => new Type[]
        {
            typeof(PumpTrainerModExcludeP1DL),
            typeof(PumpTrainerModExcludeP1UL),
            typeof(PumpTrainerModExcludeP1C),
            typeof(PumpTrainerModExcludeP1UR),
            typeof(PumpTrainerModExcludeP1DR),
        };

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.DiagonalSkipFrequency = DiagonalSkipFrequency.Value;
        }
    }
}
