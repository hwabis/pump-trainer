using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    public class NextHitObjectGenerator
    {
        private Dictionary<Column, List<Column>> nextColumnsPreviousFootLeft = new()
        {
            { Column.P1DL, [Column.P1UL, Column.P1C, Column.P1UR, Column.P1DR] },
            { Column.P1UL, [Column.P1DL, Column.P1C, Column.P1UR, Column.P1DR] },
            { Column.P1C, [Column.P1DL, Column.P1UL, Column.P1UR, Column.P1DR, Column.P2DL, Column.P2UL] },
            { Column.P1UR, [Column.P1C, Column.P1DR, Column.P2DL, Column.P2UL, Column.P2C] },
            { Column.P1DR, [Column.P1C, Column.P1UR, Column.P2DL, Column.P2UL, Column.P2C] },
            { Column.P2DL, [Column.P1UR, Column.P2UL, Column.P2C, Column.P2UR, Column.P2DR] },
            { Column.P2UL, [Column.P1DR, Column.P2DL, Column.P2C, Column.P2UR, Column.P2DR] },
            { Column.P2C, [Column.P2DL, Column.P2UL, Column.P2UR, Column.P2DR] },
            { Column.P2UR, [Column.P2C, Column.P2DR] },
            { Column.P2DR, [Column.P2C, Column.P2UR] },
        };

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootRight = [];

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootLeftLargeCrossover = new()
        {
            { Column.P1UR, [Column.P1DL] },
            { Column.P1DR, [Column.P1UL] },
            { Column.P2DL, [Column.P1C] },
            { Column.P2UL, [Column.P1C] },
            { Column.P2C, [Column.P1UR, Column.P1DR] },
            { Column.P2UR, [Column.P2DL] },
            { Column.P2DR, [Column.P2UL] },
        };

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootRightLargeCrossover = [];

        private Dictionary<Column, Column> horizontalFlips = new()
        {
            { Column.P1DL, Column.P2DR },
            { Column.P1UL, Column.P2UR },
            { Column.P1C, Column.P2C },
            { Column.P1UR, Column.P2UL },
            { Column.P1DR, Column.P2DL },
            { Column.P2DL, Column.P1DR },
            { Column.P2UL, Column.P1UR },
            { Column.P2C, Column.P1C },
            { Column.P2UR, Column.P1UL },
            { Column.P2DR, Column.P1DL },
        };

        private static Random random = new(); // TODO seedable

        private List<PumpTrainerHitObject> hitObjectsSoFar = [];

        private Foot? previousFoot = null;
        private Column? previousColumn = null;

        private List<Column> allowedColumns;

        public NextHitObjectGenerator(List<Column> allowedColumns)
        {
            initializeFootRightDictionaries();

            this.allowedColumns = allowedColumns;
        }

        public PumpTrainerHitObject GetNextHitObject(double startTime, IBeatmap beatmap)
        {
            // Always start on the left foot as the first note (for now?)
            Foot nextFoot = previousFoot == null || previousFoot == Foot.Right ? Foot.Left : Foot.Right;
            previousFoot = nextFoot;

            Column nextColumn = previousColumn == null ? allowedColumns[random.Next(allowedColumns.Count())] : getNextColumn(nextFoot);
            previousColumn = nextColumn;

            PumpTrainerHitObject nextHitObject = new()
            {
                IntendedFoot = nextFoot,
                Column = nextColumn,

                StartTime = startTime,
            };

            hitObjectsSoFar.Add(nextHitObject);

            return nextHitObject;
        }

        private Column getNextColumn(Foot nextFoot)
        {
            PumpTrainerHitObject previousHitObject = hitObjectsSoFar.Last();

            List<Column> candidateColumns = (nextFoot == Foot.Left ?
                nextColumnsPreviousFootRight[previousHitObject.Column] : nextColumnsPreviousFootLeft[previousHitObject.Column]).ToList();

            if (false) // TODO add large crossovers if it's enabled
            {
            }

            includeOnlyAllowedColumns(candidateColumns);
            banColumnsCausingBannedPatterns(candidateColumns, nextFoot == Foot.Left ? Foot.Right : Foot.Left, false); // TODO toggle whether wide swings are allowed
            return getRandomCandidateColumn(candidateColumns, 4 /* magic number to reduce likeihood of trills */);
        }

        private void includeOnlyAllowedColumns(List<Column> candidateColumns)
        {
            foreach (Column column in candidateColumns.ToList())
            {
                if (!allowedColumns.Contains(column))
                {
                    candidateColumns.Remove(column);
                }
            }
        }

        private void banColumnsCausingBannedPatterns(List<Column> candidateColumns, Foot previousFoot, bool allowWideSwings)
        {
            if (hitObjectsSoFar.Count <= 1)
            {
                // No possible bans if we're only on the first or second note
                return;
            }

            Column previousColumn = hitObjectsSoFar[^1].Column;
            Column previousPreviousColumn = hitObjectsSoFar[^2].Column;

            // Ban patterns that would only be comfortable if you spin around
            // [RLR] DL C UL
            // [RLR] UL C DL
            // [LRL] DR C UR
            // [LRL] UR C DR

            if (previousFoot == Foot.Left)
            {
                if (previousColumn == Column.P1C)
                {
                    if (previousPreviousColumn == Column.P1DL)
                    {
                        candidateColumns.Remove(Column.P1UL);
                    }
                    else if (previousPreviousColumn == Column.P1UL)
                    {
                        candidateColumns.Remove(Column.P1DL);
                    }
                }
                else if (previousColumn == Column.P2C)
                {
                    if (previousPreviousColumn == Column.P2DL || previousPreviousColumn == Column.P1DR)
                    {
                        candidateColumns.Remove(Column.P2UL);
                        candidateColumns.Remove(Column.P1UR);
                    }
                    else if (previousPreviousColumn == Column.P2UL || previousPreviousColumn == Column.P1UR)
                    {
                        candidateColumns.Remove(Column.P2DL);
                        candidateColumns.Remove(Column.P1DR);
                    }
                }
            }
            else if (previousFoot == Foot.Right)
            {
                if (previousColumn == Column.P1C)
                {
                    if (previousPreviousColumn == Column.P1DR || previousPreviousColumn == Column.P2DL)
                    {
                        candidateColumns.Remove(Column.P1UR);
                        candidateColumns.Remove(Column.P2UL);
                    }
                    else if (previousPreviousColumn == Column.P1UR || previousPreviousColumn == Column.P2UL)
                    {
                        candidateColumns.Remove(Column.P1DR);
                        candidateColumns.Remove(Column.P2DL);
                    }
                }
                else if (previousColumn == Column.P2C)
                {
                    if (previousPreviousColumn == Column.P2DR)
                    {
                        candidateColumns.Remove(Column.P2UR);
                    }
                    else if (previousPreviousColumn == Column.P2UR)
                    {
                        candidateColumns.Remove(Column.P2DR);
                    }
                }
            }

            if (!allowWideSwings)
            {
                // Ban 180 degree twists

                if (previousColumn == Column.P1C)
                {
                    if (previousPreviousColumn == Column.P1DL)
                    {
                        candidateColumns.Remove(Column.P1UR);
                    }
                    else if (previousPreviousColumn == Column.P1UL)
                    {
                        candidateColumns.Remove(Column.P1DR);
                    }
                    else if (previousPreviousColumn == Column.P1DR || previousPreviousColumn == Column.P2DL)
                    {
                        candidateColumns.Remove(Column.P1UL);
                    }
                    else if (previousPreviousColumn == Column.P1UR || previousPreviousColumn == Column.P2UL)
                    {
                        candidateColumns.Remove(Column.P1DL);
                    }
                }
                else if (previousColumn == Column.P2C)
                {
                    if (previousPreviousColumn == Column.P2DR)
                    {
                        candidateColumns.Remove(Column.P2UL);
                    }
                    else if (previousPreviousColumn == Column.P2UR)
                    {
                        candidateColumns.Remove(Column.P2DL);
                    }
                    else if (previousPreviousColumn == Column.P1DR || previousPreviousColumn == Column.P2DL)
                    {
                        candidateColumns.Remove(Column.P2UR);
                    }
                    else if (previousPreviousColumn == Column.P1UR || previousPreviousColumn == Column.P2UL)
                    {
                        candidateColumns.Remove(Column.P2DR);
                    }
                }

                // Ban ... long horizontal swings.

                if (previousColumn == Column.P1C)
                {
                    if (previousPreviousColumn == Column.P2UL)
                    {
                        candidateColumns.Remove(Column.P1UL);
                    }
                    if (previousPreviousColumn == Column.P2DL)
                    {
                        candidateColumns.Remove(Column.P1DL);
                    }
                }
                else if (previousColumn == Column.P2C)
                {
                    if (previousPreviousColumn == Column.P1UR)
                    {
                        candidateColumns.Remove(Column.P2UR);
                    }
                    if (previousPreviousColumn == Column.P1DR)
                    {
                        candidateColumns.Remove(Column.P2DR);
                    }
                }
            }
        }

        private Column getRandomCandidateColumn(List<Column> candidateColumns, int nonRepeatWeight)
        {
            if (hitObjectsSoFar.Count < 2)
            {
                return candidateColumns[random.Next(candidateColumns.Count)];
            }

            // Reduce the likelihood of trills using nonRepeatWeight

            Column previousPreviousColumn = hitObjectsSoFar[^2].Column;
            List<Column> candidateColumnsWeighted = [];

            foreach (Column candidateColumn in candidateColumns)
            {
                candidateColumnsWeighted.Add(candidateColumn);

                if (candidateColumn != previousPreviousColumn)
                {
                    for (int i = 0; i < nonRepeatWeight; i++)
                    {
                        candidateColumnsWeighted.Add(candidateColumn);
                    }
                }
            }

            return candidateColumnsWeighted[random.Next(candidateColumnsWeighted.Count)];
        }

        private void initializeFootRightDictionaries()
        {
            foreach (var entry in nextColumnsPreviousFootLeft)
            {
                Column flippedColumn = horizontalFlips[entry.Key];
                List<Column> flippedValues = nextColumnsPreviousFootLeft[flippedColumn].Select(n => horizontalFlips[n]).ToList();
                nextColumnsPreviousFootRight[entry.Key] = flippedValues;
            }

            foreach (var entry in nextColumnsPreviousFootLeftLargeCrossover)
            {
                Column flippedColumn = horizontalFlips[entry.Key];

                if (nextColumnsPreviousFootLeftLargeCrossover.ContainsKey(flippedColumn))
                {
                    List<Column> flippedValues = nextColumnsPreviousFootLeftLargeCrossover[flippedColumn].Select(n => horizontalFlips[n]).ToList();
                    nextColumnsPreviousFootRightLargeCrossover[entry.Key] = flippedValues;
                }
            }
        }
    }
}
