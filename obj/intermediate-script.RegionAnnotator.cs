// <mdk sortorder="30" />   // BlockClasses.cs
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame;
using VRageMath;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Text;
using VRage.Game.ModAPI.Ingame.Utilities;
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
":"+
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
partial class Program
{
    // ===== Commands =====

    void HandleArgument(
        string argument)
    {
        if (string.IsNullOrWhiteSpace(
                argument))
        {
            return;
        }

        string[] commands =
            argument.Split(
                new[]
                {
                    ';',
                    '\n',
                    '\r'
                },
                StringSplitOptions
                    .RemoveEmptyEntries);

        bool encounteredCommand =
            false;

        bool encounteredWarning =
            false;

        StringBuilder results =
            new StringBuilder();

        for (int i = 0;
            i < commands.Length;
            i++)
        {
            string command =
                NormalizeCommand(
                    commands[i]);

            if (command.Length == 0)
            {
                continue;
            }

            encounteredCommand =
                true;

            string result;
            bool warning;

            if (!TryHandleCommand(
                    command,
                    out result,
                    out warning))
            {
                result =
"Unknown command: \""+
                    command +
"\"";

                warning =
                    true;
            }

            if (EffectiveCruise &&
                !string.IsNullOrEmpty(
                    cruiseAuthorityWarning))
            {
                if (result.Length > 0)
                {
                    result +=
"\n";
                }

                result +=
                    cruiseAuthorityWarning;

                warning =
                    true;
            }

            if (results.Length > 0)
            {
                results.AppendLine();
            }

            results.Append(result);

            encounteredWarning |=
                warning;
        }

        if (!encounteredCommand)
        {
            return;
        }

        SetCommandResult(
            results.ToString(),
            encounteredWarning);

        Save();
    }

    bool TryHandleCommand(
        string command,
        out string result,
        out bool warning)
    {
        result =
            string.Empty;

        warning =
            false;

        string[] words =
            command.Split(
                new[] { ' ' },
                StringSplitOptions
                    .RemoveEmptyEntries);

        if (words.Length == 0)
        {
            return false;
        }

        int argumentIndex;

        if (IsDampenerCommand(
                words,
                out argumentIndex))
        {
            return TryHandleDampenerCommand(
                words,
                argumentIndex,
                out result,
                out warning);
        }

        if (IsCruiseCommand(
                words,
                out argumentIndex))
        {
            return TryHandleCruiseCommand(
                words,
                argumentIndex,
                out result,
                out warning);
        }

        if (IsParkingCommand(
                words,
                out argumentIndex))
        {
            return TryHandleParkingCommand(
                words,
                argumentIndex,
                out result,
                out warning);
        }

        if (words[0] ==
"unpark"||
            words[0] ==
"undock")
        {
            manualParkRequested =
                false;

            slaveFallbackPark =
                false;

            result =
"Parking: OFF";

            return true;
        }

        if (words[0] ==
"gear")
        {
            return TryHandleGearCommand(
                words,
                1,
                out result,
                out warning);
        }

        if (words[0] ==
"rescan"||
            words[0] ==
"scan"||
            words[0] ==
"refresh")
        {
            if (words.Length != 1)
            {
                result =
"Invalid rescan command: \""+
                    command +
"\"";

                warning =
                    true;

                return true;
            }

            RequestRescan();

            result =
"Deep rescan requested.";

            return true;
        }

        return false;
    }

    bool TryHandleDampenerCommand(
        string[] words,
        int argumentIndex,
        out string result,
        out bool warning)
    {
        result =
            string.Empty;

        warning =
            false;

        bool enabled;

        if (!TryReadToggleState(
                words,
                argumentIndex,
                scriptDampeners,
                out enabled))
        {
            result =
"Expected dampeners "+
"on, off, or toggle.";

            warning =
                true;

            return true;
        }

        SetLocalDampeners(
            enabled);

        result =
"Local dampeners: "+
            (scriptDampeners
                ? "ON"                : "OFF");

        if (mode ==
            OperatingMode.Slave)
        {
            result +=
" (master remains effective while slaved)";
        }

        return true;
    }

    bool TryHandleCruiseCommand(
        string[] words,
        int argumentIndex,
        out string result,
        out bool warning)
    {
        result =
            string.Empty;

        warning =
            false;

        if (argumentIndex >=
            words.Length)
        {
            ToggleCruise();

            result =
                GetLocalCruiseResult();

            return true;
        }

        string operation =
            words[argumentIndex];

        bool state;

        if (TryReadNamedState(
                operation,
                cruise,
                out state))
        {
            if (argumentIndex + 1 !=
                words.Length)
            {
                result =
"Unexpected text after "+
"Cruise state.";

                warning =
                    true;

                return true;
            }

            SetCruiseEnabled(
                state);

            result =
                GetLocalCruiseResult();

            return true;
        }

        double directDelta;

        if (TryParseSignedDelta(
                words,
                argumentIndex,
                out directDelta))
        {
            AdjustCruiseTargetSpeed(
                directDelta);

            result =
                GetLocalCruiseResult();

            return true;
        }

        bool increase =
            operation ==
"increase"||
            operation ==
"increment"||
            operation ==
"up"||
            operation ==
"add"||
            operation ==
"faster";

        bool decrease =
            operation ==
"decrease"||
            operation ==
"decrement"||
            operation ==
"down"||
            operation ==
"subtract"||
            operation ==
"slower";

        if (increase ||
            decrease)
        {
            if (argumentIndex + 2 !=
                words.Length)
            {
                result =
"Cruise adjustment requires "+
"one speed value.";

                warning =
                    true;

                return true;
            }

            double amount;

            if (!double.TryParse(
                    words[
                        argumentIndex + 1],
                    out amount))
            {
                result =
"Invalid Cruise speed: \""+
                    words[
                        argumentIndex + 1] +
"\"";

                warning =
                    true;

                return true;
            }

            amount =
                Math.Abs(amount);

            AdjustCruiseTargetSpeed(
                increase
                    ? amount
                    : -amount);

            result =
                GetLocalCruiseResult();

            return true;
        }

        if (operation ==
"target"||
            operation ==
"speed"||
            operation ==
"set")
        {
            if (argumentIndex + 2 !=
                words.Length)
            {
                result =
"Cruise target requires "+
"one speed value.";

                warning =
                    true;

                return true;
            }

            double target;

            if (!double.TryParse(
                    words[
                        argumentIndex + 1],
                    out target))
            {
                result =
"Invalid Cruise target: \""+
                    words[
                        argumentIndex + 1] +
"\"";

                warning =
                    true;

                return true;
            }

            cruiseTargetSpeed =
                target;

            cruiseTargetInitialized =
                true;

            result =
                GetLocalCruiseResult();

            return true;
        }

        result =
"Expected Cruise on, off, toggle, "+
"+value, -value, increase value, "+
"decrease value, or target value.";

        warning =
            true;

        return true;
    }

    bool TryHandleParkingCommand(
        string[] words,
        int argumentIndex,
        out string result,
        out bool warning)
    {
        result =
            string.Empty;

        warning =
            false;

        bool enabled;

        if (!TryReadToggleState(
                words,
                argumentIndex,
                manualParkRequested,
                out enabled))
        {
            result =
"Expected parking "+
"on, off, or toggle.";

            warning =
                true;

            return true;
        }

        manualParkRequested =
            enabled;

        slaveFallbackPark =
            false;

        result =
"Parking: "+
            (manualParkRequested
                ? "ON"                : "OFF");

        return true;
    }

    bool TryHandleGearCommand(
        string[] words,
        int argumentIndex,
        out string result,
        out bool warning)
    {
        result =
            string.Empty;

        warning =
            false;

        int gearCount =
            settings
                .GearFractions.Count;

        if (gearCount <= 0)
        {
            result =
"No gears are configured.";

            warning =
                true;

            return true;
        }

        if (argumentIndex >=
            words.Length)
        {
            selectedGear =
                (selectedGear + 1) %
                gearCount;

            result =
                GetLocalGearResult();

            return true;
        }

        if (argumentIndex + 1 !=
            words.Length)
        {
            result =
"Gear accepts one argument.";

            warning =
                true;

            return true;
        }

        string operation =
            words[argumentIndex];

        if (operation ==
"next"||
            operation ==
"up"||
            operation ==
"increase"||
            operation ==
"increment")
        {
            selectedGear =
                (selectedGear + 1) %
                gearCount;

            result =
                GetLocalGearResult();

            return true;
        }

        if (operation ==
"previous"||
            operation ==
"prev"||
            operation ==
"down"||
            operation ==
"decrease"||
            operation ==
"decrement")
        {
            selectedGear--;

            if (selectedGear < 0)
            {
                selectedGear =
                    gearCount - 1;
            }

            result =
                GetLocalGearResult();

            return true;
        }

        int requestedGear;

        if (!int.TryParse(
                operation,
                out requestedGear))
        {
            result =
"Invalid gear: \""+
                operation +
"\"";

            warning =
                true;

            return true;
        }

        if (requestedGear < 1 ||
            requestedGear > gearCount)
        {
            result =
"Gear must be between 1 and "+
                gearCount +
".";

            warning =
                true;

            return true;
        }

        selectedGear =
            requestedGear - 1;

        result =
            GetLocalGearResult();

        return true;
    }

    static bool IsDampenerCommand(
        string[] words,
        out int argumentIndex)
    {
        argumentIndex = 1;

        if (words[0] ==
"dampeners"||
            words[0] ==
"dampener"||
            words[0] ==
"dampers"||
            words[0] ==
"damper"||
            words[0] ==
"damping"||
            words[0] ==
"dampening")
        {
            return true;
        }

        if (words.Length >= 2 &&
            words[0] ==
"inertia"&&
            (words[1] ==
"dampeners"||
             words[1] ==
"dampener"||
             words[1] ==
"damping"))
        {
            argumentIndex = 2;
            return true;
        }

        return false;
    }

    static bool IsCruiseCommand(
        string[] words,
        out int argumentIndex)
    {
        argumentIndex = 1;

        if (words.Length >= 2 &&
            words[0] ==
"cruise"&&
            words[1] ==
"control")
        {
            argumentIndex = 2;
            return true;
        }

        return words[0] ==
"cruise"||
            words[0] ==
"cruisecontrol";
    }

    static bool IsParkingCommand(
        string[] words,
        out int argumentIndex)
    {
        argumentIndex = 1;

        return words[0] ==
"park"||
               words[0] ==
"parking";
    }

    static bool TryReadToggleState(
        string[] words,
        int argumentIndex,
        bool currentValue,
        out bool result)
    {
        if (argumentIndex >=
            words.Length)
        {
            result =
                !currentValue;

            return true;
        }

        if (argumentIndex + 1 !=
            words.Length)
        {
            result =
                currentValue;

            return false;
        }

        return TryReadNamedState(
            words[argumentIndex],
            currentValue,
            out result);
    }

    static bool TryReadNamedState(
        string operation,
        bool currentValue,
        out bool result)
    {
        if (operation ==
"on"||
            operation ==
"enable"||
            operation ==
"enabled"||
            operation ==
"start")
        {
            result = true;
            return true;
        }

        if (operation ==
"off"||
            operation ==
"disable"||
            operation ==
"disabled"||
            operation ==
"stop")
        {
            result = false;
            return true;
        }

        if (operation ==
"toggle"||
            operation ==
"switch")
        {
            result =
                !currentValue;

            return true;
        }

        result =
            currentValue;

        return false;
    }

    static bool TryParseSignedDelta(
        string[] words,
        int argumentIndex,
        out double delta)
    {
        delta = 0;

        if (argumentIndex >=
            words.Length)
        {
            return false;
        }

        if (argumentIndex + 1 ==
            words.Length)
        {
            string serialized =
                words[argumentIndex];

            if (serialized.Length < 2 ||
                serialized[0] != '+' &&
                serialized[0] != '-')
            {
                return false;
            }

            return double.TryParse(
                serialized,
                out delta);
        }

        if (argumentIndex + 2 ==
                words.Length &&
            (words[argumentIndex] ==
"+"||
             words[argumentIndex] ==
"-"))
        {
            double amount;

            if (!double.TryParse(
                    words[
                        argumentIndex + 1],
                    out amount))
            {
                return false;
            }

            delta =
                words[argumentIndex] ==
"+"                    ? Math.Abs(amount)
                    : -Math.Abs(amount);

            return true;
        }

        return false;
    }

    string GetLocalCruiseResult()
    {
        string result =
"Local Cruise: "+
            (cruise
                ? "ON"                : "OFF");

        if (cruiseTargetInitialized)
        {
            result +=
" @ "+
                cruiseTargetSpeed
                    .ToString("0.###") +
" m/s";
        }

        if (mode ==
            OperatingMode.Slave)
        {
            result +=
" (master remains effective while slaved)";
        }

        return result;
    }

    string GetLocalGearResult()
    {
        int gearCount =
            settings
                .GearFractions.Count;

        double percentage =
            gearCount > 0
                ? settings
                    .GearFractions[
                        MathHelper.Clamp(
                            selectedGear,
                            0,
                            gearCount - 1)] *
                  100
                : 0;

        string result =
"Local gear: "+
            (selectedGear + 1) +
"/"+
            gearCount +
" ("+
            percentage
                .ToString("0.##") +
"%)";

        if (mode ==
            OperatingMode.Slave)
        {
            result +=
" (master remains effective while slaved)";
        }

        return result;
    }

    static string NormalizeCommand(
        string command)
    {
        if (string.IsNullOrWhiteSpace(
                command))
        {
            return string.Empty;
        }

        command =
            command
                .Trim()
                .ToLowerInvariant();

        StringBuilder normalized =
            new StringBuilder(
                command.Length);

        bool previousWasWhitespace =
            false;

        for (int i = 0;
            i < command.Length;
            i++)
        {
            char character =
                command[i];

            bool whitespace =
                char.IsWhiteSpace(
                    character);

            if (whitespace)
            {
                if (!previousWasWhitespace &&
                    normalized.Length > 0)
                {
                    normalized.Append(' ');
                }

                previousWasWhitespace =
                    true;

                continue;
            }

            normalized.Append(
                character);

            previousWasWhitespace =
                false;
        }

        if (normalized.Length > 0 &&
            normalized[
                normalized.Length - 1] ==
            ' ')
        {
            normalized.Length--;
        }

        return normalized.ToString();
    }
}
partial class Program
{
    sealed class Settings
    {
        // Ownership
        public bool Greedy = true;
        public bool CanMaster = true;
        public bool CanSlave = true;

        // Parking
        public bool ParkOnlyByCommand;

        // Flight
        public bool CruiseLevelsWithGravity = true;
        public readonly List<double> GearFractions = new List<double>
        {
            0.15,
            0.50,
            1.00
        };

        // Tags
        public string UseTag = "[VT-use]";
        public string IgnoreTag = "[VT-ignore]";
        public string StatusTag = "[VT-status]";
        public string ParkTimerTag = "[VT-park]";
        public string UnparkTimerTag = "[VT-unpark]";

        // Performance
        public int Update1Skip;
        public int Update10Skip;
        public int Update100Skip;
    }

    readonly MyIni configurationIni = new MyIni();

    bool LoadConfiguration(bool initialLoad)
    {
        string customData = Me.CustomData ?? string.Empty;

        if (!initialLoad && customData == knownProgrammableBlockCustomData)
        {
            return false;
        }

        configurationIni.Clear();

        MyIniParseResult parseResult;

        if (!configurationIni.TryParse(customData, out parseResult))
        {
            Echo(
                ScriptName +
"\n\nCustom Data could not be parsed as INI:\n"+
                parseResult);

            knownProgrammableBlockCustomData = customData;
            return false;
        }

        bool oldGreedy = settings.Greedy;
        bool oldCanMaster = settings.CanMaster;
        bool oldCanSlave = settings.CanSlave;

        string oldUseTag = settings.UseTag;
        string oldIgnoreTag = settings.IgnoreTag;
        string oldStatusTag = settings.StatusTag;
        string oldParkTimerTag = settings.ParkTimerTag;
        string oldUnparkTimerTag = settings.UnparkTimerTag;

        // ===== Ownership =====

        settings.Greedy = configurationIni
            .Get(ConfigSection, "Greedy")
            .ToBoolean(settings.Greedy);

        settings.CanMaster = configurationIni
            .Get(ConfigSection, "CanMaster")
            .ToBoolean(settings.CanMaster);

        settings.CanSlave = configurationIni
            .Get(ConfigSection, "CanSlave")
            .ToBoolean(settings.CanSlave);

        // ===== Parking =====

        settings.ParkOnlyByCommand = configurationIni
            .Get("Parking", "ParkOnlyByCommand")
            .ToBoolean(settings.ParkOnlyByCommand);

        // ===== Flight =====

        settings.CruiseLevelsWithGravity = configurationIni
            .Get("Flight", "CruiseLevelsWithGravity")
            .ToBoolean(settings.CruiseLevelsWithGravity);

        ReadGearFractions(
            configurationIni.Get("Flight", "GearPercentages")
                .ToString("15; 50; 100"));

        if (selectedGear >= settings.GearFractions.Count)
        {
            selectedGear = settings.GearFractions.Count - 1;
        }

        // ===== Tags =====

        settings.UseTag = ReadNonEmptyString(
"Tags",
"Use",
            settings.UseTag);

        settings.IgnoreTag = ReadNonEmptyString(
"Tags",
"Ignore",
            settings.IgnoreTag);

        settings.StatusTag = ReadNonEmptyString(
"Tags",
"Status",
            settings.StatusTag);

        settings.ParkTimerTag = ReadNonEmptyString(
"Tags",
"ParkTimer",
            settings.ParkTimerTag);

        settings.UnparkTimerTag = ReadNonEmptyString(
"Tags",
"UnparkTimer",
            settings.UnparkTimerTag);

        // ===== Performance =====

        settings.Update1Skip = Math.Max(
            0,
            configurationIni
                .Get("Performance", "Update1Skip")
                .ToInt32(settings.Update1Skip));

        settings.Update10Skip = Math.Max(
            0,
            configurationIni
                .Get("Performance", "Update10Skip")
                .ToInt32(settings.Update10Skip));

        settings.Update100Skip = Math.Max(
            0,
            configurationIni
                .Get("Performance", "Update100Skip")
                .ToInt32(settings.Update100Skip));

        WriteConfigurationDefaults();

        string normalizedConfiguration = configurationIni.ToString();

        if (normalizedConfiguration != Me.CustomData)
        {
            Me.CustomData = normalizedConfiguration;
        }

        knownProgrammableBlockCustomData = Me.CustomData;

        bool discoverySettingsChanged =
            oldGreedy != settings.Greedy ||
            oldCanMaster != settings.CanMaster ||
            oldCanSlave != settings.CanSlave ||
            !oldUseTag.Equals(settings.UseTag, StringComparison.OrdinalIgnoreCase) ||
            !oldIgnoreTag.Equals(settings.IgnoreTag, StringComparison.OrdinalIgnoreCase) ||
            !oldStatusTag.Equals(settings.StatusTag, StringComparison.OrdinalIgnoreCase) ||
            !oldParkTimerTag.Equals(settings.ParkTimerTag, StringComparison.OrdinalIgnoreCase) ||
            !oldUnparkTimerTag.Equals(settings.UnparkTimerTag, StringComparison.OrdinalIgnoreCase);

        if (!initialLoad && discoverySettingsChanged)
        {
            RequestRescan();
        }

        return true;
    }

    string ReadNonEmptyString(
        string section,
        string key,
        string fallback)
    {
        string value = configurationIni
            .Get(section, key)
            .ToString(fallback)
            .Trim();

        return value.Length == 0 ? fallback : value;
    }

    void ReadGearFractions(
        string serializedPercentages)
    {
        string[] values =
            serializedPercentages.Split(
                new[] { ';' },
                StringSplitOptions
                    .RemoveEmptyEntries);

        List<double> parsed =
            new List<double>();

        for (int i = 0;
            i < values.Length;
            i++)
        {
            double percentage;

            if (!double.TryParse(
                    values[i].Trim(),
                    out percentage))
            {
                continue;
            }

            if (percentage > 0)
            {
                parsed.Add(
                    percentage /
                    100.0);
            }
        }

        if (parsed.Count == 0)
        {
            return;
        }

        settings.GearFractions.Clear();

        settings.GearFractions.AddRange(
            parsed);
    }

    void WriteConfigurationDefaults()
    {
        configurationIni.Set(ConfigSection, "Greedy", settings.Greedy);
        configurationIni.Set(ConfigSection, "CanMaster", settings.CanMaster);
        configurationIni.Set(ConfigSection, "CanSlave", settings.CanSlave);

        configurationIni.SetSectionComment(
            ConfigSection,
" Vector Thrust Redux ownership and coordination.\n"+
" Greedy controls eligible mechanical-subgrid blocks unless ignored.\n"+
" Main-grid player thrusters and gyros remain read-only unless explicitly tagged.");

        configurationIni.Set(
"Parking",
"ParkOnlyByCommand",
            settings.ParkOnlyByCommand);

        configurationIni.Set(
"Flight",
"CruiseLevelsWithGravity",
            settings.CruiseLevelsWithGravity);

        configurationIni.Set(
"Flight",
"GearPercentages",
            SerializeGearPercentages());

        configurationIni.Set("Tags", "Use", settings.UseTag);
        configurationIni.Set("Tags", "Ignore", settings.IgnoreTag);
        configurationIni.Set("Tags", "Status", settings.StatusTag);
        configurationIni.Set("Tags", "ParkTimer", settings.ParkTimerTag);
        configurationIni.Set("Tags", "UnparkTimer", settings.UnparkTimerTag);

        configurationIni.SetComment(
"Tags",
"Use",
" Tag may appear in a block name, group name, or block Custom Data.");

        configurationIni.SetComment(
"Tags",
"Ignore",
" Ignore always prevents Redux from modifying the block.");

        configurationIni.Set(
"Performance",
"Update1Skip",
            settings.Update1Skip);

        configurationIni.Set(
"Performance",
"Update10Skip",
            settings.Update10Skip);

        configurationIni.Set(
"Performance",
"Update100Skip",
            settings.Update100Skip);

        configurationIni.SetSectionComment(
"Performance",
" Number of matching update intervals skipped between executions.\n"+
" Heartbeat publication is never skipped.");
    }

    string SerializeGearPercentages()
    {
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < settings.GearFractions.Count; i++)
        {
            if (i > 0)
            {
                builder.Append("; ");
            }

            builder.Append(
                (settings.GearFractions[i] * 100.0)
                    .ToString("0.########"));
        }

        return builder.ToString();
    }
}
partial class Program
{
    readonly HashSet<VectorThrust> aimedNacelles =
        new HashSet<VectorThrust>();

    readonly HashSet<VectorThrust> availableNacelles =
        new HashSet<VectorThrust>();

    // ===== Scheduled work =====

    void RunUpdate100()
    {
        LoadConfiguration(false);

        // The lightweight fingerprint implementation arrives with the
        // scanner replacement. It requests a deep scan only when relevant
        // topology or discovery tags have changed.
        CheckTopologyFingerprint();
    }

    void RunUpdate10()
    {
        SelectReferenceController();
        DetectTopologyChanges();

        RefreshTemporaryControlRoles();
        SynchronizeDampenerState();
        RefreshEffectiveCapacity();

        potentialMaster =
            settings.CanMaster &&
            referenceController != null &&
            referenceController
                .IsUnderControl;

        UpdateAutomaticParkRequest();

        if (settings.CanSlave &&
            !potentialMaster &&
            !manualParkRequested)
        {
            TryReadAnySlaveHeartbeat();
        }

        if (slaveHeartbeatChangedThisWindow)
        {
            slaveHeartbeatAgeUpdate10 = 0;
        }
        else if (lastSlaveHeartbeatSequence !=
                 long.MinValue)
        {
            slaveHeartbeatAgeUpdate10++;
        }

        slaveHeartbeatFresh =
            lastSlaveHeartbeatSequence !=
                long.MinValue &&
            slaveHeartbeatAgeUpdate10 < 2;

        slaveHeartbeatChangedThisWindow =
            false;

        EvaluateOperatingMode();

        if (mode ==
            OperatingMode.Parked)
        {
            UpdateParkedRotors();
        }

        WriteStatus(false);
    }

    void RunFlightControl(
        double timeStep)
    {
        SelectReferenceController();

        if (mode ==
                OperatingMode.Parked ||
            mode ==
                OperatingMode.Initializing ||
            referenceController == null)
        {
            ClearControlledThrust();
            ReleaseGyros();
            return;
        }

        RestoreThrustersAfterPark();
        RefreshTemporaryControlRoles();
        SynchronizeDampenerState();
        RefreshEffectiveCapacity();

        Vector3D centerOfMass =
            referenceController.CenterOfMass;

        Vector3D request;

        if (mode ==
            OperatingMode.Slave)
        {
            Vector3D demand =
                VectorMath.ClampMagnitude(
                    activeSlaveCommand
                        .NormalizedForceDemand,
                    1);

            Vector3D direction =
                VectorMath.SafeNormalize(
                    demand);

            double demandFraction =
                demand.Length();

            double localCapacity =
                GetLocalProjectedCapacity(
                    direction);

            request =
                direction *
                demandFraction *
                localCapacity;
        }
        else
        {
            Vector3D constructRequest =
                CalculateLocalForceRequest(
                    timeStep);

            constructRequest -=
                GetUnmanagedConstructForceWorld();

            if (mode ==
                OperatingMode.Master)
            {
                Vector3D direction =
                    VectorMath.SafeNormalize(
                        constructRequest);

                double localCapacity =
                    GetLocalProjectedCapacity(
                        direction);

                double remoteCapacity =
                    GetRemoteReduxProjectedCapacity(
                        direction);

                double participantCapacity =
                    localCapacity +
                    remoteCapacity;

                double demandFraction =
                    participantCapacity >
                            ForceEpsilon
                        ? MathHelper.Clamp(
                            constructRequest
                                .Length() /
                            participantCapacity,
                            0,
                            1)
                        : 0;

                normalizedMasterDemand =
                    direction *
                    demandFraction;

                request =
                    direction *
                    demandFraction *
                    localCapacity;
            }
            else
            {
                normalizedMasterDemand =
                    Vector3D.Zero;

                request =
                    constructRequest;
            }
        }

        requestedForceWorld =
            request;

        AllocateControlledThrust(
            request,
            centerOfMass,
            timeStep);

        bool levelWithGravity =
            mode ==
                    OperatingMode.Slave
                ? activeSlaveCommand
                    .LevelWithGravity
                : EffectiveCruise &&
                  settings
                      .CruiseLevelsWithGravity;

        ApplyGyroControl(
            inducedTorqueWorld,
            levelWithGravity);
    }

    void RefreshEffectiveCapacity()
    {
        for (int i = 0;
            i < vectorThrusters.Count;
            i++)
        {
            vectorThrusters[i]
                .RefreshPrimaryDirection();
        }

        RefreshAvailableControlledThrust();
        RefreshDirectionalCapacity();
    }

    void RefreshAvailableControlledThrust()
    {
        availableControlledThrust = 0;

        for (int i = 0;
            i < thrusters.Count;
            i++)
        {
            Thruster thruster =
                thrusters[i];

            if (!thruster.Controlled ||
                !thruster.IsUsable)
            {
                continue;
            }

            availableControlledThrust +=
                thruster
                    .MaximumEffectiveThrust;
        }
    }

    void RefreshDirectionalCapacity()
    {
        localDirectionalCapacity.Clear();
        remoteReduxDirectionalCapacity.Clear();
        constructDirectionalCapacity.Clear();

        if (referenceController ==
            null)
        {
            return;
        }

        MatrixD matrix =
            referenceController
                .WorldMatrix;

        localDirectionalCapacity.Forward =
            GetLocalProjectedCapacity(
                matrix.Forward);

        localDirectionalCapacity.Backward =
            GetLocalProjectedCapacity(
                matrix.Backward);

        localDirectionalCapacity.Left =
            GetLocalProjectedCapacity(
                matrix.Left);

        localDirectionalCapacity.Right =
            GetLocalProjectedCapacity(
                matrix.Right);

        localDirectionalCapacity.Up =
            GetLocalProjectedCapacity(
                matrix.Up);

        localDirectionalCapacity.Down =
            GetLocalProjectedCapacity(
                matrix.Down);

        remoteReduxDirectionalCapacity.Forward =
            GetRemoteReduxProjectedCapacity(
                matrix.Forward);

        remoteReduxDirectionalCapacity.Backward =
            GetRemoteReduxProjectedCapacity(
                matrix.Backward);

        remoteReduxDirectionalCapacity.Left =
            GetRemoteReduxProjectedCapacity(
                matrix.Left);

        remoteReduxDirectionalCapacity.Right =
            GetRemoteReduxProjectedCapacity(
                matrix.Right);

        remoteReduxDirectionalCapacity.Up =
            GetRemoteReduxProjectedCapacity(
                matrix.Up);

        remoteReduxDirectionalCapacity.Down =
            GetRemoteReduxProjectedCapacity(
                matrix.Down);

        constructDirectionalCapacity.CopyFrom(
            localDirectionalCapacity);

        constructDirectionalCapacity.Add(
            remoteReduxDirectionalCapacity);
    }

    double GetLocalProjectedCapacity(
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
            i < thrusters.Count;
            i++)
        {
            Thruster thruster =
                thrusters[i];

            if (!thruster.Controlled ||
                !thruster.IsUsable)
            {
                continue;
            }

            if (thruster.Nacelle != null &&
                thruster.Nacelle
                    .Rotor
                    .Controlled)
            {
                continue;
            }

            capacity +=
                thruster
                    .GetCurrentProjectedCapacity(
                        targetDirection);
        }

        for (int i = 0;
            i < vectorThrusters.Count;
            i++)
        {
            VectorThrust nacelle =
                vectorThrusters[i];

            if (!nacelle
                    .Rotor
                    .Controlled)
            {
                continue;
            }

            capacity +=
                nacelle
                    .GetMaximumProjectedCapacity(
                        targetDirection);
        }

        return capacity;
    }

    double GetCommandProjectedCapacity(
        Vector3D targetDirection)
    {
        double capacity =
            GetLocalProjectedCapacity(
                targetDirection);

        if (mode ==
            OperatingMode.Master)
        {
            capacity +=
                GetRemoteReduxProjectedCapacity(
                    targetDirection);
        }

        return capacity;
    }

    double GetRemoteReduxProjectedCapacity(
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

        double capacity =
            GetRemoteFixedProjectedCapacity(
                remoteFixedReduxThrusters,
                targetDirection);

        for (int i = 0;
            i < remoteNacelles.Count;
            i++)
        {
            capacity +=
                remoteNacelles[i]
                    .GetMaximumProjectedCapacity(
                        targetDirection);
        }

        return capacity;
    }

    // ===== Input and force calculation =====

    Vector3D CalculateLocalForceRequest(
        double timeStep)
    {
        MyShipMass shipMass =
            referenceController
                .CalculateShipMass();

        double physicalMass =
            shipMass.PhysicalMass;

        if (physicalMass <=
            ForceEpsilon)
        {
            return Vector3D.Zero;
        }

        MyShipVelocities velocities =
            referenceController
                .GetShipVelocities();

        Vector3D velocity =
            velocities.LinearVelocity;

        Vector3D gravity =
            referenceController
                .GetNaturalGravity();

        Vector3 moveIndicator =
            referenceController
                .MoveIndicator;

        Vector3D desiredAcceleration;

        if (EffectiveCruise)
        {
            desiredAcceleration =
                CalculateCruiseAcceleration(
                    moveIndicator,
                    velocity,
                    physicalMass,
                    timeStep);
        }
        else
        {
            desiredAcceleration =
                CalculateNormalAcceleration(
                    moveIndicator,
                    velocity,
                    physicalMass,
                    timeStep);
        }

        // F_thrust + m*g = m*a
        // therefore F_thrust = m*(a - g).
        return physicalMass *
               (desiredAcceleration -
                gravity);
    }

    Vector3D CalculateNormalAcceleration(
        Vector3 moveIndicator,
        Vector3D velocity,
        double physicalMass,
        double timeStep)
    {
        Vector3D movementInput =
            Vector3D.TransformNormal(
                moveIndicator,
                referenceController
                    .WorldMatrix);

        Vector3D movementDirection =
            VectorMath.SafeNormalize(
                movementInput);

        bool hasMovementInput =
            movementDirection
                .LengthSquared() >
            VectorEpsilon;

        Vector3D desiredAcceleration =
            Vector3D.Zero;

        if (hasMovementInput)
        {
            double directionalCapacity =
                GetCommandProjectedCapacity(
                    movementDirection);

            double acceleration =
                directionalCapacity /
                physicalMass *
                EffectiveGearFraction;

            desiredAcceleration =
                movementDirection *
                acceleration;
        }

        if (!EffectiveDampeners)
        {
            return ClampAccelerationToCapacity(
                desiredAcceleration,
                physicalMass);
        }

        Vector3D velocityToDamp =
            velocity;

        if (velocityToDamp.Length() <=
            VelocityControlEpsilon)
        {
            velocityToDamp =
                Vector3D.Zero;
        }

        if (hasMovementInput)
        {
            double desiredDirectionSpeed =
                Vector3D.Dot(
                    velocityToDamp,
                    movementDirection);

            if (desiredDirectionSpeed > 0)
            {
                velocityToDamp -=
                    movementDirection *
                    desiredDirectionSpeed;
            }
        }

        if (velocityToDamp
                .LengthSquared() >
            VectorEpsilon)
        {
            desiredAcceleration +=
                -velocityToDamp /
                Math.Max(
                    timeStep,
                    MinimumTimeStep);
        }

        return ClampAccelerationToCapacity(
            desiredAcceleration,
            physicalMass);
    }

    Vector3D CalculateCruiseAcceleration(
        Vector3 moveIndicator,
        Vector3D velocity,
        double physicalMass,
        double timeStep)
    {
        if (mode !=
            OperatingMode.Slave)
        {
            EnsureCruiseTargetInitialized();

            double longitudinalInput =
                -moveIndicator.Z;

            if (Math.Abs(
                    longitudinalInput) >
                VectorEpsilon)
            {
                Vector3D adjustmentDirection =
                    longitudinalInput >= 0
                        ? referenceController
                            .WorldMatrix
                            .Forward
                        : referenceController
                            .WorldMatrix
                            .Backward;

                double capacity =
                    GetCommandProjectedCapacity(
                        adjustmentDirection);

                double availableAcceleration =
                    capacity /
                    physicalMass *
                    EffectiveGearFraction;

                cruiseTargetSpeed +=
                    longitudinalInput *
                    availableAcceleration *
                    timeStep;
            }
        }

        Vector3 lateralInput =
            moveIndicator;

        // Longitudinal input adjusts target speed rather than adding a
        // second competing acceleration request.
        lateralInput.Z = 0;

        Vector3D lateralWorldInput =
            Vector3D.TransformNormal(
                lateralInput,
                referenceController
                    .WorldMatrix);

        Vector3D lateralDirection =
            VectorMath.SafeNormalize(
                lateralWorldInput);

        Vector3D desiredAcceleration =
            Vector3D.Zero;

        if (lateralDirection
                .LengthSquared() >
            VectorEpsilon)
        {
            double lateralCapacity =
                GetCommandProjectedCapacity(
                    lateralDirection);

            desiredAcceleration +=
                lateralDirection *
                (lateralCapacity /
                 physicalMass *
                 EffectiveGearFraction);
        }

        Vector3D forward =
            referenceController
                .WorldMatrix
                .Forward;

        Vector3D desiredVelocity =
            forward *
            EffectiveCruiseTargetSpeed;

        Vector3D velocityError =
            desiredVelocity -
            velocity;

        if (!EffectiveDampeners)
        {
            // Cruise always owns longitudinal speed. The local dampener
            // mode decides whether Cruise also removes lateral drift.
            velocityError =
                VectorMath.Projection(
                    velocityError,
                    forward);
        }

        if (velocityError.Length() <=
            VelocityControlEpsilon)
        {
            velocityError =
                Vector3D.Zero;
        }

        if (velocityError
                .LengthSquared() >
            VectorEpsilon)
        {
            desiredAcceleration +=
                velocityError /
                Math.Max(
                    timeStep,
                    MinimumTimeStep);
        }

        return ClampAccelerationToCapacity(
            desiredAcceleration,
            physicalMass);
    }

    Vector3D ClampAccelerationToCapacity(
        Vector3D acceleration,
        double physicalMass)
    {
        double accelerationMagnitude =
            acceleration.Length();

        if (accelerationMagnitude <=
                VectorEpsilon ||
            physicalMass <=
                ForceEpsilon)
        {
            return Vector3D.Zero;
        }

        Vector3D direction =
            acceleration /
            accelerationMagnitude;

        double maximumAcceleration =
            GetCommandProjectedCapacity(
                direction) /
            physicalMass;

        return VectorMath.ClampMagnitude(
            acceleration,
            maximumAcceleration);
    }

    Vector3D GetUnmanagedConstructForceWorld()
    {
        Vector3D force =
            Vector3D.Zero;

        for (int i = 0;
            i < localUnmanagedThrusters.Count;
            i++)
        {
            Thruster thruster =
                localUnmanagedThrusters[i];

            if (!thruster.Controlled)
            {
                force +=
                    thruster.CurrentForceWorld;
            }
        }

        if (mode ==
            OperatingMode.Master)
        {
            for (int i = 0;
                i < remoteUnmanagedThrusters.Count;
                i++)
            {
                Thruster thruster =
                    remoteUnmanagedThrusters[i];

                if (!thruster.Controlled)
                {
                    force +=
                        thruster.CurrentForceWorld;
                }
            }
        }

        return force;
    }

    // Retained temporarily for old status and compatibility call sites.
    // It no longer includes remote Redux-managed output.
    Vector3D GetObservedForceWorld()
    {
        return GetUnmanagedConstructForceWorld();
    }

    // ===== Force allocation =====

    void AllocateControlledThrust(
        Vector3D request,
        Vector3D centerOfMass,
        double timeStep)
    {
        residualForceWorld =
            request;

        inducedTorqueWorld =
            Vector3D.Zero;

        for (int i = 0;
            i < thrusters.Count;
            i++)
        {
            if (thrusters[i].Controlled)
            {
                thrusters[i]
                    .ResetDemand();
            }
        }

        aimedNacelles.Clear();
        availableNacelles.Clear();

        // Fixed controlled sources and sources beyond an unowned rotor are
        // useful at their current orientation.
        for (int i = 0;
            i < thrusters.Count;
            i++)
        {
            Thruster thruster =
                thrusters[i];

            if (!thruster.Controlled ||
                thruster.Nacelle != null &&
                thruster.Nacelle
                    .Rotor
                    .Controlled)
            {
                continue;
            }

            AllocateSingleThruster(
                thruster,
                ref residualForceWorld,
                centerOfMass,
                ref inducedTorqueWorld);
        }

        // Secondary directions are current-orientation sources. Their
        // future direction is included in the nacelle reachability score,
        // but only current force can be allocated this tick.
        for (int i = 0;
            i < vectorThrusters.Count;
            i++)
        {
            VectorThrust nacelle =
                vectorThrusters[i];

            if (!nacelle
                    .Rotor
                    .Controlled)
            {
                continue;
            }

            nacelle.AllocateSecondary(
                ref residualForceWorld,
                centerOfMass,
                ref inducedTorqueWorld);

            availableNacelles.Add(
                nacelle);
        }

        while (availableNacelles.Count >
                   0 &&
               residualForceWorld
                   .LengthSquared() >
               ForceEpsilon *
               ForceEpsilon)
        {
            Vector3D targetDirection =
                VectorMath.SafeNormalize(
                    residualForceWorld);

            VectorThrust bestNacelle =
                null;

            NacelleAimSolution bestSolution =
                null;

            foreach (
                VectorThrust nacelle
                in availableNacelles)
            {
                NacelleAimSolution solution =
                    new NacelleAimSolution();

                if (!nacelle.TrySolveAim(
                        targetDirection,
                        solution))
                {
                    continue;
                }

                bool betterCapacity =
                    bestSolution == null ||
                    solution
                        .ReachableProjectedCapacity >
                    bestSolution
                        .ReachableProjectedCapacity +
                    ForceEpsilon;

                bool equalCapacity =
                    bestSolution != null &&
                    Math.Abs(
                        solution
                            .ReachableProjectedCapacity -
                        bestSolution
                            .ReachableProjectedCapacity) <=
                    ForceEpsilon;

                bool deterministicTieBreak =
                    equalCapacity &&
                    bestNacelle != null &&
                    nacelle
                        .Rotor
                        .EntityId <
                    bestNacelle
                        .Rotor
                        .EntityId;

                if (betterCapacity ||
                    deterministicTieBreak)
                {
                    bestNacelle =
                        nacelle;

                    bestSolution =
                        solution;
                }
            }

            if (bestNacelle == null)
            {
                break;
            }

            availableNacelles.Remove(
                bestNacelle);

            double requestedProjection =
                Vector3D.Dot(
                    residualForceWorld,
                    targetDirection);

            bestNacelle.Aim(
                bestSolution,
                requestedProjection,
                timeStep);

            aimedNacelles.Add(
                bestNacelle);

            bestNacelle.AllocatePrimary(
                ref residualForceWorld,
                centerOfMass,
                ref inducedTorqueWorld);
        }

        for (int i = 0;
            i < vectorThrusters.Count;
            i++)
        {
            VectorThrust nacelle =
                vectorThrusters[i];

            if (!nacelle
                    .Rotor
                    .Controlled)
            {
                continue;
            }

            if (!aimedNacelles.Contains(
                    nacelle))
            {
                nacelle.Rotor.Stop();
            }
        }

        ApplyCruiseThrusterSuppression();

        for (int i = 0;
            i < thrusters.Count;
            i++)
        {
            if (thrusters[i].Controlled)
            {
                thrusters[i]
                    .ApplyDemand();
            }
        }
    }

    void AllocateSingleThruster(
        Thruster thruster,
        ref Vector3D residual,
        Vector3D centerOfMass,
        ref Vector3D inducedTorque)
    {
        double contribution =
            thruster
                .AddOptimalContribution(
                    ref residual);

        if (contribution <=
            ForceEpsilon)
        {
            return;
        }

        Vector3D force =
            thruster
                .ForceDirectionWorld *
            contribution;

        Vector3D lever =
            thruster
                .TheBlock
                .GetPosition() -
            centerOfMass;

        inducedTorque +=
            Vector3D.Cross(
                lever,
                force);
    }

    void ApplyCruiseThrusterSuppression()
    {
        if (!EffectiveCruise)
        {
            ReleaseThrusterDisableReason(
                ThrusterDisableReason.Cruise);

            return;
        }

        for (int i = 0;
            i <
                mainGridReverseThrusters.Count;
            i++)
        {
            Thruster thruster =
                mainGridReverseThrusters[i];

            if (!thruster.Controlled)
            {
                continue;
            }

            if (thruster
                    .DesiredEffectiveThrust >
                ForceEpsilon)
            {
                ReleaseThrusterDisableReason(
                    thruster,
                    ThrusterDisableReason.Cruise);

                PrepareThrusterForControl(
                    thruster);

                continue;
            }

            // A zero override does not reliably prevent vanilla dampeners
            // from applying reverse thrust. Disabling only the owned
            // reverse thrusters makes Cruise authoritative.
            DisableThrusterByRedux(
                thruster,
                ThrusterDisableReason.Cruise);
        }
    }

    // ===== Gyro control =====

    void ApplyGyroControl(
        Vector3D inducedTorque,
        bool levelWithGravity)
    {
        if (controlledGyros.Count == 0 ||
            referenceController == null)
        {
            return;
        }

        double totalGyroCapacity = 0;

        for (int i = 0;
            i < controlledGyros.Count;
            i++)
        {
            totalGyroCapacity +=
                controlledGyros[i]
                    .EffectiveCapacity;
        }

        if (totalGyroCapacity <=
            VectorEpsilon)
        {
            ReleaseGyros();
            return;
        }

        Vector3D angularCommand =
            -inducedTorque /
            totalGyroCapacity *
            GyroCommandAtFullTorque;

        if (levelWithGravity)
        {
            Vector3D gravity =
                referenceController
                    .GetNaturalGravity();

            if (gravity.LengthSquared() >
                VectorEpsilon)
            {
                Vector3D desiredUp =
                    -VectorMath.SafeNormalize(
                        gravity);

                Vector3D currentUp =
                    referenceController
                        .WorldMatrix
                        .Up;

                Vector3D levelingAxis =
                    Vector3D.Cross(
                        currentUp,
                        desiredUp);

                double alignment =
                    MathHelper.Clamp(
                        Vector3D.Dot(
                            currentUp,
                            desiredUp),
                        -1,
                        1);

                double levelingAngle =
                    Math.Atan2(
                        levelingAxis.Length(),
                        alignment);

                if (levelingAxis
                        .LengthSquared() >
                    VectorEpsilon)
                {
                    levelingAxis =
                        VectorMath.SafeNormalize(
                            levelingAxis);

                    angularCommand +=
                        levelingAxis *
                        levelingAngle *
                        GyroLevelGain;
                }

                Vector3D angularVelocity =
                    referenceController
                        .GetShipVelocities()
                        .AngularVelocity;

                Vector3D rollPitchVelocity =
                    VectorMath.Rejection(
                        angularVelocity,
                        desiredUp);

                angularCommand -=
                    rollPitchVelocity *
                    GyroAngularDampingGain;
            }
        }

        if (angularCommand
                .LengthSquared() <=
            GyroWriteDeadband *
            GyroWriteDeadband)
        {
            ReleaseGyros();
            return;
        }

        for (int i = 0;
            i < controlledGyros.Count;
            i++)
        {
            controlledGyros[i]
                .ApplyWorldCommand(
                    angularCommand);
        }
    }

    void ReleaseGyros()
    {
        for (int i = 0;
            i < controlledGyros.Count;
            i++)
        {
            controlledGyros[i]
                .ReleaseOverride();
        }
    }
}
partial class Program
{
    // ===== Master heartbeat =====

    void PublishOrClearMasterHeartbeat()
    {
        if (!settings.CanMaster)
        {
            ClearMasterHeartbeat();
            return;
        }

        if (mode !=
                OperatingMode.Master ||
            referenceController == null ||
            !referenceController
                .IsUnderControl)
        {
            ClearMasterHeartbeat();
            return;
        }

        if (heartbeatController != null &&
            heartbeatController.EntityId !=
                referenceController.EntityId)
        {
            RemoveOwnedHeartbeat(
                heartbeatController);
        }

        heartbeatController =
            referenceController;

        StringBuilder section =
            new StringBuilder();

        section
            .Append('[')
            .Append(HeartbeatSection)
            .AppendLine("]");

        section
            .Append("Version=")
            .AppendLine(
                ScriptVersion);

        section
            .Append(
"MasterProgrammableBlockId=")
            .AppendLine(
                Me.EntityId.ToString());

        section
            .Append("ControllerId=")
            .AppendLine(
                referenceController
                    .EntityId
                    .ToString());

        section
            .Append("Sequence=")
            .AppendLine(
                heartbeatSequence
                    .ToString());

        section
            .Append("Demand=")
            .AppendLine(
                SerializeVector(
                    normalizedMasterDemand));

        section
            .Append("Dampeners=")
            .AppendLine(
                scriptDampeners
                    .ToString());

        section
            .Append("Cruise=")
            .AppendLine(
                cruise.ToString());

        section
            .Append(
"CruiseTargetSpeed=")
            .AppendLine(
                cruiseTargetSpeed
                    .ToString("R"));

        section
            .Append("GearIndex=")
            .AppendLine(
                selectedGear
                    .ToString());

        section
            .Append("GearCount=")
            .AppendLine(
                settings
                    .GearFractions
                    .Count
                    .ToString());

        section
            .Append("GearFraction=")
            .AppendLine(
                EffectiveGearFraction
                    .ToString("R"));

        section
            .Append(
"LevelWithGravity=")
            .AppendLine(
                (cruise &&
                 settings
                     .CruiseLevelsWithGravity)
                .ToString());

        referenceController.CustomData =
            ReplaceSection(
                referenceController
                    .CustomData,
                HeartbeatSection,
                section.ToString());
    }

    void ClearMasterHeartbeat()
    {
        if (heartbeatController ==
            null)
        {
            return;
        }

        RemoveOwnedHeartbeat(
            heartbeatController);

        heartbeatController =
            null;
    }

    void RemoveOwnedHeartbeat(
        IMyShipController controller)
    {
        if (controller == null ||
            controller.Closed)
        {
            return;
        }

        string masterId;

        if (!TryReadSectionValue(
                controller.CustomData,
                HeartbeatSection,
"MasterProgrammableBlockId",
                out masterId))
        {
            return;
        }

        long parsedId;

        if (!long.TryParse(
                masterId,
                out parsedId) ||
            parsedId !=
                Me.EntityId)
        {
            return;
        }

        controller.CustomData =
            RemoveSection(
                controller.CustomData,
                HeartbeatSection);
    }

    void RemoveOwnedHeartbeatsExcept(
        IMyShipController preservedController)
    {
        long preservedId =
            preservedController !=
                    null
                ? preservedController
                    .EntityId
                : 0;

        for (int i = 0;
            i < localControllers.Count;
            i++)
        {
            IMyShipController controller =
                localControllers[i];

            if (controller == null ||
                controller.EntityId ==
                    preservedId)
            {
                continue;
            }

            RemoveOwnedHeartbeat(
                controller);
        }

        // A controller that used to be local can become remote after a
        // connector/topology change. Sweep these too so an old section
        // cannot outlive the in-memory heartbeatController reference.
        for (int i = 0;
            i <
                remotelyReachableControllers
                    .Count;
            i++)
        {
            IMyShipController controller =
                remotelyReachableControllers[i];

            if (controller == null ||
                controller.EntityId ==
                    preservedId)
            {
                continue;
            }

            RemoveOwnedHeartbeat(
                controller);
        }
    }

    void SweepOwnedHeartbeatsAfterScan()
    {
        IMyShipController preserved =
            mode ==
                    OperatingMode.Master &&
                referenceController !=
                    null &&
                referenceController
                    .IsUnderControl
                ? referenceController
                : null;

        RemoveOwnedHeartbeatsExcept(
            preserved);

        if (preserved == null)
        {
            heartbeatController =
                null;
        }
        else
        {
            heartbeatController =
                preserved;
        }
    }

    // ===== Slave heartbeat =====

    void TryReadAnySlaveHeartbeat()
    {
        for (int i = 0;
            i <
                remotelyReachableControllers
                    .Count;
            i++)
        {
            IMyShipController controller =
                remotelyReachableControllers[i];

            // Space Engineers already elects a single actively controlled
            // controller across a connected construct. There's no need to
            // reinvent the wheel here.
            if (controller == null ||
                controller.Closed ||
                !controller.IsUnderControl)
            {
                continue;
            }

            MasterCommand command;

            if (!TryReadMasterCommand(
                    controller,
                    out command))
            {
                continue;
            }

            AcceptSlaveCommand(
                command);

            return;
        }
    }

    void ReadActiveSlaveHeartbeat()
    {
        for (int i = 0;
            i <
                remotelyReachableControllers
                    .Count;
            i++)
        {
            IMyShipController controller =
                remotelyReachableControllers[i];

            if (controller == null ||
                controller.Closed ||
                !controller.IsUnderControl)
            {
                continue;
            }

            if (activeSlaveCommand
                    .ControllerId != 0 &&
                controller.EntityId !=
                    activeSlaveCommand
                        .ControllerId)
            {
                continue;
            }

            MasterCommand command;

            if (!TryReadMasterCommand(
                    controller,
                    out command))
            {
                continue;
            }

            if (activeSlaveCommand
                    .MasterProgrammableBlockId !=
                    0 &&
                command
                    .MasterProgrammableBlockId !=
                activeSlaveCommand
                    .MasterProgrammableBlockId)
            {
                continue;
            }

            AcceptSlaveCommand(
                command);

            return;
        }
    }

    void AcceptSlaveCommand(
        MasterCommand command)
    {
        bool changed =
            command.Sequence !=
                lastSlaveHeartbeatSequence ||
            command
                .MasterProgrammableBlockId !=
                slaveMasterProgrammableBlockId ||
            command.ControllerId !=
                activeSlaveCommand
                    .ControllerId;

        if (changed)
        {
            lastSlaveHeartbeatSequence =
                command.Sequence;

            slaveMasterProgrammableBlockId =
                command
                    .MasterProgrammableBlockId;

            slaveHeartbeatAgeUpdate10 = 0;

            slaveHeartbeatChangedThisWindow =
                true;

            slaveHeartbeatFresh =
                true;
        }

        activeSlaveCommand.CopyFrom(
            command);

        if (mode ==
            OperatingMode.Slave)
        {
            SynchronizeDampenerState();
            RefreshTemporaryControlRoles();
        }
    }

    bool TryReadMasterCommand(
        IMyShipController controller,
        out MasterCommand command)
    {
        command =
            null;

        if (controller == null ||
            controller.Closed ||
            !controller.IsUnderControl)
        {
            return false;
        }

        string versionText;

        if (!TryReadSectionValue(
                controller.CustomData,
                HeartbeatSection,
"Version",
                out versionText) ||
            string.IsNullOrWhiteSpace(versionText))
        {
            return false;
        }

        if (GetVersionMajor(versionText) != GetVersionMajor(ScriptVersion))
        {
            return false;
        }
            
        string masterIdText;
        string controllerIdText;
        string sequenceText;
        string demandText;

        if (!TryReadSectionValue(
                controller.CustomData,
                HeartbeatSection,
"MasterProgrammableBlockId",
                out masterIdText) ||
            !TryReadSectionValue(
                controller.CustomData,
                HeartbeatSection,
"ControllerId",
                out controllerIdText) ||
            !TryReadSectionValue(
                controller.CustomData,
                HeartbeatSection,
"Sequence",
                out sequenceText) ||
            !TryReadSectionValue(
                controller.CustomData,
                HeartbeatSection,
"Demand",
                out demandText))
        {
            return false;
        }

        long masterId;
        long controllerId;
        long sequence;
        Vector3D demand;

        if (!long.TryParse(
                masterIdText,
                out masterId) ||
            !long.TryParse(
                controllerIdText,
                out controllerId) ||
            !long.TryParse(
                sequenceText,
                out sequence) ||
            !TryParseVector(
                demandText,
                out demand))
        {
            return false;
        }

        if (masterId ==
                Me.EntityId ||
            controllerId !=
                controller.EntityId)
        {
            return false;
        }

        string dampenersText;
        string cruiseText;
        string cruiseTargetText;
        string gearIndexText;
        string gearCountText;
        string gearFractionText;
        string levelText;

        TryReadSectionValue(
            controller.CustomData,
            HeartbeatSection,
"Dampeners",
            out dampenersText);

        TryReadSectionValue(
            controller.CustomData,
            HeartbeatSection,
"Cruise",
            out cruiseText);

        TryReadSectionValue(
            controller.CustomData,
            HeartbeatSection,
"CruiseTargetSpeed",
            out cruiseTargetText);

        TryReadSectionValue(
            controller.CustomData,
            HeartbeatSection,
"GearIndex",
            out gearIndexText);

        TryReadSectionValue(
            controller.CustomData,
            HeartbeatSection,
"GearCount",
            out gearCountText);

        TryReadSectionValue(
            controller.CustomData,
            HeartbeatSection,
"GearFraction",
            out gearFractionText);

        TryReadSectionValue(
            controller.CustomData,
            HeartbeatSection,
"LevelWithGravity",
            out levelText);

        bool commandDampeners =
            true;

        bool commandCruise;
        bool commandLevel;

        double commandCruiseTarget;
        double commandGearFraction;

        int commandGearIndex;
        int commandGearCount;

        bool parsedDampeners;

        if (bool.TryParse(
                dampenersText,
                out parsedDampeners))
        {
            commandDampeners =
                parsedDampeners;
        }

        bool.TryParse(
            cruiseText,
            out commandCruise);

        bool.TryParse(
            levelText,
            out commandLevel);

        double.TryParse(
            cruiseTargetText,
            out commandCruiseTarget);

        double.TryParse(
            gearFractionText,
            out commandGearFraction);

        int.TryParse(
            gearIndexText,
            out commandGearIndex);

        int.TryParse(
            gearCountText,
            out commandGearCount);

        command =
            new MasterCommand
            {
                MasterProgrammableBlockId =
                    masterId,

                ControllerId =
                    controllerId,

                Sequence =
                    sequence,

                NormalizedForceDemand =
                    VectorMath.ClampMagnitude(
                        demand,
                        1),

                Dampeners =
                    commandDampeners,

                Cruise =
                    commandCruise,

                CruiseTargetSpeed =
                    commandCruiseTarget,

                GearIndex =
                    Math.Max(
                        0,
                        commandGearIndex),

                GearCount =
                    Math.Max(
                        0,
                        commandGearCount),

                GearFraction =
                    MathHelper.Clamp(
                        commandGearFraction,
                        0,
                        1),

                LevelWithGravity =
                    commandLevel
            };

        return true;
    }

    void ResetSlaveHeartbeat()
    {
        slaveHeartbeatFresh =
            false;

        slaveHeartbeatChangedThisWindow =
            false;

        slaveHeartbeatAgeUpdate10 = 0;

        lastSlaveHeartbeatSequence =
            long.MinValue;

        slaveMasterProgrammableBlockId =
            0;

        activeSlaveCommand.Clear();
    }

    static int GetVersionMajor(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            return -1;
        }

        int separator = version.IndexOf('.');

        string majorText =
            separator >= 0
                ? version.Substring(0, separator)
                : version;

        int major;

        return int.TryParse(majorText, out major)
            ? major
            : -1;
    }

    // ===== Custom Data section handling =====

    static int FindSectionStart(
        string customData,
        string sectionName)
    {
        if (string.IsNullOrEmpty(
                customData))
        {
            return -1;
        }

        string header =
"["+
            sectionName +
"]";

        int searchIndex = 0;

        while (searchIndex <
               customData.Length)
        {
            int index =
                customData.IndexOf(
                    header,
                    searchIndex,
                    StringComparison
                        .OrdinalIgnoreCase);

            if (index < 0)
            {
                return -1;
            }

            bool startsLine =
                index == 0 ||
                customData[
                    index - 1] ==
                '\n';

            int after =
                index +
                header.Length;

            bool endsLine =
                after >=
                    customData.Length ||
                customData[after] ==
                    '\r' ||
                customData[after] ==
                    '\n';

            if (startsLine &&
                endsLine)
            {
                return index;
            }

            searchIndex =
                index + 1;
        }

        return -1;
    }

    static int FindNextSectionStart(
        string customData,
        int searchIndex)
    {
        while (searchIndex <
               customData.Length)
        {
            int lineStart =
                customData.IndexOf(
                    '\n',
                    searchIndex);

            if (lineStart < 0 ||
                lineStart + 1 >=
                    customData.Length)
            {
                return customData.Length;
            }

            lineStart++;

            int cursor =
                lineStart;

            while (cursor <
                       customData.Length &&
                   (customData[cursor] ==
                        ' ' ||
                    customData[cursor] ==
                        '\t' ||
                    customData[cursor] ==
                        '\r'))
            {
                cursor++;
            }

            if (cursor <
                    customData.Length &&
                customData[cursor] ==
                    '[')
            {
                int closing =
                    customData.IndexOf(
                        ']',
                        cursor + 1);

                if (closing >= 0)
                {
                    return lineStart;
                }
            }

            searchIndex =
                lineStart;
        }

        return customData.Length;
    }

    static bool TryReadSectionValue(
        string customData,
        string sectionName,
        string key,
        out string value)
    {
        value =
            null;

        int start =
            FindSectionStart(
                customData,
                sectionName);

        if (start < 0)
        {
            return false;
        }

        int end =
            FindNextSectionStart(
                customData,
                start +
                sectionName.Length +
                2);

        int headerEnd =
            customData.IndexOf(
                '\n',
                start);

        if (headerEnd < 0 ||
            headerEnd >= end)
        {
            return false;
        }

        string section =
            customData.Substring(
                headerEnd + 1,
                end -
                headerEnd -
                1);

        string[] lines =
            section
                .Replace(
"\r",
                    string.Empty)
                .Split('\n');

        for (int i = 0;
            i < lines.Length;
            i++)
        {
            string line =
                lines[i];

            int separator =
                line.IndexOf('=');

            if (separator <= 0)
            {
                continue;
            }

            string candidateKey =
                line
                    .Substring(
                        0,
                        separator)
                    .Trim();

            if (!candidateKey.Equals(
                    key,
                    StringComparison
                        .OrdinalIgnoreCase))
            {
                continue;
            }

            value =
                line
                    .Substring(
                        separator + 1)
                    .Trim();

            return true;
        }

        return false;
    }

    static string ReplaceSection(
        string customData,
        string sectionName,
        string replacement)
    {
        customData =
            customData ??
            string.Empty;

        replacement =
            replacement.TrimEnd(
                '\r',
                '\n') +
"\n";

        int start =
            FindSectionStart(
                customData,
                sectionName);

        if (start < 0)
        {
            if (customData.Length == 0)
            {
                return replacement;
            }

            string separator =
                customData.EndsWith(
"\n")
                    ? string.Empty
                    : "\n";

            return customData +
                   separator +
                   replacement;
        }

        int end =
            FindNextSectionStart(
                customData,
                start +
                sectionName.Length +
                2);

        return customData.Substring(
                   0,
                   start) +
               replacement +
               customData.Substring(
                   end);
    }

    static string RemoveSection(
        string customData,
        string sectionName)
    {
        if (string.IsNullOrEmpty(
                customData))
        {
            return customData;
        }

        int start =
            FindSectionStart(
                customData,
                sectionName);

        if (start < 0)
        {
            return customData;
        }

        int end =
            FindNextSectionStart(
                customData,
                start +
                sectionName.Length +
                2);

        string before =
            customData.Substring(
                0,
                start);

        string after =
            customData.Substring(
                end);

        if (before.EndsWith(
"\n") &&
            after.StartsWith(
"\n"))
        {
            after =
                after.Substring(1);
        }

        return before +
               after;
    }

    static string SerializeVector(
        Vector3D vector)
    {
        return vector.X
                   .ToString("R") +
";"+
               vector.Y
                   .ToString("R") +
";"+
               vector.Z
                   .ToString("R");
    }

    static bool TryParseVector(
        string serialized,
        out Vector3D vector)
    {
        vector =
            Vector3D.Zero;

        if (string.IsNullOrWhiteSpace(
                serialized))
        {
            return false;
        }

        string[] components =
            serialized.Split(';');

        if (components.Length !=
            3)
        {
            return false;
        }

        double x;
        double y;
        double z;

        if (!double.TryParse(
                components[0],
                out x) ||
            !double.TryParse(
                components[1],
                out y) ||
            !double.TryParse(
                components[2],
                out z))
        {
            return false;
        }

        vector =
            new Vector3D(
                x,
                y,
                z);

        return true;
    }
}
partial class Program
{
    IMyShipController heartbeatController;

    // ===== Controller selection =====

    void SelectReferenceController()
    {
        IMyShipController previousController =
            referenceController;

        IMyShipController selected =
            null;

        for (int i = 0;
            i < localControllers.Count;
            i++)
        {
            IMyShipController controller =
                localControllers[i];

            if (controller == null ||
                controller.Closed ||
                !controller.IsFunctional ||
                !controller.CanControlShip)
            {
                continue;
            }

            if (controller.IsUnderControl)
            {
                selected =
                    controller;

                break;
            }

            if (selected == null ||
                controller.IsMainCockpit)
            {
                selected =
                    controller;
            }
        }

        long previousId =
            previousController != null
                ? previousController
                    .EntityId
                : 0;

        long selectedId =
            selected != null
                ? selected.EntityId
                : 0;

        if (heartbeatController != null &&
            heartbeatController.EntityId !=
                selectedId)
        {
            RemoveOwnedHeartbeat(
                heartbeatController);

            heartbeatController =
                null;
        }

        if (previousId !=
            selectedId)
        {
            // A skipped control frame must not publish demand calculated
            // for the previous controller.
            normalizedMasterDemand =
                Vector3D.Zero;

            RefreshMainGridReverseThrusters();
        }

        referenceController =
            selected;

        controllerMissing =
            referenceController ==
            null;

        potentialMaster =
            settings.CanMaster &&
            referenceController !=
                null &&
            referenceController
                .IsUnderControl;
    }

    // ===== Operating state =====

    void EvaluateOperatingMode()
    {
        SelectReferenceController();

        if (mode ==
                OperatingMode.Slave &&
            !slaveHeartbeatFresh)
        {
            slaveFallbackPark =
                wasParkedBeforeSlaving;
        }

        OperatingMode requestedMode;

        if (controllerMissing ||
            Me.CubeGrid.IsStatic ||
            manualParkRequested)
        {
            requestedMode =
                OperatingMode.Parked;
        }
        else if (settings.CanSlave &&
                 slaveHeartbeatFresh &&
                 !potentialMaster)
        {
            requestedMode =
                OperatingMode.Slave;
        }
        else if (automaticParkRequested ||
                 slaveFallbackPark)
        {
            requestedMode =
                OperatingMode.Parked;
        }
        else if (potentialMaster)
        {
            requestedMode =
                OperatingMode.Master;
        }
        else
        {
            requestedMode =
                OperatingMode.Active;
        }

        if (requestedMode ==
            mode)
        {
            RefreshTemporaryControlRoles();
            return;
        }

        ChangeOperatingMode(
            requestedMode);
    }

    void ChangeOperatingMode(
        OperatingMode newMode)
    {
        OperatingMode previousMode =
            mode;

        if (previousMode ==
                OperatingMode.Master &&
            newMode !=
                OperatingMode.Master)
        {
            ClearMasterHeartbeat();
            normalizedMasterDemand =
                Vector3D.Zero;
        }

        if (previousMode ==
                OperatingMode.Parked &&
            newMode !=
                OperatingMode.Parked)
        {
            ExitPark();
        }

        if (previousMode ==
                OperatingMode.Slave &&
            newMode !=
                OperatingMode.Slave &&
            !slaveHeartbeatFresh)
        {
            slaveFallbackPark =
                wasParkedBeforeSlaving;
        }

        mode =
            newMode;

        if (newMode ==
            OperatingMode.Slave)
        {
            wasParkedBeforeSlaving =
                previousMode ==
                OperatingMode.Parked;

            slaveFallbackPark =
                false;
        }

        ApplyOperatingModeControlRoles(
            previousMode,
            newMode);

        if (newMode ==
                OperatingMode.Parked &&
            previousMode !=
                OperatingMode.Parked)
        {
            BeginPark();
        }

        forceStatusRefresh =
            true;
    }

    // ===== Automatic parking =====

    void UpdateAutomaticParkRequest()
    {
        if (settings.ParkOnlyByCommand)
        {
            automaticParkRequested =
                false;

            return;
        }

        bool shouldPark =
            false;

        for (int i = 0;
            i < parkConnectors.Count;
            i++)
        {
            if (ConnectorRequiresParking(
                    parkConnectors[i]))
            {
                shouldPark =
                    true;

                break;
            }
        }

        if (!shouldPark)
        {
            for (int i = 0;
                i <
                    parkLandingGears.Count;
                i++)
            {
                if (LandingGearRequiresParking(
                        parkLandingGears[i]))
                {
                    shouldPark =
                        true;

                    break;
                }
            }
        }

        automaticParkRequested =
            shouldPark;
    }

    bool ConnectorRequiresParking(
        ParkConnector parkConnector)
    {
        IMyShipConnector connector =
            parkConnector.Block;

        if (connector == null ||
            connector.Closed ||
            connector.Status !=
                MyShipConnectorStatus
                    .Connected)
        {
            return false;
        }

        IMyShipConnector other =
            connector.OtherConnector;

        if (other == null)
        {
            return false;
        }

        GridNode targetNode;

        if (!gridNodes.TryGetValue(
                other.CubeGrid.EntityId,
                out targetNode))
        {
            return other
                .CubeGrid
                .IsStatic;
        }

        GridComponent target =
            targetNode.Component;

        if (target == null)
        {
            return other
                .CubeGrid
                .IsStatic;
        }

        if (target.HasStaticGrid)
        {
            return true;
        }

        if (target.Controllers.Count ==
            0)
        {
            // Dynamic controller-less attachment: likely cargo.
            return false;
        }

        if (potentialMaster &&
            target.HasSlaveCapableRedux)
        {
            // A piloted master remains active so the connected Redux PB
            // can wake from park and temporarily become a slave.
            return false;
        }

        return true;
    }

    bool LandingGearRequiresParking(
        ParkLandingGear parkLandingGear)
    {
        IMyLandingGear landingGear =
            parkLandingGear.Block;

        if (landingGear == null ||
            landingGear.Closed ||
            !landingGear.IsFunctional)
        {
            return false;
        }

        // The PB landing-gear API does not expose the attached entity.
        // Redux therefore cannot distinguish terrain from another ship.
        return landingGear.IsLocked;
    }

    // ===== Parking =====

    void BeginPark()
    {
        ClearControlledThrust();
        ReleaseGyros();

        Vector3D gravity =
            referenceController !=
                    null
                ? referenceController
                    .GetNaturalGravity()
                : Vector3D.Zero;

        Vector3D localRootCenter =
            Me.CubeGrid
                .WorldAABB
                .Center;

        for (int i = 0;
            i < thrusters.Count;
            i++)
        {
            Thruster thruster =
                thrusters[i];

            if (!thruster.Controlled)
            {
                continue;
            }

            thruster.ClearOverride();

            DisableThrusterByRedux(
                thruster,
                ThrusterDisableReason.Park);
        }

        parkRotorTargetAngles.Clear();

        for (int i = 0;
            i < controlledRotors.Count;
            i++)
        {
            Rotor rotor =
                controlledRotors[i];

            if (!rotor.Controlled)
            {
                continue;
            }

            rotor.BeginPark(
                gravity,
                localRootCenter);
        }

        TriggerTimers(
            parkTimers);
            
        Save();
    }

    void ExitPark()
    {
        ReleaseThrusterDisableReason(
            ThrusterDisableReason.Park);

        parkThrusterEnabledState.Clear();

        for (int i = 0;
            i < controlledRotors.Count;
            i++)
        {
            Rotor rotor =
                controlledRotors[i];

            rotor.CancelPark();
            rotor.Stop();
        }

        // Park targets are no longer authoritative once flight resumes.
        parkRotorTargetAngles.Clear();

        normalizedMasterDemand =
            Vector3D.Zero;

        TriggerTimers(
            unparkTimers);
            
        Save();
    }

    void EnsureNewCacheIsParked()
    {
        for (int i = 0;
            i < thrusters.Count;
            i++)
        {
            Thruster thruster =
                thrusters[i];

            RestorePersistedThrusterState(
                thruster);

            if (!thruster.Controlled)
            {
                continue;
            }

            thruster.ClearOverride();

            DisableThrusterByRedux(
                thruster,
                ThrusterDisableReason.Park);
        }

        ReleaseGyros();

        Vector3D gravity =
            referenceController !=
                    null
                ? referenceController
                    .GetNaturalGravity()
                : Vector3D.Zero;

        Vector3D localRootCenter =
            Me.CubeGrid
                .WorldAABB
                .Center;

        for (int i = 0;
            i < controlledRotors.Count;
            i++)
        {
            Rotor rotor =
                controlledRotors[i];

            if (!rotor.Controlled)
            {
                rotor.Stop();
                continue;
            }

            double targetAngle;

            if (parkRotorTargetAngles
                    .TryGetValue(
                        rotor.EntityId,
                        out targetAngle))
            {
                rotor.RestoreParkTarget(
                    targetAngle);
            }
            else
            {
                // Newly discovered joints get a target; existing joints
                // retain theirs across wrapper replacement.
                rotor.BeginPark(
                    gravity,
                    localRootCenter);
            }

            // This is lightweight. UpdatePark only writes when the
            // resulting command exceeds JointWriteDeadbandRad.
            rotor.UpdatePark();
        }
    }

    void UpdateParkedRotors()
    {
        for (int i = 0;
            i < controlledRotors.Count;
            i++)
        {
            controlledRotors[i]
                .UpdatePark();
        }
    }

    void ClearControlledThrust()
    {
        for (int i = 0;
            i < thrusters.Count;
            i++)
        {
            if (thrusters[i].Controlled)
            {
                thrusters[i]
                    .ClearOverride();
            }
        }
    }

    void RestoreThrustersAfterPark()
    {
        ReleaseThrusterDisableReason(
            ThrusterDisableReason.Park);

        parkThrusterEnabledState.Clear();
    }

    void RestoreParkedThruster(
        long entityId,
        IMyThrust block)
    {
        ReleaseThrusterDisableReason(
            entityId,
            block,
            ThrusterDisableReason.Park);
    }

    void TriggerTimers(
        List<IMyTimerBlock> timers)
    {
        for (int i = 0;
            i < timers.Count;
            i++)
        {
            IMyTimerBlock timer =
                timers[i];

            if (timer == null ||
                timer.Closed ||
                !timer.IsFunctional)
            {
                continue;
            }

            timer.Trigger();
        }
    }
}
partial class Program : MyGridProgram
{
    // ===== Script identity =====

    const string ScriptName = "Vector Thrust Redux";
    const string ScriptVersion = "0.2.0";
    const string ConfigSection = "Vector Thrust Redux";
    const string HeartbeatSection = "Vector Thrust Redux Heartbeat";
    const string SurfaceSelector = "VT-Redux:";

    // ===== Storage sections =====

    const string StateStorageSection = "State";
    const string DisabledThrusterStorageSection = "Disabled Thrusters";
    const string ParkRotorStorageSection = "Park Rotor Targets";
    const string TopologyStorageSection = "Topology";

    // ===== Numerical tolerances =====

    const double VectorEpsilon = 1e-8;
    const double ForceEpsilon = 1e-3;
    const double AngleEpsilon = 1e-4;
    const double EqualLimitEpsilon = 1e-4;
    const double DirectionBucketCosine = 1.0 - 1e-6;
    const double ParallelAxisCosine = 1.0 - 1e-4;
    const double DirectParkAlignmentCosine = 1.0 - 1e-4;

    // VTOS's proven internal aggressivity range is retained, but Redux
    // derives the active gain from requested/reachable capacity instead
    // of exposing another pile of ship-specific tuning knobs.
    const double MinimumJointResponseGain = 0.1;
    const double ParkingJointResponseGain = 1.0;
    const double MaximumJointResponseGain = 4.0;

    const double MaximumJointVelocityRad = Math.PI;
    const double JointWriteDeadbandRad = 1e-3;

    // VTOS suppressed corrections below this measured ship velocity.
    // This is an engine-facing control tolerance, not a player setting.
    const double VelocityControlEpsilon = 0.01;

    // VTOS's effective normalized thrust-write hysteresis was 0.75%.
    const double ThrustWriteDeadbandFraction = 0.0075;

    const double GyroLevelGain = 4.0;
    const double GyroAngularDampingGain = 1.5;
    const double GyroCommandAtFullTorque = 30.0;
    const double GyroWriteDeadband = 1e-3;

    // Wiki gyro-force values. The PB API accepts angular commands rather
    // than a direct torque request, so these serve as capacity weights.
    const double SmallGridGyroCapacity = 448000.0;
    const double LargeGridGyroCapacity = 33600000.0;
    const double SmallGridPrototechGyroCapacity = 4480000.0;
    const double LargeGridPrototechGyroCapacity = 201600000.0;

    const double MinimumTimeStep = 1.0 / 120.0;
    const double MaximumTimeStep = 0.25;

    // ===== Configuration and persisted state =====

    readonly Settings settings = new Settings();
    readonly MyIni storageIni = new MyIni();

    string knownProgrammableBlockCustomData = string.Empty;

    bool cruise;
    bool scriptDampeners = true;
    bool manualParkRequested;
    int selectedGear;

    double cruiseTargetSpeed;
    bool cruiseTargetInitialized;

    // Redux may disable one block for several simultaneous reasons. The
    // original Enabled state is restored only after the final reason ends.
    readonly Dictionary<long, DisabledThrusterState> disabledThrusterStates =
        new Dictionary<long, DisabledThrusterState>();

    // Park targets live outside disposable Rotor wrappers so a periodic
    // deep scan does not casually forget where the bloody joint was going.
    readonly Dictionary<long, double> parkRotorTargetAngles =
        new Dictionary<long, double>();

    TopologyFingerprint persistedTopologyFingerprint;
    bool persistedTopologyFingerprintValid;

    // Retained until the parking implementation is migrated to the
    // reason-aware disabledThrusterStates map in Functions.cs.
    readonly Dictionary<long, bool> parkThrusterEnabledState =
        new Dictionary<long, bool>();

    // ===== Runtime state =====

    OperatingMode mode = OperatingMode.Initializing;

    IMyShipController referenceController;

    bool automaticParkRequested;
    bool controllerMissing = true;
    bool potentialMaster;
    bool slaveHeartbeatFresh;
    bool slaveFallbackPark;
    bool wasParkedBeforeSlaving;

    long heartbeatSequence;
    long lastSlaveHeartbeatSequence = long.MinValue;
    long slaveMasterProgrammableBlockId;
    int slaveHeartbeatAgeUpdate10;
    bool slaveHeartbeatChangedThisWindow;

    MasterCommand activeSlaveCommand = new MasterCommand();

    Vector3D requestedForceWorld;
    Vector3D residualForceWorld;
    Vector3D normalizedMasterDemand;
    Vector3D inducedTorqueWorld;

    double availableControlledThrust;
    double lastControlTimeStep;
    double accumulatedControlTime;

    int update1SkipCounter;
    int update10SkipCounter;
    int update100SkipCounter;

    bool forceStatusRefresh;

    string lastCommandResult = string.Empty;
    bool lastCommandWasWarning;
    string cruiseAuthorityWarning = string.Empty;

    // The game cannot toggle cockpit dampeners normally when no main-grid
    // thrusters exist, so this controls whether cockpit state is treated
    // as input or merely synchronized from scriptDampeners.
    bool hasMainGridThrusters;

    // ===== Direction-aware capacity =====

    readonly DirectionalCapacity localDirectionalCapacity =
        new DirectionalCapacity();

    readonly DirectionalCapacity remoteReduxDirectionalCapacity =
        new DirectionalCapacity();

    readonly DirectionalCapacity constructDirectionalCapacity =
        new DirectionalCapacity();

    // ===== Cached construct model =====

    readonly List<IMyShipController> localControllers =
        new List<IMyShipController>();

    readonly List<IMyShipController> remotelyReachableControllers =
        new List<IMyShipController>();

    readonly List<Thruster> thrusters =
        new List<Thruster>();

    readonly List<Thruster> controlledThrusters =
        new List<Thruster>();

    readonly List<Thruster> fixedControlledThrusters =
        new List<Thruster>();

    readonly List<Thruster> observedReadOnlyThrusters =
        new List<Thruster>();

    // These replace the ambiguous observedReadOnlyThrusters model as the
    // scanner is migrated. Keeping them separate prevents every Redux PB
    // from subtracting every other Redux PB's previous-frame output.
    readonly List<Thruster> localUnmanagedThrusters =
        new List<Thruster>();

    readonly List<Thruster> remoteReduxThrusters =
        new List<Thruster>();

    readonly List<Thruster> remoteUnmanagedThrusters =
        new List<Thruster>();

    readonly List<Thruster> mainGridReverseThrusters =
        new List<Thruster>();

    readonly List<Rotor> controlledRotors =
        new List<Rotor>();

    readonly List<VectorThrust> vectorThrusters =
        new List<VectorThrust>();

    // Retained until per-nacelle allocation replaces group allocation.
    readonly List<VectorThrustGroup> vectorThrustGroups =
        new List<VectorThrustGroup>();

    readonly List<Gyro> controlledGyros =
        new List<Gyro>();

    readonly List<ParkConnector> parkConnectors =
        new List<ParkConnector>();

    readonly List<ParkLandingGear> parkLandingGears =
        new List<ParkLandingGear>();

    readonly List<IMyTimerBlock> parkTimers =
        new List<IMyTimerBlock>();

    readonly List<IMyTimerBlock> unparkTimers =
        new List<IMyTimerBlock>();

    readonly List<StatusSurface> statusSurfaces =
        new List<StatusSurface>();

    readonly Dictionary<long, GridNode> gridNodes =
        new Dictionary<long, GridNode>();

    readonly StringBuilder echoBuilder =
        new StringBuilder();

    readonly StringBuilder statusBuilder =
        new StringBuilder();

    IEnumerator<int> scanStateMachine;
    bool rescanRequested = true;

    TopologyFingerprint lastTopologyFingerprint;
    bool topologyFingerprintInitialized;

    RuntimeTracker runtimeTracker;

    public Program()
    {
        runtimeTracker =
            new RuntimeTracker(this);

        LoadStorage();
        LoadConfiguration(true);

        Runtime.UpdateFrequency =
            UpdateFrequency.Update1 |
            UpdateFrequency.Update10 |
            UpdateFrequency.Update100;

        RequestRescan();
    }

    public void Save()
    {
        storageIni.Clear();

        storageIni.Set(
            StateStorageSection,
"Cruise",
            cruise);

        storageIni.Set(
            StateStorageSection,
"Dampeners",
            scriptDampeners);

        storageIni.Set(
            StateStorageSection,
"ManualPark",
            manualParkRequested);

        storageIni.Set(
            StateStorageSection,
"Gear",
            selectedGear);

        storageIni.Set(
            StateStorageSection,
"CruiseTargetSpeed",
            cruiseTargetSpeed);

        storageIni.Set(
            StateStorageSection,
"CruiseTargetInitialized",
            cruiseTargetInitialized);

        foreach (
            KeyValuePair<long, DisabledThrusterState> pair
            in disabledThrusterStates)
        {
            DisabledThrusterState state =
                pair.Value;

            string serialized =
                (state.OriginalEnabled
                    ? "1"                    : "0") +
";"+
                ((int)state.Reasons).ToString();

            storageIni.Set(
                DisabledThrusterStorageSection,
                pair.Key.ToString(),
                serialized);
        }

        foreach (
            KeyValuePair<long, double> pair
            in parkRotorTargetAngles)
        {
            storageIni.Set(
                ParkRotorStorageSection,
                pair.Key.ToString(),
                pair.Value);
        }

        if (topologyFingerprintInitialized)
        {
            storageIni.Set(
                TopologyStorageSection,
"Count",
                lastTopologyFingerprint.Count);

            storageIni.Set(
                TopologyStorageSection,
"Xor",
                lastTopologyFingerprint.Xor.ToString());

            storageIni.Set(
                TopologyStorageSection,
"Sum",
                lastTopologyFingerprint.Sum.ToString());
        }
        else if (persistedTopologyFingerprintValid)
        {
            storageIni.Set(
                TopologyStorageSection,
"Count",
                persistedTopologyFingerprint.Count);

            storageIni.Set(
                TopologyStorageSection,
"Xor",
                persistedTopologyFingerprint.Xor.ToString());

            storageIni.Set(
                TopologyStorageSection,
"Sum",
                persistedTopologyFingerprint.Sum.ToString());
        }

        Storage =
            storageIni.ToString();
    }

    public void Main(
        string argument,
        UpdateType updateSource)
    {
        runtimeTracker.BeginRun();

        double elapsedSeconds =
            Runtime.TimeSinceLastRun.TotalSeconds;

        if (elapsedSeconds <
            MinimumTimeStep)
        {
            elapsedSeconds =
                MinimumTimeStep;
        }
        else if (elapsedSeconds >
                 MaximumTimeStep)
        {
            elapsedSeconds =
                MaximumTimeStep;
        }

        accumulatedControlTime +=
            elapsedSeconds;

        bool explicitRun =
            (updateSource &
             (UpdateType.Terminal |
              UpdateType.Trigger |
              UpdateType.Script)) != 0 ||
            !string.IsNullOrWhiteSpace(
                argument);

        // A pending scan may complete here and make a newly selected
        // controller available to an explicit command.
        ContinueRescan();

        if (explicitRun)
        {
            // Commands such as dampeners must apply to the controller the
            // player is using now, not the one selected on the prior tick.
            SelectReferenceController();
            HandleArgument(argument);
        }

        if ((updateSource &
             UpdateType.Update100) != 0 &&
            IsScheduled(
                ref update100SkipCounter,
                settings.Update100Skip))
        {
            RunUpdate100();
        }

        if ((updateSource &
             UpdateType.Update10) != 0 &&
            IsScheduled(
                ref update10SkipCounter,
                settings.Update10Skip))
        {
            RunUpdate10();
        }

        if ((updateSource &
             UpdateType.Update1) != 0)
        {
            heartbeatSequence++;

            if (mode ==
                OperatingMode.Slave)
            {
                ReadActiveSlaveHeartbeat();
            }

            EvaluateOperatingMode();

            if (IsScheduled(
                    ref update1SkipCounter,
                    settings.Update1Skip))
            {
                lastControlTimeStep =
                    MathHelper.Clamp(
                        accumulatedControlTime,
                        MinimumTimeStep,
                        MaximumTimeStep);

                accumulatedControlTime = 0;

                RunFlightControl(
                    lastControlTimeStep);
            }

            // A PB that cannot master has nothing to publish. The second
            // condition still allows an old owned heartbeat to be cleared
            // immediately after CanMaster is disabled.
            if (settings.CanMaster ||
                heartbeatController != null)
            {
                PublishOrClearMasterHeartbeat();
            }
        }

        if (explicitRun)
        {
            EvaluateOperatingMode();

            if (settings.CanMaster ||
                heartbeatController != null)
            {
                PublishOrClearMasterHeartbeat();
            }

            forceStatusRefresh = true;
        }

        if (forceStatusRefresh)
        {
            WriteStatus(true);
            forceStatusRefresh = false;
        }

        runtimeTracker.EndRun();
    }

    static bool IsScheduled(
        ref int counter,
        int skippedIntervals)
    {
        if (counter <
            skippedIntervals)
        {
            counter++;
            return false;
        }

        counter = 0;
        return true;
    }

    void LoadStorage()
    {
        if (string.IsNullOrWhiteSpace(
                Storage))
        {
            return;
        }

        MyIniParseResult parseResult;

        if (!storageIni.TryParse(
                Storage,
                out parseResult))
        {
            return;
        }

        cruise =
            storageIni
                .Get(
                    StateStorageSection,
"Cruise")
                .ToBoolean(false);

        scriptDampeners =
            storageIni
                .Get(
                    StateStorageSection,
"Dampeners")
                .ToBoolean(true);

        manualParkRequested =
            storageIni
                .Get(
                    StateStorageSection,
"ManualPark")
                .ToBoolean(false);

        selectedGear =
            Math.Max(
                0,
                storageIni
                    .Get(
                        StateStorageSection,
"Gear")
                    .ToInt32(0));

        cruiseTargetSpeed =
            storageIni
                .Get(
                    StateStorageSection,
"CruiseTargetSpeed")
                .ToDouble(0);

        cruiseTargetInitialized =
            storageIni
                .Get(
                    StateStorageSection,
"CruiseTargetInitialized")
                .ToBoolean(false);

        LoadDisabledThrusterStates();
        LoadParkRotorTargets();
        LoadPersistedTopologyFingerprint();
    }

    void LoadDisabledThrusterStates()
    {
        List<MyIniKey> keys =
            new List<MyIniKey>();

        storageIni.GetKeys(
            DisabledThrusterStorageSection,
            keys);

        for (int i = 0;
            i < keys.Count;
            i++)
        {
            MyIniKey key =
                keys[i];

            long entityId;

            if (!long.TryParse(
                    key.Name,
                    out entityId))
            {
                continue;
            }

            string serialized =
                storageIni
                    .Get(key)
                    .ToString();

            string[] components =
                serialized.Split(';');

            if (components.Length != 2)
            {
                continue;
            }

            bool originalEnabled =
                components[0] == "1";

            int serializedReasons;

            if (!int.TryParse(
                    components[1],
                    out serializedReasons))
            {
                continue;
            }

            ThrusterDisableReason reasons =
                (ThrusterDisableReason)
                    serializedReasons;

            if (reasons ==
                ThrusterDisableReason.None)
            {
                continue;
            }

            disabledThrusterStates[
                entityId] =
                new DisabledThrusterState
                {
                    OriginalEnabled =
                        originalEnabled,
                    Reasons = reasons
                };

            // Compatibility until Functions.cs is migrated.
            if ((reasons &
                 ThrusterDisableReason.Park) != 0)
            {
                parkThrusterEnabledState[
                    entityId] =
                    originalEnabled;
            }
        }
    }

    void LoadParkRotorTargets()
    {
        List<MyIniKey> keys =
            new List<MyIniKey>();

        storageIni.GetKeys(
            ParkRotorStorageSection,
            keys);

        for (int i = 0;
            i < keys.Count;
            i++)
        {
            MyIniKey key =
                keys[i];

            long entityId;

            if (!long.TryParse(
                    key.Name,
                    out entityId))
            {
                continue;
            }

            double target =
                storageIni
                    .Get(key)
                    .ToDouble(
                        double.NaN);

            if (double.IsNaN(target) ||
                double.IsInfinity(target))
            {
                continue;
            }

            parkRotorTargetAngles[
                entityId] =
                target;
        }
    }

    void LoadPersistedTopologyFingerprint()
    {
        long count =
            storageIni
                .Get(
                    TopologyStorageSection,
"Count")
                .ToInt64(-1);

        string xorText =
            storageIni
                .Get(
                    TopologyStorageSection,
"Xor")
                .ToString();

        string sumText =
            storageIni
                .Get(
                    TopologyStorageSection,
"Sum")
                .ToString();

        ulong xor;
        ulong sum;

        if (count < 0 ||
            !ulong.TryParse(
                xorText,
                out xor) ||
            !ulong.TryParse(
                sumText,
                out sum))
        {
            return;
        }

        persistedTopologyFingerprint =
            new TopologyFingerprint
            {
                Count = count,
                Xor = xor,
                Sum = sum
            };

        persistedTopologyFingerprintValid =
            true;
    }
}
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

        if (remotelyReachableControllers.Count == 0)
        {
            ResetSlaveHeartbeat();
        }

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

        //lastTopologyFingerprint = CalculateTopologyFingerprint();

        //topologyFingerprintInitialized = true;

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
":"+
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
partial class Program
{
    // ===== Effective movement state =====

    bool EffectiveDampeners
    {
        get
        {
            return mode ==
                    OperatingMode.Slave
                ? activeSlaveCommand
                    .Dampeners
                : scriptDampeners;
        }
    }

    bool EffectiveCruise
    {
        get
        {
            return mode ==
                    OperatingMode.Slave
                ? activeSlaveCommand
                    .Cruise
                : cruise;
        }
    }

    double EffectiveCruiseTargetSpeed
    {
        get
        {
            return mode ==
                    OperatingMode.Slave
                ? activeSlaveCommand
                    .CruiseTargetSpeed
                : cruiseTargetSpeed;
        }
    }

    double EffectiveGearFraction
    {
        get
        {
            if (mode ==
                OperatingMode.Slave)
            {
                return MathHelper.Clamp(
                    activeSlaveCommand
                        .GearFraction,
                    0,
                    1);
            }

            if (settings
                    .GearFractions.Count == 0)
            {
                return 0;
            }

            return MathHelper.Clamp(
                settings.GearFractions[
                    MathHelper.Clamp(
                        selectedGear,
                        0,
                        settings
                            .GearFractions
                            .Count -
                        1)],
                0,
                1);
        }
    }

    // ===== Command feedback =====

    void SetCommandResult(
        string result,
        bool warning)
    {
        lastCommandResult =
            result ?? string.Empty;

        lastCommandWasWarning =
            warning;

        forceStatusRefresh =
            true;
    }

    void ClearCommandResult()
    {
        lastCommandResult =
            string.Empty;

        lastCommandWasWarning =
            false;
    }

    // ===== Local movement-state mutation =====

    void SetLocalDampeners(
        bool enabled)
    {
        scriptDampeners =
            enabled;

        // Keep every local controller synchronized. This is required when
        // the game refuses to toggle dampeners because the main grid has
        // no directly controllable thrusters.
        SetControllerDampeners(
            enabled);
    }

    void SetControllerDampeners(
        bool enabled)
    {
        for (int i = 0;
            i < localControllers.Count;
            i++)
        {
            IMyShipController controller =
                localControllers[i];

            if (controller == null ||
                controller.Closed ||
                !controller.IsFunctional)
            {
                continue;
            }

            if (controller
                    .DampenersOverride ==
                enabled)
            {
                continue;
            }

            controller
                .DampenersOverride =
                enabled;
        }
    }

    void SynchronizeDampenerState()
    {
        if (mode ==
            OperatingMode.Slave)
        {
            // Adoption is temporary. Do not overwrite the local persisted
            // state with the master's state.
            SetControllerDampeners(
                activeSlaveCommand
                    .Dampeners);

            return;
        }

        if (!hasMainGridThrusters)
        {
            // The cockpit UI cannot be trusted as input when Keen thinks
            // there are no controllable thrusters.
            SetControllerDampeners(
                scriptDampeners);

            return;
        }

        if (referenceController ==
                null ||
            referenceController.Closed)
        {
            return;
        }

        bool controllerState =
            referenceController
                .DampenersOverride;

        if (controllerState ==
            scriptDampeners)
        {
            return;
        }

        scriptDampeners =
            controllerState;

        SetControllerDampeners(
            scriptDampeners);
    }

    void SetCruiseEnabled(
        bool enabled)
    {
        if (enabled &&
            !cruise)
        {
            InitializeCruiseTargetFromVelocity();
        }

        cruise = enabled;

        if (!cruise)
        {
            ReleaseThrusterDisableReason(
                ThrusterDisableReason.Cruise);

            ReleaseTemporaryControlRole(
                ControlRole.Cruise);
        }

        RefreshTemporaryControlRoles();
        RefreshCruiseAuthorityWarning();
    }

    void ToggleCruise()
    {
        SetCruiseEnabled(
            !cruise);
    }

    void InitializeCruiseTargetFromVelocity()
    {
        if (referenceController ==
                null ||
            referenceController.Closed)
        {
            cruiseTargetSpeed = 0;
            cruiseTargetInitialized =
                false;

            return;
        }

        Vector3D velocity =
            referenceController
                .GetShipVelocities()
                .LinearVelocity;

        cruiseTargetSpeed =
            Vector3D.Dot(
                velocity,
                referenceController
                    .WorldMatrix
                    .Forward);

        cruiseTargetInitialized =
            true;
    }

    void EnsureCruiseTargetInitialized()
    {
        if (cruiseTargetInitialized)
        {
            return;
        }

        InitializeCruiseTargetFromVelocity();
    }

    void AdjustCruiseTargetSpeed(
        double delta)
    {
        EnsureCruiseTargetInitialized();

        cruiseTargetSpeed +=
            delta;

        SetCommandResult(
"Cruise target: "+
            cruiseTargetSpeed
                .ToString("0.###") +
" m/s",
            false);
    }

    // ===== Redux-disabled thruster state =====

    bool WasThrusterDisabledByRedux(
        long entityId)
    {
        DisabledThrusterState state;

        if (!disabledThrusterStates
                .TryGetValue(
                    entityId,
                    out state))
        {
            return false;
        }

        // A player-disabled thruster must not become capacity merely
        // because Redux later added another disable reason.
        return state.OriginalEnabled &&
               state.Reasons !=
                   ThrusterDisableReason.None;
    }

    bool TryGetDisabledThrusterState(
        long entityId,
        out DisabledThrusterState state)
    {
        return disabledThrusterStates
            .TryGetValue(
                entityId,
                out state);
    }

    void DisableThrusterByRedux(
        Thruster thruster,
        ThrusterDisableReason reason)
    {
        if (thruster == null)
        {
            return;
        }

        DisableThrusterByRedux(
            thruster.TheBlock,
            reason);
    }

    void DisableThrusterByRedux(
        IMyThrust block,
        ThrusterDisableReason reason)
    {
        if (block == null ||
            block.Closed ||
            reason ==
                ThrusterDisableReason.None)
        {
            return;
        }

        DisabledThrusterState state;

        if (!disabledThrusterStates
                .TryGetValue(
                    block.EntityId,
                    out state))
        {
            state =
                new DisabledThrusterState
                {
                    OriginalEnabled =
                        block.Enabled,
                    Reasons =
                        ThrusterDisableReason.None
                };

            disabledThrusterStates.Add(
                block.EntityId,
                state);
        }

        state.Reasons |=
            reason;

        if (block.Enabled)
        {
            block.Enabled =
                false;
        }

        if ((state.Reasons &
             ThrusterDisableReason.Park) != 0)
        {
            // Compatibility bridge for old parking code until that code
            // is replaced.
            parkThrusterEnabledState[
                block.EntityId] =
                state.OriginalEnabled;
        }
    }

    void PrepareThrusterForControl(
        Thruster thruster)
    {
        if (thruster == null)
        {
            return;
        }

        PrepareThrusterForControl(
            thruster.TheBlock);
    }

    void PrepareThrusterForControl(
        IMyThrust block)
    {
        if (block == null ||
            block.Closed)
        {
            return;
        }

        DisabledThrusterState state;

        if (!disabledThrusterStates
                .TryGetValue(
                    block.EntityId,
                    out state))
        {
            return;
        }

        if (state.OriginalEnabled &&
            !block.Enabled)
        {
            block.Enabled =
                true;
        }
    }

    void ReleaseThrusterDisableReason(
        Thruster thruster,
        ThrusterDisableReason reason)
    {
        if (thruster == null)
        {
            return;
        }

        ReleaseThrusterDisableReason(
            thruster.EntityId,
            thruster.TheBlock,
            reason);
    }

    void ReleaseThrusterDisableReason(
        long entityId,
        IMyThrust block,
        ThrusterDisableReason reason)
    {
        DisabledThrusterState state;

        if (!disabledThrusterStates
                .TryGetValue(
                    entityId,
                    out state))
        {
            return;
        }

        state.Reasons &=
            ~reason;

        if ((reason &
             ThrusterDisableReason.Park) != 0)
        {
            parkThrusterEnabledState.Remove(
                entityId);
        }

        if (state.Reasons !=
            ThrusterDisableReason.None)
        {
            if (block != null &&
                !block.Closed &&
                block.Enabled)
            {
                block.Enabled =
                    false;
            }

            return;
        }

        if (block != null &&
            !block.Closed)
        {
            block.Enabled =
                state.OriginalEnabled;
        }

        disabledThrusterStates.Remove(
            entityId);
    }

    void ReleaseThrusterDisableReason(
        ThrusterDisableReason reason)
    {
        if (disabledThrusterStates.Count ==
            0)
        {
            return;
        }

        List<long> entityIds =
            new List<long>(
                disabledThrusterStates.Keys);

        for (int i = 0;
            i < entityIds.Count;
            i++)
        {
            long entityId =
                entityIds[i];

            IMyThrust block =
                FindKnownThrusterBlock(
                    entityId);

            ReleaseThrusterDisableReason(
                entityId,
                block,
                reason);
        }
    }

    IMyThrust FindKnownThrusterBlock(
        long entityId)
    {
        for (int i = 0;
            i < thrusters.Count;
            i++)
        {
            Thruster thruster =
                thrusters[i];

            if (thruster.EntityId ==
                entityId)
            {
                return thruster.TheBlock;
            }
        }

        return null;
    }

    void RestorePersistedThrusterState(
        Thruster thruster)
    {
        if (thruster == null)
        {
            return;
        }

        DisabledThrusterState state;

        if (!disabledThrusterStates
                .TryGetValue(
                    thruster.EntityId,
                    out state))
        {
            return;
        }

        if (state.Reasons ==
            ThrusterDisableReason.None)
        {
            disabledThrusterStates.Remove(
                thruster.EntityId);

            return;
        }

        if (thruster.TheBlock.Enabled)
        {
            thruster.TheBlock.Enabled =
                false;
        }
    }

    void PruneDisabledThrusterStates(
        HashSet<long> knownThrusterIds)
    {
        if (disabledThrusterStates.Count ==
            0)
        {
            return;
        }

        List<long> staleIds =
            new List<long>();

        foreach (
            KeyValuePair<long, DisabledThrusterState> pair
            in disabledThrusterStates)
        {
            if (!knownThrusterIds.Contains(
                    pair.Key))
            {
                staleIds.Add(
                    pair.Key);
            }
        }

        for (int i = 0;
            i < staleIds.Count;
            i++)
        {
            long entityId =
                staleIds[i];

            disabledThrusterStates.Remove(
                entityId);

            parkThrusterEnabledState.Remove(
                entityId);
        }
    }

    // ===== Temporary block authority =====

    void RefreshTemporaryControlRoles()
    {
        bool slave =
            mode ==
            OperatingMode.Slave;

        bool cruiseActive =
            EffectiveCruise;

        for (int i = 0;
            i < thrusters.Count;
            i++)
        {
            Thruster thruster =
                thrusters[i];

            bool onMainGrid =
                thruster
                    .TheBlock
                    .CubeGrid ==
                Me.CubeGrid;

            bool generallyEligible =
                !thruster.IsIgnored &&
                (settings.Greedy ||
                 thruster
                     .IsExplicitlyUsed);

            bool slaveAuthority =
                slave &&
                onMainGrid &&
                generallyEligible;

            bool cruiseAuthority =
                cruiseActive &&
                onMainGrid &&
                IsMainGridReverseThruster(
                    thruster) &&
                generallyEligible;

            thruster.SetControlRole(
                ControlRole.Slave,
                slaveAuthority);

            thruster.SetControlRole(
                ControlRole.Cruise,
                cruiseAuthority);

            if (!cruiseAuthority)
            {
                ReleaseThrusterDisableReason(
                    thruster,
                    ThrusterDisableReason
                        .Cruise);
            }
        }

        for (int i = 0;
            i < controlledGyros.Count;
            i++)
        {
            Gyro gyro =
                controlledGyros[i];

            bool onMainGrid =
                gyro
                    .TheBlock
                    .CubeGrid ==
                Me.CubeGrid;

            bool eligible =
                !gyro.IsIgnored &&
                (settings.Greedy ||
                 gyro.IsExplicitlyUsed);

            gyro.SetControlRole(
                ControlRole.Slave,
                slave &&
                onMainGrid &&
                eligible);
        }

        RefreshMainGridReverseThrusters();
    }

    void ReleaseTemporaryControlRole(
        ControlRole role)
    {
        for (int i = 0;
            i < thrusters.Count;
            i++)
        {
            thrusters[i]
                .SetControlRole(
                    role,
                    false);
        }

        for (int i = 0;
            i < controlledGyros.Count;
            i++)
        {
            controlledGyros[i]
                .SetControlRole(
                    role,
                    false);
        }
    }

    bool IsMainGridReverseThruster(
        Thruster thruster)
    {
        if (thruster == null ||
            referenceController ==
                null ||
            thruster
                .TheBlock
                .CubeGrid !=
            Me.CubeGrid)
        {
            return false;
        }

        return Vector3D.Dot(
                   VectorMath.SafeNormalize(
                       thruster
                           .ForceDirectionWorld),
                   referenceController
                       .WorldMatrix
                       .Backward) >=
               DirectionBucketCosine;
    }

    void RefreshMainGridReverseThrusters()
    {
        mainGridReverseThrusters.Clear();

        if (referenceController ==
            null)
        {
            return;
        }

        for (int i = 0;
            i < thrusters.Count;
            i++)
        {
            Thruster thruster =
                thrusters[i];

            if (IsMainGridReverseThruster(
                    thruster))
            {
                mainGridReverseThrusters.Add(
                    thruster);
            }
        }

        RefreshCruiseAuthorityWarning();
    }

    void RefreshMainGridThrusterState()
    {
        hasMainGridThrusters =
            false;

        for (int i = 0;
            i < thrusters.Count;
            i++)
        {
            Thruster thruster =
                thrusters[i];

            if (thruster.TheBlock.CubeGrid ==
                Me.CubeGrid)
            {
                hasMainGridThrusters =
                    true;

                break;
            }
        }

        RefreshMainGridReverseThrusters();
    }

    void RefreshCruiseAuthorityWarning()
    {
        cruiseAuthorityWarning =
            string.Empty;

        if (!EffectiveCruise ||
            mainGridReverseThrusters.Count ==
                0)
        {
            return;
        }

        if (settings.Greedy)
        {
            bool allIgnored =
                true;

            for (int i = 0;
                i <
                    mainGridReverseThrusters
                        .Count;
                i++)
            {
                if (!mainGridReverseThrusters[i]
                        .IsIgnored)
                {
                    allIgnored =
                        false;

                    break;
                }
            }

            if (allIgnored)
            {
                cruiseAuthorityWarning =
"Cruise cannot control main-grid "+
"reverse thrusters; all are "+
                    settings.IgnoreTag +
".";
            }

            return;
        }

        bool hasExplicitlyUsed =
            false;

        for (int i = 0;
            i <
                mainGridReverseThrusters
                    .Count;
            i++)
        {
            Thruster thruster =
                mainGridReverseThrusters[i];

            if (!thruster.IsIgnored &&
                thruster.IsExplicitlyUsed)
            {
                hasExplicitlyUsed =
                    true;

                break;
            }
        }

        if (!hasExplicitlyUsed)
        {
            cruiseAuthorityWarning =
"Cruise cannot control main-grid "+
"reverse thrusters; add "+
                settings.UseTag +
".";
        }
    }

    // ===== Operating-mode role transitions =====

    void ApplyOperatingModeControlRoles(
        OperatingMode previousMode,
        OperatingMode newMode)
    {
        if (previousMode ==
                OperatingMode.Slave &&
            newMode !=
                OperatingMode.Slave)
        {
            ReleaseTemporaryControlRole(
                ControlRole.Slave);

            // Restore the locally persisted mode after temporarily
            // adopting the master's dampener state.
            SetControllerDampeners(
                scriptDampeners);
        }

        if (previousMode !=
                OperatingMode.Slave &&
            newMode ==
                OperatingMode.Slave)
        {
            SetControllerDampeners(
                activeSlaveCommand
                    .Dampeners);
        }

        RefreshTemporaryControlRoles();
    }
}
partial class Program
{
    // ===== Status =====

    void WriteStatus(
        bool force)
    {
        echoBuilder.Clear();

        echoBuilder
            .AppendLine(
                ScriptName)
            .Append("v")
            .AppendLine(
                ScriptVersion)
            .AppendLine();

        echoBuilder
            .Append("Mode: ")
            .AppendLine(
                GetModeText(
                    mode));

        echoBuilder
            .Append("Controller: ")
            .AppendLine(
                referenceController !=
                        null
                    ? referenceController
                        .CustomName
                    : "NONE");

        AppendMovementStateStatus(
            echoBuilder);

        echoBuilder
            .Append("Nacelles: ")
            .AppendLine(
                vectorThrusters
                    .Count
                    .ToString());

        echoBuilder
            .Append(
"Controlled thrust: ")
            .Append(
                (availableControlledThrust /
                 1000.0)
                .ToString("0.##"))
            .AppendLine(" kN");

        echoBuilder
            .Append(
"Capacity F/B: ")
            .Append(
                (localDirectionalCapacity
                     .Forward /
                 1000.0)
                .ToString("0.##"))
            .Append("/")
            .Append(
                (localDirectionalCapacity
                     .Backward /
                 1000.0)
                .ToString("0.##"))
            .AppendLine(" kN");

        echoBuilder
            .Append("Residual: ")
            .Append(
                (residualForceWorld
                     .Length() /
                 1000.0)
                .ToString("0.##"))
            .AppendLine(" kN");

        echoBuilder
            .Append("Gyros: ")
            .AppendLine(
                controlledGyros
                    .Count
                    .ToString());

        if (mode ==
            OperatingMode.Slave)
        {
            echoBuilder
                .Append(
"Heartbeat age: ")
                .Append(
                    slaveHeartbeatAgeUpdate10)
                .AppendLine("/2");
        }

        if (!string.IsNullOrEmpty(
                cruiseAuthorityWarning))
        {
            echoBuilder
                .AppendLine()
                .Append("WARNING: ")
                .AppendLine(
                    cruiseAuthorityWarning);
        }

        if (!string.IsNullOrEmpty(
                lastCommandResult))
        {
            echoBuilder
                .AppendLine()
                .Append(
                    lastCommandWasWarning
                        ? "WARNING: "                        : "Command: ")
                .AppendLine(
                    lastCommandResult);
        }

        echoBuilder
            .Append("Runtime: ")
            .Append(
                Runtime
                    .LastRunTimeMs
                    .ToString("0.###"))
            .Append(" ms | avg ")
            .Append(
                runtimeTracker
                    .AverageRuntime
                    .ToString("0.###"))
            .Append(" | max ")
            .AppendLine(
                runtimeTracker
                    .MaximumRuntime
                    .ToString("0.###"));

        echoBuilder
            .Append("Instructions: ")
            .Append(
                Runtime
                    .CurrentInstructionCount)
            .Append("/")
            .AppendLine(
                Runtime
                    .MaxInstructionCount
                    .ToString());

        Echo(
            echoBuilder.ToString());

        if (!force &&
            statusSurfaces.Count == 0)
        {
            return;
        }

        statusBuilder.Clear();

        statusBuilder
            .AppendLine(
"VECTOR THRUST REDUX")
            .Append("MODE  ")
            .AppendLine(
                GetModeText(mode)
                    .ToUpperInvariant());

        AppendCompactMovementStateStatus(
            statusBuilder);

        statusBuilder
            .Append("VECTORS ")
            .AppendLine(
                vectorThrusters
                    .Count
                    .ToString())
            .Append("THRUST ")
            .Append(
                (availableControlledThrust /
                 1000.0)
                .ToString("0.0"))
            .AppendLine(" kN")
            .Append("ERROR ")
            .Append(
                (residualForceWorld
                     .Length() /
                 1000.0)
                .ToString("0.0"))
            .AppendLine(" kN");

        if (!string.IsNullOrEmpty(
                cruiseAuthorityWarning))
        {
            statusBuilder
                .Append("WARN ")
                .AppendLine(
                    cruiseAuthorityWarning);
        }
            
        if (!string.IsNullOrEmpty(
                lastCommandResult))
        {
            statusBuilder
                .Append(
                    lastCommandWasWarning
                        ? "WARN "                        : "CMD  ")
                .AppendLine(
                    lastCommandResult);
        }

        string status =
            statusBuilder.ToString();

        for (int i = 0;
            i < statusSurfaces.Count;
            i++)
        {
            statusSurfaces[i]
                .Write(status);
        }
    }

    void AppendMovementStateStatus(
        StringBuilder builder)
    {
        if (mode !=
            OperatingMode.Slave)
        {
            builder
                .Append("Dampeners: ")
                .AppendLine(
                    scriptDampeners
                        ? "ON"                        : "OFF");

            builder
                .Append("Cruise: ")
                .Append(
                    cruise
                        ? "ON"                        : "OFF");

            if (cruiseTargetInitialized)
            {
                builder
                    .Append(" @ ")
                    .Append(
                        cruiseTargetSpeed
                            .ToString(
"0.###"))
                    .Append(" m/s");
            }

            builder.AppendLine();

            AppendLocalGearStatus(
                builder);

            return;
        }

        builder
            .Append("Dampeners: local ")
            .Append(
                scriptDampeners
                    ? "ON"                    : "OFF")
            .Append(" | master ")
            .Append(
                activeSlaveCommand
                    .Dampeners
                    ? "ON"                    : "OFF")
            .Append(" | effective ")
            .AppendLine(
                EffectiveDampeners
                    ? "ON"                    : "OFF");

        builder
            .Append("Cruise: local ")
            .Append(
                cruise
                    ? "ON"                    : "OFF");

        if (cruiseTargetInitialized)
        {
            builder
                .Append(" @ ")
                .Append(
                    cruiseTargetSpeed
                        .ToString("0.###"));
        }

        builder
            .Append(" | master ")
            .Append(
                activeSlaveCommand
                    .Cruise
                    ? "ON"                    : "OFF")
            .Append(" @ ")
            .Append(
                activeSlaveCommand
                    .CruiseTargetSpeed
                    .ToString("0.###"))
            .AppendLine(" m/s");

        builder
            .Append("Gear: local ")
            .Append(
                selectedGear + 1)
            .Append("/")
            .Append(
                settings
                    .GearFractions
                    .Count)
            .Append(" | master ")
            .Append(
                activeSlaveCommand
                    .GearIndex +
                1)
            .Append("/")
            .Append(
                activeSlaveCommand
                    .GearCount)
            .Append(" (")
            .Append(
                (activeSlaveCommand
                     .GearFraction *
                 100)
                .ToString("0.##"))
            .AppendLine("%)");
    }

    void AppendCompactMovementStateStatus(
        StringBuilder builder)
    {
        if (mode !=
            OperatingMode.Slave)
        {
            builder
                .Append("DAMP  ")
                .AppendLine(
                    scriptDampeners
                        ? "ON"                        : "OFF")
                .Append("CRUISE ")
                .Append(
                    cruise
                        ? "ON"                        : "OFF");

            if (cruiseTargetInitialized)
            {
                builder
                    .Append(" ")
                    .Append(
                        cruiseTargetSpeed
                            .ToString("0.##"))
                    .Append("m/s");
            }

            builder
                .AppendLine()
                .Append("GEAR  ")
                .Append(
                    selectedGear + 1)
                .Append("/")
                .AppendLine(
                    settings
                        .GearFractions
                        .Count
                        .ToString());

            return;
        }

        builder
            .Append("DAMP  L:")
            .Append(
                scriptDampeners
                    ? "ON"                    : "OFF")
            .Append(" M:")
            .AppendLine(
                activeSlaveCommand
                        .Dampeners
                    ? "ON"                    : "OFF")
            .Append("CRUISE L:")
            .Append(
                cruise
                    ? "ON"                    : "OFF")
            .Append(" M:")
            .Append(
                activeSlaveCommand
                        .Cruise
                    ? "ON"                    : "OFF")
            .Append(" ")
            .Append(
                activeSlaveCommand
                    .CruiseTargetSpeed
                    .ToString("0.##"))
            .AppendLine("m/s")
            .Append("GEAR  L:")
            .Append(
                selectedGear + 1)
            .Append("/")
            .Append(
                settings
                    .GearFractions
                    .Count)
            .Append(" M:")
            .Append(
                activeSlaveCommand
                    .GearIndex +
                1)
            .Append("/")
            .AppendLine(
                activeSlaveCommand
                    .GearCount
                    .ToString());
    }

    void AppendLocalGearStatus(
        StringBuilder builder)
    {
        int gearCount =
            settings
                .GearFractions
                .Count;

        int clampedGear =
            gearCount > 0
                ? MathHelper.Clamp(
                    selectedGear,
                    0,
                    gearCount - 1)
                : 0;

        double percentage =
            gearCount > 0
                ? settings
                    .GearFractions[
                        clampedGear] *
                  100
                : 0;

        builder
            .Append("Gear: ")
            .Append(
                clampedGear + 1)
            .Append("/")
            .Append(
                gearCount)
            .Append(" (")
            .Append(
                percentage
                    .ToString("0.##"))
            .AppendLine("%)");
    }
}
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

        return unchecked(
            (ulong)(uint)
                value.GetHashCode());
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
