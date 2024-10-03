using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    public class NextHitObjectGenerator
    {
        private Dictionary<Column, List<Column>> nextColumnsPreviousFootLeft = new()
        {
            { Column.P1DL, [Column.P1UL, Column.P1C, Column.P1UR, Column.P1DR] },
            { Column.P1UL, [Column.P1DL, Column.P1C, Column.P1UR, Column.P1DR] },
            { Column.P1C, [Column.P1UR, Column.P1DR, Column.P2DL, Column.P2UL] },
            { Column.P1UR, [Column.P1DR, Column.P2DL, Column.P2UL, Column.P2C] },
            { Column.P1DR, [Column.P1UR, Column.P2DL, Column.P2UL, Column.P2C] },
            { Column.P2DL, [Column.P1UR, Column.P2UL, Column.P2C, Column.P2UR, Column.P2DR] },
            { Column.P2UL, [Column.P1DR, Column.P2DL, Column.P2C, Column.P2UR, Column.P2DR] },
            { Column.P2C, [Column.P2UR, Column.P2DR] },
            { Column.P2UR, [Column.P2DR] },
            { Column.P2DR, [Column.P2UR] },
        };

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootRight = [];

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootLeftHorizontalTwist = new()
        {
            { Column.P1C, [Column.P1DL, Column.P1UL] },
            { Column.P1UR, [Column.P1C] },
            { Column.P1DR, [Column.P1C] },
            { Column.P2C, [Column.P2DL, Column.P2UL] },
            { Column.P2UR, [Column.P2C] },
            { Column.P2DR, [Column.P2C] },
        };

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootRightHorizontalTwist = [];

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootLeftDiagonalSkip = new()
        {
            { Column.P1UR, [Column.P1DL] },
            { Column.P1DR, [Column.P1UL] },
            { Column.P2UR, [Column.P2DL] },
            { Column.P2DR, [Column.P2UL] },
        };

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootRightDiagonalSkip = [];

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

        private ImmutableDictionary<Column, int> columnToPhysicalColumn = new Dictionary<Column, int>()
        {
            { Column.P1DL, 0 },
            { Column.P1UL, 0 },
            { Column.P1C, 1 },
            { Column.P1UR, 2 },
            { Column.P1DR, 2 },
            { Column.P2DL, 3 },
            { Column.P2UL, 3 },
            { Column.P2C, 4 },
            { Column.P2UR, 5 },
            { Column.P2DR, 5 },
        }.ToImmutableDictionary();

        private static Random random = new();

        private List<PumpTrainerHitObject> hitObjectsSoFar = [];

        private Foot? previousFoot = null;
        private Column? previousColumn = null;

        public PumpTrainerBeatmapConverterSettings Settings = new();

        public NextHitObjectGenerator()
        {
            initializeFootRightDictionaries();
        }

        public PumpTrainerHitObject GetNextHitObject(double startTime, IBeatmap beatmap)
        {
            // Always start on the left foot as the first note (for now?)
            Foot nextFoot = previousFoot == null || previousFoot == Foot.Right ? Foot.Left : Foot.Right;

            Column nextColumn = previousColumn == null ? Settings.AllowedColumns[random.Next(Settings.AllowedColumns.Count())]
                : getNextColumn(nextFoot, (Column)previousColumn);

            PumpTrainerHitObject nextHitObject = new(nextColumn)
            {
                IntendedFoot = nextFoot,
                StartTime = startTime,
            };

            hitObjectsSoFar.Add(nextHitObject);

            previousFoot = nextFoot;
            previousColumn = nextColumn;

            return nextHitObject;
        }

        private Column getNextColumn(Foot nextFoot, Column previousColumn)
        {
            List<Column> candidateColumns = getCandidateColumns(nextFoot, previousColumn);

            includeOnlyAllowedColumns(candidateColumns);
            banColumnsCausingBannedPatterns(candidateColumns, nextFoot == Foot.Left ? Foot.Right : Foot.Left);

            if (candidateColumns.Count == 0)
            {
                // We messed up. Do the whole thing again but don't ban anyone. Everything will fix itself eventually lol.......]
                // TODO figure out how to remove this. It can mess stuff up by adding columns that are supposed to be excluded and adding banned patterns
                candidateColumns = getCandidateColumns(nextFoot, previousColumn);
            }

            return getRandomCandidateColumn(candidateColumns, 4 /* magic number to reduce likeihood of trills */);
        }

        private List<Column> getCandidateColumns(Foot nextFoot, Column previousColumn)
        {
            List<Column> candidateColumns = (nextFoot == Foot.Left ?
                nextColumnsPreviousFootRight[previousColumn] : nextColumnsPreviousFootLeft[previousColumn]).ToList();

            possiblyAddHorizontalTwistsToCandidates(candidateColumns);
            possiblyAddDiagonalSkipsToCandidates(candidateColumns);

            // The only reason we're removing singles twists after adding them to the candidates via the main dictionaries instead
            // of putting the twists in separate dictionaries then adding the twists in that separate dictionary conditionally based on the setting
            // is because this kind of twist is super common in doubles charts, and playing without them in doubles would be super weird.
            // So, it would feel weird to not include them in the main dictionaries.
            possiblyBanSinglesTwists(candidateColumns);

            return candidateColumns;
        }

        private void possiblyAddHorizontalTwistsToCandidates(List<Column> candidates)
        {
            Column previousColumnNonNull;

            if (previousColumn != null)
            {
                previousColumnNonNull = (Column)previousColumn;
            }
            else
            {
                return;
            }

            if (random.NextDouble() < Settings.HorizontalTwistFrequency)
            {
                List<Column> twistColumnsToAdd;

                if (previousFoot == Foot.Left)
                {
                    if (nextColumnsPreviousFootLeftHorizontalTwist.ContainsKey(previousColumnNonNull))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootLeftHorizontalTwist[previousColumnNonNull];
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (nextColumnsPreviousFootRightHorizontalTwist.ContainsKey(previousColumnNonNull))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootRightHorizontalTwist[previousColumnNonNull];
                    }
                    else
                    {
                        return;
                    }
                }

                candidates.AddRange(twistColumnsToAdd);
            }
        }

        private void possiblyAddDiagonalSkipsToCandidates(List<Column> candidates)
        {
            Column previousColumnNonNull;

            if (previousColumn != null)
            {
                previousColumnNonNull = (Column)previousColumn;
            }
            else
            {
                return;
            }

            if (random.NextDouble() < Settings.DiagonalSkipFrequency)
            {
                List<Column> twistColumnsToAdd;

                if (previousFoot == Foot.Left)
                {
                    if (nextColumnsPreviousFootLeftDiagonalSkip.ContainsKey(previousColumnNonNull))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootLeftDiagonalSkip[previousColumnNonNull];
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (nextColumnsPreviousFootRightDiagonalSkip.ContainsKey(previousColumnNonNull))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootRightDiagonalSkip[previousColumnNonNull];
                    }
                    else
                    {
                        return;
                    }
                }

                candidates.AddRange(twistColumnsToAdd);
            }
        }

        private void possiblyBanSinglesTwists(List<Column> candidates)
        {
            Column previousColumnNonNull;

            if (previousColumn != null)
            {
                previousColumnNonNull = (Column)previousColumn;
            }
            else
            {
                return;
            }

            if (random.NextDouble() > Settings.SinglesTwistFrequency)
            {
                if (previousFoot == Foot.Left)
                {
                    switch (previousColumnNonNull)
                    {
                        case Column.P1DL:
                            candidates.Remove(Column.P1UL);
                            return;
                        case Column.P1UL:
                            candidates.Remove(Column.P1DL);
                            return;
                        case Column.P2DL:
                            candidates.Remove(Column.P2UL);
                            return;
                        case Column.P2UL:
                            candidates.Remove(Column.P2DL);
                            return;
                    }
                }
                else
                {
                    switch (previousColumnNonNull)
                    {
                        case Column.P1DR:
                            candidates.Remove(Column.P1UR);
                            return;
                        case Column.P1UR:
                            candidates.Remove(Column.P1DR);
                            return;
                        case Column.P2DR:
                            candidates.Remove(Column.P2UR);
                            return;
                        case Column.P2UR:
                            candidates.Remove(Column.P2DR);
                            return;
                    }
                }
            }
        }

        private void includeOnlyAllowedColumns(List<Column> candidateColumns)
        {
            foreach (Column column in candidateColumns.ToList())
            {
                if (!Settings.AllowedColumns.Contains(column))
                {
                    candidateColumns.Remove(column);
                }
            }
        }

        /// <summary>
        /// Bans columns the given list of candidates based on the columns two most recent hit objects in the generated beatmap.
        /// </summary>
        /// <param name="candidateColumns">Candidate columns to ban from.</param>
        /// <param name="previousFoot">Associated foot of the most recent hit object in the beatmap.</param>
        private void banColumnsCausingBannedPatterns(List<Column> candidateColumns, Foot previousFoot)
        {
            if (hitObjectsSoFar.Count <= 1)
            {
                // No possible bans if we're only on the first or second note
                return;
            }

            Column previousColumn = hitObjectsSoFar[^1].Column;
            Column previousPreviousColumn = hitObjectsSoFar[^2].Column;

            // Ban spins no matter what
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

            // Ban diagonal twists
            if (random.NextDouble() > Settings.DiagonalTwistFrequency)
            {
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
                    else if (previousPreviousColumn == Column.P1DR)
                    {
                        candidateColumns.Remove(Column.P1UL);
                    }
                    else if (previousPreviousColumn == Column.P1UR)
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
                    else if (previousPreviousColumn == Column.P2DL)
                    {
                        candidateColumns.Remove(Column.P2UR);
                    }
                    else if (previousPreviousColumn == Column.P2UL)
                    {
                        candidateColumns.Remove(Column.P2DR);
                    }
                }
            }

            // Ban horizontal triples
            if (random.NextDouble() > Settings.HorizontalTripleFrequency)
            {
                int previousPhysicalColumn = columnToPhysicalColumn[previousColumn];
                int previousPreviousPhysicalColumn = columnToPhysicalColumn[previousPreviousColumn];

                if (previousPhysicalColumn - previousPreviousPhysicalColumn == 1)
                {
                    // Going right!
                    List<Column> columnsToBan = columnToPhysicalColumn.Where(entry => entry.Value == previousPhysicalColumn + 1).Select(entry => entry.Key).ToList();
                    candidateColumns.RemoveAll(columnsToBan.Contains);
                }
                else if (previousPhysicalColumn - previousPreviousPhysicalColumn == -1)
                {
                    // Going left!
                    List<Column> columnsToBan = columnToPhysicalColumn.Where(entry => entry.Value == previousPhysicalColumn - 1).Select(entry => entry.Key).ToList();
                    candidateColumns.RemoveAll(columnsToBan.Contains);
                }
            }
        }

        private Column getRandomCandidateColumn(List<Column> candidateColumns, int nonRepeatExtraWeight)
        {
            if (candidateColumns.Count == 0)
            {
                throw new Exception("There were no candidate columns :(");
            }

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
                    for (int i = 0; i < nonRepeatExtraWeight; i++)
                    {
                        candidateColumnsWeighted.Add(candidateColumn);
                    }
                }

                if (Settings.SinglesTwistFrequency > 0)
                {
                    if (candidateColumn == Column.P1C || candidateColumn == Column.P2C)
                    {
                        for (int i = 0; i < Settings.SinglesTwistFrequency; i++)
                        {
                            candidateColumnsWeighted.Add(candidateColumn);
                        }
                    }
                }
            }

            return candidateColumnsWeighted[random.Next(candidateColumnsWeighted.Count)];
        }

        private void initializeFootRightDictionaries()
        {
            foreach (Column column in Enum.GetValues(typeof(Column)))
            {
                Column flippedColumn = horizontalFlips[column];

                if (nextColumnsPreviousFootLeft.ContainsKey(flippedColumn))
                {
                    nextColumnsPreviousFootRight[column] = nextColumnsPreviousFootLeft[flippedColumn].Select(n => horizontalFlips[n]).ToList();
                }

                if (nextColumnsPreviousFootLeftHorizontalTwist.ContainsKey(flippedColumn))
                {
                    nextColumnsPreviousFootRightHorizontalTwist[column] = nextColumnsPreviousFootLeftHorizontalTwist[flippedColumn].Select(n => horizontalFlips[n]).ToList();
                }

                if (nextColumnsPreviousFootLeftDiagonalSkip.ContainsKey(flippedColumn))
                {
                    nextColumnsPreviousFootRightDiagonalSkip[column] = nextColumnsPreviousFootLeftDiagonalSkip[flippedColumn].Select(n => horizontalFlips[n]).ToList();
                }
            }
        }
    }
}
