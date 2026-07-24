// <mdk sortorder="30" />   // BlockClasses.cs
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        // ===== Thrusters =====

        sealed class Thruster
        {
            readonly Program program;

            float lastOverridePercentage =
                float.NaN;

            ControlRole controlRoles;

            public readonly IMyThrust TheBlock;
            public readonly BlockTags Tags;

            public VectorThrust Nacelle;

            public bool IsPrimaryNacelleThruster;

            public double DesiredEffectiveThrust;

            public ControlRole ControlRoles
            {
                get
                {
                    return controlRoles;
                }
            }

            public bool Controlled
            {
                get
                {
                    return controlRoles !=
                           ControlRole.None;
                }

                set
                {
                    SetControlRole(
                        ControlRole.Normal,
                        value);
                }
            }

            public long EntityId
            {
                get
                {
                    return TheBlock.EntityId;
                }
            }

            public Vector3D ForceDirectionWorld
            {
                get
                {
                    return TheBlock
                        .WorldMatrix
                        .Backward;
                }
            }

            public Vector3D ExhaustDirectionWorld
            {
                get
                {
                    return TheBlock
                        .WorldMatrix
                        .Forward;
                }
            }

            public double MaximumEffectiveThrust
            {
                get
                {
                    if (TheBlock == null ||
                        TheBlock.Closed ||
                        !TheBlock.IsFunctional)
                    {
                        return 0;
                    }

                    double thrust =
                        TheBlock
                            .MaxEffectiveThrust;

                    return thrust >
                           ForceEpsilon
                        ? thrust
                        : 0;
                }
            }

            public Vector3D CurrentForceWorld
            {
                get
                {
                    if (TheBlock == null ||
                        TheBlock.Closed ||
                        !TheBlock.IsFunctional)
                    {
                        return Vector3D.Zero;
                    }

                    return ForceDirectionWorld *
                           TheBlock.CurrentThrust;
                }
            }

            public bool IsIgnored
            {
                get
                {
                    return (Tags &
                            BlockTags.Ignore) != 0;
                }
            }

            public bool IsExplicitlyUsed
            {
                get
                {
                    return (Tags &
                            BlockTags.Use) != 0;
                }
            }

            public bool IsUsable
            {
                get
                {
                    if (TheBlock == null ||
                        TheBlock.Closed ||
                        !TheBlock.IsFunctional ||
                        IsIgnored ||
                        MaximumEffectiveThrust <=
                            ForceEpsilon)
                    {
                        return false;
                    }

                    // A block disabled by Redux remains capacity Redux can
                    // restore. A player-disabled block remains unavailable.
                    return TheBlock.Enabled ||
                           program
                               .WasThrusterDisabledByRedux(
                                   EntityId);
                }
            }

            public Thruster(
                IMyThrust block,
                Program program,
                BlockTags tags,
                bool controlled)
            {
                TheBlock = block;
                this.program = program;
                Tags = tags;

                if (controlled &&
                    !IsIgnored)
                {
                    controlRoles =
                        ControlRole.Normal;
                }
            }

            public void SetControlRole(
                ControlRole role,
                bool enabled)
            {
                if (IsIgnored)
                {
                    controlRoles =
                        ControlRole.None;

                    ClearOverride();
                    return;
                }

                if (enabled)
                {
                    controlRoles |= role;
                    return;
                }

                controlRoles &= ~role;

                if (controlRoles ==
                    ControlRole.None)
                {
                    ClearOverride();
                }
            }

            public bool HasControlRole(
                ControlRole role)
            {
                return (controlRoles &
                        role) != 0;
            }

            public void ResetDemand()
            {
                DesiredEffectiveThrust = 0;
            }

            public double GetCurrentProjectedCapacity(
                Vector3D targetDirection)
            {
                if (!IsUsable)
                {
                    return 0;
                }

                targetDirection =
                    VectorMath.SafeNormalize(
                        targetDirection);

                if (targetDirection
                        .LengthSquared() <=
                    VectorEpsilon)
                {
                    return 0;
                }

                double alignment =
                    Vector3D.Dot(
                        ForceDirectionWorld,
                        targetDirection);

                return alignment > 0
                    ? alignment *
                      MaximumEffectiveThrust
                    : 0;
            }

            public double AddOptimalContribution(
                ref Vector3D residual)
            {
                if (!Controlled ||
                    !IsUsable)
                {
                    return 0;
                }

                Vector3D direction =
                    VectorMath.SafeNormalize(
                        ForceDirectionWorld);

                double projection =
                    Vector3D.Dot(
                        residual,
                        direction);

                if (projection <=
                    ForceEpsilon)
                {
                    return 0;
                }

                double available =
                    MaximumEffectiveThrust -
                    DesiredEffectiveThrust;

                if (available <=
                    ForceEpsilon)
                {
                    return 0;
                }

                double added =
                    Math.Min(
                        projection,
                        available);

                DesiredEffectiveThrust +=
                    added;

                residual -=
                    direction * added;

                return added;
            }

            public void ApplyDemand()
            {
                if (!Controlled ||
                    TheBlock == null ||
                    TheBlock.Closed)
                {
                    return;
                }

                if (DesiredEffectiveThrust >
                    ForceEpsilon)
                {
                    program
                        .PrepareThrusterForControl(
                            this);
                }

                double maximumEffective =
                    MaximumEffectiveThrust;

                float requestedPercentage =
                    maximumEffective >
                            ForceEpsilon
                        ? (float)
                            MathHelper.Clamp(
                                DesiredEffectiveThrust /
                                maximumEffective,
                                0,
                                1)
                        : 0;

                if (!float.IsNaN(
                        lastOverridePercentage) &&
                    Math.Abs(
                        requestedPercentage -
                        lastOverridePercentage) <
                            ThrustWriteDeadbandFraction &&
                    Math.Abs(
                        TheBlock
                            .ThrustOverridePercentage -
                        requestedPercentage) <
                            ThrustWriteDeadbandFraction)
                {
                    return;
                }

                TheBlock
                    .ThrustOverridePercentage =
                    requestedPercentage;

                lastOverridePercentage =
                    requestedPercentage;
            }

            public void ClearOverride()
            {
                DesiredEffectiveThrust = 0;

                if (TheBlock == null ||
                    TheBlock.Closed)
                {
                    return;
                }

                if (Math.Abs(
                        TheBlock
                            .ThrustOverridePercentage) >
                    ForceEpsilon)
                {
                    TheBlock
                        .ThrustOverridePercentage = 0;
                }

                lastOverridePercentage = 0;
            }

            public void Release()
            {
                ClearOverride();

                controlRoles =
                    ControlRole.None;
            }
        }

        // ===== Rotors and hinges =====

        sealed class Rotor
        {
            readonly Program program;

            double lastWrittenTargetVelocity =
                double.NaN;

            double lastObservedAngle =
                double.NaN;

            double measuredAngularVelocity;
            double lastNonzeroCommandSign;

            double parkTargetAngle;
            bool parkTargetInitialized;
            bool parkSettled;

            ControlRole controlRoles;

            public readonly IMyMotorStator TheBlock;
            public readonly BlockTags Tags;

            public VectorThrust Nacelle;

            public ControlRole ControlRoles
            {
                get
                {
                    return controlRoles;
                }
            }

            public bool Controlled
            {
                get
                {
                    return controlRoles !=
                           ControlRole.None;
                }

                set
                {
                    SetControlRole(
                        ControlRole.Normal,
                        value);
                }
            }

            public long EntityId
            {
                get
                {
                    return TheBlock.EntityId;
                }
            }

            public bool IsIgnored
            {
                get
                {
                    return (Tags &
                            BlockTags.Ignore) != 0;
                }
            }

            public bool IsExplicitlyUsed
            {
                get
                {
                    return (Tags &
                            BlockTags.Use) != 0;
                }
            }

            public bool IsHinge
            {
                get
                {
                    return TheBlock
                        .BlockDefinition
                        .SubtypeId
                        .IndexOf(
                            "Hinge",
                            StringComparison
                                .OrdinalIgnoreCase) >= 0;
                }
            }

            public Vector3D AxisWorld
            {
                get
                {
                    return TheBlock
                        .WorldMatrix
                        .Up;
                }
            }

            public double MeasuredAngularVelocity
            {
                get
                {
                    return measuredAngularVelocity;
                }
            }

            public double PreferredCommandSign
            {
                get
                {
                    return lastNonzeroCommandSign;
                }
            }

            public bool IsPhysicallyMovable
            {
                get
                {
                    if (TheBlock == null ||
                        TheBlock.Closed ||
                        TheBlock.Top == null ||
                        !TheBlock.IsFunctional ||
                        !TheBlock.Enabled ||
                        TheBlock.RotorLock)
                    {
                        return false;
                    }

                    double lower =
                        TheBlock.LowerLimitRad;

                    double upper =
                        TheBlock.UpperLimitRad;

                    return !HasFiniteLowerLimit(
                               lower) ||
                           !HasFiniteUpperLimit(
                               upper) ||
                           Math.Abs(
                               upper -
                               lower) >
                           EqualLimitEpsilon;
                }
            }

            public Rotor(
                IMyMotorStator block,
                Program program,
                BlockTags tags,
                bool controlled)
            {
                TheBlock = block;
                this.program = program;
                Tags = tags;

                if (controlled &&
                    !IsIgnored)
                {
                    controlRoles =
                        ControlRole.Normal;
                }
            }

            public void SetControlRole(
                ControlRole role,
                bool enabled)
            {
                if (IsIgnored)
                {
                    controlRoles =
                        ControlRole.None;

                    Stop();
                    return;
                }

                if (enabled)
                {
                    controlRoles |= role;
                    return;
                }

                controlRoles &= ~role;

                if (controlRoles ==
                    ControlRole.None)
                {
                    Stop();
                }
            }

            public bool HasControlRole(
                ControlRole role)
            {
                return (controlRoles &
                        role) != 0;
            }

            public void ObserveMotion(
                double timeStep)
            {
                if (TheBlock == null ||
                    TheBlock.Closed)
                {
                    measuredAngularVelocity = 0;
                    lastObservedAngle =
                        double.NaN;

                    return;
                }

                double currentAngle =
                    TheBlock.Angle;

                if (double.IsNaN(
                        lastObservedAngle) ||
                    timeStep <=
                        VectorEpsilon)
                {
                    measuredAngularVelocity = 0;
                }
                else
                {
                    double delta =
                        currentAngle -
                        lastObservedAngle;

                    // Angle wraps on unlimited rotors. Hinges and ordinary
                    // limited rotors cannot legitimately move a full turn
                    // between observations, so normalization remains safe.
                    delta =
                        VectorMath.NormalizeAngle(
                            delta);

                    measuredAngularVelocity =
                        delta / timeStep;
                }

                lastObservedAngle =
                    currentAngle;
            }

            public double Point(
                Vector3D desiredForceWorld)
            {
                return Point(
                    desiredForceWorld,
                    1,
                    program.lastControlTimeStep);
            }

            public double Point(
                Vector3D desiredForceWorld,
                double demandFraction,
                double timeStep)
            {
                if (!Controlled ||
                    !IsPhysicallyMovable ||
                    Nacelle == null ||
                    desiredForceWorld
                        .LengthSquared() <=
                    VectorEpsilon)
                {
                    Stop();
                    return 0;
                }

                CancelPark();
                ObserveMotion(timeStep);

                Vector3D currentForce =
                    Nacelle
                        .PrimaryForceDirectionWorld;

                double rawCommandAngle =
                    GetStableCommandAngle(
                        desiredForceWorld,
                        currentForce);

                double reachableCommandAngle =
                    ClampCommandDeltaToLimits(
                        rawCommandAngle);

                demandFraction =
                    MathHelper.Clamp(
                        demandFraction,
                        0,
                        1);

                double responseGain =
                    MinimumJointResponseGain +
                    (MaximumJointResponseGain -
                     MinimumJointResponseGain) *
                    demandFraction;

                double predictedError =
                    reachableCommandAngle -
                    measuredAngularVelocity *
                    Math.Max(
                        timeStep,
                        MinimumTimeStep);

                // If current motion is already expected to cross the target
                // by the next observation, coast instead of immediately
                // commanding a full reversal.
                if (reachableCommandAngle *
                    predictedError <= 0)
                {
                    predictedError = 0;
                }

                SetTargetVelocity(
                    predictedError *
                    responseGain);

                Vector3D predictedForce =
                    VectorMath.RotateAroundAxis(
                        currentForce,
                        AxisWorld,
                        -reachableCommandAngle);

                return VectorMath.CosBetween(
                    predictedForce,
                    desiredForceWorld);
            }

            public void PointToSolution(
                NacelleAimSolution solution,
                double demandFraction,
                double timeStep)
            {
                if (solution == null ||
                    solution.Nacelle !=
                        Nacelle ||
                    !Controlled ||
                    !IsPhysicallyMovable)
                {
                    Stop();
                    return;
                }

                CancelPark();
                ObserveMotion(timeStep);

                double commandAngle =
                    solution.CommandAngle;

                demandFraction =
                    MathHelper.Clamp(
                        demandFraction,
                        0,
                        1);

                double responseGain =
                    MinimumJointResponseGain +
                    (MaximumJointResponseGain -
                     MinimumJointResponseGain) *
                    demandFraction;

                double predictedError =
                    commandAngle -
                    measuredAngularVelocity *
                    Math.Max(
                        timeStep,
                        MinimumTimeStep);

                if (commandAngle *
                    predictedError <= 0)
                {
                    predictedError = 0;
                }

                SetTargetVelocity(
                    predictedError *
                    responseGain);
            }

            public bool TryGetReachableCommandAngle(
                Vector3D desiredForceWorld,
                out double commandAngle,
                out double predictedAlignment)
            {
                commandAngle = 0;
                predictedAlignment = 0;

                if (!IsPhysicallyMovable ||
                    Nacelle == null)
                {
                    return false;
                }

                Vector3D currentForce =
                    Nacelle
                        .PrimaryForceDirectionWorld;

                if (currentForce
                        .LengthSquared() <=
                        VectorEpsilon ||
                    desiredForceWorld
                        .LengthSquared() <=
                        VectorEpsilon)
                {
                    return false;
                }

                double rawCommandAngle =
                    GetStableCommandAngle(
                        desiredForceWorld,
                        currentForce);

                commandAngle =
                    ClampCommandDeltaToLimits(
                        rawCommandAngle);

                Vector3D predictedForce =
                    VectorMath.RotateAroundAxis(
                        currentForce,
                        AxisWorld,
                        -commandAngle);

                predictedAlignment =
                    VectorMath.CosBetween(
                        predictedForce,
                        desiredForceWorld);

                return true;
            }

            public double GetReachableCommandAngle(
                Vector3D desiredForceWorld,
                Vector3D currentForceWorld)
            {
                return ClampCommandDeltaToLimits(
                    GetStableCommandAngle(
                        desiredForceWorld,
                        currentForceWorld));
            }

            public double ClampCommandDelta(
                double rawCommandDelta)
            {
                return ClampCommandDeltaToLimits(
                    rawCommandDelta);
            }

            public void BeginPark(
                Vector3D naturalGravity,
                Vector3D localRootCenter)
            {
                parkTargetInitialized = false;
                parkSettled = false;

                if (!Controlled ||
                    !IsPhysicallyMovable ||
                    Nacelle == null)
                {
                    Stop();
                    parkSettled = true;

                    program
                        .parkRotorTargetAngles
                        .Remove(EntityId);

                    return;
                }

                double commandAngle;
                double predictedAlignment;

                if (naturalGravity
                        .LengthSquared() >
                    VectorEpsilon)
                {
                    Vector3D gravityOpposingForce =
                        -VectorMath.SafeNormalize(
                            naturalGravity);

                    if (TryGetReachableCommandAngle(
                            gravityOpposingForce,
                            out commandAngle,
                            out predictedAlignment) &&
                        predictedAlignment >=
                            DirectParkAlignmentCosine)
                    {
                        SetParkTargetFromCommandDelta(
                            commandAngle);

                        return;
                    }
                }

                Vector3D branchCenter =
                    Nacelle
                        .GetBranchCenterWorld();

                Vector3D pivot =
                    TheBlock.GetPosition();

                Vector3D branchOffset =
                    branchCenter -
                    pivot;

                Vector3D rootOffset =
                    localRootCenter -
                    pivot;

                Vector3D branchPlanar =
                    VectorMath.Rejection(
                        branchOffset,
                        AxisWorld);

                Vector3D rootPlanar =
                    VectorMath.Rejection(
                        rootOffset,
                        AxisWorld);

                if (branchPlanar
                        .LengthSquared() <=
                        VectorEpsilon ||
                    rootPlanar
                        .LengthSquared() <=
                        VectorEpsilon)
                {
                    SetParkTargetFromCommandDelta(
                        0);

                    return;
                }

                commandAngle =
                    GetStableCommandAngle(
                        rootPlanar,
                        branchPlanar);

                commandAngle =
                    ClampCommandDeltaToLimits(
                        commandAngle);

                SetParkTargetFromCommandDelta(
                    commandAngle);
            }

            public void RestoreParkTarget(
                double targetAngle)
            {
                parkTargetAngle =
                    targetAngle;

                if (HasFiniteLowerLimit(
                        TheBlock.LowerLimitRad))
                {
                    parkTargetAngle =
                        Math.Max(
                            parkTargetAngle,
                            TheBlock
                                .LowerLimitRad);
                }

                if (HasFiniteUpperLimit(
                        TheBlock.UpperLimitRad))
                {
                    parkTargetAngle =
                        Math.Min(
                            parkTargetAngle,
                            TheBlock
                                .UpperLimitRad);
                }

                parkTargetInitialized = true;
                parkSettled = false;
            }

            public void UpdatePark()
            {
                if (!parkTargetInitialized ||
                    !Controlled ||
                    !IsPhysicallyMovable)
                {
                    Stop();
                    parkSettled = true;
                    return;
                }

                double error =
                    parkTargetAngle -
                    TheBlock.Angle;

                if (!HasAnyFiniteLimit())
                {
                    error =
                        VectorMath.NormalizeAngle(
                            error);
                }

                double requestedVelocity =
                    error *
                    ParkingJointResponseGain;

                parkSettled =
                    Math.Abs(
                        requestedVelocity) <=
                    JointWriteDeadbandRad;

                SetTargetVelocity(
                    parkSettled
                        ? 0
                        : requestedVelocity);
            }

            public void CancelPark()
            {
                parkTargetInitialized = false;
                parkSettled = false;
            }

            public void Stop()
            {
                SetTargetVelocity(0);
            }

            public void Release()
            {
                Stop();

                controlRoles =
                    ControlRole.None;
            }

            double GetStableCommandAngle(
                Vector3D targetDirection,
                Vector3D currentDirection)
            {
                double commandAngle =
                    VectorMath.RotorCommandAngle(
                        targetDirection,
                        currentDirection,
                        AxisWorld);

                // At exactly opposite directions atan2 may choose either half
                // turn from microscopic cross-product noise. Retain the last
                // deliberate turn direction rather than twitching each tick.
                if (Math.Abs(
                        Math.Abs(
                            commandAngle) -
                        Math.PI) <=
                    AngleEpsilon)
                {
                    if (lastNonzeroCommandSign != 0)
                    {
                        commandAngle =
                            Math.Abs(
                                commandAngle) *
                            lastNonzeroCommandSign;
                    }
                    else
                    {
                        commandAngle =
                            Math.Abs(
                                commandAngle);
                    }
                }

                return commandAngle;
            }

            void SetParkTargetFromCommandDelta(
                double commandAngle)
            {
                parkTargetAngle =
                    TheBlock.Angle +
                    commandAngle;

                if (HasFiniteLowerLimit(
                        TheBlock.LowerLimitRad))
                {
                    parkTargetAngle =
                        Math.Max(
                            parkTargetAngle,
                            TheBlock
                                .LowerLimitRad);
                }

                if (HasFiniteUpperLimit(
                        TheBlock.UpperLimitRad))
                {
                    parkTargetAngle =
                        Math.Min(
                            parkTargetAngle,
                            TheBlock
                                .UpperLimitRad);
                }

                parkTargetInitialized = true;
                parkSettled = false;

                program
                    .parkRotorTargetAngles[
                        EntityId] =
                    parkTargetAngle;
            }

            double ClampCommandDeltaToLimits(
                double rawCommandDelta)
            {
                rawCommandDelta =
                    VectorMath.NormalizeAngle(
                        rawCommandDelta);

                bool finiteLower =
                    HasFiniteLowerLimit(
                        TheBlock.LowerLimitRad);

                bool finiteUpper =
                    HasFiniteUpperLimit(
                        TheBlock.UpperLimitRad);

                if (!finiteLower &&
                    !finiteUpper)
                {
                    return rawCommandDelta;
                }

                double currentAngle =
                    TheBlock.Angle;

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
                            TheBlock
                                .LowerLimitRad -
                            AngleEpsilon)
                    {
                        continue;
                    }

                    if (finiteUpper &&
                        candidateAngle >
                            TheBlock
                                .UpperLimitRad +
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
                            TheBlock
                                .LowerLimitRad);
                }

                if (finiteUpper)
                {
                    requestedAngle =
                        Math.Min(
                            requestedAngle,
                            TheBlock
                                .UpperLimitRad);
                }

                return requestedAngle -
                       currentAngle;
            }

            void SetTargetVelocity(
                double velocityRad)
            {
                if (TheBlock == null ||
                    TheBlock.Closed)
                {
                    return;
                }

                velocityRad =
                    MathHelper.Clamp(
                        velocityRad,
                        -MaximumJointVelocityRad,
                        MaximumJointVelocityRad);

                if (Math.Abs(
                        velocityRad) <=
                    JointWriteDeadbandRad)
                {
                    velocityRad = 0;
                }

                if (velocityRad != 0)
                {
                    lastNonzeroCommandSign =
                        Math.Sign(
                            velocityRad);
                }

                if (!double.IsNaN(
                        lastWrittenTargetVelocity) &&
                    Math.Abs(
                        lastWrittenTargetVelocity -
                        velocityRad) <
                            JointWriteDeadbandRad &&
                    Math.Abs(
                        TheBlock
                            .TargetVelocityRad -
                        velocityRad) <
                            JointWriteDeadbandRad)
                {
                    return;
                }

                TheBlock.TargetVelocityRad =
                    (float)velocityRad;

                lastWrittenTargetVelocity =
                    velocityRad;
            }

            bool HasAnyFiniteLimit()
            {
                return HasFiniteLowerLimit(
                           TheBlock
                               .LowerLimitRad) ||
                       HasFiniteUpperLimit(
                           TheBlock
                               .UpperLimitRad);
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

        // ===== Vector nacelles =====

        sealed class VectorThrust
        {
            sealed class DirectionBucket
            {
                public Vector3D
                    LocalExhaustDirection;

                public double
                    EffectiveThrust;
            }

            readonly Program program;

            readonly List<DirectionBucket>
                directionBuckets =
                    new List<DirectionBucket>();

            readonly List<double>
                candidateCommandAngles =
                    new List<double>();

            public readonly Rotor Rotor;

            public readonly List<Thruster> Thrusters =
                new List<Thruster>();

            public readonly List<IMyCubeGrid> BranchGrids =
                new List<IMyCubeGrid>();

            public Vector3D PrimaryExhaustDirectionLocal =
                Vector3D.Zero;

            public double PrimaryEffectiveThrust;

            public Vector3D RequiredForceWorld =
                Vector3D.Zero;

            public VectorThrust(
                Rotor rotor,
                Program program)
            {
                Rotor = rotor;
                this.program = program;

                rotor.Nacelle = this;
            }

            public Vector3D AxisWorld
            {
                get
                {
                    return Rotor.AxisWorld;
                }
            }

            public Vector3D PrimaryForceDirectionWorld
            {
                get
                {
                    IMyCubeGrid topGrid =
                        Rotor.TheBlock.TopGrid;

                    if (topGrid == null ||
                        PrimaryExhaustDirectionLocal
                            .LengthSquared() <=
                        VectorEpsilon)
                    {
                        return Vector3D.Zero;
                    }

                    Vector3D exhaust =
                        VectorMath
                            .LocalToWorldDirection(
                                PrimaryExhaustDirectionLocal,
                                topGrid.WorldMatrix);

                    return -VectorMath.SafeNormalize(
                        exhaust);
                }
            }

            public void RefreshPrimaryDirection()
            {
                directionBuckets.Clear();

                for (int i = 0;
                    i < Thrusters.Count;
                    i++)
                {
                    Thrusters[i]
                        .IsPrimaryNacelleThruster =
                        false;
                }

                IMyCubeGrid topGrid =
                    Rotor.TheBlock.TopGrid;

                if (topGrid == null)
                {
                    PrimaryExhaustDirectionLocal =
                        Vector3D.Zero;

                    PrimaryEffectiveThrust = 0;
                    return;
                }

                MatrixD topWorldMatrix =
                    topGrid.WorldMatrix;

                for (int i = 0;
                    i < Thrusters.Count;
                    i++)
                {
                    Thruster thruster =
                        Thrusters[i];

                    if (!thruster.Controlled ||
                        !thruster.IsUsable)
                    {
                        continue;
                    }

                    double effective =
                        thruster
                            .MaximumEffectiveThrust;

                    if (effective <=
                        ForceEpsilon)
                    {
                        continue;
                    }

                    Vector3D localExhaust =
                        VectorMath.SafeNormalize(
                            VectorMath
                                .WorldToLocalDirection(
                                    thruster
                                        .ExhaustDirectionWorld,
                                    topWorldMatrix));

                    DirectionBucket bucket =
                        null;

                    for (int j = 0;
                        j <
                            directionBuckets.Count;
                        j++)
                    {
                        if (Vector3D.Dot(
                                directionBuckets[j]
                                    .LocalExhaustDirection,
                                localExhaust) >=
                            DirectionBucketCosine)
                        {
                            bucket =
                                directionBuckets[j];

                            break;
                        }
                    }

                    if (bucket == null)
                    {
                        bucket =
                            new DirectionBucket
                            {
                                LocalExhaustDirection =
                                    localExhaust
                            };

                        directionBuckets.Add(
                            bucket);
                    }

                    bucket.EffectiveThrust +=
                        effective;
                }

                DirectionBucket strongest =
                    null;

                for (int i = 0;
                    i <
                        directionBuckets.Count;
                    i++)
                {
                    if (strongest == null ||
                        directionBuckets[i]
                            .EffectiveThrust >
                        strongest
                            .EffectiveThrust)
                    {
                        strongest =
                            directionBuckets[i];
                    }
                }

                if (strongest == null)
                {
                    PrimaryExhaustDirectionLocal =
                        Vector3D.Zero;

                    PrimaryEffectiveThrust = 0;
                    return;
                }

                PrimaryExhaustDirectionLocal =
                    strongest
                        .LocalExhaustDirection;

                PrimaryEffectiveThrust =
                    strongest
                        .EffectiveThrust;

                for (int i = 0;
                    i < Thrusters.Count;
                    i++)
                {
                    Thruster thruster =
                        Thrusters[i];

                    if (!thruster.Controlled ||
                        !thruster.IsUsable)
                    {
                        continue;
                    }

                    Vector3D localExhaust =
                        VectorMath.SafeNormalize(
                            VectorMath
                                .WorldToLocalDirection(
                                    thruster
                                        .ExhaustDirectionWorld,
                                    topWorldMatrix));

                    thruster
                        .IsPrimaryNacelleThruster =
                        Vector3D.Dot(
                            localExhaust,
                            PrimaryExhaustDirectionLocal) >=
                        DirectionBucketCosine;
                }
            }

            public bool TrySolveAim(
                Vector3D targetDirection,
                NacelleAimSolution output)
            {
                if (output == null)
                {
                    return false;
                }

                output.Clear();
                output.Nacelle = this;

                targetDirection =
                    VectorMath.SafeNormalize(
                        targetDirection);

                if (!Rotor.Controlled ||
                    !Rotor.IsPhysicallyMovable ||
                    targetDirection
                        .LengthSquared() <=
                    VectorEpsilon)
                {
                    return false;
                }

                candidateCommandAngles.Clear();

                AddCandidateCommandAngle(0);

                Vector3D primaryDirection =
                    PrimaryForceDirectionWorld;

                if (primaryDirection
                        .LengthSquared() >
                    VectorEpsilon)
                {
                    AddCandidateCommandAngle(
                        Rotor
                            .GetReachableCommandAngle(
                                targetDirection,
                                primaryDirection));
                }

                for (int i = 0;
                    i < Thrusters.Count;
                    i++)
                {
                    Thruster thruster =
                        Thrusters[i];

                    if (!thruster.Controlled ||
                        !thruster.IsUsable)
                    {
                        continue;
                    }

                    AddCandidateCommandAngle(
                        Rotor
                            .GetReachableCommandAngle(
                                targetDirection,
                                thruster
                                    .ForceDirectionWorld));
                }

                double currentAngle =
                    Rotor.TheBlock.Angle;

                double lower =
                    Rotor.TheBlock
                        .LowerLimitRad;

                double upper =
                    Rotor.TheBlock
                        .UpperLimitRad;

                if (!double.IsNaN(lower) &&
                    !double.IsInfinity(lower) &&
                    lower > -1e20)
                {
                    AddCandidateCommandAngle(
                        lower -
                        currentAngle);
                }

                if (!double.IsNaN(upper) &&
                    !double.IsInfinity(upper) &&
                    upper < 1e20)
                {
                    AddCandidateCommandAngle(
                        upper -
                        currentAngle);
                }

                double currentCapacity =
                    EvaluateProjectedCapacity(
                        targetDirection,
                        0);

                double bestCapacity =
                    -1;

                double bestAngle = 0;

                for (int i = 0;
                    i <
                        candidateCommandAngles.Count;
                    i++)
                {
                    double candidate =
                        Rotor.ClampCommandDelta(
                            candidateCommandAngles[i]);

                    double capacity =
                        EvaluateProjectedCapacity(
                            targetDirection,
                            candidate);

                    if (capacity >
                        bestCapacity +
                        ForceEpsilon)
                    {
                        bestCapacity =
                            capacity;

                        bestAngle =
                            candidate;

                        continue;
                    }

                    if (Math.Abs(
                            capacity -
                            bestCapacity) >
                        ForceEpsilon)
                    {
                        continue;
                    }

                    double candidateMagnitude =
                        Math.Abs(candidate);

                    double bestMagnitude =
                        Math.Abs(bestAngle);

                    if (candidateMagnitude <
                        bestMagnitude -
                        AngleEpsilon)
                    {
                        bestAngle =
                            candidate;

                        continue;
                    }

                    if (Math.Abs(
                            candidateMagnitude -
                            bestMagnitude) <=
                            AngleEpsilon &&
                        Rotor
                            .PreferredCommandSign != 0 &&
                        Math.Sign(candidate) ==
                            Rotor
                                .PreferredCommandSign)
                    {
                        bestAngle =
                            candidate;
                    }
                }

                if (bestCapacity <=
                    ForceEpsilon)
                {
                    return false;
                }

                output.CommandAngle =
                    bestAngle;

                output
                    .ReachableProjectedCapacity =
                    bestCapacity;

                output
                    .CurrentProjectedCapacity =
                    currentCapacity;

                Vector3D predictedPrimary =
                    VectorMath.RotateAroundAxis(
                        primaryDirection,
                        AxisWorld,
                        -bestAngle);

                output.Alignment =
                    VectorMath.CosBetween(
                        predictedPrimary,
                        targetDirection);

                return true;
            }

            public double GetMaximumProjectedCapacity(
                Vector3D targetDirection)
            {
                NacelleAimSolution solution =
                    new NacelleAimSolution();

                return TrySolveAim(
                           targetDirection,
                           solution)
                    ? solution
                        .ReachableProjectedCapacity
                    : 0;
            }

            public double Aim(
                Vector3D desiredForceWorld)
            {
                RequiredForceWorld =
                    desiredForceWorld;

                if (desiredForceWorld
                        .LengthSquared() <=
                    VectorEpsilon)
                {
                    Rotor.Stop();
                    return 0;
                }

                NacelleAimSolution solution =
                    new NacelleAimSolution();

                if (!TrySolveAim(
                        desiredForceWorld,
                        solution))
                {
                    Rotor.Stop();
                    return 0;
                }

                double demandFraction =
                    solution
                            .ReachableProjectedCapacity >
                        ForceEpsilon
                        ? MathHelper.Clamp(
                            desiredForceWorld
                                .Length() /
                            solution
                                .ReachableProjectedCapacity,
                            0,
                            1)
                        : 0;

                Rotor.PointToSolution(
                    solution,
                    demandFraction,
                    program.lastControlTimeStep);

                return solution.Alignment;
            }

            public void Aim(
                NacelleAimSolution solution,
                double requestedProjectedThrust,
                double timeStep)
            {
                if (solution == null ||
                    solution.Nacelle != this ||
                    solution
                            .ReachableProjectedCapacity <=
                        ForceEpsilon)
                {
                    RequiredForceWorld =
                        Vector3D.Zero;

                    Rotor.Stop();
                    return;
                }

                double demandFraction =
                    MathHelper.Clamp(
                        requestedProjectedThrust /
                        solution
                            .ReachableProjectedCapacity,
                        0,
                        1);

                Rotor.PointToSolution(
                    solution,
                    demandFraction,
                    timeStep);
            }

            public double AllocatePrimary(
                ref Vector3D residual,
                Vector3D centerOfMassWorld,
                ref Vector3D inducedTorque)
            {
                double allocated = 0;

                for (int i = 0;
                    i < Thrusters.Count;
                    i++)
                {
                    Thruster thruster =
                        Thrusters[i];

                    if (!thruster
                            .IsPrimaryNacelleThruster)
                    {
                        continue;
                    }

                    double contribution =
                        thruster
                            .AddOptimalContribution(
                                ref residual);

                    if (contribution <=
                        ForceEpsilon)
                    {
                        continue;
                    }

                    Vector3D force =
                        thruster
                            .ForceDirectionWorld *
                        contribution;

                    Vector3D lever =
                        thruster
                            .TheBlock
                            .GetPosition() -
                        centerOfMassWorld;

                    inducedTorque +=
                        Vector3D.Cross(
                            lever,
                            force);

                    allocated +=
                        contribution;
                }

                return allocated;
            }

            public double AllocateSecondary(
                ref Vector3D residual,
                Vector3D centerOfMassWorld,
                ref Vector3D inducedTorque)
            {
                double allocated = 0;

                for (int i = 0;
                    i < Thrusters.Count;
                    i++)
                {
                    Thruster thruster =
                        Thrusters[i];

                    if (thruster
                            .IsPrimaryNacelleThruster)
                    {
                        continue;
                    }

                    double contribution =
                        thruster
                            .AddOptimalContribution(
                                ref residual);

                    if (contribution <=
                        ForceEpsilon)
                    {
                        continue;
                    }

                    Vector3D force =
                        thruster
                            .ForceDirectionWorld *
                        contribution;

                    Vector3D lever =
                        thruster
                            .TheBlock
                            .GetPosition() -
                        centerOfMassWorld;

                    inducedTorque +=
                        Vector3D.Cross(
                            lever,
                            force);

                    allocated +=
                        contribution;
                }

                return allocated;
            }

            public Vector3D GetBranchCenterWorld()
            {
                if (BranchGrids.Count == 0)
                {
                    return Rotor
                               .TheBlock
                               .TopGrid != null
                        ? Rotor
                            .TheBlock
                            .TopGrid
                            .WorldAABB
                            .Center
                        : Rotor
                            .TheBlock
                            .GetPosition();
                }

                Vector3D center =
                    Vector3D.Zero;

                int validGrids = 0;

                for (int i = 0;
                    i < BranchGrids.Count;
                    i++)
                {
                    IMyCubeGrid grid =
                        BranchGrids[i];

                    if (grid == null ||
                        grid.Closed)
                    {
                        continue;
                    }

                    center +=
                        grid.WorldAABB.Center;

                    validGrids++;
                }

                return validGrids > 0
                    ? center /
                      validGrids
                    : Rotor
                        .TheBlock
                        .GetPosition();
            }

            void AddCandidateCommandAngle(
                double commandAngle)
            {
                commandAngle =
                    Rotor.ClampCommandDelta(
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

                    if (!thruster.Controlled ||
                        !thruster.IsUsable)
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
        }

        // Retained for source compatibility until Functions.cs and
        // Sequences.cs switch entirely to per-nacelle allocation.
        sealed class VectorThrustGroup
        {
            public readonly List<VectorThrust> Nacelles =
                new List<VectorThrust>();

            public Vector3D AxisWorld
            {
                get
                {
                    return Nacelles.Count > 0
                        ? VectorMath.SafeNormalize(
                            Nacelles[0]
                                .AxisWorld)
                        : Vector3D.Zero;
                }
            }

            public double EffectiveCapacity
            {
                get
                {
                    double capacity = 0;

                    for (int i = 0;
                        i < Nacelles.Count;
                        i++)
                    {
                        capacity +=
                            Nacelles[i]
                                .PrimaryEffectiveThrust;
                    }

                    return capacity;
                }
            }

            public Vector3D ReachableComponent(
                Vector3D force)
            {
                return VectorMath.Rejection(
                    force,
                    AxisWorld);
            }

            public double Score(
                Vector3D residual)
            {
                Vector3D reachable =
                    ReachableComponent(
                        residual);

                if (reachable
                        .LengthSquared() <=
                    VectorEpsilon)
                {
                    return 0;
                }

                return Math.Min(
                    reachable.Length(),
                    EffectiveCapacity);
            }
        }

        // ===== Gyroscopes =====

        sealed class Gyro
        {
            readonly Program program;

            bool lastOverride;

            float lastPitch =
                float.NaN;

            float lastYaw =
                float.NaN;

            float lastRoll =
                float.NaN;

            ControlRole controlRoles;

            public readonly IMyGyro TheBlock;
            public readonly BlockTags Tags;

            public readonly double NominalCapacity;

            public ControlRole ControlRoles
            {
                get
                {
                    return controlRoles;
                }
            }

            public bool Controlled
            {
                get
                {
                    return controlRoles !=
                           ControlRole.None;
                }

                set
                {
                    SetControlRole(
                        ControlRole.Normal,
                        value);
                }
            }

            public bool IsIgnored
            {
                get
                {
                    return (Tags &
                            BlockTags.Ignore) != 0;
                }
            }

            public bool IsExplicitlyUsed
            {
                get
                {
                    return (Tags &
                            BlockTags.Use) != 0;
                }
            }

            public double EffectiveCapacity
            {
                get
                {
                    if (!Controlled ||
                        TheBlock == null ||
                        TheBlock.Closed ||
                        !TheBlock.IsFunctional ||
                        !TheBlock.Enabled)
                    {
                        return 0;
                    }

                    return NominalCapacity *
                           MathHelper.Clamp(
                               TheBlock.GyroPower,
                               0,
                               1);
                }
            }

            public Gyro(
                IMyGyro block,
                Program program,
                BlockTags tags,
                bool controlled)
            {
                TheBlock = block;
                this.program = program;
                Tags = tags;

                if (controlled &&
                    !IsIgnored)
                {
                    controlRoles =
                        ControlRole.Normal;
                }

                bool smallGrid =
                    block
                        .CubeGrid
                        .GridSizeEnum ==
                    VRage.Game
                        .MyCubeSize.Small;

                bool prototech =
                    block
                        .BlockDefinition
                        .SubtypeId
                        .IndexOf(
                            "Prototech",
                            StringComparison
                                .OrdinalIgnoreCase) >= 0;

                if (prototech)
                {
                    NominalCapacity =
                        smallGrid
                            ? SmallGridPrototechGyroCapacity
                            : LargeGridPrototechGyroCapacity;
                }
                else
                {
                    NominalCapacity =
                        smallGrid
                            ? SmallGridGyroCapacity
                            : LargeGridGyroCapacity;
                }
            }

            public void SetControlRole(
                ControlRole role,
                bool enabled)
            {
                if (IsIgnored)
                {
                    controlRoles =
                        ControlRole.None;

                    ReleaseOverride();
                    return;
                }

                if (enabled)
                {
                    controlRoles |= role;
                    return;
                }

                controlRoles &= ~role;

                if (controlRoles ==
                    ControlRole.None)
                {
                    ReleaseOverride();
                }
            }

            public bool HasControlRole(
                ControlRole role)
            {
                return (controlRoles &
                        role) != 0;
            }

            public void ApplyWorldCommand(
                Vector3D worldAngularCommand)
            {
                if (!Controlled ||
                    TheBlock == null ||
                    TheBlock.Closed ||
                    EffectiveCapacity <=
                        VectorEpsilon)
                {
                    ReleaseOverride();
                    return;
                }

                worldAngularCommand =
                    VectorMath.ClampMagnitude(
                        worldAngularCommand,
                        GyroCommandAtFullTorque);

                Vector3D localCommand =
                    VectorMath
                        .WorldToLocalDirection(
                            worldAngularCommand,
                            TheBlock.WorldMatrix);

                float pitch =
                    (float)localCommand.X;

                float yaw =
                    (float)localCommand.Y;

                float roll =
                    (float)localCommand.Z;

                bool shouldOverride =
                    localCommand
                        .LengthSquared() >
                    GyroWriteDeadband *
                    GyroWriteDeadband;

                if (!shouldOverride)
                {
                    ReleaseOverride();
                    return;
                }

                if (!lastOverride ||
                    Math.Abs(
                        pitch -
                        lastPitch) >
                    GyroWriteDeadband)
                {
                    TheBlock.Pitch =
                        pitch;

                    lastPitch =
                        pitch;
                }

                if (!lastOverride ||
                    Math.Abs(
                        yaw -
                        lastYaw) >
                    GyroWriteDeadband)
                {
                    TheBlock.Yaw =
                        yaw;

                    lastYaw =
                        yaw;
                }

                if (!lastOverride ||
                    Math.Abs(
                        roll -
                        lastRoll) >
                    GyroWriteDeadband)
                {
                    TheBlock.Roll =
                        roll;

                    lastRoll =
                        roll;
                }

                if (!lastOverride ||
                    !TheBlock.GyroOverride)
                {
                    TheBlock.GyroOverride =
                        true;
                }

                lastOverride = true;
            }

            public void ReleaseOverride()
            {
                if (TheBlock == null ||
                    TheBlock.Closed)
                {
                    return;
                }

                if (TheBlock.GyroOverride)
                {
                    TheBlock.GyroOverride =
                        false;
                }

                if (Math.Abs(
                        TheBlock.Pitch) >
                    GyroWriteDeadband)
                {
                    TheBlock.Pitch = 0;
                }

                if (Math.Abs(
                        TheBlock.Yaw) >
                    GyroWriteDeadband)
                {
                    TheBlock.Yaw = 0;
                }

                if (Math.Abs(
                        TheBlock.Roll) >
                    GyroWriteDeadband)
                {
                    TheBlock.Roll = 0;
                }

                lastOverride = false;
                lastPitch = 0;
                lastYaw = 0;
                lastRoll = 0;
            }

            public void Release()
            {
                ReleaseOverride();

                controlRoles =
                    ControlRole.None;
            }
        }

        // ===== Status surfaces =====

        sealed class StatusSurface
        {
            public readonly IMyTerminalBlock Owner;
            public readonly IMyTextSurface Surface;
            public readonly int SurfaceIndex;

            bool initialized;

            public string Key
            {
                get
                {
                    return Owner.EntityId +
                           ":" +
                           SurfaceIndex;
                }
            }

            public StatusSurface(
                IMyTerminalBlock owner,
                IMyTextSurface surface,
                int surfaceIndex)
            {
                Owner = owner;
                Surface = surface;
                SurfaceIndex =
                    surfaceIndex;
            }

            public void Write(
                string text)
            {
                if (Owner == null ||
                    Owner.Closed ||
                    Surface == null)
                {
                    return;
                }

                if (!initialized)
                {
                    Surface.ContentType =
                        VRage.Game.GUI
                            .TextPanel
                            .ContentType
                            .TEXT_AND_IMAGE;

                    Surface.Font =
                        "Monospace";

                    Surface.FontSize =
                        0.8f;

                    Surface.Alignment =
                        VRage.Game.GUI
                            .TextPanel
                            .TextAlignment
                            .LEFT;

                    initialized = true;
                }

                Surface.WriteText(
                    text,
                    false);
            }
        }
    }
}
