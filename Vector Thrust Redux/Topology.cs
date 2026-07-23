// <mdk sortorder="75" />   // Topology.cs
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        readonly List<IMyTerminalBlock> topologyFingerprintBlocks =
            new List<IMyTerminalBlock>();

        readonly List<IMyBlockGroup> topologyFingerprintGroups =
            new List<IMyBlockGroup>();

        readonly List<IMyTerminalBlock> topologyFingerprintGroupBlocks =
            new List<IMyTerminalBlock>();

        // ===== Lightweight topology fingerprint =====

        void CheckTopologyFingerprint()
        {
            TopologyFingerprint fingerprint =
                CalculateTopologyFingerprint();

            if (!topologyFingerprintInitialized)
            {
                lastTopologyFingerprint =
                    fingerprint;

                topologyFingerprintInitialized =
                    true;

                // Startup already requests a deep scan. Do not immediately
                // request the same scan twice.
                return;
            }

            if (fingerprint ==
                lastTopologyFingerprint)
            {
                return;
            }

            lastTopologyFingerprint =
                fingerprint;

            RequestRescan();
        }

        TopologyFingerprint CalculateTopologyFingerprint()
        {
            TopologyFingerprint fingerprint =
                new TopologyFingerprint();

            topologyFingerprintBlocks.Clear();

            GridTerminalSystem.GetBlocks(
                topologyFingerprintBlocks);

            for (int i = 0;
                i <
                    topologyFingerprintBlocks
                        .Count;
                i++)
            {
                IMyTerminalBlock block =
                    topologyFingerprintBlocks[i];

                int type =
                    GetTopologyTypeDiscriminator(
                        block);

                if (type == 0)
                {
                    continue;
                }

                ulong entryHash =
                    TopologyHashOffset;

                entryHash =
                    MixTopologyHash(
                        entryHash,
                        unchecked(
                            (ulong)
                                block.EntityId));

                entryHash =
                    MixTopologyHash(
                        entryHash,
                        unchecked(
                            (ulong)
                                block
                                    .CubeGrid
                                    .EntityId));

                entryHash =
                    MixTopologyHash(
                        entryHash,
                        unchecked(
                            (ulong)type));

                entryHash =
                    MixTopologyHash(
                        entryHash,
                        block
                            .CubeGrid
                            .IsStatic
                            ? 1UL
                            : 0UL);

                BlockTags tags =
                    GetTagsFromText(
                        block.CustomName) |
                    GetTagsFromText(
                        block.CustomData);

                entryHash =
                    MixTopologyHash(
                        entryHash,
                        unchecked(
                            (ulong)(int)tags));

                IMyMotorStator rotor =
                    block as
                        IMyMotorStator;

                if (rotor != null)
                {
                    entryHash =
                        MixTopologyHash(
                            entryHash,
                            rotor.TopGrid !=
                                    null
                                ? unchecked(
                                    (ulong)
                                        rotor
                                            .TopGrid
                                            .EntityId)
                                : 0UL);

                    entryHash =
                        MixTopologyHash(
                            entryHash,
                            GetStableDoubleHash(
                                rotor
                                    .LowerLimitRad));

                    entryHash =
                        MixTopologyHash(
                            entryHash,
                            GetStableDoubleHash(
                                rotor
                                    .UpperLimitRad));
                }

                IMyPistonBase piston =
                    block as
                        IMyPistonBase;

                if (piston != null)
                {
                    entryHash =
                        MixTopologyHash(
                            entryHash,
                            piston.TopGrid !=
                                    null
                                ? unchecked(
                                    (ulong)
                                        piston
                                            .TopGrid
                                            .EntityId)
                                : 0UL);
                }

                IMyShipConnector connector =
                    block as
                        IMyShipConnector;

                if (connector != null)
                {
                    entryHash =
                        MixTopologyHash(
                            entryHash,
                            connector
                                    .OtherConnector !=
                                null
                                ? unchecked(
                                    (ulong)
                                        connector
                                            .OtherConnector
                                            .EntityId)
                                : 0UL);
                }

                IMyProgrammableBlock programmableBlock =
                    block as
                        IMyProgrammableBlock;

                if (programmableBlock !=
                    null)
                {
                    bool redux =
                        IsReduxProgrammableBlock(
                            programmableBlock);

                    entryHash =
                        MixTopologyHash(
                            entryHash,
                            redux
                                ? 1UL
                                : 0UL);

                    if (redux)
                    {
                        entryHash =
                            MixTopologyHash(
                                entryHash,
                                ReadReduxCanSlave(
                                    programmableBlock)
                                    ? 1UL
                                    : 0UL);

                        entryHash =
                            MixTopologyHash(
                                entryHash,
                                ReadReduxGreedy(
                                    programmableBlock)
                                    ? 1UL
                                    : 0UL);
                    }
                }

                AddTopologyEntry(
                    ref fingerprint,
                    entryHash,
                    block.EntityId);
            }

            AddGroupTopologyFingerprint(
                ref fingerprint);

            return fingerprint;
        }

        void AddGroupTopologyFingerprint(
            ref TopologyFingerprint fingerprint)
        {
            topologyFingerprintGroups.Clear();

            GridTerminalSystem.GetBlockGroups(
                topologyFingerprintGroups);

            for (int i = 0;
                i <
                    topologyFingerprintGroups
                        .Count;
                i++)
            {
                IMyBlockGroup group =
                    topologyFingerprintGroups[i];

                BlockTags groupTags =
                    GetTagsFromText(
                        group.Name);

                // Untagged groups cannot alter Redux discovery authority.
                if (groupTags ==
                    BlockTags.None)
                {
                    continue;
                }

                topologyFingerprintGroupBlocks.Clear();

                group.GetBlocks(
                    topologyFingerprintGroupBlocks);

                ulong groupHash =
                    TopologyHashOffset;

                groupHash =
                    MixTopologyHash(
                        groupHash,
                        HashStableText(
                            group.Name));

                groupHash =
                    MixTopologyHash(
                        groupHash,
                        unchecked(
                            (ulong)
                                (int)groupTags));

                ulong memberXor = 0;
                ulong memberSum = 0;

                for (int j = 0;
                    j <
                        topologyFingerprintGroupBlocks
                            .Count;
                    j++)
                {
                    ulong member =
                        MixTopologyHash(
                            TopologyHashOffset,
                            unchecked(
                                (ulong)
                                    topologyFingerprintGroupBlocks[j]
                                        .EntityId));

                    memberXor ^=
                        RotateLeft(
                            member,
                            (int)(
                                topologyFingerprintGroupBlocks[j]
                                    .EntityId &
                                63));

                    memberSum +=
                        member *
                        TopologyHashPrime;
                }

                groupHash =
                    MixTopologyHash(
                        groupHash,
                        memberXor);

                groupHash =
                    MixTopologyHash(
                        groupHash,
                        memberSum);

                long stableGroupIdentity =
                    unchecked(
                        (long)
                            HashStableText(
                                group.Name));

                AddTopologyEntry(
                    ref fingerprint,
                    groupHash,
                    stableGroupIdentity);
            }
        }

        static void AddTopologyEntry(
            ref TopologyFingerprint fingerprint,
            ulong entryHash,
            long identity)
        {
            fingerprint.Count++;

            fingerprint.Xor ^=
                RotateLeft(
                    entryHash,
                    (int)(identity & 63));

            fingerprint.Sum +=
                entryHash *
                TopologyHashPrime;
        }

        static int GetTopologyTypeDiscriminator(
            IMyTerminalBlock block)
        {
            if (block is
                IMyMotorStator)
            {
                return 1;
            }

            if (block is
                IMyPistonBase)
            {
                return 2;
            }

            if (block is
                IMyThrust)
            {
                return 3;
            }

            if (block is
                IMyGyro)
            {
                return 4;
            }

            if (block is
                IMyShipController)
            {
                return 5;
            }

            if (block is
                IMyShipConnector)
            {
                return 6;
            }

            if (block is
                IMyLandingGear)
            {
                return 7;
            }

            if (block is
                IMyProgrammableBlock)
            {
                return 8;
            }

            if (block is
                IMyTimerBlock)
            {
                return 9;
            }

            if (block is
                    IMyTextPanel ||
                block is
                    IMyTextSurfaceProvider)
            {
                return 10;
            }

            return 0;
        }

        static ulong GetStableDoubleHash(
            double value)
        {
            if (double.IsNaN(value))
            {
                return ulong.MaxValue;
            }

            if (double.IsPositiveInfinity(
                    value))
            {
                return ulong.MaxValue -
                       1;
            }

            if (double.IsNegativeInfinity(
                    value))
            {
                return ulong.MaxValue -
                       2;
            }

            long bits =
                BitConverter.DoubleToInt64Bits(
                    value);

            return unchecked(
                (ulong)bits);
        }

        static ulong HashStableText(
            string text)
        {
            ulong hash =
                TopologyHashOffset;

            if (string.IsNullOrEmpty(
                    text))
            {
                return hash;
            }

            for (int i = 0;
                i < text.Length;
                i++)
            {
                char character =
                    char.ToUpperInvariant(
                        text[i]);

                hash ^=
                    character;

                hash *=
                    TopologyHashPrime;
            }

            return hash;
        }

        static ulong MixTopologyHash(
            ulong hash,
            ulong value)
        {
            hash ^=
                value;

            hash *=
                TopologyHashPrime;

            hash ^=
                value >> 32;

            hash *=
                TopologyHashPrime;

            return hash;
        }

        static ulong RotateLeft(
            ulong value,
            int count)
        {
            count &=
                63;

            if (count == 0)
            {
                return value;
            }

            return value << count |
                   value >>
                   (64 - count);
        }

        const ulong TopologyHashOffset =
            14695981039346656037UL;

        const ulong TopologyHashPrime =
            1099511628211UL;

        // ===== Fast known-topology checks =====

        void DetectTopologyChanges()
        {
            HashSet<long> seenConnectors =
                new HashSet<long>();

            for (int i = 0;
                i < topologyConnectors.Count;
                i++)
            {
                IMyShipConnector connector =
                    topologyConnectors[i];

                if (connector == null ||
                    connector.Closed)
                {
                    continue;
                }

                long targetId =
                    connector
                            .OtherConnector !=
                        null
                        ? connector
                            .OtherConnector
                            .EntityId
                        : 0;

                long previousTarget;

                if (!observedConnectorTargets
                        .TryGetValue(
                            connector.EntityId,
                            out previousTarget) ||
                    previousTarget !=
                        targetId)
                {
                    observedConnectorTargets[
                        connector.EntityId] =
                        targetId;

                    RequestRescan();
                }

                seenConnectors.Add(
                    connector.EntityId);
            }

            RemoveUnseenConnectorObservations(
                seenConnectors);

            HashSet<long> seenLandingGears =
                new HashSet<long>();

            for (int i = 0;
                i < parkLandingGears.Count;
                i++)
            {
                IMyLandingGear gear =
                    parkLandingGears[i]
                        .Block;

                if (gear == null ||
                    gear.Closed)
                {
                    continue;
                }

                bool previous;

                if (!observedLandingGearLocks
                        .TryGetValue(
                            gear.EntityId,
                            out previous) ||
                    previous !=
                        gear.IsLocked)
                {
                    observedLandingGearLocks[
                        gear.EntityId] =
                        gear.IsLocked;

                    RequestRescan();
                }

                seenLandingGears.Add(
                    gear.EntityId);
            }

            RemoveUnseenLandingGearObservations(
                seenLandingGears);
        }

        void RemoveUnseenConnectorObservations(
            HashSet<long> seen)
        {
            if (observedConnectorTargets.Count ==
                0)
            {
                return;
            }

            List<long> removed =
                new List<long>();

            foreach (
                KeyValuePair<long, long> pair
                in observedConnectorTargets)
            {
                if (!seen.Contains(
                        pair.Key))
                {
                    removed.Add(
                        pair.Key);
                }
            }

            for (int i = 0;
                i < removed.Count;
                i++)
            {
                observedConnectorTargets.Remove(
                    removed[i]);
            }
        }

        void RemoveUnseenLandingGearObservations(
            HashSet<long> seen)
        {
            if (observedLandingGearLocks.Count ==
                0)
            {
                return;
            }

            List<long> removed =
                new List<long>();

            foreach (
                KeyValuePair<long, bool> pair
                in observedLandingGearLocks)
            {
                if (!seen.Contains(
                        pair.Key))
                {
                    removed.Add(
                        pair.Key);
                }
            }

            for (int i = 0;
                i < removed.Count;
                i++)
            {
                observedLandingGearLocks.Remove(
                    removed[i]);
            }
        }
    }
}
