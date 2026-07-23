// <mdk sortorder="95" />   // ScanCommit.cs
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        IEnumerable<int> BuildSnapshotParkingAndStatus(
            ScanSnapshot snapshot)
        {
            // ===== Parking connectors =====

            for (int i = 0;
                i <
                    snapshot.RawConnectors.Count;
                i++)
            {
                IMyShipConnector block =
                    snapshot.RawConnectors[i];

                GridNode node;

                if (!snapshot
                        .GridNodes
                        .TryGetValue(
                            block
                                .CubeGrid
                                .EntityId,
                            out node) ||
                    !node.IncludedForControl)
                {
                    continue;
                }

                snapshot
                    .TopologyConnectors
                    .Add(block);

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        block.EntityId);

                if (!CanReadParkingBlock(
                        tags))
                {
                    continue;
                }

                snapshot
                    .ParkConnectors
                    .Add(
                        new ParkConnector
                        {
                            Block =
                                block,

                            Edge =
                                FindConnectorEdge(
                                    snapshot
                                        .ConnectorEdges,
                                    block)
                        });

                yield return 1;
            }

            // ===== Parking landing gear =====

            for (int i = 0;
                i <
                    snapshot
                        .RawLandingGears
                        .Count;
                i++)
            {
                IMyLandingGear block =
                    snapshot
                        .RawLandingGears[i];

                GridNode node;

                if (!snapshot
                        .GridNodes
                        .TryGetValue(
                            block
                                .CubeGrid
                                .EntityId,
                            out node) ||
                    !node.IncludedForControl)
                {
                    continue;
                }

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        block.EntityId);

                if (!CanReadParkingBlock(
                        tags))
                {
                    continue;
                }

                snapshot
                    .ParkLandingGears
                    .Add(
                        new ParkLandingGear
                        {
                            Block =
                                block
                        });

                yield return 1;
            }

            // ===== Local timer hooks =====

            for (int i = 0;
                i <
                    snapshot.RawTimers.Count;
                i++)
            {
                IMyTimerBlock timer =
                    snapshot.RawTimers[i];

                if (timer.CubeGrid !=
                    Me.CubeGrid)
                {
                    continue;
                }

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        timer.EntityId);

                if ((tags &
                     BlockTags.ParkTimer) != 0)
                {
                    snapshot.ParkTimers.Add(
                        timer);
                }

                if ((tags &
                     BlockTags.UnparkTimer) !=
                    0)
                {
                    snapshot
                        .UnparkTimers
                        .Add(timer);
                }

                yield return 1;
            }

            DiscoverStatusSurfaces(
                snapshot);
        }

        // ===== Atomic snapshot commit =====

        void CommitScanSnapshot(
            ScanSnapshot snapshot)
        {
            HashSet<long> newThrusterIds =
                new HashSet<long>();

            HashSet<long> newControlledRotorIds =
                new HashSet<long>();

            HashSet<long> newGyroIds =
                new HashSet<long>();

            for (int i = 0;
                i <
                    snapshot.Thrusters.Count;
                i++)
            {
                newThrusterIds.Add(
                    snapshot
                        .Thrusters[i]
                        .EntityId);
            }

            for (int i = 0;
                i <
                    snapshot
                        .ControlledRotors
                        .Count;
                i++)
            {
                newControlledRotorIds.Add(
                    snapshot
                        .ControlledRotors[i]
                        .EntityId);
            }

            for (int i = 0;
                i <
                    snapshot.Gyros.Count;
                i++)
            {
                newGyroIds.Add(
                    snapshot
                        .Gyros[i]
                        .TheBlock
                        .EntityId);
            }

            // Release blocks that left the local construct entirely.
            for (int i = 0;
                i < thrusters.Count;
                i++)
            {
                Thruster oldThruster =
                    thrusters[i];

                if (newThrusterIds.Contains(
                        oldThruster.EntityId))
                {
                    continue;
                }

                oldThruster.Release();

                RestoreParkedThruster(
                    oldThruster.EntityId,
                    oldThruster.TheBlock);

                ReleaseThrusterDisableReason(
                    oldThruster.EntityId,
                    oldThruster.TheBlock,
                    ThrusterDisableReason
                        .Cruise);

                ReleaseThrusterDisableReason(
                    oldThruster.EntityId,
                    oldThruster.TheBlock,
                    ThrusterDisableReason
                        .Standby);
            }

            // A previously owned rotor may still exist but no longer have an
            // owned thruster. Stop it once while relinquishing authority.
            for (int i = 0;
                i < controlledRotors.Count;
                i++)
            {
                Rotor oldRotor =
                    controlledRotors[i];

                if (!newControlledRotorIds
                        .Contains(
                            oldRotor.EntityId))
                {
                    oldRotor.Release();
                }
            }

            for (int i = 0;
                i < controlledGyros.Count;
                i++)
            {
                Gyro oldGyro =
                    controlledGyros[i];

                if (!newGyroIds.Contains(
                        oldGyro
                            .TheBlock
                            .EntityId))
                {
                    oldGyro.Release();
                }
            }

            ReplaceContents(
                localControllers,
                snapshot.LocalControllers);

            ReplaceContents(
                remotelyReachableControllers,
                snapshot.RemoteControllers);

            ReplaceContents(
                thrusters,
                snapshot.Thrusters);

            ReplaceContents(
                controlledThrusters,
                snapshot.ControlledThrusters);

            ReplaceContents(
                fixedControlledThrusters,
                snapshot.FixedControlledThrusters);

            ReplaceContents(
                observedReadOnlyThrusters,
                snapshot.ObservedReadOnlyThrusters);

            ReplaceContents(
                localUnmanagedThrusters,
                snapshot.LocalUnmanagedThrusters);

            ReplaceContents(
                remoteReduxThrusters,
                snapshot.RemoteReduxThrusters);

            ReplaceContents(
                remoteUnmanagedThrusters,
                snapshot.RemoteUnmanagedThrusters);

            ReplaceContents(
                remoteFixedReduxThrusters,
                snapshot.RemoteFixedReduxThrusters);

            ReplaceContents(
                controlledRotors,
                snapshot.ControlledRotors);

            ReplaceContents(
                vectorThrusters,
                snapshot.VectorThrusters);

            ReplaceContents(
                vectorThrustGroups,
                snapshot.Groups);

            ReplaceContents(
                remoteNacelles,
                snapshot.RemoteNacelles);

            ReplaceContents(
                controlledGyros,
                snapshot.Gyros);

            ReplaceContents(
                parkConnectors,
                snapshot.ParkConnectors);

            ReplaceContents(
                parkLandingGears,
                snapshot.ParkLandingGears);

            ReplaceContents(
                connectorEdges,
                snapshot.ConnectorEdges);

            ReplaceContents(
                topologyConnectors,
                snapshot.TopologyConnectors);

            ReplaceContents(
                parkTimers,
                snapshot.ParkTimers);

            ReplaceContents(
                unparkTimers,
                snapshot.UnparkTimers);

            ReplaceContents(
                statusSurfaces,
                snapshot.StatusSurfaces);

            gridNodes.Clear();

            foreach (
                KeyValuePair<long, GridNode> pair
                in snapshot.GridNodes)
            {
                gridNodes.Add(
                    pair.Key,
                    pair.Value);
            }

            // Restore the physical disabled state before capacity or authority
            // is evaluated using the new wrappers.
            for (int i = 0;
                i < thrusters.Count;
                i++)
            {
                RestorePersistedThrusterState(
                    thrusters[i]);
            }

            PruneDisabledThrusterStates(
                newThrusterIds);

            SelectReferenceController();
            RefreshMainGridThrusterState();
            RefreshTemporaryControlRoles();

            // Clear any stale output on wrappers that still exist but no
            // longer have normal or temporary authority.
            for (int i = 0;
                i < thrusters.Count;
                i++)
            {
                if (!thrusters[i].Controlled)
                {
                    thrusters[i]
                        .ClearOverride();
                }
            }

            for (int i = 0;
                i < controlledGyros.Count;
                i++)
            {
                if (!controlledGyros[i]
                        .Controlled)
                {
                    controlledGyros[i]
                        .ReleaseOverride();
                }
            }

            if (mode ==
                OperatingMode.Parked)
            {
                EnsureNewCacheIsParked();
            }

            SweepOwnedHeartbeatsAfterScan();

            lastTopologyFingerprint =
                CalculateTopologyFingerprint();

            topologyFingerprintInitialized =
                true;

            forceStatusRefresh =
                true;
        }

        // ===== Status surface discovery =====

        void DiscoverStatusSurfaces(
            ScanSnapshot snapshot)
        {
            HashSet<string> addedSurfaces =
                new HashSet<string>(
                    StringComparer.Ordinal);

            List<int> selectedIndices =
                new List<int>();

            for (int i = 0;
                i < snapshot.Blocks.Count;
                i++)
            {
                IMyTerminalBlock block =
                    snapshot.Blocks[i];

                GridNode node;

                if (!snapshot
                        .GridNodes
                        .TryGetValue(
                            block
                                .CubeGrid
                                .EntityId,
                            out node) ||
                    node.Component !=
                        snapshot.RootComponent)
                {
                    continue;
                }

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        block.EntityId);

                IMyTextPanel panel =
                    block as
                        IMyTextPanel;

                if (panel != null &&
                    (tags &
                     BlockTags.Status) != 0)
                {
                    AddStatusSurface(
                        snapshot
                            .StatusSurfaces,
                        addedSurfaces,
                        block,
                        panel,
                        0);
                }

                IMyTextSurfaceProvider provider =
                    block as
                        IMyTextSurfaceProvider;

                if (provider == null ||
                    provider.SurfaceCount <= 0)
                {
                    continue;
                }

                selectedIndices.Clear();

                GetSelectedSurfaceIndices(
                    block.CustomData,
                    provider.SurfaceCount,
                    selectedIndices);

                if ((tags &
                     BlockTags.Status) != 0 &&
                    selectedIndices.Count ==
                        0)
                {
                    selectedIndices.Add(
                        0);
                }

                for (int index = 0;
                    index <
                        selectedIndices.Count;
                    index++)
                {
                    int surfaceIndex =
                        selectedIndices[index];

                    AddStatusSurface(
                        snapshot
                            .StatusSurfaces,
                        addedSurfaces,
                        block,
                        provider.GetSurface(
                            surfaceIndex),
                        surfaceIndex);
                }
            }
        }

        void GetSelectedSurfaceIndices(
            string customData,
            int surfaceCount,
            List<int> output)
        {
            if (string.IsNullOrEmpty(
                    customData))
            {
                return;
            }

            string[] lines =
                customData
                    .Replace(
                        "\r",
                        string.Empty)
                    .Split('\n');

            for (int i = 0;
                i < lines.Length;
                i++)
            {
                string line =
                    lines[i].Trim();

                if (!line.StartsWith(
                        SurfaceSelector,
                        StringComparison
                            .OrdinalIgnoreCase))
                {
                    continue;
                }

                string serializedIndex =
                    line
                        .Substring(
                            SurfaceSelector
                                .Length)
                        .Trim();

                int index;

                if (!int.TryParse(
                        serializedIndex,
                        out index) ||
                    index < 0 ||
                    index >= surfaceCount ||
                    output.Contains(
                        index))
                {
                    continue;
                }

                output.Add(
                    index);
            }
        }

        static void AddStatusSurface(
            List<StatusSurface> output,
            HashSet<string> added,
            IMyTerminalBlock owner,
            IMyTextSurface surface,
            int surfaceIndex)
        {
            string key =
                owner.EntityId +
                ":" +
                surfaceIndex;

            if (!added.Add(key))
            {
                return;
            }

            output.Add(
                new StatusSurface(
                    owner,
                    surface,
                    surfaceIndex));
        }

        // ===== Discovery policy =====

        bool CanReadParkingBlock(
            BlockTags tags)
        {
            if ((tags &
                 BlockTags.Ignore) != 0)
            {
                return false;
            }

            return settings.Greedy ||
                   (tags &
                    BlockTags.Use) != 0;
        }

        bool IsReduxProgrammableBlock(
            IMyProgrammableBlock programmableBlock)
        {
            if (programmableBlock ==
                null)
            {
                return false;
            }

            return FindSectionStart(
                       programmableBlock
                           .CustomData,
                       ConfigSection) >=
                   0;
        }

        bool ReadReduxCanSlave(
            IMyProgrammableBlock programmableBlock)
        {
            string serialized;

            if (!TryReadSectionValue(
                    programmableBlock.CustomData,
                    ConfigSection,
                    "CanSlave",
                    out serialized))
            {
                return true;
            }

            bool value;

            return bool.TryParse(
                       serialized,
                       out value)
                ? value
                : true;
        }

        BlockTags GetTagsFromText(
            string text)
        {
            if (string.IsNullOrEmpty(
                    text))
            {
                return BlockTags.None;
            }

            BlockTags result =
                BlockTags.None;

            if (ContainsTag(
                    text,
                    settings.UseTag))
            {
                result |=
                    BlockTags.Use;
            }

            if (ContainsTag(
                    text,
                    settings.IgnoreTag))
            {
                result |=
                    BlockTags.Ignore;
            }

            if (ContainsTag(
                    text,
                    settings.StatusTag))
            {
                result |=
                    BlockTags.Status;
            }

            if (ContainsTag(
                    text,
                    settings.ParkTimerTag))
            {
                result |=
                    BlockTags.ParkTimer;
            }

            if (ContainsTag(
                    text,
                    settings.UnparkTimerTag))
            {
                result |=
                    BlockTags.UnparkTimer;
            }

            return result;
        }

        static bool ContainsTag(
            string text,
            string tag)
        {
            return !string.IsNullOrEmpty(
                       text) &&
                   !string.IsNullOrEmpty(
                       tag) &&
                   text.IndexOf(
                       tag,
                       StringComparison
                           .OrdinalIgnoreCase) >=
                   0;
        }

        static void MergeTags(
            Dictionary<long, BlockTags> tags,
            long entityId,
            BlockTags additionalTags)
        {
            if (additionalTags ==
                BlockTags.None)
            {
                return;
            }

            BlockTags existing;

            tags.TryGetValue(
                entityId,
                out existing);

            tags[entityId] =
                existing |
                additionalTags;
        }

        static BlockTags GetTags(
            Dictionary<long, BlockTags> tags,
            long entityId)
        {
            BlockTags result;

            return tags.TryGetValue(
                       entityId,
                       out result)
                ? result
                : BlockTags.None;
        }

        // ===== General scan helpers =====

        static ConnectorEdge FindConnectorEdge(
            List<ConnectorEdge> edges,
            IMyShipConnector connector)
        {
            for (int i = 0;
                i < edges.Count;
                i++)
            {
                if (edges[i]
                            .A
                            .EntityId ==
                        connector.EntityId ||
                    edges[i]
                            .B
                            .EntityId ==
                        connector.EntityId)
                {
                    return edges[i];
                }
            }

            return null;
        }

        static void AddUniqueGrid(
            List<IMyCubeGrid> grids,
            IMyCubeGrid grid)
        {
            for (int i = 0;
                i < grids.Count;
                i++)
            {
                if (grids[i]
                        .EntityId ==
                    grid.EntityId)
                {
                    return;
                }
            }

            grids.Add(
                grid);
        }

        static void ReplaceContents<T>(
            List<T> target,
            List<T> source)
        {
            target.Clear();
            target.AddRange(
                source);
        }
    }
}
