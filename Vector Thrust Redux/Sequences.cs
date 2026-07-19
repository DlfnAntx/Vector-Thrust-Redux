// <mdk sortorder="40" />   // Sequences.cs
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        sealed class ScanSnapshot
        {
            public readonly List<IMyTerminalBlock> Blocks =
                new List<IMyTerminalBlock>();

            public readonly List<IMyShipController> Controllers =
                new List<IMyShipController>();

            public readonly List<IMyThrust> RawThrusters =
                new List<IMyThrust>();

            public readonly List<IMyMotorStator> RawRotors =
                new List<IMyMotorStator>();

            public readonly List<IMyPistonBase> RawPistons =
                new List<IMyPistonBase>();

            public readonly List<IMyGyro> RawGyros =
                new List<IMyGyro>();

            public readonly List<IMyShipConnector> RawConnectors =
                new List<IMyShipConnector>();

            public readonly List<IMyLandingGear> RawLandingGears =
                new List<IMyLandingGear>();

            public readonly List<IMyTimerBlock> RawTimers =
                new List<IMyTimerBlock>();

            public readonly List<IMyProgrammableBlock> RawProgrammableBlocks =
                new List<IMyProgrammableBlock>();

            public readonly Dictionary<long, BlockTags> Tags =
                new Dictionary<long, BlockTags>();

            public readonly Dictionary<long, GridNode> GridNodes =
                new Dictionary<long, GridNode>();

            public readonly List<GridComponent> Components =
                new List<GridComponent>();

            public readonly List<ConnectorEdge> ConnectorEdges =
                new List<ConnectorEdge>();

            public readonly List<IMyShipController> LocalControllers =
                new List<IMyShipController>();

            public readonly List<IMyShipController> RemoteControllers =
                new List<IMyShipController>();

            public readonly List<Thruster> Thrusters =
                new List<Thruster>();

            public readonly List<Thruster> ControlledThrusters =
                new List<Thruster>();

            public readonly List<Thruster> FixedControlledThrusters =
                new List<Thruster>();

            public readonly List<Thruster> ObservedReadOnlyThrusters =
                new List<Thruster>();

            public readonly List<Rotor> ControlledRotors =
                new List<Rotor>();

            public readonly List<VectorThrust> VectorThrusters =
                new List<VectorThrust>();

            public readonly List<VectorThrustGroup> Groups =
                new List<VectorThrustGroup>();

            public readonly List<Gyro> ControlledGyros =
                new List<Gyro>();

            public readonly List<ParkConnector> ParkConnectors =
                new List<ParkConnector>();

            public readonly List<ParkLandingGear> ParkLandingGears =
                new List<ParkLandingGear>();

            public readonly List<IMyShipConnector> TopologyConnectors =
                new List<IMyShipConnector>();

            public readonly List<IMyTimerBlock> ParkTimers =
                new List<IMyTimerBlock>();

            public readonly List<IMyTimerBlock> UnparkTimers =
                new List<IMyTimerBlock>();

            public readonly List<StatusSurface> StatusSurfaces =
                new List<StatusSurface>();

            public GridComponent RootComponent;
        }

        readonly List<ConnectorEdge> connectorEdges =
            new List<ConnectorEdge>();

        readonly List<IMyShipConnector> topologyConnectors =
            new List<IMyShipConnector>();

        readonly Dictionary<long, long> observedConnectorTargets =
            new Dictionary<long, long>();

        readonly Dictionary<long, bool> observedLandingGearLocks =
            new Dictionary<long, bool>();

        void RequestRescan()
        {
            rescanRequested = true;
        }

        void ContinueRescan()
        {
            if (scanStateMachine == null)
            {
                if (!rescanRequested)
                {
                    return;
                }

                rescanRequested = false;
                scanStateMachine = ScanConstruct().GetEnumerator();
            }

            int maximumInstructions = Runtime.MaxInstructionCount;
            int instructionBudget =
                Math.Max(1000, maximumInstructions * 3 / 4);

            int steps = 0;

            while (scanStateMachine != null &&
                   Runtime.CurrentInstructionCount <
                       instructionBudget &&
                   steps < 512)
            {
                steps++;

                if (scanStateMachine.MoveNext())
                {
                    continue;
                }

                scanStateMachine.Dispose();
                scanStateMachine = null;

                if (rescanRequested)
                {
                    rescanRequested = false;
                    scanStateMachine =
                        ScanConstruct().GetEnumerator();
                }
            }
        }

        IEnumerable<int> ScanConstruct()
        {
            ScanSnapshot snapshot = new ScanSnapshot();

            // ===== Collect blocks =====

            GridTerminalSystem.GetBlocks(snapshot.Blocks);

            List<IMyBlockGroup> groups =
                new List<IMyBlockGroup>();

            GridTerminalSystem.GetBlockGroups(groups);

            List<IMyTerminalBlock> groupBlocks =
                new List<IMyTerminalBlock>();

            for (int i = 0; i < groups.Count; i++)
            {
                IMyBlockGroup group = groups[i];

                BlockTags groupTags =
                    GetTagsFromText(group.Name);

                if (groupTags == BlockTags.None)
                {
                    continue;
                }

                groupBlocks.Clear();
                group.GetBlocks(groupBlocks);

                for (int j = 0;
                    j < groupBlocks.Count;
                    j++)
                {
                    MergeTags(
                        snapshot.Tags,
                        groupBlocks[j].EntityId,
                        groupTags);
                }

                yield return 1;
            }

            for (int i = 0;
                i < snapshot.Blocks.Count;
                i++)
            {
                IMyTerminalBlock block =
                    snapshot.Blocks[i];

                BlockTags directTags =
                    GetTagsFromText(block.CustomName) |
                    GetTagsFromText(block.CustomData);

                MergeTags(
                    snapshot.Tags,
                    block.EntityId,
                    directTags);

                GetOrCreateNode(
                    snapshot.GridNodes,
                    block.CubeGrid);

                IMyShipController controller =
                    block as IMyShipController;

                if (controller != null)
                {
                    snapshot.Controllers.Add(controller);
                }

                IMyThrust thrust =
                    block as IMyThrust;

                if (thrust != null)
                {
                    snapshot.RawThrusters.Add(thrust);
                }

                IMyMotorStator rotor =
                    block as IMyMotorStator;

                if (rotor != null)
                {
                    snapshot.RawRotors.Add(rotor);
                }

                IMyPistonBase piston =
                    block as IMyPistonBase;

                if (piston != null)
                {
                    snapshot.RawPistons.Add(piston);
                }

                IMyGyro gyro =
                    block as IMyGyro;

                if (gyro != null)
                {
                    snapshot.RawGyros.Add(gyro);
                }

                IMyShipConnector connector =
                    block as IMyShipConnector;

                if (connector != null)
                {
                    snapshot.RawConnectors.Add(connector);
                }

                IMyLandingGear landingGear =
                    block as IMyLandingGear;

                if (landingGear != null)
                {
                    snapshot.RawLandingGears.Add(
                        landingGear);
                }

                IMyTimerBlock timer =
                    block as IMyTimerBlock;

                if (timer != null)
                {
                    snapshot.RawTimers.Add(timer);
                }

                IMyProgrammableBlock programmableBlock =
                    block as IMyProgrammableBlock;

                if (programmableBlock != null)
                {
                    snapshot.RawProgrammableBlocks.Add(
                        programmableBlock);
                }

                yield return 1;
            }

            GridNode rootNode =
                GetOrCreateNode(
                    snapshot.GridNodes,
                    Me.CubeGrid);

            // ===== Mechanical graph =====

            for (int i = 0;
                i < snapshot.RawRotors.Count;
                i++)
            {
                IMyMotorStator rotor =
                    snapshot.RawRotors[i];

                if (rotor.TopGrid == null)
                {
                    continue;
                }

                AddMechanicalEdge(
                    snapshot.GridNodes,
                    rotor.CubeGrid,
                    rotor.TopGrid,
                    rotor);

                yield return 1;
            }

            for (int i = 0;
                i < snapshot.RawPistons.Count;
                i++)
            {
                IMyPistonBase piston =
                    snapshot.RawPistons[i];

                if (piston.TopGrid == null)
                {
                    continue;
                }

                AddMechanicalEdge(
                    snapshot.GridNodes,
                    piston.CubeGrid,
                    piston.TopGrid,
                    piston);

                yield return 1;
            }

            // Connector counterpart grids may not otherwise contain a block
            // visible during the first pass.
            for (int i = 0;
                i < snapshot.RawConnectors.Count;
                i++)
            {
                IMyShipConnector other =
                    snapshot.RawConnectors[i]
                        .OtherConnector;

                if (other != null)
                {
                    GetOrCreateNode(
                        snapshot.GridNodes,
                        other.CubeGrid);
                }

                yield return 1;
            }

            BuildMechanicalComponents(snapshot);

            // ===== Component metadata =====

            for (int i = 0;
                i < snapshot.Controllers.Count;
                i++)
            {
                IMyShipController controller =
                    snapshot.Controllers[i];

                GridNode node;

                if (!snapshot.GridNodes.TryGetValue(
                        controller.CubeGrid.EntityId,
                        out node))
                {
                    continue;
                }

                node.Component.Controllers.Add(controller);

                if (controller.CubeGrid == Me.CubeGrid)
                {
                    snapshot.LocalControllers.Add(
                        controller);
                }

                yield return 1;
            }

            for (int i = 0;
                i < snapshot.RawProgrammableBlocks.Count;
                i++)
            {
                IMyProgrammableBlock programmableBlock =
                    snapshot.RawProgrammableBlocks[i];

                if (!IsReduxProgrammableBlock(
                        programmableBlock))
                {
                    continue;
                }

                GridNode node;

                if (!snapshot.GridNodes.TryGetValue(
                        programmableBlock
                            .CubeGrid.EntityId,
                        out node))
                {
                    continue;
                }

                node.Component
                    .ReduxProgrammableBlocks
                    .Add(programmableBlock);

                if (ReadReduxCanSlave(
                        programmableBlock))
                {
                    node.Component
                        .HasSlaveCapableRedux = true;
                }

                yield return 1;
            }

            // ===== Connector graph =====

            HashSet<long> connectorPairs =
                new HashSet<long>();

            for (int i = 0;
                i < snapshot.RawConnectors.Count;
                i++)
            {
                IMyShipConnector connector =
                    snapshot.RawConnectors[i];

                IMyShipConnector other =
                    connector.OtherConnector;

                if (other == null)
                {
                    continue;
                }

                long smaller = Math.Min(
                    connector.EntityId,
                    other.EntityId);

                long larger = Math.Max(
                    connector.EntityId,
                    other.EntityId);

                // Entity IDs are positive in normal game operation. This hash
                // is only for de-duplicating both ends of the same connector
                // pair during one scan.
                long pairKey =
                    unchecked(smaller * 397L ^ larger);

                if (!connectorPairs.Add(pairKey))
                {
                    continue;
                }

                GridNode nodeA;
                GridNode nodeB;

                if (!snapshot.GridNodes.TryGetValue(
                        connector.CubeGrid.EntityId,
                        out nodeA) ||
                    !snapshot.GridNodes.TryGetValue(
                        other.CubeGrid.EntityId,
                        out nodeB))
                {
                    continue;
                }

                snapshot.ConnectorEdges.Add(
                    new ConnectorEdge
                    {
                        A = connector,
                        B = other,
                        NodeA = nodeA,
                        NodeB = nodeB
                    });

                yield return 1;
            }

            snapshot.RootComponent =
                rootNode.Component;

            MarkConnectedReachability(snapshot);
            IncludeControlledComponents(
                snapshot,
                rootNode);

            // ===== Remote heartbeat controllers =====

            for (int i = 0;
                i < snapshot.Components.Count;
                i++)
            {
                GridComponent component =
                    snapshot.Components[i];

                if (!component
                        .ReachableThroughConnection ||
                    component.IncludedForControl ||
                    component
                        .ReduxProgrammableBlocks.Count == 0)
                {
                    continue;
                }

                for (int j = 0;
                    j < component.Controllers.Count;
                    j++)
                {
                    snapshot.RemoteControllers.Add(
                        component.Controllers[j]);
                }

                yield return 1;
            }

            // ===== Rotor wrappers =====

            Dictionary<long, Rotor> rotorByEntityId =
                new Dictionary<long, Rotor>();

            for (int i = 0;
                i < snapshot.RawRotors.Count;
                i++)
            {
                IMyMotorStator block =
                    snapshot.RawRotors[i];

                GridNode node;

                if (!snapshot.GridNodes.TryGetValue(
                        block.CubeGrid.EntityId,
                        out node) ||
                    !node.IncludedForControl)
                {
                    continue;
                }

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        block.EntityId);

                bool controlled =
                    CanControlGeneralBlock(tags);

                Rotor rotor = new Rotor(
                    block,
                    this,
                    tags,
                    controlled);

                rotorByEntityId.Add(
                    block.EntityId,
                    rotor);

                if (controlled)
                {
                    snapshot.ControlledRotors.Add(
                        rotor);
                }

                yield return 1;
            }

            Dictionary<long, VectorThrust> nacelleByRotor =
                new Dictionary<long, VectorThrust>();

            // ===== Thruster authority and nacelles =====

            for (int i = 0;
                i < snapshot.RawThrusters.Count;
                i++)
            {
                IMyThrust block =
                    snapshot.RawThrusters[i];

                GridNode node;

                if (!snapshot.GridNodes.TryGetValue(
                        block.CubeGrid.EntityId,
                        out node))
                {
                    continue;
                }

                bool physicallyRelevant =
                    node.IncludedForControl ||
                    node.Component
                        .ReachableThroughConnection;

                if (!physicallyRelevant)
                {
                    continue;
                }

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        block.EntityId);

                Rotor nearestMovableRotor =
                    FindNearestMovableRotor(
                        node,
                        rotorByEntityId);

                bool onMechanicalSubgrid =
                    node.IncludedForControl &&
                    node.Depth > 0;

                bool controlled =
                    CanControlThruster(
                        tags,
                        node.IncludedForControl,
                        onMechanicalSubgrid,
                        nearestMovableRotor);

                Thruster thruster = new Thruster(
                    block,
                    this,
                    tags,
                    controlled);

                snapshot.Thrusters.Add(thruster);

                if (!controlled)
                {
                    snapshot
                        .ObservedReadOnlyThrusters
                        .Add(thruster);

                    yield return 1;
                    continue;
                }

                snapshot.ControlledThrusters.Add(
                    thruster);

                if (nearestMovableRotor == null ||
                    !nearestMovableRotor.Controlled)
                {
                    snapshot
                        .FixedControlledThrusters
                        .Add(thruster);

                    yield return 1;
                    continue;
                }

                VectorThrust nacelle;

                if (!nacelleByRotor.TryGetValue(
                        nearestMovableRotor.EntityId,
                        out nacelle))
                {
                    nacelle = new VectorThrust(
                        nearestMovableRotor,
                        this);

                    nacelleByRotor.Add(
                        nearestMovableRotor.EntityId,
                        nacelle);

                    snapshot.VectorThrusters.Add(
                        nacelle);
                }

                thruster.Nacelle = nacelle;
                nacelle.Thrusters.Add(thruster);

                AddNacelleBranchGrids(
                    nacelle,
                    node,
                    nearestMovableRotor);

                yield return 1;
            }

            // A Redux-owned rotor without an assigned nacelle must not retain
            // a velocity left behind by an earlier topology.
            for (int i = 0;
                i < snapshot.ControlledRotors.Count;
                i++)
            {
                Rotor rotor =
                    snapshot.ControlledRotors[i];

                if (rotor.Nacelle == null &&
                    Math.Abs(
                        rotor.TheBlock
                            .TargetVelocityRad) >
                        JointWriteDeadbandRad)
                {
                    rotor.TheBlock
                        .TargetVelocityRad = 0;
                }

                yield return 1;
            }

            for (int i = 0;
                i < snapshot.VectorThrusters.Count;
                i++)
            {
                snapshot.VectorThrusters[i]
                    .RefreshPrimaryDirection();

                AddNacelleToGroup(
                    snapshot.Groups,
                    snapshot.VectorThrusters[i]);

                yield return 1;
            }

            // ===== Gyro authority =====

            for (int i = 0;
                i < snapshot.RawGyros.Count;
                i++)
            {
                IMyGyro block =
                    snapshot.RawGyros[i];

                GridNode node;

                if (!snapshot.GridNodes.TryGetValue(
                        block.CubeGrid.EntityId,
                        out node) ||
                    !node.IncludedForControl ||
                    !IsSupportedGyro(block))
                {
                    continue;
                }

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        block.EntityId);

                bool controlled =
                    CanControlGyro(
                        tags,
                        node.Depth > 0);

                if (!controlled)
                {
                    continue;
                }

                snapshot.ControlledGyros.Add(
                    new Gyro(
                        block,
                        this,
                        tags,
                        true));

                yield return 1;
            }

            // ===== Parking blocks =====

            for (int i = 0;
                i < snapshot.RawConnectors.Count;
                i++)
            {
                IMyShipConnector block =
                    snapshot.RawConnectors[i];

                GridNode node;

                if (!snapshot.GridNodes.TryGetValue(
                        block.CubeGrid.EntityId,
                        out node) ||
                    !node.IncludedForControl)
                {
                    continue;
                }

                snapshot.TopologyConnectors.Add(
                    block);

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        block.EntityId);

                if (!CanReadParkingBlock(tags))
                {
                    continue;
                }

                snapshot.ParkConnectors.Add(
                    new ParkConnector
                    {
                        Block = block,
                        Edge = FindConnectorEdge(
                            snapshot.ConnectorEdges,
                            block)
                    });

                yield return 1;
            }

            for (int i = 0;
                i < snapshot.RawLandingGears.Count;
                i++)
            {
                IMyLandingGear block =
                    snapshot.RawLandingGears[i];

                GridNode node;

                if (!snapshot.GridNodes.TryGetValue(
                        block.CubeGrid.EntityId,
                        out node) ||
                    !node.IncludedForControl)
                {
                    continue;
                }

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        block.EntityId);

                if (!CanReadParkingBlock(tags))
                {
                    continue;
                }

                snapshot.ParkLandingGears.Add(
                    new ParkLandingGear
                    {
                        Block = block
                    });

                yield return 1;
            }

            // ===== Local timer hooks =====

            for (int i = 0;
                i < snapshot.RawTimers.Count;
                i++)
            {
                IMyTimerBlock timer =
                    snapshot.RawTimers[i];

                // Requiring the PB grid prevents two docked ships with the
                // same timer tags from setting each other's machinery off.
                if (timer.CubeGrid != Me.CubeGrid)
                {
                    continue;
                }

                BlockTags tags =
                    GetTags(
                        snapshot.Tags,
                        timer.EntityId);

                if ((tags & BlockTags.ParkTimer) != 0)
                {
                    snapshot.ParkTimers.Add(timer);
                }

                if ((tags & BlockTags.UnparkTimer) != 0)
                {
                    snapshot.UnparkTimers.Add(timer);
                }

                yield return 1;
            }

            DiscoverStatusSurfaces(snapshot);

            CommitScanSnapshot(snapshot);

            yield return 1;
        }

        void BuildMechanicalComponents(
            ScanSnapshot snapshot)
        {
            List<GridNode> queue =
                new List<GridNode>();

            foreach (KeyValuePair<long, GridNode> pair
                in snapshot.GridNodes)
            {
                GridNode seed = pair.Value;

                if (seed.Component != null)
                {
                    continue;
                }

                GridComponent component =
                    new GridComponent();

                snapshot.Components.Add(component);

                queue.Clear();
                queue.Add(seed);
                seed.Component = component;

                for (int index = 0;
                    index < queue.Count;
                    index++)
                {
                    GridNode node = queue[index];

                    component.Nodes.Add(node);

                    if (node.Grid.IsStatic)
                    {
                        component.HasStaticGrid = true;
                    }

                    for (int edgeIndex = 0;
                        edgeIndex <
                            node.MechanicalEdges.Count;
                        edgeIndex++)
                    {
                        GridNode neighbor =
                            node.MechanicalEdges[
                                edgeIndex]
                            .Other(node);

                        if (neighbor.Component != null)
                        {
                            continue;
                        }

                        neighbor.Component = component;
                        queue.Add(neighbor);
                    }
                }
            }
        }

        void MarkConnectedReachability(
            ScanSnapshot snapshot)
        {
            GridComponent root =
                snapshot.RootComponent;

            if (root == null)
            {
                return;
            }

            List<GridComponent> queue =
                new List<GridComponent>();

            root.ReachableThroughConnection = true;
            queue.Add(root);

            for (int index = 0;
                index < queue.Count;
                index++)
            {
                GridComponent component =
                    queue[index];

                for (int i = 0;
                    i < snapshot
                        .ConnectorEdges.Count;
                    i++)
                {
                    ConnectorEdge edge =
                        snapshot.ConnectorEdges[i];

                    GridComponent other = null;

                    if (edge.NodeA.Component ==
                        component)
                    {
                        other = edge.NodeB.Component;
                    }
                    else if (edge.NodeB.Component ==
                             component)
                    {
                        other = edge.NodeA.Component;
                    }

                    if (other == null ||
                        other
                            .ReachableThroughConnection)
                    {
                        continue;
                    }

                    other.ReachableThroughConnection =
                        true;

                    queue.Add(other);
                }
            }
        }

        void IncludeControlledComponents(
            ScanSnapshot snapshot,
            GridNode rootNode)
        {
            GridComponent root =
                snapshot.RootComponent;

            if (root == null)
            {
                return;
            }

            root.IncludedForControl = true;

            AssignComponentDepth(
                root,
                rootNode,
                0);

            bool addedComponent;

            do
            {
                addedComponent = false;

                for (int i = 0;
                    i < snapshot
                        .ConnectorEdges.Count;
                    i++)
                {
                    ConnectorEdge edge =
                        snapshot.ConnectorEdges[i];

                    bool aIncluded =
                        edge.NodeA.Component
                            .IncludedForControl;

                    bool bIncluded =
                        edge.NodeB.Component
                            .IncludedForControl;

                    if (aIncluded == bIncluded)
                    {
                        continue;
                    }

                    GridNode source =
                        aIncluded
                            ? edge.NodeA
                            : edge.NodeB;

                    GridNode target =
                        aIncluded
                            ? edge.NodeB
                            : edge.NodeA;

                    GridComponent targetComponent =
                        target.Component;

                    // Another running Redux instance owns its mechanical
                    // component, whether or not it accepts slave commands.
                    if (targetComponent
                            .ReduxProgrammableBlocks
                            .Count > 0)
                    {
                        continue;
                    }

                    if (!settings.CanMaster)
                    {
                        continue;
                    }

                    targetComponent
                        .IncludedForControl = true;

                    int sourceDepth =
                        source.Depth == int.MaxValue
                            ? 0
                            : source.Depth;

                    // Connector-attached cargo without its own Redux is treated
                    // at the attachment grid's depth, not as a fictitious extra
                    // mechanical joint.
                    AssignComponentDepth(
                        targetComponent,
                        target,
                        sourceDepth);

                    addedComponent = true;
                }
            }
            while (addedComponent);

            foreach (KeyValuePair<long, GridNode> pair
                in snapshot.GridNodes)
            {
                pair.Value.IncludedForControl =
                    pair.Value.Component
                        .IncludedForControl;
            }
        }

        void AssignComponentDepth(
            GridComponent component,
            GridNode start,
            int baseDepth)
        {
            List<GridNode> queue =
                new List<GridNode>();

            if (start.Depth > baseDepth)
            {
                start.Depth = baseDepth;
                start.Parent = null;
                start.ParentEdge = null;
            }

            queue.Add(start);

            for (int index = 0;
                index < queue.Count;
                index++)
            {
                GridNode node = queue[index];

                for (int i = 0;
                    i < node.MechanicalEdges.Count;
                    i++)
                {
                    GridEdge edge =
                        node.MechanicalEdges[i];

                    GridNode neighbor =
                        edge.Other(node);

                    if (neighbor.Component !=
                        component)
                    {
                        continue;
                    }

                    int proposedDepth =
                        node.Depth + 1;

                    if (proposedDepth >=
                        neighbor.Depth)
                    {
                        continue;
                    }

                    neighbor.Depth = proposedDepth;
                    neighbor.Parent = node;
                    neighbor.ParentEdge = edge;

                    queue.Add(neighbor);
                }
            }
        }

        Rotor FindNearestMovableRotor(
            GridNode node,
            Dictionary<long, Rotor> rotorByEntityId)
        {
            GridNode current = node;

            while (current != null &&
                   current.Parent != null)
            {
                GridEdge edge =
                    current.ParentEdge;

                IMyMotorStator stator =
                    edge != null
                        ? edge.Mechanism
                            as IMyMotorStator
                        : null;

                if (stator != null &&
                    stator.TopGrid ==
                        current.Grid)
                {
                    Rotor rotor;

                    if (rotorByEntityId.TryGetValue(
                            stator.EntityId,
                            out rotor) &&
                        rotor.IsPhysicallyMovable)
                    {
                        // The nearest physically movable joint defines the
                        // one-axis nacelle. If it is ignored/unowned, Redux
                        // treats everything beyond it as a dynamic static
                        // source rather than reaching through it to another
                        // joint and fighting the player.
                        return rotor.Controlled
                            ? rotor
                            : null;
                    }
                }

                current = current.Parent;
            }

            return null;
        }

        void AddNacelleBranchGrids(
            VectorThrust nacelle,
            GridNode thrusterNode,
            Rotor rotor)
        {
            GridNode current = thrusterNode;

            while (current != null)
            {
                AddUniqueGrid(
                    nacelle.BranchGrids,
                    current.Grid);

                if (current.ParentEdge != null &&
                    current.ParentEdge.Mechanism
                        .EntityId ==
                    rotor.EntityId)
                {
                    break;
                }

                current = current.Parent;
            }
        }

        void AddNacelleToGroup(
            List<VectorThrustGroup> groups,
            VectorThrust nacelle)
        {
            Vector3D axis =
                VectorMath.SafeNormalize(
                    nacelle.AxisWorld);

            for (int i = 0;
                i < groups.Count;
                i++)
            {
                Vector3D existingAxis =
                    groups[i].AxisWorld;

                if (Math.Abs(
                        Vector3D.Dot(
                            axis,
                            existingAxis)) <
                    ParallelAxisCosine)
                {
                    continue;
                }

                groups[i].Nacelles.Add(nacelle);
                return;
            }

            VectorThrustGroup group =
                new VectorThrustGroup();

            group.Nacelles.Add(nacelle);
            groups.Add(group);
        }

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

                if (!snapshot.GridNodes.TryGetValue(
                        block.CubeGrid.EntityId,
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
                    block as IMyTextPanel;

                if (panel != null &&
                    (tags & BlockTags.Status) != 0)
                {
                    AddStatusSurface(
                        snapshot.StatusSurfaces,
                        addedSurfaces,
                        block,
                        panel,
                        0);
                }

                IMyTextSurfaceProvider provider =
                    block as IMyTextSurfaceProvider;

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

                if ((tags & BlockTags.Status) != 0 &&
                    selectedIndices.Count == 0)
                {
                    selectedIndices.Add(0);
                }

                for (int index = 0;
                    index < selectedIndices.Count;
                    index++)
                {
                    int surfaceIndex =
                        selectedIndices[index];

                    AddStatusSurface(
                        snapshot.StatusSurfaces,
                        addedSurfaces,
                        block,
                        provider.GetSurface(
                            surfaceIndex),
                        surfaceIndex);
                }
            }
        }

        void CommitScanSnapshot(
            ScanSnapshot snapshot)
        {
            HashSet<long> newThrusterIds =
                new HashSet<long>();

            HashSet<long> newRotorIds =
                new HashSet<long>();

            HashSet<long> newGyroIds =
                new HashSet<long>();

            for (int i = 0;
                i < snapshot
                    .ControlledThrusters.Count;
                i++)
            {
                newThrusterIds.Add(
                    snapshot.ControlledThrusters[i]
                        .EntityId);
            }

            for (int i = 0;
                i < snapshot
                    .ControlledRotors.Count;
                i++)
            {
                newRotorIds.Add(
                    snapshot.ControlledRotors[i]
                        .EntityId);
            }

            for (int i = 0;
                i < snapshot
                    .ControlledGyros.Count;
                i++)
            {
                newGyroIds.Add(
                    snapshot.ControlledGyros[i]
                        .TheBlock.EntityId);
            }

            for (int i = 0;
                i < controlledThrusters.Count;
                i++)
            {
                Thruster oldThruster =
                    controlledThrusters[i];

                if (!newThrusterIds.Contains(
                        oldThruster.EntityId))
                {
                    oldThruster.Release();
                    RestoreParkedThruster(
                        oldThruster.EntityId,
                        oldThruster.TheBlock);
                }
            }

            for (int i = 0;
                i < controlledRotors.Count;
                i++)
            {
                Rotor oldRotor =
                    controlledRotors[i];

                if (!newRotorIds.Contains(
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
                        oldGyro.TheBlock.EntityId))
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
                controlledRotors,
                snapshot.ControlledRotors);

            ReplaceContents(
                vectorThrusters,
                snapshot.VectorThrusters);

            ReplaceContents(
                vectorThrustGroups,
                snapshot.Groups);

            ReplaceContents(
                controlledGyros,
                snapshot.ControlledGyros);

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

            foreach (KeyValuePair<long, GridNode> pair
                in snapshot.GridNodes)
            {
                gridNodes.Add(pair.Key, pair.Value);
            }

            SelectReferenceController();

            if (mode == OperatingMode.Parked)
            {
                EnsureNewCacheIsParked();
            }

            forceStatusRefresh = true;
        }

        BlockTags GetTagsFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BlockTags.None;
            }

            BlockTags result = BlockTags.None;

            if (ContainsTag(
                    text,
                    settings.UseTag))
            {
                result |= BlockTags.Use;
            }

            if (ContainsTag(
                    text,
                    settings.IgnoreTag))
            {
                result |= BlockTags.Ignore;
            }

            if (ContainsTag(
                    text,
                    settings.StatusTag))
            {
                result |= BlockTags.Status;
            }

            if (ContainsTag(
                    text,
                    settings.ParkTimerTag))
            {
                result |= BlockTags.ParkTimer;
            }

            if (ContainsTag(
                    text,
                    settings.UnparkTimerTag))
            {
                result |= BlockTags.UnparkTimer;
            }

            return result;
        }

        bool CanControlGeneralBlock(
            BlockTags tags)
        {
            if ((tags & BlockTags.Ignore) != 0)
            {
                return false;
            }

            return settings.Greedy ||
                   (tags & BlockTags.Use) != 0;
        }

        bool CanControlThruster(
            BlockTags tags,
            bool includedForControl,
            bool onMechanicalSubgrid,
            Rotor nearestMovableRotor)
        {
            if (!includedForControl ||
                (tags & BlockTags.Ignore) != 0)
            {
                return false;
            }

            bool explicitlyUsed =
                (tags & BlockTags.Use) != 0;

            if (!settings.Greedy)
            {
                return explicitlyUsed;
            }

            // Main-grid normal thrusters stay with the player unless they are
            // explicitly handed to Redux. Subgrid thrusters normally cannot
            // be controlled by the cockpit and are therefore script-owned.
            return explicitlyUsed ||
                   onMechanicalSubgrid ||
                   nearestMovableRotor != null;
        }

        bool CanControlGyro(
            BlockTags tags,
            bool onMechanicalSubgrid)
        {
            if ((tags & BlockTags.Ignore) != 0)
            {
                return false;
            }

            bool explicitlyUsed =
                (tags & BlockTags.Use) != 0;

            if (!settings.Greedy)
            {
                return explicitlyUsed;
            }

            // Main-grid gyros remain available to vanilla player control.
            return explicitlyUsed ||
                   onMechanicalSubgrid;
        }

        bool CanReadParkingBlock(
            BlockTags tags)
        {
            if ((tags & BlockTags.Ignore) != 0)
            {
                return false;
            }

            return settings.Greedy ||
                   (tags & BlockTags.Use) != 0;
        }

        bool IsSupportedGyro(IMyGyro gyro)
        {
            string subtype =
                gyro.BlockDefinition.SubtypeId;

            if (subtype.Equals(
                    "SmallBlockGyro",
                    StringComparison.OrdinalIgnoreCase) ||
                subtype.Equals(
                    "LargeBlockGyro",
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Keen has used both Gyro and Gyroscope suffixes in definition
            // naming across content branches. Why, just why?
            return subtype.Equals(
                       "SmallPrototechGyro",
                       StringComparison.OrdinalIgnoreCase) ||
                   subtype.Equals(
                       "LargePrototechGyro",
                       StringComparison.OrdinalIgnoreCase) ||
                   subtype.Equals(
                       "SmallPrototechGyroscope",
                       StringComparison.OrdinalIgnoreCase) ||
                   subtype.Equals(
                       "LargePrototechGyroscope",
                       StringComparison.OrdinalIgnoreCase);
        }

        bool IsReduxProgrammableBlock(
            IMyProgrammableBlock programmableBlock)
        {
            if (programmableBlock == null)
            {
                return false;
            }

            return FindSectionStart(
                       programmableBlock.CustomData,
                       ConfigSection) >= 0;
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

        void GetSelectedSurfaceIndices(
            string customData,
            int surfaceCount,
            List<int> output)
        {
            if (string.IsNullOrEmpty(customData))
            {
                return;
            }

            string[] lines = customData.Replace(
                    "\r",
                    string.Empty)
                .Split('\n');

            for (int i = 0;
                i < lines.Length;
                i++)
            {
                string line = lines[i].Trim();

                if (!line.StartsWith(
                        SurfaceSelector,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string serializedIndex =
                    line.Substring(
                        SurfaceSelector.Length)
                    .Trim();

                int index;

                if (!int.TryParse(
                        serializedIndex,
                        out index) ||
                    index < 0 ||
                    index >= surfaceCount ||
                    output.Contains(index))
                {
                    continue;
                }

                output.Add(index);
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

        static void AddUniqueGrid(
            List<IMyCubeGrid> grids,
            IMyCubeGrid grid)
        {
            for (int i = 0;
                i < grids.Count;
                i++)
            {
                if (grids[i].EntityId ==
                    grid.EntityId)
                {
                    return;
                }
            }

            grids.Add(grid);
        }

        static ConnectorEdge FindConnectorEdge(
            List<ConnectorEdge> edges,
            IMyShipConnector connector)
        {
            for (int i = 0;
                i < edges.Count;
                i++)
            {
                if (edges[i].A.EntityId ==
                        connector.EntityId ||
                    edges[i].B.EntityId ==
                        connector.EntityId)
                {
                    return edges[i];
                }
            }

            return null;
        }

        static GridNode GetOrCreateNode(
            Dictionary<long, GridNode> nodes,
            IMyCubeGrid grid)
        {
            GridNode node;

            if (!nodes.TryGetValue(
                    grid.EntityId,
                    out node))
            {
                node = new GridNode(grid);
                nodes.Add(grid.EntityId, node);
            }

            return node;
        }

        static void AddMechanicalEdge(
            Dictionary<long, GridNode> nodes,
            IMyCubeGrid gridA,
            IMyCubeGrid gridB,
            IMyTerminalBlock mechanism)
        {
            GridNode a =
                GetOrCreateNode(nodes, gridA);

            GridNode b =
                GetOrCreateNode(nodes, gridB);

            GridEdge edge =
                new GridEdge(a, b, mechanism);

            a.MechanicalEdges.Add(edge);
            b.MechanicalEdges.Add(edge);
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
                existing | additionalTags;
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

        static bool ContainsTag(
            string text,
            string tag)
        {
            return !string.IsNullOrEmpty(text) &&
                   !string.IsNullOrEmpty(tag) &&
                   text.IndexOf(
                       tag,
                       StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static void ReplaceContents<T>(
            List<T> target,
            List<T> source)
        {
            target.Clear();
            target.AddRange(source);
        }
    }
}
