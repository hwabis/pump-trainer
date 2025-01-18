using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    public class PumpTrainerHitObjectGenerator
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

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootLeftLargeTwist = new()
        {
            { Column.P2DL, [Column.P1C] },
            { Column.P2UL, [Column.P1C] },
            { Column.P2C, [Column.P1UR, Column.P1DR] },
        };

        private Dictionary<Column, List<Column>> nextColumnsPreviousFootRightLargeTwist = [];

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

        public PumpTrainerHitObjectGeneratorSettings Settings = new();

        // Updated for every newly generated note
        private PumpTrainerHitObjectGeneratorSettingsPerHitObject perHitObjectsettings;

        private int fullDoublesEdgeStreak = 0;
        private int p1SinglesEdgeStreak = 0;

        public PumpTrainerHitObjectGenerator()
        {
            initializeFootRightDictionaries();
        }

        /// <summary>
        /// Generates a hitobject solely based off the settings of this class and previously generated hitobjects.
        /// Does NOT take into account any rhythms between the hitobject being generated and previously generated hitobjects.
        /// </summary>
        /// <param name="startTime">The start time of the next hitobject to generate.</param>
        /// <param name="beatmap">The source beatmap we're generating objects based off. Typically an osu! beatmap.</param>
        /// <param name="perHitObjectsettings">The settings to apply for the generated hit object.</param>
        /// <returns></returns>
        public PumpTrainerHitObject GetNextHitObject(double startTime, IBeatmap beatmap, PumpTrainerHitObjectGeneratorSettingsPerHitObject perHitObjectsettings)
        {
            this.perHitObjectsettings = perHitObjectsettings;

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
            List<Column> candidateColumns = getCandidateColumns(nextFoot, previousColumn, false);
            includeOnlyAllowedColumns(candidateColumns);
            banColumnsCausingBannedPatterns(candidateColumns, nextFoot == Foot.Left ? Foot.Right : Foot.Left);

            if (candidateColumns.Count == 0)
            {
                // We messed up. Do the whole thing again but this time, allow all twists.
                // todo There are many combination of mods that can break beatmap generation
                // particularly when you have an "island" of included columns

                candidateColumns = getCandidateColumns(nextFoot, previousColumn, true);
                includeOnlyAllowedColumns(candidateColumns);
                banColumnsCausingBannedPatterns(candidateColumns, nextFoot == Foot.Left ? Foot.Right : Foot.Left);
            }

            return getRandomCandidateColumnWeighted(candidateColumns);
        }

        private List<Column> getCandidateColumns(Foot nextFoot, Column previousColumn, bool guaranteeTwistCandidates)
        {
            List<Column> candidateColumns = (nextFoot == Foot.Left ?
                nextColumnsPreviousFootRight[previousColumn] : nextColumnsPreviousFootLeft[previousColumn]).ToList();

            possiblyAddHorizontalTwistsToCandidates(candidateColumns, previousColumn, guaranteeTwistCandidates);
            possiblyAddDiagonalSkipsToCandidates(candidateColumns, previousColumn);
            possiblyAddLargeHorizontalTwistsToCandidates(candidateColumns, previousColumn);

            // Easy mod bans
            possiblyBanFarColumns(candidateColumns, previousColumn, guaranteeTwistCandidates);

            return candidateColumns;
        }

        private void possiblyAddHorizontalTwistsToCandidates(List<Column> candidates, Column previousColumn, bool alwaysAdd)
        {
            if (random.NextDouble() < Settings.HorizontalTwistFrequency || alwaysAdd)
            {
                List<Column> twistColumnsToAdd;

                if (previousFoot == Foot.Left)
                {
                    if (nextColumnsPreviousFootLeftHorizontalTwist.ContainsKey(previousColumn))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootLeftHorizontalTwist[previousColumn];
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (nextColumnsPreviousFootRightHorizontalTwist.ContainsKey(previousColumn))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootRightHorizontalTwist[previousColumn];
                    }
                    else
                    {
                        return;
                    }
                }

                candidates.AddRange(twistColumnsToAdd);
            }
        }

        private void possiblyAddDiagonalSkipsToCandidates(List<Column> candidates, Column previousColumn)
        {
            if (random.NextDouble() < Settings.DiagonalSkipFrequency)
            {
                List<Column> twistColumnsToAdd;

                if (previousFoot == Foot.Left)
                {
                    if (nextColumnsPreviousFootLeftDiagonalSkip.ContainsKey(previousColumn))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootLeftDiagonalSkip[previousColumn];
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (nextColumnsPreviousFootRightDiagonalSkip.ContainsKey(previousColumn))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootRightDiagonalSkip[previousColumn];
                    }
                    else
                    {
                        return;
                    }
                }

                candidates.AddRange(twistColumnsToAdd);
            }
        }

        private void possiblyAddLargeHorizontalTwistsToCandidates(List<Column> candidates, Column previousColumn)
        {
            if (random.NextDouble() < Settings.LargeTwistFrequency)
            {
                List<Column> twistColumnsToAdd;

                if (previousFoot == Foot.Left)
                {
                    if (nextColumnsPreviousFootLeftLargeTwist.ContainsKey(previousColumn))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootLeftLargeTwist[previousColumn];
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (nextColumnsPreviousFootRightLargeTwist.ContainsKey(previousColumn))
                    {
                        twistColumnsToAdd = nextColumnsPreviousFootRightLargeTwist[previousColumn];
                    }
                    else
                    {
                        return;
                    }
                }

                candidates.AddRange(twistColumnsToAdd);
            }
        }

        private void possiblyBanFarColumns(List<Column> candidates, Column previousColumn, bool neverBan)
        {
            if (neverBan)
            {
                return;
            }

            if (random.NextDouble() > Settings.FarColumnsFrequency)
            {
                List<Column> columnsToBan;
                int previousPhysicalColumn = columnToPhysicalColumn[previousColumn];

                if (previousColumn == Column.P1C || previousColumn == Column.P2C)
                {
                    // Just ban everything that's two columns away
                    columnsToBan =
                        columnToPhysicalColumn
                        .Where(entry => entry.Value == previousPhysicalColumn + 2 || entry.Value == previousPhysicalColumn - 2)
                        .Select(entry => entry.Key)
                        .ToList();
                }
                else
                {
                    // Ban everything that's two columns away and that's also a center panel
                    columnsToBan =
                        columnToPhysicalColumn
                        .Where(entry =>
                            (entry.Value == previousPhysicalColumn + 2 || entry.Value == previousPhysicalColumn - 2)
                                && (entry.Key == Column.P1C || entry.Key == Column.P2C))
                        .Select(entry => entry.Key)
                        .ToList();
                }

                candidates.RemoveAll(columnsToBan.Contains);
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
        /// A pattern is a group of 3 notes (not 2).
        /// </summary>
        /// <param name="candidateColumns">Candidate columns to ban from.</param>
        /// <param name="previousFoot">Associated foot of the most recent hit object in the beatmap.</param>
        private void banColumnsCausingBannedPatterns(List<Column> candidateColumns, Foot previousFoot)
        {
            // todo this function and maybe this whole class can be broken down into ban types and stuff
            // so we can just add the types of bans we want to a list instead of this hugh mungus method

            if (hitObjectsSoFar.Count <= 1)
            {
                // No possible bans if we're only on the first or second note
                return;
            }

            Column previousColumn = hitObjectsSoFar[^1].Column;
            Column previousPreviousColumn = hitObjectsSoFar[^2].Column;

            // Ban spins no matter what (this covers large twist spins as well)
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

            // Ban large horizontal triples no matter what. This is where all three columns go in one direction, and they are not adjacent to each other.
            {
                int previousPhysicalColumn = columnToPhysicalColumn[previousColumn];
                int previousPreviousPhysicalColumn = columnToPhysicalColumn[previousPreviousColumn];

                List<Column> columnsToBan = [];

                // If the two previous columns are adjacent, then we have an extra column of leniency for the column of the next hit object
                // (so that we don't mistakenly ban a regular horizontal triple, where all the columns are adjacent).
                // If the two previous columns already have a gap between them, then we can't be lenient and we must ban everything going in that direction.
                int banColumnLeniency = Math.Abs(previousPhysicalColumn - previousPreviousPhysicalColumn) >= 2 ? 0 : 1;

                if (previousPhysicalColumn > previousPreviousPhysicalColumn)
                {
                    // Going right! Ban everything to the right
                    columnsToBan = columnToPhysicalColumn.Where(entry => entry.Value > previousPhysicalColumn + banColumnLeniency).Select(entry => entry.Key).ToList();
                }
                else if (previousPhysicalColumn < previousPreviousPhysicalColumn)
                {
                    // Going left! Ban everything to the left
                    columnsToBan = columnToPhysicalColumn.Where(entry => entry.Value < previousPhysicalColumn - banColumnLeniency).Select(entry => entry.Key).ToList();
                }

                candidateColumns.RemoveAll(columnsToBan.Contains);
            }

            // Ban diagonal twists (single pad)
            if (!Settings.AllowDiagonalTwists)
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

            // Ban horizontal triples (horizontally adjacent to each other)
            if (random.NextDouble() > perHitObjectsettings.HorizontalTripleFrequency)
            {
                int previousPhysicalColumn = columnToPhysicalColumn[previousColumn];
                int previousPreviousPhysicalColumn = columnToPhysicalColumn[previousPreviousColumn];

                List<Column> columnsToBan = [];

                if (previousPhysicalColumn > previousPreviousPhysicalColumn)
                {
                    // Going right! Ban everything to the right
                    columnsToBan = columnToPhysicalColumn.Where(entry => entry.Value > previousPhysicalColumn).Select(entry => entry.Key).ToList();
                }
                else if (previousPhysicalColumn < previousPreviousPhysicalColumn)
                {
                    // Going left! Ban everything to the left
                    columnsToBan = columnToPhysicalColumn.Where(entry => entry.Value < previousPhysicalColumn).Select(entry => entry.Key).ToList();
                }

                // Don't ban columns where all three notes are on the same singles pad. This would interfere with horizontal twists.
                // Just manually check all four possibilities.
                if (previousPhysicalColumn == 1 && previousPreviousPhysicalColumn == 0)
                {
                    columnsToBan.RemoveAll(column => columnToPhysicalColumn[column] == 2);
                }
                if (previousPhysicalColumn == 1 && previousPreviousPhysicalColumn == 2)
                {
                    columnsToBan.RemoveAll(column => columnToPhysicalColumn[column] == 0);
                }
                if (previousPhysicalColumn == 4 && previousPreviousPhysicalColumn == 3)
                {
                    columnsToBan.RemoveAll(column => columnToPhysicalColumn[column] == 5);
                }
                if (previousPhysicalColumn == 4 && previousPreviousPhysicalColumn == 5)
                {
                    columnsToBan.RemoveAll(column => columnToPhysicalColumn[column] == 3);
                }

                candidateColumns.RemoveAll(columnsToBan.Contains);
            }

            // Ban corner patterns
            if (random.NextDouble() > perHitObjectsettings.CornersFrequency)
            {
                // Ban 90 degree patterns
                if (previousPreviousColumn == Column.P1UL && (previousColumn == Column.P1UR || previousColumn == Column.P1DL))
                {
                    candidateColumns.Remove(Column.P1DR);
                }
                else if (previousPreviousColumn == Column.P1DL && (previousColumn == Column.P1DR || previousColumn == Column.P1UL))
                {
                    candidateColumns.Remove(Column.P1UR);
                }
                else if (previousPreviousColumn == Column.P1UR && (previousColumn == Column.P1UL || previousColumn == Column.P1DR))
                {
                    candidateColumns.Remove(Column.P1DL);
                }
                else if (previousPreviousColumn == Column.P1DR && (previousColumn == Column.P1DL || previousColumn == Column.P1UR))
                {
                    candidateColumns.Remove(Column.P1UL);
                }
                else if (previousPreviousColumn == Column.P2UL && (previousColumn == Column.P2UR || previousColumn == Column.P2DL))
                {
                    candidateColumns.Remove(Column.P2DR);
                }
                else if (previousPreviousColumn == Column.P2DL && (previousColumn == Column.P2DR || previousColumn == Column.P2UL))
                {
                    candidateColumns.Remove(Column.P2UR);
                }
                else if (previousPreviousColumn == Column.P2UR && (previousColumn == Column.P2UL || previousColumn == Column.P2DR))
                {
                    candidateColumns.Remove(Column.P2DL);
                }
                else if (previousPreviousColumn == Column.P2DR && (previousColumn == Column.P2DL || previousColumn == Column.P2UR))
                {
                    candidateColumns.Remove(Column.P2UL);
                }

                // Ban V patterns
                if (previousPreviousColumn == Column.P1UL && (previousColumn == Column.P1DR || previousColumn == Column.P1DL))
                {
                    candidateColumns.Remove(Column.P1UR);
                }
                else if (previousPreviousColumn == Column.P1DL && (previousColumn == Column.P1UR || previousColumn == Column.P1UL))
                {
                    candidateColumns.Remove(Column.P1DR);
                }
                else if (previousPreviousColumn == Column.P1UR && (previousColumn == Column.P1DL || previousColumn == Column.P1DR))
                {
                    candidateColumns.Remove(Column.P1UL);
                }
                else if (previousPreviousColumn == Column.P1DR && (previousColumn == Column.P1UL || previousColumn == Column.P1UR))
                {
                    candidateColumns.Remove(Column.P1DL);
                }
                else if (previousPreviousColumn == Column.P2UL && (previousColumn == Column.P2DR || previousColumn == Column.P2DL))
                {
                    candidateColumns.Remove(Column.P2UR);
                }
                else if (previousPreviousColumn == Column.P2DL && (previousColumn == Column.P2UR || previousColumn == Column.P2UL))
                {
                    candidateColumns.Remove(Column.P2DR);
                }
                else if (previousPreviousColumn == Column.P2UR && (previousColumn == Column.P2DL || previousColumn == Column.P2DR))
                {
                    candidateColumns.Remove(Column.P2UL);
                }
                else if (previousPreviousColumn == Column.P2DR && (previousColumn == Column.P2UL || previousColumn == Column.P2UR))
                {
                    candidateColumns.Remove(Column.P2DL);
                }
            }
        }

        private Column getRandomCandidateColumnWeighted(List<Column> candidateColumns)
        {
            if (candidateColumns.Count == 0)
            {
                throw new Exception("There were no candidate columns :(");
            }

            if (hitObjectsSoFar.Count < 2)
            {
                return candidateColumns[random.Next(candidateColumns.Count)];
            }

            Column previousPreviousColumn = hitObjectsSoFar[^2].Column;
            List<Column> candidateColumnsWeighted = [];

            foreach (Column candidateColumn in candidateColumns)
            {
                candidateColumnsWeighted.Add(candidateColumn);

                // Reduce the likelihood of trills
                if (candidateColumn != previousPreviousColumn)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        candidateColumnsWeighted.Add(candidateColumn);
                    }
                }

                // If full-doubles, decrease the likelihood of a panel on the same column as the previous if on the edge
                if (Settings.AllowedColumns.Count == 10 &&
                    fullDoublesEdgeStreak > 0 &&
                    previousColumn != null &&
                    columnToPhysicalColumn[candidateColumn] != columnToPhysicalColumn[previousColumn.Value])
                {
                    for (int i = 0; i < 4 * fullDoublesEdgeStreak; i++)
                    {
                        candidateColumnsWeighted.Add(candidateColumn);
                    }
                }

                // Also apply for singles
                if (Settings.AllowedColumns.SequenceEqual([Column.P1DL, Column.P1UL, Column.P1C, Column.P1UR, Column.P1DR]) &&
                    p1SinglesEdgeStreak > 0 &&
                    previousColumn != null &&
                    columnToPhysicalColumn[candidateColumn] != columnToPhysicalColumn[previousColumn.Value])
                {
                    for (int i = 0; i < 4 * p1SinglesEdgeStreak; i++)
                    {
                        candidateColumnsWeighted.Add(candidateColumn);
                    }
                }
            }

            Column selectedColumn = candidateColumnsWeighted[random.Next(candidateColumnsWeighted.Count)];

            fullDoublesEdgeStreak =
                columnToPhysicalColumn[selectedColumn] == 0 || columnToPhysicalColumn[selectedColumn] == 5  ?
                fullDoublesEdgeStreak + 1 : 0;

            p1SinglesEdgeStreak =
                columnToPhysicalColumn[selectedColumn] == 0 || columnToPhysicalColumn[selectedColumn] == 2 ?
                p1SinglesEdgeStreak + 1 : 0;

            return selectedColumn;
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

                if (nextColumnsPreviousFootLeftLargeTwist.ContainsKey(flippedColumn))
                {
                    nextColumnsPreviousFootRightLargeTwist[column] = nextColumnsPreviousFootLeftLargeTwist[flippedColumn].Select(n => horizontalFlips[n]).ToList();
                }
            }
        }
    }
}
