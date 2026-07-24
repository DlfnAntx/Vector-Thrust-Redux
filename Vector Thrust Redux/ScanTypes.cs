// <mdk sortorder="80" />   // ScanTypes.cs
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

            public readonly List<Thruster> LocalUnmanagedThrusters =
                new List<Thruster>();

            public readonly List<Thruster> RemoteReduxThrusters =
                new List<Thruster>();

            public readonly List<Thruster> RemoteUnmanagedThrusters =
                new List<Thruster>();

            public readonly List<Thruster> RemoteFixedReduxThrusters =
                new List<Thruster>();

            public readonly List<Rotor> RotorCandidates =
                new List<Rotor>();

            public readonly List<Rotor> ControlledRotors =
                new List<Rotor>();

            public readonly List<VectorThrust> VectorThrusters =
                new List<VectorThrust>();

            public readonly List<RemoteNacelleCapacityModel> RemoteNacelles =
                new List<RemoteNacelleCapacityModel>();

            // Kept empty by the new allocator. It survives only for status and
            // source compatibility until the old type is fully removed.
            public readonly List<VectorThrustGroup> Groups =
                new List<VectorThrustGroup>();

            // This list contains every non-ignored gyro Redux may temporarily
            // control. Its Normal role may still be absent.
            public readonly List<Gyro> Gyros =
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

        sealed class RemoteNacelleCapacityModel
        {
            readonly List<double> candidateCommandAngles =
                new List<double>();

            public readonly IMyMotorStator Rotor;

            public readonly List<Thruster> Thrusters =
                new List<Thruster>();

            public RemoteNacelleCapacityModel(
                IMyMotorStator rotor)
            {
                Rotor = rotor;
            }

            public Vector3D AxisWorld
            {
                get
                {
                    return Rotor
                        .WorldMatrix
                        .Up;
                }
            }

            public double GetMaximumProjectedCapacity(
                Vector3D targetDirection)
            {
                targetDirection =
                    VectorMath.SafeNormalize(
                        targetDirection);

                if (Rotor == null ||
                    Rotor.Closed ||
                    Rotor.TopGrid == null ||
                    !Rotor.IsFunctional ||
                    targetDirection
                        .LengthSquared() <=
                    VectorEpsilon)
                {
                    return 0;
                }

                candidateCommandAngles.Clear();

                AddCandidate(0);

                for (int i = 0;
                    i < Thrusters.Count;
                    i++)
                {
                    Thruster thruster =
                        Thrusters[i];

                    if (!IsPotentiallyUsable(
                            thruster))
                    {
                        continue;
                    }

                    double commandAngle =
                        VectorMath.RotorCommandAngle(
                            targetDirection,
                            thruster
                                .ForceDirectionWorld,
                            AxisWorld);

                    AddCandidate(
                        ClampCommandDeltaToLimits(
                            commandAngle));
                }

                double currentAngle =
                    Rotor.Angle;

                if (HasFiniteLowerLimit(
                        Rotor.LowerLimitRad))
                {
                    AddCandidate(
                        Rotor.LowerLimitRad -
                        currentAngle);
                }

                if (HasFiniteUpperLimit(
                        Rotor.UpperLimitRad))
                {
                    AddCandidate(
                        Rotor.UpperLimitRad -
                        currentAngle);
                }

                double bestCapacity = 0;
                double bestMovement =
                    double.MaxValue;

                for (int i = 0;
                    i <
                        candidateCommandAngles.Count;
                    i++)
                {
                    double candidate =
                        ClampCommandDeltaToLimits(
                            candidateCommandAngles[i]);

                    double capacity =
                        EvaluateProjectedCapacity(
                            targetDirection,
                            candidate);

                    double movement =
                        Math.Abs(candidate);

                    if (capacity >
                            bestCapacity +
                            ForceEpsilon ||
                        Math.Abs(
                            capacity -
                            bestCapacity) <=
                            ForceEpsilon &&
                        movement <
                            bestMovement)
                    {
                        bestCapacity =
                            capacity;

                        bestMovement =
                            movement;
                    }
                }

                return bestCapacity;
            }

            void AddCandidate(
                double commandAngle)
            {
                commandAngle =
                    ClampCommandDeltaToLimits(
                        commandAngle);

                for (int i = 0;
                    i <
                        candidateCommandAngles.Count;
                    i++)
                {
                    if (Math.Abs(
                            candidateCommandAngles[i] -
                            commandAngle) <=
                        AngleEpsilon)
                    {
                        return;
                    }
                }

                candidateCommandAngles.Add(
                    commandAngle);
            }

            double EvaluateProjectedCapacity(
                Vector3D targetDirection,
                double commandAngle)
            {
                double capacity = 0;

                for (int i = 0;
                    i < Thrusters.Count;
                    i++)
                {
                    Thruster thruster =
                        Thrusters[i];

                    if (!IsPotentiallyUsable(
                            thruster))
                    {
                        continue;
                    }

                    Vector3D predictedDirection =
                        VectorMath.RotateAroundAxis(
                            thruster
                                .ForceDirectionWorld,
                            AxisWorld,
                            -commandAngle);

                    double alignment =
                        Vector3D.Dot(
                            predictedDirection,
                            targetDirection);

                    if (alignment <= 0)
                    {
                        continue;
                    }

                    capacity +=
                        alignment *
                        thruster
                            .MaximumEffectiveThrust;
                }

                return capacity;
            }

            static bool IsPotentiallyUsable(
                Thruster thruster)
            {
                if (thruster == null ||
                    thruster.IsIgnored ||
                    thruster.TheBlock == null ||
                    thruster.TheBlock.Closed ||
                    !thruster
                        .TheBlock
                        .IsFunctional)
                {
                    return false;
                }

                // A connected master cannot see the remote PB's Storage map,
                // so it cannot distinguish player-disabled from Redux-parked
                // remote thrusters. Capacity therefore models functional
                // potential; the slave remains the final authority.
                return thruster
                           .MaximumEffectiveThrust >
                       ForceEpsilon;
            }

            double ClampCommandDeltaToLimits(
                double rawCommandDelta)
            {
                rawCommandDelta =
                    VectorMath.NormalizeAngle(
                        rawCommandDelta);

                bool finiteLower =
                    HasFiniteLowerLimit(
                        Rotor.LowerLimitRad);

                bool finiteUpper =
                    HasFiniteUpperLimit(
                        Rotor.UpperLimitRad);

                if (!finiteLower &&
                    !finiteUpper)
                {
                    return rawCommandDelta;
                }

                double currentAngle =
                    Rotor.Angle;

                double bestDelta =
                    double.NaN;

                double bestMagnitude =
                    double.MaxValue;

                for (int turn = -2;
                    turn <= 2;
                    turn++)
                {
                    double candidateDelta =
                        rawCommandDelta +
                        turn *
                        MathHelper.TwoPi;

                    double candidateAngle =
                        currentAngle +
                        candidateDelta;

                    if (finiteLower &&
                        candidateAngle <
                            Rotor.LowerLimitRad -
                            AngleEpsilon)
                    {
                        continue;
                    }

                    if (finiteUpper &&
                        candidateAngle >
                            Rotor.UpperLimitRad +
                            AngleEpsilon)
                    {
                        continue;
                    }

                    double magnitude =
                        Math.Abs(
                            candidateDelta);

                    if (magnitude <
                        bestMagnitude)
                    {
                        bestMagnitude =
                            magnitude;

                        bestDelta =
                            candidateDelta;
                    }
                }

                if (!double.IsNaN(
                        bestDelta))
                {
                    return bestDelta;
                }

                double requestedAngle =
                    currentAngle +
                    rawCommandDelta;

                if (finiteLower)
                {
                    requestedAngle =
                        Math.Max(
                            requestedAngle,
                            Rotor.LowerLimitRad);
                }

                if (finiteUpper)
                {
                    requestedAngle =
                        Math.Min(
                            requestedAngle,
                            Rotor.UpperLimitRad);
                }

                return requestedAngle -
                       currentAngle;
            }

            static bool HasFiniteLowerLimit(
                double value)
            {
                return !double.IsNaN(
                           value) &&
                       !double.IsInfinity(
                           value) &&
                       value > -1e20;
            }

            static bool HasFiniteUpperLimit(
                double value)
            {
                return !double.IsNaN(
                           value) &&
                       !double.IsInfinity(
                           value) &&
                       value < 1e20;
            }
        }

        readonly List<ConnectorEdge> connectorEdges =
            new List<ConnectorEdge>();

        readonly List<IMyShipConnector> topologyConnectors =
            new List<IMyShipConnector>();

        readonly Dictionary<long, long> observedConnectorTargets =
            new Dictionary<long, long>();

        readonly Dictionary<long, bool> observedLandingGearLocks =
            new Dictionary<long, bool>();

        readonly List<RemoteNacelleCapacityModel> remoteNacelles =
            new List<RemoteNacelleCapacityModel>();

        readonly List<Thruster> remoteFixedReduxThrusters =
            new List<Thruster>();

        void RequestRescan()
        {
            rescanRequested =
                true;
        }

        void ContinueRescan()
        {
            if (scanStateMachine == null)
            {
                if (!rescanRequested)
                {
                    return;
                }

                rescanRequested =
                    false;

                scanStateMachine =
                    ScanConstruct()
                        .GetEnumerator();
            }

            int maximumInstructions =
                Runtime.MaxInstructionCount;

            int instructionBudget =
                Math.Max(
                    1000,
                    maximumInstructions *
                    3 /
                    4);

            int steps = 0;

            while (scanStateMachine !=
                       null &&
                   Runtime
                       .CurrentInstructionCount <
                       instructionBudget &&
                   steps < 512)
            {
                steps++;

                if (scanStateMachine
                    .MoveNext())
                {
                    continue;
                }

                scanStateMachine.Dispose();

                scanStateMachine =
                    null;

                if (rescanRequested)
                {
                    rescanRequested =
                        false;

                    scanStateMachine =
                        ScanConstruct()
                            .GetEnumerator();
                }
            }
        }

        static double GetRemoteFixedProjectedCapacity(
            List<Thruster> remoteFixedThrusters,
            Vector3D targetDirection)
        {
            targetDirection =
                VectorMath.SafeNormalize(
                    targetDirection);

            if (targetDirection
                    .LengthSquared() <=
                VectorEpsilon)
            {
                return 0;
            }

            double capacity = 0;

            for (int i = 0;
                i <
                    remoteFixedThrusters.Count;
                i++)
            {
                Thruster thruster =
                    remoteFixedThrusters[i];

                if (thruster == null ||
                    thruster.IsIgnored ||
                    thruster.TheBlock == null ||
                    thruster.TheBlock.Closed ||
                    !thruster
                        .TheBlock
                        .IsFunctional)
                {
                    continue;
                }

                double alignment =
                    Vector3D.Dot(
                        thruster
                            .ForceDirectionWorld,
                        targetDirection);

                if (alignment <= 0)
                {
                    continue;
                }

                capacity +=
                    alignment *
                    thruster
                        .MaximumEffectiveThrust;
            }

            return capacity;
        }
    }
}
