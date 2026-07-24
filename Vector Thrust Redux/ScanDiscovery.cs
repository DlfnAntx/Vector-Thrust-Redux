// <mdk sortorder="85" />   // ScanDiscovery.cs
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        IEnumerable<int> ScanConstruct()
        {
            ScanSnapshot snapshot =
                new ScanSnapshot();

            foreach (int step in
                CollectSnapshotBlocks(
                    snapshot))
            {
                yield return step;
            }

            foreach (int step in
                BuildSnapshotMechanicalGraph(
                    snapshot))
            {
                yield return step;
            }

            BuildMechanicalComponents(
                snapshot);

            foreach (int step in
                BuildSnapshotComponentMetadata(
                    snapshot))
            {
                yield return step;
            }

            foreach (int step in
                BuildSnapshotConnectorGraph(
                    snapshot))
            {
                yield return step;
            }

            GridNode rootNode =
                GetOrCreateNode(
                    snapshot.GridNodes,
                    Me.CubeGrid);

            snapshot.RootComponent =
                rootNode.Component;

            MarkConnectedReachability(
                snapshot);

            IncludeControlledComponents(
                snapshot,
                rootNode);

            AssignRemoteReduxDepths(
                snapshot);

            foreach (int step in
                DiscoverRemoteControllers(
                    snapshot))
            {
                yield return step;
            }

            // Supplied by ScanControlModel.cs.
            foreach (int step in
                BuildSnapshotControlModel(
                    snapshot))
            {
                yield return step;
            }

            // Supplied by ScanCommit.cs.
            foreach (int step in
                BuildSnapshotParkingAndStatus(
                    snapshot))
            {
                yield return step;
            }

            CommitScanSnapshot(
                snapshot);

            yield return 1;
        }

        // ===== Block collection =====

        IEnumerable<int> CollectSnapshotBlocks(
            ScanSnapshot snapshot)
        {
            GridTerminalSystem.GetBlocks(
                snapshot.Blocks);

            List<IMyBlockGroup> groups =
                new List<IMyBlockGroup>();

            GridTerminalSystem.GetBlockGroups(
                groups);

            List<IMyTerminalBlock> groupBlocks =
                new List<IMyTerminalBlock>();

            for (int i = 0;
                i < groups.Count;
                i++)
            {
                IMyBlockGroup group =
                    groups[i];

                BlockTags groupTags =
                    GetTagsFromText(
                        group.Name);

                if (groupTags ==
                    BlockTags.None)
                {
                    continue;
                }

                groupBlocks.Clear();

                group.GetBlocks(
                    groupBlocks);

                for (int j = 0;
                    j < groupBlocks.Count;
                    j++)
                {
                    MergeTags(
                        snapshot.Tags,
                        groupBlocks[j]
                            .EntityId,
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
                    GetTagsFromText(
                        block.CustomName) |
                    GetTagsFromText(
                        block.CustomData);

                MergeTags(
                    snapshot.Tags,
                    block.EntityId,
                    directTags);

                GetOrCreateNode(
                    snapshot.GridNodes,
                    block.CubeGrid);

                IMyShipController controller =
                    block as
                        IMyShipController;

                if (controller != null)
                {
                    snapshot.Controllers.Add(
                        controller);
                }

                IMyThrust thrust =
                    block as
                        IMyThrust;

                if (thrust != null)
                {
                    snapshot.RawThrusters.Add(
                        thrust);
                }

                IMyMotorStator rotor =
                    block as
                        IMyMotorStator;

                if (rotor != null)
                {
                    snapshot.RawRotors.Add(
                        rotor);
                }

                IMyPistonBase piston =
                    block as
                        IMyPistonBase;

                if (piston != null)
                {
                    snapshot.RawPistons.Add(
                        piston);
                }

                IMyGyro gyro =
                    block as
                        IMyGyro;

                if (gyro != null)
                {
                    snapshot.RawGyros.Add(
                        gyro);
                }

                IMyShipConnector connector =
                    block as
                        IMyShipConnector;

                if (connector != null)
                {
                    snapshot.RawConnectors.Add(
                        connector);
                }

                IMyLandingGear landingGear =
                    block as
                        IMyLandingGear;

                if (landingGear != null)
                {
                    snapshot
                        .RawLandingGears
                        .Add(
                            landingGear);
                }

                IMyTimerBlock timer =
                    block as
                        IMyTimerBlock;

                if (timer != null)
                {
                    snapshot.RawTimers.Add(
                        timer);
                }

                IMyProgrammableBlock programmableBlock =
                    block as
                        IMyProgrammableBlock;

                if (programmableBlock != null)
                {
                    snapshot
                        .RawProgrammableBlocks
                        .Add(
                            programmableBlock);
                }

                yield return 1;
            }

            // Ensure the PB grid exists even if a temporarily incomplete
            // terminal query returned no other block on it.
            GetOrCreateNode(
                snapshot.GridNodes,
                Me.CubeGrid);
        }

        // ===== Mechanical graph =====

        IEnumerable<int> BuildSnapshotMechanicalGraph(
            ScanSnapshot snapshot)
        {
            for (int i = 0;
                i <
                    snapshot.RawRotors.Count;
                i++)
            {
                IMyMotorStator rotor =
                    snapshot.RawRotors[i];

                if (rotor.TopGrid ==
                    null)
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
                i <
                    snapshot.RawPistons.Count;
                i++)
            {
                IMyPistonBase piston =
                    snapshot.RawPistons[i];

                if (piston.TopGrid ==
                    null)
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

            // A counterpart grid may not otherwise contain a block visible
            // during the first terminal-system pass.
            for (int i = 0;
                i <
                    snapshot.RawConnectors.Count;
                i++)
            {
                IMyShipConnector other =
                    snapshot
                        .RawConnectors[i]
                        .OtherConnector;

                if (other != null)
                {
                    GetOrCreateNode(
                        snapshot.GridNodes,
                        other.CubeGrid);
                }

                yield return 1;
            }
        }

        void BuildMechanicalComponents(
            ScanSnapshot snapshot)
        {
            List<GridNode> queue =
                new List<GridNode>();

            foreach (
                KeyValuePair<long, GridNode> pair
                in snapshot.GridNodes)
            {
                GridNode seed =
                    pair.Value;

                if (seed.Component !=
                    null)
                {
                    continue;
                }

                GridComponent component =
                    new GridComponent();

                snapshot.Components.Add(
                    component);

                queue.Clear();
                queue.Add(seed);

                seed.Component =
                    component;

                for (int index = 0;
                    index < queue.Count;
                    index++)
                {
                    GridNode node =
                        queue[index];

                    component.Nodes.Add(
                        node);

                    if (node.Grid.IsStatic)
                    {
                        component.HasStaticGrid =
                            true;
                    }

                    for (int edgeIndex = 0;
                        edgeIndex <
                            node
                                .MechanicalEdges
                                .Count;
                        edgeIndex++)
                    {
                        GridNode neighbor =
                            node
                                .MechanicalEdges[
                                    edgeIndex]
                                .Other(node);

                        if (neighbor.Component !=
                            null)
                        {
                            continue;
                        }

                        neighbor.Component =
                            component;

                        queue.Add(
                            neighbor);
                    }
                }
            }
        }

        // ===== Component metadata =====

        IEnumerable<int> BuildSnapshotComponentMetadata(
            ScanSnapshot snapshot)
        {
            for (int i = 0;
                i <
                    snapshot.Controllers.Count;
                i++)
            {
                IMyShipController controller =
                    snapshot.Controllers[i];

                GridNode node;

                if (!snapshot
                        .GridNodes
                        .TryGetValue(
                            controller
                                .CubeGrid
                                .EntityId,
                            out node))
                {
                    continue;
                }

                node.Component
                    .Controllers
                    .Add(controller);

                if (controller.CubeGrid ==
                    Me.CubeGrid)
                {
                    snapshot
                        .LocalControllers
                        .Add(controller);
                }

                yield return 1;
            }

            for (int i = 0;
                i <
                    snapshot
                        .RawProgrammableBlocks
                        .Count;
                i++)
            {
                IMyProgrammableBlock programmableBlock =
                    snapshot
                        .RawProgrammableBlocks[i];

                if (!IsReduxProgrammableBlock(
                        programmableBlock))
                {
                    continue;
                }

                GridNode node;

                if (!snapshot
                        .GridNodes
                        .TryGetValue(
                            programmableBlock
                                .CubeGrid
                                .EntityId,
                            out node))
                {
                    continue;
                }

                GridComponent component =
                    node.Component;

                component
                    .ReduxProgrammableBlocks
                    .Add(
                        programmableBlock);

                bool canSlave =
                    ReadReduxCanSlave(
                        programmableBlock);

                if (canSlave)
                {
                    component
                        .HasSlaveCapableRedux =
                        true;
                }

                if (component
                        .PrimaryReduxProgrammableBlock ==
                        null ||
                    programmableBlock
                        .EntityId <
                    component
                        .PrimaryReduxProgrammableBlock
                        .EntityId)
                {
                    component
                        .PrimaryReduxProgrammableBlock =
                        programmableBlock;

                    component.ReduxGreedy =
                        ReadReduxGreedy(
                            programmableBlock);

                    component.ReduxCanSlave =
                        canSlave;
                }

                yield return 1;
            }
        }

        bool ReadReduxGreedy(
            IMyProgrammableBlock programmableBlock)
        {
            string serialized;

            if (!TryReadSectionValue(
                    programmableBlock.CustomData,
                    ConfigSection,
                    "Greedy",
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

        // ===== Connector graph =====

        IEnumerable<int> BuildSnapshotConnectorGraph(
            ScanSnapshot snapshot)
        {
            HashSet<ConnectorPairKey> connectorPairs =
                new HashSet<ConnectorPairKey>();

            for (int i = 0;
                i <
                    snapshot.RawConnectors.Count;
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

                ConnectorPairKey pair =
                    new ConnectorPairKey(
                        connector.EntityId,
                        other.EntityId);

                if (!connectorPairs.Add(
                        pair))
                {
                    continue;
                }

                GridNode nodeA;
                GridNode nodeB;

                if (!snapshot
                        .GridNodes
                        .TryGetValue(
                            connector
                                .CubeGrid
                                .EntityId,
                            out nodeA) ||
                    !snapshot
                        .GridNodes
                        .TryGetValue(
                            other
                                .CubeGrid
                                .EntityId,
                            out nodeB))
                {
                    continue;
                }

                snapshot
                    .ConnectorEdges
                    .Add(
                        new ConnectorEdge
                        {
                            A =
                                connector,

                            B =
                                other,

                            NodeA =
                                nodeA,

                            NodeB =
                                nodeB
                        });

                yield return 1;
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

            root.ReachableThroughConnection =
                true;

            queue.Add(root);

            for (int index = 0;
                index < queue.Count;
                index++)
            {
                GridComponent component =
                    queue[index];

                for (int i = 0;
                    i <
                        snapshot
                            .ConnectorEdges
                            .Count;
                    i++)
                {
                    ConnectorEdge edge =
                        snapshot
                            .ConnectorEdges[i];

                    GridComponent other =
                        null;

                    if (edge.NodeA.Component ==
                        component)
                    {
                        other =
                            edge.NodeB.Component;
                    }
                    else if (edge.NodeB.Component ==
                             component)
                    {
                        other =
                            edge.NodeA.Component;
                    }

                    if (other == null ||
                        other
                            .ReachableThroughConnection)
                    {
                        continue;
                    }

                    other
                        .ReachableThroughConnection =
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

            root.IncludedForControl =
                true;

            AssignComponentDepth(
                root,
                rootNode,
                0);

            bool addedComponent;

            do
            {
                addedComponent =
                    false;

                for (int i = 0;
                    i <
                        snapshot
                            .ConnectorEdges
                            .Count;
                    i++)
                {
                    ConnectorEdge edge =
                        snapshot
                            .ConnectorEdges[i];

                    bool aIncluded =
                        edge
                            .NodeA
                            .Component
                            .IncludedForControl;

                    bool bIncluded =
                        edge
                            .NodeB
                            .Component
                            .IncludedForControl;

                    if (aIncluded ==
                        bIncluded)
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

                    // A connected mechanical component with another Redux PB
                    // remains independently owned and may become a slave.
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
                        .IncludedForControl =
                        true;

                    int sourceDepth =
                        source.Depth ==
                            int.MaxValue
                            ? 0
                            : source.Depth;

                    AssignComponentDepth(
                        targetComponent,
                        target,
                        sourceDepth);

                    addedComponent =
                        true;
                }
            }
            while (addedComponent);

            foreach (
                KeyValuePair<long, GridNode> pair
                in snapshot.GridNodes)
            {
                pair.Value
                    .IncludedForControl =
                    pair.Value
                        .Component
                        .IncludedForControl;
            }
        }

        void AssignRemoteReduxDepths(
            ScanSnapshot snapshot)
        {
            for (int i = 0;
                i <
                    snapshot.Components.Count;
                i++)
            {
                GridComponent component =
                    snapshot.Components[i];

                if (component ==
                        snapshot.RootComponent ||
                    component.IncludedForControl ||
                    !component
                        .ReachableThroughConnection ||
                    component
                        .PrimaryReduxProgrammableBlock ==
                        null)
                {
                    continue;
                }

                GridNode rootNode;

                if (!snapshot
                        .GridNodes
                        .TryGetValue(
                            component
                                .PrimaryReduxProgrammableBlock
                                .CubeGrid
                                .EntityId,
                            out rootNode))
                {
                    continue;
                }

                AssignComponentDepth(
                    component,
                    rootNode,
                    0);
            }
        }

        void AssignComponentDepth(
            GridComponent component,
            GridNode start,
            int baseDepth)
        {
            List<GridNode> queue =
                new List<GridNode>();

            if (start.Depth >
                baseDepth)
            {
                start.Depth =
                    baseDepth;

                start.Parent =
                    null;

                start.ParentEdge =
                    null;
            }

            queue.Add(start);

            for (int index = 0;
                index < queue.Count;
                index++)
            {
                GridNode node =
                    queue[index];

                for (int i = 0;
                    i <
                        node
                            .MechanicalEdges
                            .Count;
                    i++)
                {
                    GridEdge edge =
                        node
                            .MechanicalEdges[i];

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

                    neighbor.Depth =
                        proposedDepth;

                    neighbor.Parent =
                        node;

                    neighbor.ParentEdge =
                        edge;

                    queue.Add(
                        neighbor);
                }
            }
        }

        IEnumerable<int> DiscoverRemoteControllers(
            ScanSnapshot snapshot)
        {
            for (int i = 0;
                i <
                    snapshot.Components.Count;
                i++)
            {
                GridComponent component =
                    snapshot.Components[i];

                if (!component
                        .ReachableThroughConnection ||
                    component
                        .IncludedForControl ||
                    component
                        .ReduxProgrammableBlocks
                        .Count == 0)
                {
                    continue;
                }

                for (int j = 0;
                    j <
                        component
                            .Controllers
                            .Count;
                    j++)
                {
                    snapshot
                        .RemoteControllers
                        .Add(
                            component
                                .Controllers[j]);
                }

                yield return 1;
            }
        }

        // ===== Graph helpers =====

        static GridNode GetOrCreateNode(
            Dictionary<long, GridNode> nodes,
            IMyCubeGrid grid)
        {
            GridNode node;

            if (!nodes.TryGetValue(
                    grid.EntityId,
                    out node))
            {
                node =
                    new GridNode(grid);

                nodes.Add(
                    grid.EntityId,
                    node);
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
                GetOrCreateNode(
                    nodes,
                    gridA);

            GridNode b =
                GetOrCreateNode(
                    nodes,
                    gridB);

            GridEdge edge =
                new GridEdge(
                    a,
                    b,
                    mechanism);

            a.MechanicalEdges.Add(
                edge);

            b.MechanicalEdges.Add(
                edge);
        }
    }
}
