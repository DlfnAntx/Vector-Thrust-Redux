// <mdk sortorder="90" />   // ScanControlModel.cs
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        IEnumerable<int> BuildSnapshotControlModel(
            ScanSnapshot snapshot)
        {
            Dictionary<long, Rotor> localRotorById =
                new Dictionary<long, Rotor>();

            Dictionary<long, IMyMotorStator> remoteRotorById =
                new Dictionary<long, IMyMotorStator>();

            Dictionary<long, List<Thruster>> localThrustersByRotor =
                new Dictionary<long, List<Thruster>>();

            Dictionary<long, List<Thruster>> remoteThrustersByRotor =
                new Dictionary<long, List<Thruster>>();

            // ===== Rotor candidates =====

            for (int i = 0;
                i <
                    snapshot.RawRotors.Count;
                i++)
            {
                IMyMotorStator block =
                    snapshot.RawRotors[i];

                GridNode node;

                if (!snapshot
                        .GridNodes
                        .TryGetValue(
                            block
                                .CubeGrid
                                .EntityId,
                            out node))
                {
                    continue;
                }

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        block.EntityId);

                if (node
                    .IncludedForControl)
                {
                    // Ownership is deliberately false here. It is finalized
                    // only after controlled thrusters have been associated.
                    Rotor rotor =
                        new Rotor(
                            block,
                            this,
                            tags,
                            false);

                    localRotorById.Add(
                        block.EntityId,
                        rotor);

                    snapshot
                        .RotorCandidates
                        .Add(rotor);
                }
                else if (IsRemoteReduxComponent(
                             snapshot,
                             node.Component))
                {
                    remoteRotorById.Add(
                        block.EntityId,
                        block);
                }

                yield return 1;
            }

            // ===== Thrusters =====

            for (int i = 0;
                i <
                    snapshot
                        .RawThrusters
                        .Count;
                i++)
            {
                IMyThrust block =
                    snapshot.RawThrusters[i];

                GridNode node;

                if (!snapshot
                        .GridNodes
                        .TryGetValue(
                            block
                                .CubeGrid
                                .EntityId,
                            out node))
                {
                    continue;
                }

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        block.EntityId);

                if (node.IncludedForControl)
                {
                    BuildLocalThrusterModel(
                        snapshot,
                        node,
                        block,
                        tags,
                        localRotorById,
                        localThrustersByRotor);
                }
                else if (IsRemoteReduxComponent(
                             snapshot,
                             node.Component))
                {
                    BuildRemoteReduxThrusterModel(
                        snapshot,
                        node,
                        block,
                        tags,
                        remoteRotorById,
                        remoteThrustersByRotor);
                }
                else if (node
                             .Component
                             .ReachableThroughConnection)
                {
                    Thruster thruster =
                        new Thruster(
                            block,
                            this,
                            tags,
                            false);

                    snapshot
                        .RemoteUnmanagedThrusters
                        .Add(thruster);

                    snapshot
                        .ObservedReadOnlyThrusters
                        .Add(thruster);
                }

                yield return 1;
            }

            FinalizeLocalRotorOwnership(
                snapshot,
                localThrustersByRotor);

            FinalizeRemoteRotorModels(
                snapshot,
                remoteRotorById,
                remoteThrustersByRotor);

            foreach (int step in
                BuildSnapshotGyros(
                    snapshot))
            {
                yield return step;
            }
        }

        void BuildLocalThrusterModel(
            ScanSnapshot snapshot,
            GridNode node,
            IMyThrust block,
            BlockTags tags,
            Dictionary<long, Rotor> rotorById,
            Dictionary<long, List<Thruster>> thrustersByRotor)
        {
            Rotor nearestRotor =
                FindNearestRotorCandidate(
                    node,
                    rotorById);

            bool onMechanicalSubgrid =
                node.Depth >
                0;

            bool explicitlyUsed =
                (tags &
                 BlockTags.Use) != 0;

            bool ignored =
                (tags &
                 BlockTags.Ignore) != 0;

            bool normallyControlled =
                !ignored &&
                (settings.Greedy
                    ? explicitlyUsed ||
                      onMechanicalSubgrid ||
                      nearestRotor != null
                    : explicitlyUsed);

            Thruster thruster =
                new Thruster(
                    block,
                    this,
                    tags,
                    normallyControlled);

            snapshot.Thrusters.Add(
                thruster);

            if (normallyControlled)
            {
                snapshot
                    .ControlledThrusters
                    .Add(thruster);
            }
            else
            {
                snapshot
                    .LocalUnmanagedThrusters
                    .Add(thruster);

                snapshot
                    .ObservedReadOnlyThrusters
                    .Add(thruster);
            }

            if (nearestRotor == null)
            {
                if (normallyControlled)
                {
                    snapshot
                        .FixedControlledThrusters
                        .Add(thruster);
                }

                return;
            }

            List<Thruster> associated;

            if (!thrustersByRotor
                    .TryGetValue(
                        nearestRotor.EntityId,
                        out associated))
            {
                associated =
                    new List<Thruster>();

                thrustersByRotor.Add(
                    nearestRotor.EntityId,
                    associated);
            }

            associated.Add(
                thruster);
        }

        void FinalizeLocalRotorOwnership(
            ScanSnapshot snapshot,
            Dictionary<long, List<Thruster>> thrustersByRotor)
        {
            for (int i = 0;
                i <
                    snapshot
                        .RotorCandidates
                        .Count;
                i++)
            {
                Rotor rotor =
                    snapshot
                        .RotorCandidates[i];

                List<Thruster> associated;

                bool hasAssociatedThrusters =
                    thrustersByRotor
                        .TryGetValue(
                            rotor.EntityId,
                            out associated);

                bool hasControlledThrusters =
                    false;

                if (hasAssociatedThrusters)
                {
                    for (int j = 0;
                        j < associated.Count;
                        j++)
                    {
                        if (associated[j]
                            .Controlled)
                        {
                            hasControlledThrusters =
                                true;

                            break;
                        }
                    }
                }

                // Ignore always wins. Outside Greedy mode the rotor itself
                // must be explicitly used; a used thruster does not silently
                // grant movement authority over an untagged mechanism.
                bool controlled =
                    !rotor.IsIgnored &&
                    (rotor.IsExplicitlyUsed ||
                     settings.Greedy &&
                     hasControlledThrusters);

                rotor.SetControlRole(
                    ControlRole.Normal,
                    controlled);

                if (!controlled)
                {
                    if (hasAssociatedThrusters)
                    {
                        for (int j = 0;
                            j < associated.Count;
                            j++)
                        {
                            Thruster thruster =
                                associated[j];

                            if (thruster.Controlled &&
                                !snapshot
                                    .FixedControlledThrusters
                                    .Contains(
                                        thruster))
                            {
                                snapshot
                                    .FixedControlledThrusters
                                    .Add(
                                        thruster);
                            }
                        }
                    }

                    continue;
                }

                snapshot
                    .ControlledRotors
                    .Add(rotor);

                if (!hasControlledThrusters)
                {
                    // Explicit [VT-use] grants authority, but without usable thrust
                    // there is no vector solution to calculate. Stop stale motion.
                    if (rotor.IsExplicitlyUsed)
                    {
                        rotor.Stop();
                    }

                    continue;
                }

                VectorThrust nacelle =
                    new VectorThrust(
                        rotor,
                        this);

                snapshot
                    .VectorThrusters
                    .Add(nacelle);

                for (int j = 0;
                    j < associated.Count;
                    j++)
                {
                    Thruster thruster =
                        associated[j];

                    if (!thruster.Controlled)
                    {
                        continue;
                    }

                    thruster.Nacelle =
                        nacelle;

                    nacelle.Thrusters.Add(
                        thruster);

                    GridNode thrusterNode;

                    if (snapshot
                            .GridNodes
                            .TryGetValue(
                                thruster
                                    .TheBlock
                                    .CubeGrid
                                    .EntityId,
                                out thrusterNode))
                    {
                        AddNacelleBranchGrids(
                            nacelle,
                            thrusterNode,
                            rotor);
                    }
                }

                nacelle
                    .RefreshPrimaryDirection();
            }
        }

        void BuildRemoteReduxThrusterModel(
            ScanSnapshot snapshot,
            GridNode node,
            IMyThrust block,
            BlockTags tags,
            Dictionary<long, IMyMotorStator> rotorById,
            Dictionary<long, List<Thruster>> thrustersByRotor)
        {
            GridComponent component =
                node.Component;

            IMyMotorStator nearestRotor =
                FindNearestRawRotor(
                    node,
                    rotorById);

            bool explicitlyUsed =
                (tags &
                 BlockTags.Use) != 0;

            bool ignored =
                (tags &
                 BlockTags.Ignore) != 0;

            // The connected master knows this component will temporarily be a
            // slave. Greedy slaves may therefore use their main-grid blocks as
            // well as their ordinary mechanical-subgrid blocks.
            bool controlledByRemoteRedux =
                !ignored &&
                component.ReduxCanSlave &&
                (component.ReduxGreedy ||
                 explicitlyUsed);

            Thruster thruster =
                new Thruster(
                    block,
                    this,
                    tags,
                    false);

            if (!controlledByRemoteRedux)
            {
                snapshot
                    .RemoteUnmanagedThrusters
                    .Add(thruster);

                snapshot
                    .ObservedReadOnlyThrusters
                    .Add(thruster);

                return;
            }

            snapshot
                .RemoteReduxThrusters
                .Add(thruster);

            if (nearestRotor == null)
            {
                snapshot
                    .RemoteFixedReduxThrusters
                    .Add(thruster);

                return;
            }

            List<Thruster> associated;

            if (!thrustersByRotor
                    .TryGetValue(
                        nearestRotor.EntityId,
                        out associated))
            {
                associated =
                    new List<Thruster>();

                thrustersByRotor.Add(
                    nearestRotor.EntityId,
                    associated);
            }

            associated.Add(
                thruster);
        }

        void FinalizeRemoteRotorModels(
            ScanSnapshot snapshot,
            Dictionary<long, IMyMotorStator> rotorById,
            Dictionary<long, List<Thruster>> thrustersByRotor)
        {
            foreach (
                KeyValuePair<long, List<Thruster>> pair
                in thrustersByRotor)
            {
                IMyMotorStator rotorBlock;

                if (!rotorById.TryGetValue(
                        pair.Key,
                        out rotorBlock))
                {
                    AddRemoteThrustersAsFixed(
                        snapshot,
                        pair.Value);

                    continue;
                }

                GridNode rotorNode;

                if (!snapshot
                        .GridNodes
                        .TryGetValue(
                            rotorBlock
                                .CubeGrid
                                .EntityId,
                            out rotorNode))
                {
                    AddRemoteThrustersAsFixed(
                        snapshot,
                        pair.Value);

                    continue;
                }

                BlockTags rotorTags =
                    GetTags(
                        snapshot.Tags,
                        rotorBlock.EntityId);

                bool ignored =
                    (rotorTags &
                     BlockTags.Ignore) != 0;

                bool explicitlyUsed =
                    (rotorTags &
                     BlockTags.Use) != 0;

                GridComponent component =
                    rotorNode.Component;

                bool controlledByRemoteRedux =
                    !ignored &&
                    component.ReduxCanSlave &&
                    (explicitlyUsed ||
                     component.ReduxGreedy &&
                     pair.Value.Count > 0);

                if (!controlledByRemoteRedux ||
                    rotorBlock.TopGrid == null ||
                    !rotorBlock.IsFunctional ||
                    !rotorBlock.Enabled ||
                    rotorBlock.RotorLock)
                {
                    AddRemoteThrustersAsFixed(
                        snapshot,
                        pair.Value);

                    continue;
                }

                RemoteNacelleCapacityModel model =
                    new RemoteNacelleCapacityModel(
                        rotorBlock);

                model.Thrusters.AddRange(
                    pair.Value);

                snapshot
                    .RemoteNacelles
                    .Add(model);
            }
        }

        static void AddRemoteThrustersAsFixed(
            ScanSnapshot snapshot,
            List<Thruster> thrusters)
        {
            for (int i = 0;
                i < thrusters.Count;
                i++)
            {
                Thruster thruster =
                    thrusters[i];

                if (!snapshot
                        .RemoteFixedReduxThrusters
                        .Contains(
                            thruster))
                {
                    snapshot
                        .RemoteFixedReduxThrusters
                        .Add(thruster);
                }
            }
        }

        IEnumerable<int> BuildSnapshotGyros(
            ScanSnapshot snapshot)
        {
            for (int i = 0;
                i <
                    snapshot.RawGyros.Count;
                i++)
            {
                IMyGyro block =
                    snapshot.RawGyros[i];

                GridNode node;

                if (!snapshot
                        .GridNodes
                        .TryGetValue(
                            block
                                .CubeGrid
                                .EntityId,
                            out node) ||
                    !node.IncludedForControl ||
                    !IsSupportedGyro(
                        block))
                {
                    continue;
                }

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        block.EntityId);

                bool ignored =
                    (tags &
                     BlockTags.Ignore) != 0;

                if (ignored)
                {
                    continue;
                }

                bool onMechanicalSubgrid =
                    node.Depth >
                    0;

                bool explicitlyUsed =
                    (tags &
                     BlockTags.Use) != 0;

                bool normalAuthority =
                    settings.Greedy
                        ? explicitlyUsed ||
                          onMechanicalSubgrid
                        : explicitlyUsed;

                bool mayNeedTemporarySlaveAuthority =
                    block.CubeGrid ==
                    Me.CubeGrid;

                if (!normalAuthority &&
                    !mayNeedTemporarySlaveAuthority)
                {
                    continue;
                }

                Gyro gyro =
                    new Gyro(
                        block,
                        this,
                        tags,
                        normalAuthority);

                snapshot.Gyros.Add(
                    gyro);

                yield return 1;
            }
        }

        Rotor FindNearestRotorCandidate(
            GridNode node,
            Dictionary<long, Rotor> rotorById)
        {
            GridNode current =
                node;

            while (current != null &&
                   current.Parent != null)
            {
                GridEdge edge =
                    current.ParentEdge;

                IMyMotorStator stator =
                    edge != null
                        ? edge.Mechanism as
                            IMyMotorStator
                        : null;

                if (stator != null &&
                    stator.TopGrid ==
                        current.Grid)
                {
                    Rotor rotor;

                    if (rotorById.TryGetValue(
                            stator.EntityId,
                            out rotor))
                    {
                        // The nearest physical joint owns the branch boundary
                        // even when ignored or locked. Never reach through it
                        // and start moving some unrelated ancestor joint.
                        return rotor;
                    }
                }

                current =
                    current.Parent;
            }

            return null;
        }

        IMyMotorStator FindNearestRawRotor(
            GridNode node,
            Dictionary<long, IMyMotorStator> rotorById)
        {
            GridNode current =
                node;

            while (current != null &&
                   current.Parent != null)
            {
                GridEdge edge =
                    current.ParentEdge;

                IMyMotorStator stator =
                    edge != null
                        ? edge.Mechanism as
                            IMyMotorStator
                        : null;

                if (stator != null &&
                    stator.TopGrid ==
                        current.Grid)
                {
                    IMyMotorStator rotor;

                    if (rotorById.TryGetValue(
                            stator.EntityId,
                            out rotor))
                    {
                        return rotor;
                    }
                }

                current =
                    current.Parent;
            }

            return null;
        }

        void AddNacelleBranchGrids(
            VectorThrust nacelle,
            GridNode thrusterNode,
            Rotor rotor)
        {
            GridNode current =
                thrusterNode;

            while (current != null)
            {
                AddUniqueGrid(
                    nacelle.BranchGrids,
                    current.Grid);

                if (current.ParentEdge !=
                        null &&
                    current.ParentEdge
                        .Mechanism
                        .EntityId ==
                    rotor.EntityId)
                {
                    break;
                }

                current =
                    current.Parent;
            }
        }

        static bool IsRemoteReduxComponent(
            ScanSnapshot snapshot,
            GridComponent component)
        {
            return component != null &&
                   component !=
                       snapshot.RootComponent &&
                   !component
                       .IncludedForControl &&
                   component
                       .ReachableThroughConnection &&
                   component
                       .PrimaryReduxProgrammableBlock !=
                       null;
        }

        bool IsSupportedGyro(
            IMyGyro gyro)
        {
            string subtype =
                gyro
                    .BlockDefinition
                    .SubtypeId;

            if (subtype.Equals(
                    "SmallBlockGyro",
                    StringComparison
                        .OrdinalIgnoreCase) ||
                subtype.Equals(
                    "LargeBlockGyro",
                    StringComparison
                        .OrdinalIgnoreCase))
            {
                return true;
            }

            return subtype.Equals(
                       "SmallPrototechGyro",
                       StringComparison
                           .OrdinalIgnoreCase) ||
                   subtype.Equals(
                       "LargePrototechGyro",
                       StringComparison
                           .OrdinalIgnoreCase) ||
                   subtype.Equals(
                       "SmallPrototechGyroscope",
                       StringComparison
                           .OrdinalIgnoreCase) ||
                   subtype.Equals(
                       "LargePrototechGyroscope",
                       StringComparison
                           .OrdinalIgnoreCase);
        }
    }
}
