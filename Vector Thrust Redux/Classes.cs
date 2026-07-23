// <mdk sortorder="20" />   // Classes.cs
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
        enum OperatingMode
        {
            Initializing,
            Active,
            Master,
            Slave,
            Parked
        }

        [Flags]
        enum BlockTags
        {
            None = 0,
            Use = 1,
            Ignore = 2,
            Status = 4,
            ParkTimer = 8,
            UnparkTimer = 16
        }

        [Flags]
        enum ControlRole
        {
            None = 0,
            Normal = 1,
            Cruise = 2,
            Slave = 4
        }

        [Flags]
        enum ThrusterDisableReason
        {
            None = 0,
            Park = 1,
            Cruise = 2,
            Standby = 4
        }

        sealed class DisabledThrusterState
        {
            public bool OriginalEnabled;
            public ThrusterDisableReason Reasons;
        }

        sealed class DirectionalCapacity
        {
            public double Forward;
            public double Backward;
            public double Left;
            public double Right;
            public double Up;
            public double Down;

            public void Clear()
            {
                Forward = 0;
                Backward = 0;
                Left = 0;
                Right = 0;
                Up = 0;
                Down = 0;
            }

            public void Add(
                DirectionalCapacity other)
            {
                if (other == null)
                {
                    return;
                }

                Forward += other.Forward;
                Backward += other.Backward;
                Left += other.Left;
                Right += other.Right;
                Up += other.Up;
                Down += other.Down;
            }

            public void CopyFrom(
                DirectionalCapacity other)
            {
                if (other == null)
                {
                    Clear();
                    return;
                }

                Forward = other.Forward;
                Backward = other.Backward;
                Left = other.Left;
                Right = other.Right;
                Up = other.Up;
                Down = other.Down;
            }

            public double GetSignedAxisCapacity(
                Vector3D localDirection)
            {
                double capacity = 0;

                if (localDirection.X > 0)
                {
                    capacity +=
                        Right *
                        localDirection.X;
                }
                else if (localDirection.X < 0)
                {
                    capacity +=
                        Left *
                        -localDirection.X;
                }

                if (localDirection.Y > 0)
                {
                    capacity +=
                        Up *
                        localDirection.Y;
                }
                else if (localDirection.Y < 0)
                {
                    capacity +=
                        Down *
                        -localDirection.Y;
                }

                if (localDirection.Z > 0)
                {
                    capacity +=
                        Backward *
                        localDirection.Z;
                }
                else if (localDirection.Z < 0)
                {
                    capacity +=
                        Forward *
                        -localDirection.Z;
                }

                return capacity;
            }
        }

        sealed class NacelleAimSolution
        {
            public VectorThrust Nacelle;
            public double CommandAngle;
            public double ReachableProjectedCapacity;
            public double CurrentProjectedCapacity;
            public double Alignment;

            public void Clear()
            {
                Nacelle = null;
                CommandAngle = 0;
                ReachableProjectedCapacity = 0;
                CurrentProjectedCapacity = 0;
                Alignment = 0;
            }
        }

        struct TopologyFingerprint :
            IEquatable<TopologyFingerprint>
        {
            public long Count;
            public ulong Xor;
            public ulong Sum;

            public bool Equals(
                TopologyFingerprint other)
            {
                return Count ==
                           other.Count &&
                       Xor ==
                           other.Xor &&
                       Sum ==
                           other.Sum;
            }

            public override bool Equals(
                object obj)
            {
                return obj is
                           TopologyFingerprint &&
                       Equals(
                           (TopologyFingerprint)
                               obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash =
                        (int)Count ^
                        (int)(Count >> 32);

                    hash =
                        hash * 397 ^
                        (int)Xor ^
                        (int)(Xor >> 32);

                    hash =
                        hash * 397 ^
                        (int)Sum ^
                        (int)(Sum >> 32);

                    return hash;
                }
            }

            public static bool operator ==(
                TopologyFingerprint left,
                TopologyFingerprint right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(
                TopologyFingerprint left,
                TopologyFingerprint right)
            {
                return !left.Equals(right);
            }
        }

        struct ConnectorPairKey :
            IEquatable<ConnectorPairKey>
        {
            public readonly long A;
            public readonly long B;

            public ConnectorPairKey(
                long first,
                long second)
            {
                A = Math.Min(
                    first,
                    second);

                B = Math.Max(
                    first,
                    second);
            }

            public bool Equals(
                ConnectorPairKey other)
            {
                return A == other.A &&
                       B == other.B;
            }

            public override bool Equals(
                object obj)
            {
                return obj is
                           ConnectorPairKey &&
                       Equals(
                           (ConnectorPairKey)
                               obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashA =
                        (int)A ^
                        (int)(A >> 32);

                    int hashB =
                        (int)B ^
                        (int)(B >> 32);

                    return hashA * 397 ^
                           hashB;
                }
            }
        }

        sealed class MasterCommand
        {
            public long MasterProgrammableBlockId;
            public long ControllerId;
            public long Sequence;

            // Direction is world-space. Magnitude is the requested fraction
            // of each participant's projected capacity in that direction.
            public Vector3D NormalizedForceDemand;

            public bool Dampeners;
            public bool Cruise;
            public double CruiseTargetSpeed;

            public int GearIndex;
            public int GearCount;
            public double GearFraction;

            public bool LevelWithGravity;

            public void CopyFrom(
                MasterCommand other)
            {
                MasterProgrammableBlockId =
                    other.MasterProgrammableBlockId;

                ControllerId =
                    other.ControllerId;

                Sequence =
                    other.Sequence;

                NormalizedForceDemand =
                    other.NormalizedForceDemand;

                Dampeners =
                    other.Dampeners;

                Cruise =
                    other.Cruise;

                CruiseTargetSpeed =
                    other.CruiseTargetSpeed;

                GearIndex =
                    other.GearIndex;

                GearCount =
                    other.GearCount;

                GearFraction =
                    other.GearFraction;

                LevelWithGravity =
                    other.LevelWithGravity;
            }

            public void Clear()
            {
                MasterProgrammableBlockId = 0;
                ControllerId = 0;
                Sequence = 0;

                NormalizedForceDemand =
                    Vector3D.Zero;

                Dampeners = true;
                Cruise = false;
                CruiseTargetSpeed = 0;

                GearIndex = 0;
                GearCount = 0;
                GearFraction = 0;

                LevelWithGravity = false;
            }
        }

        sealed class RuntimeTracker
        {
            readonly Program program;

            double averageRuntime;
            double maximumRuntime;
            double previousRuntime;
            int samples;

            public double AverageRuntime
            {
                get
                {
                    return averageRuntime;
                }
            }

            public double MaximumRuntime
            {
                get
                {
                    return maximumRuntime;
                }
            }

            public double PreviousRuntime
            {
                get
                {
                    return previousRuntime;
                }
            }

            public RuntimeTracker(
                Program program)
            {
                this.program = program;
            }

            public void BeginRun()
            {
                previousRuntime =
                    program.Runtime
                        .LastRunTimeMs;
            }

            public void EndRun()
            {
                double runtime =
                    program.Runtime
                        .LastRunTimeMs;

                samples++;

                if (samples == 1)
                {
                    averageRuntime =
                        runtime;

                    maximumRuntime =
                        runtime;

                    return;
                }

                // A small EMA is enough for status without keeping a bloody
                // queue of samples around forever.
                averageRuntime +=
                    (runtime -
                     averageRuntime) *
                    0.05;

                if (runtime >
                    maximumRuntime)
                {
                    maximumRuntime =
                        runtime;
                }
                else if (samples %
                         600 == 0)
                {
                    maximumRuntime =
                        averageRuntime;
                }
            }
        }

        public static class VectorMath
        {
            // Vector helpers retained from Whiplash141's math utilities used
            // by VTOS where Redux follows the same method.

            /// <summary>
            /// Normalizes a vector only if it is non-zero and non-unit.
            /// </summary>
            public static Vector3D SafeNormalize(
                Vector3D vector)
            {
                if (Vector3D.IsZero(
                        vector))
                {
                    return Vector3D.Zero;
                }

                if (Vector3D.IsUnit(
                        ref vector))
                {
                    return vector;
                }

                return Vector3D.Normalize(
                    vector);
            }

            /// <summary>
            /// Rejects vector a from vector b.
            /// </summary>
            public static Vector3D Rejection(
                Vector3D a,
                Vector3D b)
            {
                double denominator =
                    b.LengthSquared();

                if (a.LengthSquared() <=
                        VectorEpsilon ||
                    denominator <=
                        VectorEpsilon)
                {
                    return Vector3D.Zero;
                }

                return a -
                       Vector3D.Dot(a, b) /
                       denominator *
                       b;
            }

            /// <summary>
            /// Projects vector a onto vector b.
            /// </summary>
            public static Vector3D Projection(
                Vector3D a,
                Vector3D b)
            {
                double denominator =
                    b.LengthSquared();

                if (a.LengthSquared() <=
                        VectorEpsilon ||
                    denominator <=
                        VectorEpsilon)
                {
                    return Vector3D.Zero;
                }

                return Vector3D.Dot(a, b) /
                       denominator *
                       b;
            }

            public static double CosBetween(
                Vector3D a,
                Vector3D b)
            {
                double denominator =
                    Math.Sqrt(
                        a.LengthSquared() *
                        b.LengthSquared());

                if (denominator <=
                    VectorEpsilon)
                {
                    return 0;
                }

                return MathHelper.Clamp(
                    Vector3D.Dot(a, b) /
                    denominator,
                    -1,
                    1);
            }

            public static Vector3D ClampMagnitude(
                Vector3D vector,
                double maximumLength)
            {
                if (maximumLength <= 0)
                {
                    return Vector3D.Zero;
                }

                double lengthSquared =
                    vector.LengthSquared();

                double maximumSquared =
                    maximumLength *
                    maximumLength;

                if (lengthSquared <=
                    maximumSquared)
                {
                    return vector;
                }

                if (lengthSquared <=
                    VectorEpsilon)
                {
                    return Vector3D.Zero;
                }

                return vector *
                       (maximumLength /
                        Math.Sqrt(
                            lengthSquared));
            }

            public static double NormalizeAngle(
                double angle)
            {
                while (angle >
                       Math.PI)
                {
                    angle -=
                        MathHelper.TwoPi;
                }

                while (angle <
                       -Math.PI)
                {
                    angle +=
                        MathHelper.TwoPi;
                }

                return angle;
            }

            public static Vector3D RotateAroundAxis(
                Vector3D vector,
                Vector3D axis,
                double angle)
            {
                axis =
                    SafeNormalize(axis);

                if (axis.LengthSquared() <=
                    VectorEpsilon)
                {
                    return vector;
                }

                double cosine =
                    Math.Cos(angle);

                double sine =
                    Math.Sin(angle);

                return vector * cosine +
                       Vector3D.Cross(
                           axis,
                           vector) *
                       sine +
                       axis *
                       Vector3D.Dot(
                           axis,
                           vector) *
                       (1.0 - cosine);
            }

            /// <summary>
            /// Returns the signed command-space angle used by an SE rotor.
            /// </summary>
            public static double RotorCommandAngle(
                Vector3D targetDirection,
                Vector3D currentDirection,
                Vector3D rotorAxis)
            {
                Vector3D targetPlanar =
                    Rejection(
                        targetDirection,
                        rotorAxis);

                Vector3D currentPlanar =
                    Rejection(
                        currentDirection,
                        rotorAxis);

                if (targetPlanar
                        .LengthSquared() <=
                        VectorEpsilon ||
                    currentPlanar
                        .LengthSquared() <=
                        VectorEpsilon)
                {
                    return 0;
                }

                targetPlanar =
                    SafeNormalize(
                        targetPlanar);

                currentPlanar =
                    SafeNormalize(
                        currentPlanar);

                rotorAxis =
                    SafeNormalize(
                        rotorAxis);

                // Cross(target, current) matches SE TargetVelocityRad's
                // observed command sign.
                return Math.Atan2(
                    Vector3D.Dot(
                        rotorAxis,
                        Vector3D.Cross(
                            targetPlanar,
                            currentPlanar)),
                    Vector3D.Dot(
                        targetPlanar,
                        currentPlanar));
            }

            public static Vector3D WorldToLocalDirection(
                Vector3D worldDirection,
                MatrixD worldMatrix)
            {
                return Vector3D.TransformNormal(
                    worldDirection,
                    MatrixD.Transpose(
                        worldMatrix));
            }

            public static Vector3D LocalToWorldDirection(
                Vector3D localDirection,
                MatrixD worldMatrix)
            {
                return Vector3D.TransformNormal(
                    localDirection,
                    worldMatrix);
            }
        }

        static string GetModeText(
            OperatingMode value)
        {
            switch (value)
            {
                case OperatingMode.Initializing:
                    return "Initializing";

                case OperatingMode.Active:
                    return "Active";

                case OperatingMode.Master:
                    return "Master";

                case OperatingMode.Slave:
                    return "Slave";

                case OperatingMode.Parked:
                    return "Parked";

                default:
                    return "Unknown";
            }
        }

        sealed class GridNode
        {
            public readonly IMyCubeGrid Grid;

            public readonly List<GridEdge> MechanicalEdges =
                new List<GridEdge>();

            public GridComponent Component;
            public GridNode Parent;
            public GridEdge ParentEdge;

            public int Depth =
                int.MaxValue;

            public bool IncludedForControl;

            public GridNode(
                IMyCubeGrid grid)
            {
                Grid = grid;
            }
        }

        sealed class GridEdge
        {
            public readonly GridNode A;
            public readonly GridNode B;

            public readonly IMyTerminalBlock Mechanism;

            public GridEdge(
                GridNode a,
                GridNode b,
                IMyTerminalBlock mechanism)
            {
                A = a;
                B = b;
                Mechanism = mechanism;
            }

            public GridNode Other(
                GridNode node)
            {
                return node == A
                    ? B
                    : A;
            }
        }

        sealed class GridComponent
        {
            public readonly List<GridNode> Nodes =
                new List<GridNode>();

            public readonly List<IMyShipController> Controllers =
                new List<IMyShipController>();

            public readonly List<IMyProgrammableBlock> ReduxProgrammableBlocks =
                new List<IMyProgrammableBlock>();

            public bool IncludedForControl;
            public bool ReachableThroughConnection;
            public bool HasStaticGrid;

            public bool HasSlaveCapableRedux;

            // Parsed from the owning remote Redux PB. This lets the connected
            // master infer capacity without IGC or slave-side publication.
            public bool ReduxGreedy = true;
            public bool ReduxCanSlave = true;

            public IMyProgrammableBlock PrimaryReduxProgrammableBlock;
        }

        sealed class ConnectorEdge
        {
            public IMyShipConnector A;
            public IMyShipConnector B;

            public GridNode NodeA;
            public GridNode NodeB;
        }

        sealed class ParkConnector
        {
            public IMyShipConnector Block;
            public ConnectorEdge Edge;
        }

        sealed class ParkLandingGear
        {
            public IMyLandingGear Block;
        }
    }
}
