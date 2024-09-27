using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Mods.ExcludeColumns
{
    public abstract class PumpTrainerExcludeColumnMod : Mod, IApplicableToBeatmapConverter
    {
        public override string Name => "Exclude " + ExcludedColumn.ToString();
        public override string Acronym => ExcludedColumn.ToString();
        public override LocalisableString Description => "Excludes the column " + ExcludedColumn;
        public override ModType Type => ModType.Conversion;
        public override double ScoreMultiplier => 1;

        public abstract Column ExcludedColumn { get; }

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.AllowedColumns.Remove(ExcludedColumn);
        }
    }
}
