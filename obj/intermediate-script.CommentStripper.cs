// <mdk sortorder="30" />   // BlockClasses.cs
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame;
using VRageMath;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Text;
using VRage.Game.ModAPI.Ingame.Utilities;
class Program : MyGridProgram
{

    sealed class Thruster
    {
        readonly Program program;

        float lastOverridePercentage = float.NaN;

        public readonly IMyThrust TheBlock;
        public readonly BlockTags Tags;

        public VectorThrust Nacelle;

        public bool Controlled,IsPrimaryNacelleThruster;

        public double DesiredEffectiveThrust;

        public long EntityId
        {
            get { return TheBlock.EntityId; }
        }

        public Vector3D ForceDirectionWorld
        {
            get { return TheBlock.WorldMatrix.Backward; }
        }

        public Vector3D ExhaustDirectionWorld
        {
            get { return TheBlock.WorldMatrix.Forward; }
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

                double thrust = TheBlock.MaxEffectiveThrust;

                return thrust > ForceEpsilon ? thrust : 0;
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

                return ForceDirectionWorld * TheBlock.CurrentThrust;
            }
        }

        public bool IsIgnored
        {
            get { return (Tags & BlockTags.Ignore) != 0; }
        }

        public bool IsUsable
        {
            get
            {
                if (TheBlock == null ||
                    TheBlock.Closed ||
                    !TheBlock.IsFunctional ||
                    MaximumEffectiveThrust <= ForceEpsilon)
                {
                    return false;
                }

                return TheBlock.Enabled ||
                       program.WasThrusterDisabledByPark(EntityId);
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
            Controlled = controlled;
        }

        public void ResetDemand()
        {
            DesiredEffectiveThrust = 0;
        }

        public double AddOptimalContribution(ref Vector3D residual)
        {
            if (!Controlled || !IsUsable)
            {
                return 0;
            }

            Vector3D direction = VectorMath.SafeNormalize(
                ForceDirectionWorld);

            double projection = Vector3D.Dot(residual, direction);

            if (projection <= ForceEpsilon)
            {
                return 0;
            }

            double available =
                MaximumEffectiveThrust - DesiredEffectiveThrust;

            if (available <= ForceEpsilon)
            {
                return 0;
            }

            double added = Math.Min(projection, available);

            DesiredEffectiveThrust += added;
            residual -= direction * added;

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

            double maximumEffective = MaximumEffectiveThrust;

            float requestedPercentage =
                maximumEffective > ForceEpsilon
                    ? (float)MathHelper.Clamp(
                        DesiredEffectiveThrust / maximumEffective,
                        0,
                        1)
                    : 0;

            if (!float.IsNaN(lastOverridePercentage) &&
                Math.Abs(
                    requestedPercentage -
                    lastOverridePercentage) < 1e-4f &&
                Math.Abs(
                    TheBlock.ThrustOverridePercentage -
                    requestedPercentage) < 1e-4f)
            {
                return;
            }

            TheBlock.ThrustOverridePercentage =
                lastOverridePercentage =
                    requestedPercentage;
        }

        public void ClearOverride()
        {
            DesiredEffectiveThrust = 0;

            if (TheBlock == null || TheBlock.Closed)
            {
                return;
            }

            if (Math.Abs(TheBlock.ThrustOverridePercentage) > 1e-5f)
            {
                TheBlock.ThrustOverridePercentage = 0;
            }

            lastOverridePercentage = 0;
        }

        public void Release()
        {
            ClearOverride();
            Controlled = false;
        }
    }


    sealed class Rotor
    {
        readonly Program program;

        double lastWrittenTargetVelocity = double.NaN,parkTargetAngle;
        bool parkTargetInitialized,parkSettled;

        public readonly IMyMotorStator TheBlock;
        public readonly BlockTags Tags;

        public VectorThrust Nacelle;

        public bool Controlled;

        public long EntityId
        {
            get { return TheBlock.EntityId; }
        }

        public bool IsHinge
        {
            get
            {
                return TheBlock.BlockDefinition.SubtypeId
                    .IndexOf(
"Hinge",
                        StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        public Vector3D AxisWorld
        {
            get { return TheBlock.WorldMatrix.Up; }
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

                double lower = TheBlock.LowerLimitRad;
                double upper = TheBlock.UpperLimitRad;

                return !HasFiniteLowerLimit(lower) ||
                       !HasFiniteUpperLimit(upper) ||
                       Math.Abs(upper - lower) >
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
            Controlled = controlled;
        }

        public double Point(Vector3D desiredForceWorld)
        {
            if (!Controlled ||
                !IsPhysicallyMovable ||
                Nacelle == null)
            {
                SetTargetVelocity(0);
                return 0;
            }

            CancelPark();

            Vector3D currentForce =
                Nacelle.PrimaryForceDirectionWorld;

            double rawCommandAngle =
                VectorMath.RotorCommandAngle(
                    desiredForceWorld,
                    currentForce,
                    AxisWorld);

            double reachableCommandAngle =
                ClampCommandDeltaToLimits(rawCommandAngle);

            SetTargetVelocity(
                reachableCommandAngle * JointResponseGain);

            Vector3D predictedForce =
                VectorMath.RotateAroundAxis(
                    currentForce,
                    AxisWorld,
                    -reachableCommandAngle);

            return VectorMath.CosBetween(
                predictedForce,
                desiredForceWorld);
        }

        public bool TryGetReachableCommandAngle(
            Vector3D desiredForceWorld,
            out double commandAngle,
            out double predictedAlignment)
        {
            commandAngle = 0;
            predictedAlignment = 0;

            if (!Controlled ||
                !IsPhysicallyMovable ||
                Nacelle == null)
            {
                return false;
            }

            Vector3D currentForce =
                Nacelle.PrimaryForceDirectionWorld;

            if (currentForce.LengthSquared() <= VectorEpsilon ||
                desiredForceWorld.LengthSquared() <= VectorEpsilon)
            {
                return false;
            }

            double rawCommandAngle =
                VectorMath.RotorCommandAngle(
                    desiredForceWorld,
                    currentForce,
                    AxisWorld);

            commandAngle =
                ClampCommandDeltaToLimits(rawCommandAngle);

            Vector3D predictedForce =
                VectorMath.RotateAroundAxis(
                    currentForce,
                    AxisWorld,
                    -commandAngle);

            predictedAlignment = VectorMath.CosBetween(
                predictedForce,
                desiredForceWorld);

            return true;
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
                SetTargetVelocity(0);
                parkSettled = true;
                return;
            }

            double commandAngle;
            double predictedAlignment;

            if (naturalGravity.LengthSquared() > VectorEpsilon)
            {
                Vector3D gravityOpposingForce =
                    -VectorMath.SafeNormalize(naturalGravity);

                if (TryGetReachableCommandAngle(
                        gravityOpposingForce,
                        out commandAngle,
                        out predictedAlignment) &&
                    predictedAlignment >=
                        DirectParkAlignmentCosine)
                {
                    SetParkTargetFromCommandDelta(commandAngle);
                    return;
                }
            }

            Vector3D branchCenter =
                Nacelle.GetBranchCenterWorld();

            Vector3D pivot = TheBlock.GetPosition();
            Vector3D branchOffset = branchCenter - pivot;
            Vector3D rootOffset = localRootCenter - pivot;

            Vector3D branchPlanar =
                VectorMath.Rejection(branchOffset, AxisWorld);

            Vector3D rootPlanar =
                VectorMath.Rejection(rootOffset, AxisWorld);

            if (branchPlanar.LengthSquared() <= VectorEpsilon ||
                rootPlanar.LengthSquared() <= VectorEpsilon)
            {
                SetParkTargetFromCommandDelta(0);
                return;
            }

            commandAngle = VectorMath.RotorCommandAngle(
                rootPlanar,
                branchPlanar,
                AxisWorld);

            commandAngle =
                ClampCommandDeltaToLimits(commandAngle);

            SetParkTargetFromCommandDelta(commandAngle);
        }

        public void UpdatePark()
        {
            if (parkSettled)
            {
                return;
            }

            if (!parkTargetInitialized ||
                !Controlled ||
                !IsPhysicallyMovable)
            {
                SetTargetVelocity(0);
                parkSettled = true;
                return;
            }

            double error = parkTargetAngle - TheBlock.Angle;

            if (!HasAnyFiniteLimit())
            {
                error = VectorMath.NormalizeAngle(error);
            }

            if (Math.Abs(error) <= AngleEpsilon)
            {
                SetTargetVelocity(0);
                parkSettled = true;
                return;
            }

            SetTargetVelocity(error * JointResponseGain);
        }

        public void CancelPark()
        {
            parkTargetInitialized = false;
            parkSettled = false;
        }

        public void Release()
        {
            SetTargetVelocity(0);
            Controlled = false;
        }

        void SetParkTargetFromCommandDelta(
            double commandAngle)
        {
            parkTargetAngle =
                TheBlock.Angle + commandAngle;

            if (HasFiniteLowerLimit(TheBlock.LowerLimitRad))
            {
                parkTargetAngle = Math.Max(
                    parkTargetAngle,
                    TheBlock.LowerLimitRad);
            }

            if (HasFiniteUpperLimit(TheBlock.UpperLimitRad))
            {
                parkTargetAngle = Math.Min(
                    parkTargetAngle,
                    TheBlock.UpperLimitRad);
            }

            parkTargetInitialized = true;
        }

        double ClampCommandDeltaToLimits(
            double rawCommandDelta)
        {
            rawCommandDelta =
                VectorMath.NormalizeAngle(rawCommandDelta);

            bool finiteLower =
                HasFiniteLowerLimit(TheBlock.LowerLimitRad);

            bool finiteUpper =
                HasFiniteUpperLimit(TheBlock.UpperLimitRad);

            if (!finiteLower && !finiteUpper)
            {
                return rawCommandDelta;
            }

            double currentAngle = TheBlock.Angle;
            double bestDelta = double.NaN;
            double bestMagnitude = double.MaxValue;

            for (int turn = -2; turn <= 2; turn++)
            {
                double candidateDelta =
                    rawCommandDelta +
                    turn * MathHelper.TwoPi;

                double candidateAngle =
                    currentAngle + candidateDelta;

                if (finiteLower &&
                    candidateAngle <
                        TheBlock.LowerLimitRad - AngleEpsilon)
                {
                    continue;
                }

                if (finiteUpper &&
                    candidateAngle >
                        TheBlock.UpperLimitRad + AngleEpsilon)
                {
                    continue;
                }

                double magnitude = Math.Abs(candidateDelta);

                if (magnitude < bestMagnitude)
                {
                    bestMagnitude = magnitude;
                    bestDelta = candidateDelta;
                }
            }

            if (!double.IsNaN(bestDelta))
            {
                return bestDelta;
            }

            double requestedAngle =
                currentAngle + rawCommandDelta;

            if (finiteLower)
            {
                requestedAngle = Math.Max(
                    requestedAngle,
                    TheBlock.LowerLimitRad);
            }

            if (finiteUpper)
            {
                requestedAngle = Math.Min(
                    requestedAngle,
                    TheBlock.UpperLimitRad);
            }

            return requestedAngle - currentAngle;
        }

        void SetTargetVelocity(double velocityRad)
        {
            if (TheBlock == null || TheBlock.Closed)
            {
                return;
            }

            velocityRad = MathHelper.Clamp(
                velocityRad,
                -MaximumJointVelocityRad,
                MaximumJointVelocityRad);

            if (Math.Abs(velocityRad) <=
                JointWriteDeadbandRad)
            {
                velocityRad = 0;
            }

            if (!double.IsNaN(lastWrittenTargetVelocity) &&
                Math.Abs(
                    lastWrittenTargetVelocity -
                    velocityRad) <
                        JointWriteDeadbandRad &&
                Math.Abs(
                    TheBlock.TargetVelocityRad -
                    velocityRad) <
                        JointWriteDeadbandRad)
            {
                return;
            }

            TheBlock.TargetVelocityRad =
                (float)velocityRad;

            lastWrittenTargetVelocity = velocityRad;
        }

        bool HasAnyFiniteLimit()
        {
            return HasFiniteLowerLimit(
                       TheBlock.LowerLimitRad) ||
                   HasFiniteUpperLimit(
                       TheBlock.UpperLimitRad);
        }

        static bool HasFiniteLowerLimit(double value)
        {
            return !double.IsNaN(value) &&
                   !double.IsInfinity(value) &&
                   value > -1e20;
        }

        static bool HasFiniteUpperLimit(double value)
        {
            return !double.IsNaN(value) &&
                   !double.IsInfinity(value) &&
                   value < 1e20;
        }
    }


    sealed class VectorThrust
    {
        sealed class DirectionBucket
        {
            public Vector3D LocalExhaustDirection;
            public double EffectiveThrust;
        }

        readonly Program program;
        readonly List<DirectionBucket> directionBuckets =
            new List<DirectionBucket>();

        public readonly Rotor Rotor;

        public readonly List<Thruster> Thrusters =
            new List<Thruster>();

        public readonly List<IMyCubeGrid> BranchGrids =
            new List<IMyCubeGrid>();

        public Vector3D PrimaryExhaustDirectionLocal =
            Vector3D.Zero,RequiredForceWorld =
            Vector3D.Zero;

        public double PrimaryEffectiveThrust;

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
            get { return Rotor.AxisWorld; }
        }

        public Vector3D PrimaryForceDirectionWorld
        {
            get
            {
                IMyCubeGrid topGrid =
                    Rotor.TheBlock.TopGrid;

                if (topGrid == null ||
                    PrimaryExhaustDirectionLocal
                        .LengthSquared() <= VectorEpsilon)
                {
                    return Vector3D.Zero;
                }

                Vector3D exhaust =
                    VectorMath.LocalToWorldDirection(
                        PrimaryExhaustDirectionLocal,
                        topGrid.WorldMatrix);

                return -VectorMath.SafeNormalize(exhaust);
            }
        }

        public void RefreshPrimaryDirection()
        {
            directionBuckets.Clear();

            for (int i = 0; i < Thrusters.Count; i++)
            {
                Thrusters[i].IsPrimaryNacelleThruster =
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

            for (int i = 0; i < Thrusters.Count; i++)
            {
                Thruster thruster = Thrusters[i];

                if (!thruster.Controlled)
                {
                    continue;
                }

                double effective =
                    thruster.MaximumEffectiveThrust;

                if (effective <= ForceEpsilon)
                {
                    continue;
                }

                Vector3D localExhaust =
                    VectorMath.SafeNormalize(
                        VectorMath.WorldToLocalDirection(
                            thruster.ExhaustDirectionWorld,
                            topWorldMatrix));

                DirectionBucket bucket = null;

                for (int j = 0;
                    j < directionBuckets.Count;
                    j++)
                {
                    if (Vector3D.Dot(
                            directionBuckets[j]
                                .LocalExhaustDirection,
                            localExhaust) >=
                        DirectionBucketCosine)
                    {
                        bucket = directionBuckets[j];
                        break;
                    }
                }

                if (bucket == null)
                {
                    bucket = new DirectionBucket
                    {
                        LocalExhaustDirection =
                            localExhaust
                    };

                    directionBuckets.Add(bucket);
                }

                bucket.EffectiveThrust += effective;
            }

            DirectionBucket strongest = null;

            for (int i = 0;
                i < directionBuckets.Count;
                i++)
            {
                if (strongest == null ||
                    directionBuckets[i]
                        .EffectiveThrust >
                    strongest.EffectiveThrust)
                {
                    strongest = directionBuckets[i];
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
                strongest.LocalExhaustDirection;

            PrimaryEffectiveThrust =
                strongest.EffectiveThrust;

            for (int i = 0; i < Thrusters.Count; i++)
            {
                Thruster thruster = Thrusters[i];

                Vector3D localExhaust =
                    VectorMath.SafeNormalize(
                        VectorMath.WorldToLocalDirection(
                            thruster.ExhaustDirectionWorld,
                            topWorldMatrix));

                thruster.IsPrimaryNacelleThruster =
                    Vector3D.Dot(
                        localExhaust,
                        PrimaryExhaustDirectionLocal) >=
                    DirectionBucketCosine;
            }
        }

        public double Aim(Vector3D desiredForceWorld)
        {
            RequiredForceWorld = desiredForceWorld;

            if (desiredForceWorld.LengthSquared() <=
                VectorEpsilon)
            {
                Rotor.Point(Vector3D.Zero);
                return 0;
            }

            return Rotor.Point(desiredForceWorld);
        }

        public double AllocatePrimary(
            ref Vector3D residual,
            Vector3D centerOfMassWorld,
            ref Vector3D inducedTorque)
        {
            double allocated = 0;

            for (int i = 0; i < Thrusters.Count; i++)
            {
                Thruster thruster = Thrusters[i];

                if (!thruster
                        .IsPrimaryNacelleThruster)
                {
                    continue;
                }

                double contribution =
                    thruster.AddOptimalContribution(
                        ref residual);

                if (contribution <= ForceEpsilon)
                {
                    continue;
                }

                Vector3D force =
                    thruster.ForceDirectionWorld *
                    contribution;

                Vector3D lever =
                    thruster.TheBlock.GetPosition() -
                    centerOfMassWorld;

                inducedTorque +=
                    Vector3D.Cross(lever, force);

                allocated += contribution;
            }

            return allocated;
        }

        public double AllocateSecondary(
            ref Vector3D residual,
            Vector3D centerOfMassWorld,
            ref Vector3D inducedTorque)
        {
            double allocated = 0;

            for (int i = 0; i < Thrusters.Count; i++)
            {
                Thruster thruster = Thrusters[i];

                if (thruster
                    .IsPrimaryNacelleThruster)
                {
                    continue;
                }

                double contribution =
                    thruster.AddOptimalContribution(
                        ref residual);

                if (contribution <= ForceEpsilon)
                {
                    continue;
                }

                Vector3D force =
                    thruster.ForceDirectionWorld *
                    contribution;

                Vector3D lever =
                    thruster.TheBlock.GetPosition() -
                    centerOfMassWorld;

                inducedTorque +=
                    Vector3D.Cross(lever, force);

                allocated += contribution;
            }

            return allocated;
        }

        public Vector3D GetBranchCenterWorld()
        {
            if (BranchGrids.Count == 0)
            {
                return Rotor.TheBlock.TopGrid != null
                    ? Rotor.TheBlock.TopGrid
                        .WorldAABB.Center
                    : Rotor.TheBlock.GetPosition();
            }

            Vector3D center = Vector3D.Zero;
            int validGrids = 0;

            for (int i = 0;
                i < BranchGrids.Count;
                i++)
            {
                IMyCubeGrid grid = BranchGrids[i];

                if (grid == null || grid.Closed)
                {
                    continue;
                }

                center += grid.WorldAABB.Center;
                validGrids++;
            }

            return validGrids > 0
                ? center / validGrids
                : Rotor.TheBlock.GetPosition();
        }
    }

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
                        Nacelles[0].AxisWorld)
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

        public double Score(Vector3D residual)
        {
            Vector3D reachable =
                ReachableComponent(residual);

            if (reachable.LengthSquared() <=
                VectorEpsilon)
            {
                return 0;
            }

            return Math.Min(
                reachable.Length(),
                EffectiveCapacity);
        }
    }


    sealed class Gyro
    {
        readonly Program program;

        bool lastOverride;
        float lastPitch = float.NaN,lastYaw = float.NaN,lastRoll = float.NaN;

        public readonly IMyGyro TheBlock;
        public readonly BlockTags Tags;

        public readonly double NominalCapacity;

        public bool Controlled;

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
            Controlled = controlled;

            bool smallGrid =
                block.CubeGrid.GridSizeEnum ==
                VRage.Game.MyCubeSize.Small;

            bool prototech =
                block.BlockDefinition.SubtypeId
                    .IndexOf(
"Prototech",
                        StringComparison.OrdinalIgnoreCase) >= 0;

            if (prototech)
            {
                NominalCapacity = smallGrid
                    ? SmallGridPrototechGyroCapacity
                    : LargeGridPrototechGyroCapacity;
            }
            else
            {
                NominalCapacity = smallGrid
                    ? SmallGridGyroCapacity
                    : LargeGridGyroCapacity;
            }
        }

        public void ApplyWorldCommand(
            Vector3D worldAngularCommand)
        {
            if (!Controlled ||
                TheBlock == null ||
                TheBlock.Closed ||
                EffectiveCapacity <= VectorEpsilon)
            {
                ReleaseOverride();
                return;
            }

            worldAngularCommand =
                VectorMath.ClampMagnitude(
                    worldAngularCommand,
                    GyroCommandAtFullTorque);

            Vector3D localCommand =
                VectorMath.WorldToLocalDirection(
                    worldAngularCommand,
                    TheBlock.WorldMatrix);

            float pitch = (float)localCommand.X;
            float yaw = (float)localCommand.Y;
            float roll = (float)localCommand.Z;

            bool shouldOverride =
                localCommand.LengthSquared() >
                GyroWriteDeadband *
                GyroWriteDeadband;

            if (!shouldOverride)
            {
                ReleaseOverride();
                return;
            }

            if (!lastOverride ||
                Math.Abs(pitch - lastPitch) >
                    GyroWriteDeadband)
            {
                TheBlock.Pitch = pitch;
                lastPitch = pitch;
            }

            if (!lastOverride ||
                Math.Abs(yaw - lastYaw) >
                    GyroWriteDeadband)
            {
                TheBlock.Yaw = yaw;
                lastYaw = yaw;
            }

            if (!lastOverride ||
                Math.Abs(roll - lastRoll) >
                    GyroWriteDeadband)
            {
                TheBlock.Roll = roll;
                lastRoll = roll;
            }

            if (!lastOverride ||
                !TheBlock.GyroOverride)
            {
                TheBlock.GyroOverride = true;
            }

            lastOverride = true;
        }

        public void ReleaseOverride()
        {
            if (TheBlock == null || TheBlock.Closed)
            {
                return;
            }

            if (TheBlock.GyroOverride)
            {
                TheBlock.GyroOverride = false;
            }

            if (Math.Abs(TheBlock.Pitch) >
                GyroWriteDeadband)
            {
                TheBlock.Pitch = 0;
            }

            if (Math.Abs(TheBlock.Yaw) >
                GyroWriteDeadband)
            {
                TheBlock.Yaw = 0;
            }

            if (Math.Abs(TheBlock.Roll) >
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
            Controlled = false;
        }
    }


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
            SurfaceIndex = surfaceIndex;
        }

        public void Write(string text)
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
                    VRage.Game.GUI.TextPanel
                        .ContentType.TEXT_AND_IMAGE;

                Surface.Font = "Monospace";
                Surface.FontSize = 0.8f;
                Surface.Alignment =
                    VRage.Game.GUI.TextPanel
                        .TextAlignment.LEFT;

                initialized = true;
            }

            Surface.WriteText(text, false);
        }
    }
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

    sealed class MasterCommand
    {
        public long MasterProgrammableBlockId,ControllerId,Sequence;

        public Vector3D NormalizedForceDemand;

        public bool Cruise,LevelWithGravity;

        public void CopyFrom(MasterCommand other)
        {
            MasterProgrammableBlockId = other.MasterProgrammableBlockId;
            ControllerId = other.ControllerId;
            Sequence = other.Sequence;
            NormalizedForceDemand = other.NormalizedForceDemand;
            Cruise = other.Cruise;
            LevelWithGravity = other.LevelWithGravity;
        }
    }

    sealed class RuntimeTracker
    {
        readonly Program program;

        double averageRuntime;
        double maximumRuntime,previousRuntime;
        int samples;

        public double AverageRuntime
        {
            get { return averageRuntime; }
        }

        public double MaximumRuntime
        {
            get { return maximumRuntime; }
        }

        public double PreviousRuntime
        {
            get { return previousRuntime; }
        }

        public RuntimeTracker(Program program)
        {
            this.program = program;
        }

        public void BeginRun()
        {
            previousRuntime = program.Runtime.LastRunTimeMs;
        }

        public void EndRun()
        {
            double runtime = program.Runtime.LastRunTimeMs;

            samples++;

            if (samples == 1)
            {
                averageRuntime = runtime;
                maximumRuntime = runtime;
                return;
            }

            averageRuntime += (runtime - averageRuntime) * 0.05;

            if (runtime > maximumRuntime)
            {
                maximumRuntime = runtime;
            }
            else if (samples % 600 == 0)
            {
                maximumRuntime = averageRuntime;
            }
        }
    }

    public static class VectorMath
    {

public static Vector3D SafeNormalize(Vector3D vector)
        {
            if (Vector3D.IsZero(vector))
            {
                return Vector3D.Zero;
            }

            if (Vector3D.IsUnit(ref vector))
            {
                return vector;
            }

            return Vector3D.Normalize(vector);
        }

public static Vector3D Rejection(Vector3D a, Vector3D b)
        {
            double denominator = b.LengthSquared();

            if (a.LengthSquared() <= VectorEpsilon ||
                denominator <= VectorEpsilon)
            {
                return Vector3D.Zero;
            }

            return a - Vector3D.Dot(a, b) / denominator * b;
        }

public static Vector3D Projection(Vector3D a, Vector3D b)
        {
            double denominator = b.LengthSquared();

            if (a.LengthSquared() <= VectorEpsilon ||
                denominator <= VectorEpsilon)
            {
                return Vector3D.Zero;
            }

            return Vector3D.Dot(a, b) / denominator * b;
        }

        public static double CosBetween(Vector3D a, Vector3D b)
        {
            double denominator = Math.Sqrt(
                a.LengthSquared() * b.LengthSquared());

            if (denominator <= VectorEpsilon)
            {
                return 0;
            }

            return MathHelper.Clamp(
                Vector3D.Dot(a, b) / denominator,
                -1,
                1);
        }

        public static Vector3D ClampMagnitude(
            Vector3D vector,
            double maximumLength)
        {
            double lengthSquared = vector.LengthSquared();
            double maximumSquared = maximumLength * maximumLength;

            if (lengthSquared <= maximumSquared)
            {
                return vector;
            }

            if (lengthSquared <= VectorEpsilon)
            {
                return Vector3D.Zero;
            }

            return vector * (maximumLength / Math.Sqrt(lengthSquared));
        }

        public static double NormalizeAngle(double angle)
        {
            while (angle > Math.PI)
            {
                angle -= MathHelper.TwoPi;
            }

            while (angle < -Math.PI)
            {
                angle += MathHelper.TwoPi;
            }

            return angle;
        }

        public static Vector3D RotateAroundAxis(
            Vector3D vector,
            Vector3D axis,
            double angle)
        {
            axis = SafeNormalize(axis);

            if (axis.LengthSquared() <= VectorEpsilon)
            {
                return vector;
            }

            double cosine = Math.Cos(angle);
            double sine = Math.Sin(angle);

            return vector * cosine +
                   Vector3D.Cross(axis, vector) * sine +
                   axis * Vector3D.Dot(axis, vector) * (1.0 - cosine);
        }

public static double RotorCommandAngle(
            Vector3D targetDirection,
            Vector3D currentDirection,
            Vector3D rotorAxis)
        {
            Vector3D targetPlanar = Rejection(
                targetDirection,
                rotorAxis);

            Vector3D currentPlanar = Rejection(
                currentDirection,
                rotorAxis);

            if (targetPlanar.LengthSquared() <= VectorEpsilon ||
                currentPlanar.LengthSquared() <= VectorEpsilon)
            {
                return 0;
            }

            targetPlanar = SafeNormalize(targetPlanar);
            currentPlanar = SafeNormalize(currentPlanar);
            rotorAxis = SafeNormalize(rotorAxis);

            return Math.Atan2(
                Vector3D.Dot(
                    rotorAxis,
                    Vector3D.Cross(targetPlanar, currentPlanar)),
                Vector3D.Dot(targetPlanar, currentPlanar));
        }

        public static Vector3D WorldToLocalDirection(
            Vector3D worldDirection,
            MatrixD worldMatrix)
        {
            return Vector3D.TransformNormal(
                worldDirection,
                MatrixD.Transpose(worldMatrix));
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

    sealed class GridNode
    {
        public readonly IMyCubeGrid Grid;
        public readonly List<GridEdge> MechanicalEdges =
            new List<GridEdge>();

        public GridComponent Component;
        public GridNode Parent;
        public GridEdge ParentEdge;

        public int Depth = int.MaxValue;
        public bool IncludedForControl;

        public GridNode(IMyCubeGrid grid)
        {
            Grid = grid;
        }
    }

    sealed class GridEdge
    {
        public readonly GridNode A,B;
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

        public GridNode Other(GridNode node)
        {
            return node == A ? B : A;
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

        public bool IncludedForControl,ReachableThroughConnection,HasStaticGrid,HasSlaveCapableRedux;
    }

    sealed class ConnectorEdge
    {
        public IMyShipConnector A,B;

        public GridNode NodeA,NodeB;
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
    sealed class Settings
    {
        public bool Greedy = true,CanMaster = true,CanSlave = true,ParkOnlyByCommand,CruiseLevelsWithGravity = true;
        public readonly List<double> GearFractions = new List<double>
        {
            0.15,
            0.50,
            1.00
        };

        public string UseTag = "[VT-use]",IgnoreTag = "[VT-ignore]",StatusTag = "[VT-status]",ParkTimerTag = "[VT-park]",UnparkTimerTag = "[VT-unpark]";

        public int Update1Skip,Update10Skip,Update100Skip;
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


        settings.Greedy = configurationIni
            .Get(ConfigSection, "Greedy")
            .ToBoolean(settings.Greedy);

        settings.CanMaster = configurationIni
            .Get(ConfigSection, "CanMaster")
            .ToBoolean(settings.CanMaster);

        settings.CanSlave = configurationIni
            .Get(ConfigSection, "CanSlave")
            .ToBoolean(settings.CanSlave);


        settings.ParkOnlyByCommand = configurationIni
            .Get("Parking", "ParkOnlyByCommand")
            .ToBoolean(settings.ParkOnlyByCommand);


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

    void ReadGearFractions(string serializedPercentages)
    {
        string[] values = serializedPercentages.Split(
            new[] { ';', ',' },
            StringSplitOptions.RemoveEmptyEntries);

        List<double> parsed = new List<double>();

        for (int i = 0; i < values.Length; i++)
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
                parsed.Add(percentage / 100.0);
            }
        }

        if (parsed.Count == 0)
        {
            return;
        }

        settings.GearFractions.Clear();
        settings.GearFractions.AddRange(parsed);
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
    readonly List<VectorThrustGroup> groupAllocationWork =
        new List<VectorThrustGroup>();

    readonly HashSet<VectorThrust> aimedNacelles =
        new HashSet<VectorThrust>();

    IMyShipController heartbeatController;


    void RunUpdate100()
    {
        LoadConfiguration(false);
        RequestRescan();
    }

    void RunUpdate10()
    {
        SelectReferenceController();
        DetectTopologyChanges();
        RefreshEffectiveCapacity();

        potentialMaster =
            settings.CanMaster &&
            referenceController != null &&
            referenceController.IsUnderControl;

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

        slaveHeartbeatChangedThisWindow = false;

        EvaluateOperatingMode();

        if (mode == OperatingMode.Parked)
        {
            UpdateParkedRotors();
        }

        WriteStatus(false);
    }

    void RunFlightControl(double timeStep)
    {
        SelectReferenceController();

        if (mode == OperatingMode.Parked ||
            mode == OperatingMode.Initializing ||
            referenceController == null)
        {
            ClearControlledThrust();
            ReleaseGyros();
            return;
        }

        RestoreThrustersAfterPark();
        RefreshAvailableControlledThrust();

        Vector3D centerOfMass =
            referenceController.CenterOfMass;

        Vector3D request;

        if (mode == OperatingMode.Slave)
        {
            request =
                activeSlaveCommand
                    .NormalizedForceDemand *
                availableControlledThrust;

            request -= GetObservedForceWorld();
        }
        else
        {
            request = CalculateLocalForceRequest(
                timeStep);

            if (availableControlledThrust >
                ForceEpsilon)
            {
                normalizedMasterDemand =
                    VectorMath.ClampMagnitude(
                        request /
                        availableControlledThrust,
                        1.0);
            }
            else
            {
                normalizedMasterDemand =
                    Vector3D.Zero;
            }
        }

        requestedForceWorld = request;

        AllocateControlledThrust(
            request,
            centerOfMass);

        bool levelWithGravity =
            mode == OperatingMode.Slave
                ? activeSlaveCommand
                    .LevelWithGravity
                : cruise &&
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
    }

    void RefreshAvailableControlledThrust()
    {
        availableControlledThrust = 0;

        for (int i = 0;
            i < controlledThrusters.Count;
            i++)
        {
            Thruster thruster =
                controlledThrusters[i];

            if (!thruster.IsUsable)
            {
                continue;
            }

            availableControlledThrust +=
                thruster.MaximumEffectiveThrust;
        }
    }


    Vector3D CalculateLocalForceRequest(
        double timeStep)
    {
        MyShipMass shipMass =
            referenceController
                .CalculateShipMass();

        double physicalMass =
            shipMass.PhysicalMass;

        if (physicalMass <= ForceEpsilon)
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

        Vector3D movementInput =
            Vector3D.TransformNormal(
                referenceController
                    .MoveIndicator,
                referenceController
                    .WorldMatrix);

        bool hasMovementInput =
            movementInput.LengthSquared() >
            VectorEpsilon;

        Vector3D movementDirection =
            VectorMath.SafeNormalize(
                movementInput);

        double maximumAcceleration =
            availableControlledThrust /
            physicalMass;

        double gearFraction =
            settings.GearFractions[
                MathHelper.Clamp(
                    selectedGear,
                    0,
                    settings
                        .GearFractions.Count - 1)];

        Vector3D desiredAcceleration =
            movementDirection *
            maximumAcceleration *
            gearFraction;

        scriptDampeners =
            referenceController
                .DampenersOverride;

        if (scriptDampeners)
        {
            Vector3D velocityToDamp =
                velocity;

            if (hasMovementInput)
            {
                double desiredDirectionSpeed =
                    Vector3D.Dot(
                        velocity,
                        movementDirection);

                if (desiredDirectionSpeed > 0)
                {
                    velocityToDamp -=
                        movementDirection *
                        desiredDirectionSpeed;
                }
            }

            if (cruise)
            {
                Vector3D forward =
                    referenceController
                        .WorldMatrix.Forward;

                double forwardSpeed =
                    Vector3D.Dot(
                        velocityToDamp,
                        forward);

                if (forwardSpeed > 0)
                {
                    velocityToDamp -=
                        forward *
                        forwardSpeed;
                }
            }

            Vector3D dampingAcceleration =
                -velocityToDamp /
                Math.Max(
                    timeStep,
                    MinimumTimeStep);

            dampingAcceleration =
                VectorMath.ClampMagnitude(
                    dampingAcceleration,
                    maximumAcceleration);

            desiredAcceleration +=
                dampingAcceleration;
        }

        desiredAcceleration =
            VectorMath.ClampMagnitude(
                desiredAcceleration,
                maximumAcceleration);

        Vector3D requiredAppliedForce =
            physicalMass *
            (desiredAcceleration - gravity);

        return requiredAppliedForce -
               GetObservedForceWorld();
    }

    Vector3D GetObservedForceWorld()
    {
        Vector3D force = Vector3D.Zero;

        for (int i = 0;
            i <
                observedReadOnlyThrusters.Count;
            i++)
        {
            force +=
                observedReadOnlyThrusters[i]
                    .CurrentForceWorld;
        }

        return force;
    }


    void AllocateControlledThrust(
        Vector3D request,
        Vector3D centerOfMass)
    {
        residualForceWorld = request;
        inducedTorqueWorld = Vector3D.Zero;

        for (int i = 0;
            i < controlledThrusters.Count;
            i++)
        {
            controlledThrusters[i]
                .ResetDemand();
        }

        aimedNacelles.Clear();

        for (int i = 0;
            i <
                fixedControlledThrusters.Count;
            i++)
        {
            AllocateSingleThruster(
                fixedControlledThrusters[i],
                ref residualForceWorld,
                centerOfMass,
                ref inducedTorqueWorld);
        }

        for (int i = 0;
            i < vectorThrusters.Count;
            i++)
        {
            vectorThrusters[i]
                .AllocateSecondary(
                    ref residualForceWorld,
                    centerOfMass,
                    ref inducedTorqueWorld);
        }

        groupAllocationWork.Clear();

        for (int i = 0;
            i < vectorThrustGroups.Count;
            i++)
        {
            groupAllocationWork.Add(
                vectorThrustGroups[i]);
        }

        while (groupAllocationWork.Count > 0 &&
               residualForceWorld
                   .LengthSquared() >
               ForceEpsilon * ForceEpsilon)
        {
            int bestIndex = -1;
            double bestScore = ForceEpsilon;

            for (int i = 0;
                i <
                    groupAllocationWork.Count;
                i++)
            {
                double score =
                    groupAllocationWork[i]
                        .Score(
                            residualForceWorld);

                if (score <= bestScore)
                {
                    continue;
                }

                bestScore = score;
                bestIndex = i;
            }

            if (bestIndex < 0)
            {
                break;
            }

            VectorThrustGroup group =
                groupAllocationWork[
                    bestIndex];

            groupAllocationWork.RemoveAt(
                bestIndex);

            Vector3D reachableRequest =
                group.ReachableComponent(
                    residualForceWorld);

            if (reachableRequest
                    .LengthSquared() <=
                VectorEpsilon)
            {
                continue;
            }

            for (int i = 0;
                i < group.Nacelles.Count;
                i++)
            {
                VectorThrust nacelle =
                    group.Nacelles[i];

                nacelle.Aim(reachableRequest);
                aimedNacelles.Add(nacelle);
            }

            for (int i = 0;
                i < group.Nacelles.Count;
                i++)
            {
                group.Nacelles[i]
                    .AllocatePrimary(
                        ref residualForceWorld,
                        centerOfMass,
                        ref inducedTorqueWorld);
            }
        }

        for (int i = 0;
            i < vectorThrusters.Count;
            i++)
        {
            VectorThrust nacelle =
                vectorThrusters[i];

            if (!aimedNacelles.Contains(
                    nacelle))
            {
                nacelle.Aim(
                    Vector3D.Zero);
            }
        }

        for (int i = 0;
            i < controlledThrusters.Count;
            i++)
        {
            controlledThrusters[i]
                .ApplyDemand();
        }
    }

    void AllocateSingleThruster(
        Thruster thruster,
        ref Vector3D residual,
        Vector3D centerOfMass,
        ref Vector3D inducedTorque)
    {
        double contribution =
            thruster.AddOptimalContribution(
                ref residual);

        if (contribution <= ForceEpsilon)
        {
            return;
        }

        Vector3D force =
            thruster.ForceDirectionWorld *
            contribution;

        Vector3D lever =
            thruster.TheBlock.GetPosition() -
            centerOfMass;

        inducedTorque +=
            Vector3D.Cross(lever, force);
    }


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
                        .WorldMatrix.Up;

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

        if (angularCommand.LengthSquared() <=
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


    void SelectReferenceController()
    {
        IMyShipController selected = null;

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
                selected = controller;
                break;
            }

            if (selected == null ||
                controller.IsMainCockpit)
            {
                selected = controller;
            }
        }

        referenceController = selected;
        controllerMissing =
            referenceController == null;

        potentialMaster =
            settings.CanMaster &&
            referenceController != null &&
            referenceController.IsUnderControl;
    }

    void EvaluateOperatingMode()
    {
        SelectReferenceController();

        if (mode == OperatingMode.Slave &&
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

        if (requestedMode == mode)
        {
            return;
        }

        ChangeOperatingMode(requestedMode);
    }

    void ChangeOperatingMode(
        OperatingMode newMode)
    {
        OperatingMode previousMode = mode;

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

        mode = newMode;

        if (newMode ==
            OperatingMode.Slave)
        {
            wasParkedBeforeSlaving =
                previousMode ==
                OperatingMode.Parked;

            slaveFallbackPark = false;
        }

        if (newMode ==
                OperatingMode.Parked &&
            previousMode !=
                OperatingMode.Parked)
        {
            ClearMasterHeartbeat();
            BeginPark();
        }

        forceStatusRefresh = true;
    }


    void UpdateAutomaticParkRequest()
    {
        if (settings.ParkOnlyByCommand)
        {
            automaticParkRequested = false;
            return;
        }

        bool shouldPark = false;

        for (int i = 0;
            i < parkConnectors.Count;
            i++)
        {
            if (ConnectorRequiresParking(
                    parkConnectors[i]))
            {
                shouldPark = true;
                break;
            }
        }

        if (!shouldPark)
        {
            for (int i = 0;
                i < parkLandingGears.Count;
                i++)
            {
                if (LandingGearRequiresParking(
                        parkLandingGears[i]))
                {
                    shouldPark = true;
                    break;
                }
            }
        }

        automaticParkRequested = shouldPark;
    }

    bool ConnectorRequiresParking(
        ParkConnector parkConnector)
    {
        IMyShipConnector connector =
            parkConnector.Block;

        if (connector == null ||
            connector.Closed ||
            connector.Status !=
                MyShipConnectorStatus.Connected)
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
            return other.CubeGrid.IsStatic;
        }

        GridComponent target =
            targetNode.Component;

        if (target == null)
        {
            return other.CubeGrid.IsStatic;
        }

        if (target.HasStaticGrid)
        {
            return true;
        }

        if (target.Controllers.Count == 0)
        {
            return false;
        }

        if (potentialMaster &&
            target.HasSlaveCapableRedux)
        {
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

        return landingGear.IsLocked;
    }

    void BeginPark()
    {
        ClearControlledThrust();
        ReleaseGyros();

        Vector3D gravity =
            referenceController != null
                ? referenceController
                    .GetNaturalGravity()
                : Vector3D.Zero;

        Vector3D localRootCenter =
            Me.CubeGrid.WorldAABB.Center;

        for (int i = 0;
            i < controlledThrusters.Count;
            i++)
        {
            Thruster thruster =
                controlledThrusters[i];

            long entityId =
                thruster.EntityId;

            if (!parkThrusterEnabledState
                    .ContainsKey(entityId))
            {
                parkThrusterEnabledState.Add(
                    entityId,
                    thruster.TheBlock.Enabled);
            }

            thruster.ClearOverride();
            thruster.TheBlock.Enabled = false;
        }

        for (int i = 0;
            i < controlledRotors.Count;
            i++)
        {
            controlledRotors[i]
                .BeginPark(
                    gravity,
                    localRootCenter);
        }

        TriggerTimers(parkTimers);
    }

    void ExitPark()
    {
        RestoreThrustersAfterPark();

        for (int i = 0;
            i < controlledRotors.Count;
            i++)
        {
            controlledRotors[i]
                .CancelPark();
        }

        TriggerTimers(unparkTimers);
    }

    void EnsureNewCacheIsParked()
    {
        for (int i = 0;
            i < controlledThrusters.Count;
            i++)
        {
            Thruster thruster =
                controlledThrusters[i];

            if (!parkThrusterEnabledState
                    .ContainsKey(
                        thruster.EntityId))
            {
                parkThrusterEnabledState.Add(
                    thruster.EntityId,
                    thruster.TheBlock.Enabled);
            }

            thruster.ClearOverride();
            thruster.TheBlock.Enabled = false;
        }

        ReleaseGyros();

        for (int i = 0;
            i < controlledRotors.Count;
            i++)
        {
            if (Math.Abs(
                    controlledRotors[i]
                        .TheBlock
                        .TargetVelocityRad) >
                JointWriteDeadbandRad)
            {
                controlledRotors[i]
                    .TheBlock
                    .TargetVelocityRad = 0;
            }
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
            i < controlledThrusters.Count;
            i++)
        {
            controlledThrusters[i]
                .ClearOverride();
        }
    }

    void RestoreThrustersAfterPark()
    {
        if (parkThrusterEnabledState.Count == 0)
        {
            return;
        }

        for (int i = 0;
            i < controlledThrusters.Count;
            i++)
        {
            Thruster thruster =
                controlledThrusters[i];

            bool wasEnabled;

            if (!parkThrusterEnabledState
                    .TryGetValue(
                        thruster.EntityId,
                        out wasEnabled))
            {
                continue;
            }

            thruster.TheBlock.Enabled =
                wasEnabled;
        }

        parkThrusterEnabledState.Clear();
    }

    void RestoreParkedThruster(
        long entityId,
        IMyThrust block)
    {
        bool wasEnabled;

        if (!parkThrusterEnabledState
                .TryGetValue(
                    entityId,
                    out wasEnabled))
        {
            return;
        }

        if (block != null &&
            !block.Closed)
        {
            block.Enabled = wasEnabled;
        }

        parkThrusterEnabledState.Remove(
            entityId);
    }

    bool WasThrusterDisabledByPark(
        long entityId)
    {
        return parkThrusterEnabledState
            .ContainsKey(entityId);
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


    void HandleArgument(string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            return;
        }

        string[] commands =
            argument.ToLowerInvariant()
                .Split(
                    new[] { ';', '\n', '\r' },
                    StringSplitOptions
                        .RemoveEmptyEntries);

        for (int i = 0;
            i < commands.Length;
            i++)
        {
            string command =
                commands[i].Trim();

            if (command == "park")
            {
                manualParkRequested =
                    !manualParkRequested;

                slaveFallbackPark = false;
            }
            else if (command == "park on")
            {
                manualParkRequested = true;
                slaveFallbackPark = false;
            }
            else if (command == "park off"||
                     command == "unpark")
            {
                manualParkRequested = false;
                slaveFallbackPark = false;
            }
            else if (command == "cruise")
            {
                cruise = !cruise;
            }
            else if (command == "cruise on")
            {
                cruise = true;
            }
            else if (command == "cruise off")
            {
                cruise = false;
            }
            else if (command == "dampeners")
            {
                scriptDampeners =
                    !scriptDampeners;

                if (referenceController != null)
                {
                    referenceController
                        .DampenersOverride =
                        scriptDampeners;
                }
            }
            else if (command == "gear")
            {
                selectedGear++;

                if (selectedGear >=
                    settings
                        .GearFractions.Count)
                {
                    selectedGear = 0;
                }
            }
            else if (command == "rescan")
            {
                RequestRescan();
            }
        }

        Save();
    }


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
                connector.OtherConnector != null
                    ? connector
                        .OtherConnector
                        .EntityId
                    : 0;

            long previousTarget;

            if (!observedConnectorTargets
                    .TryGetValue(
                        connector.EntityId,
                        out previousTarget) ||
                previousTarget != targetId)
            {
                observedConnectorTargets[
                    connector.EntityId] =
                    targetId;

                RequestRescan();
            }

            seenConnectors.Add(
                connector.EntityId);
        }

        for (int i = 0;
            i < parkLandingGears.Count;
            i++)
        {
            IMyLandingGear gear =
                parkLandingGears[i].Block;

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
                previous != gear.IsLocked)
            {
                observedLandingGearLocks[
                    gear.EntityId] =
                    gear.IsLocked;

                RequestRescan();
            }
        }
    }


    void PublishOrClearMasterHeartbeat()
    {
        if (mode != OperatingMode.Master ||
            referenceController == null)
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

        section.Append('[')
            .Append(HeartbeatSection)
            .AppendLine("]");

        section.Append("Version=")
            .AppendLine(ScriptVersion);

        section.Append("MasterProgrammableBlockId=")
            .AppendLine(Me.EntityId.ToString());

        section.Append("ControllerId=")
            .AppendLine(
                referenceController.EntityId
                    .ToString());

        section.Append("Sequence=")
            .AppendLine(
                heartbeatSequence.ToString());

        section.Append("Demand=")
            .AppendLine(
                SerializeVector(
                    normalizedMasterDemand));

        section.Append("Cruise=")
            .AppendLine(
                cruise.ToString());

        section.Append("LevelWithGravity=")
            .AppendLine(
                (cruise &&
                settings
                    .CruiseLevelsWithGravity)
                .ToString());

        referenceController.CustomData =
            ReplaceSection(
                referenceController.CustomData,
                HeartbeatSection,
                section.ToString());
    }

    void ClearMasterHeartbeat()
    {
        if (heartbeatController == null)
        {
            return;
        }

        RemoveOwnedHeartbeat(
            heartbeatController);

        heartbeatController = null;
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
            parsedId != Me.EntityId)
        {
            return;
        }

        controller.CustomData =
            RemoveSection(
                controller.CustomData,
                HeartbeatSection);
    }

    void TryReadAnySlaveHeartbeat()
    {
        for (int i = 0;
            i <
                remotelyReachableControllers.Count;
            i++)
        {
            MasterCommand command;

            if (!TryReadMasterCommand(
                    remotelyReachableControllers[i],
                    out command))
            {
                continue;
            }

            AcceptSlaveCommand(command);
            return;
        }
    }

    void ReadActiveSlaveHeartbeat()
    {
        for (int i = 0;
            i <
                remotelyReachableControllers.Count;
            i++)
        {
            IMyShipController controller =
                remotelyReachableControllers[i];

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
                    .MasterProgrammableBlockId != 0 &&
                command
                    .MasterProgrammableBlockId !=
                activeSlaveCommand
                    .MasterProgrammableBlockId)
            {
                continue;
            }

            AcceptSlaveCommand(command);
            return;
        }
    }

    void AcceptSlaveCommand(
        MasterCommand command)
    {
        if (command.Sequence !=
                lastSlaveHeartbeatSequence ||
            command
                .MasterProgrammableBlockId !=
            slaveMasterProgrammableBlockId)
        {
            lastSlaveHeartbeatSequence =
                command.Sequence;

            slaveMasterProgrammableBlockId =
                command
                    .MasterProgrammableBlockId;

            slaveHeartbeatAgeUpdate10 = 0;
            slaveHeartbeatChangedThisWindow =
                true;
            slaveHeartbeatFresh = true;
        }

        activeSlaveCommand.CopyFrom(
            command);
    }

    bool TryReadMasterCommand(
        IMyShipController controller,
        out MasterCommand command)
    {
        command = null;

        if (controller == null ||
            controller.Closed)
        {
            return false;
        }

        string masterIdText;
        string controllerIdText;
        string sequenceText;
        string demandText;
        string cruiseText;
        string levelText;

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

        TryReadSectionValue(
            controller.CustomData,
            HeartbeatSection,
"Cruise",
            out cruiseText);

        TryReadSectionValue(
            controller.CustomData,
            HeartbeatSection,
"LevelWithGravity",
            out levelText);

        bool commandCruise;
        bool commandLevel;

        bool.TryParse(
            cruiseText,
            out commandCruise);

        bool.TryParse(
            levelText,
            out commandLevel);

        command = new MasterCommand
        {
            MasterProgrammableBlockId =
                masterId,
            ControllerId = controllerId,
            Sequence = sequence,
            NormalizedForceDemand =
                VectorMath.ClampMagnitude(
                    demand,
                    1),
            Cruise = commandCruise,
            LevelWithGravity =
                commandLevel
        };

        return true;
    }


    static int FindSectionStart(
        string customData,
        string sectionName)
    {
        if (string.IsNullOrEmpty(customData))
        {
            return -1;
        }

        string header =
"["+ sectionName + "]";

        int searchIndex = 0;

        while (searchIndex <
               customData.Length)
        {
            int index = customData.IndexOf(
                header,
                searchIndex,
                StringComparison.OrdinalIgnoreCase);

            if (index < 0)
            {
                return -1;
            }

            bool startsLine =
                index == 0 ||
                customData[index - 1] == '\n';

            int after = index + header.Length;

            bool endsLine =
                after >= customData.Length ||
                customData[after] == '\r' ||
                customData[after] == '\n';

            if (startsLine && endsLine)
            {
                return index;
            }

            searchIndex = index + 1;
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

            int cursor = lineStart;

            while (cursor <
                       customData.Length &&
                   (customData[cursor] == ' ' ||
                    customData[cursor] == '\t' ||
                    customData[cursor] == '\r'))
            {
                cursor++;
            }

            if (cursor <
                    customData.Length &&
                customData[cursor] == '[')
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

            searchIndex = lineStart;
        }

        return customData.Length;
    }

    static bool TryReadSectionValue(
        string customData,
        string sectionName,
        string key,
        out string value)
    {
        value = null;

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
                sectionName.Length + 2);

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
                end - headerEnd - 1);

        string[] lines =
            section.Replace(
"\r",
                    string.Empty)
                .Split('\n');

        for (int i = 0;
            i < lines.Length;
            i++)
        {
            string line = lines[i];

            int separator =
                line.IndexOf('=');

            if (separator <= 0)
            {
                continue;
            }

            string candidateKey =
                line.Substring(
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
                line.Substring(
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
            customData ?? string.Empty;

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
                customData.EndsWith("\n")
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
                sectionName.Length + 2);

        return customData.Substring(0, start) +
               replacement +
               customData.Substring(end);
    }

    static string RemoveSection(
        string customData,
        string sectionName)
    {
        if (string.IsNullOrEmpty(customData))
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
                sectionName.Length + 2);

        string before =
            customData.Substring(0, start);

        string after =
            customData.Substring(end);

        if (before.EndsWith("\n") &&
            after.StartsWith("\n"))
        {
            after = after.Substring(1);
        }

        return before + after;
    }

    static string SerializeVector(
        Vector3D vector)
    {
        return vector.X.ToString("R") +
";"+
            vector.Y.ToString("R") +
";"+
            vector.Z.ToString("R");
    }

    static bool TryParseVector(
        string serialized,
        out Vector3D vector)
    {
        vector = Vector3D.Zero;

        if (string.IsNullOrWhiteSpace(
                serialized))
        {
            return false;
        }

        string[] components =
            serialized.Split(';');

        if (components.Length != 3)
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

        vector = new Vector3D(x, y, z);
        return true;
    }


    void WriteStatus(bool force)
    {
        echoBuilder.Clear();

        echoBuilder
            .AppendLine(ScriptName)
            .Append("v")
            .AppendLine(ScriptVersion)
            .AppendLine();

        echoBuilder
            .Append("Mode: ")
            .AppendLine(mode.ToString());

        echoBuilder
            .Append("Controller: ")
            .AppendLine(
                referenceController != null
                    ? referenceController
                        .CustomName
                    : "NONE");

        echoBuilder
            .Append("Dampeners: ")
            .AppendLine(
                scriptDampeners
                    ? "ON"                    : "OFF");

        echoBuilder
            .Append("Cruise: ")
            .AppendLine(
                cruise
                    ? "ON"                    : "OFF");

        echoBuilder
            .Append("Gear: ")
            .Append(selectedGear + 1)
            .Append("/")
            .Append(settings
                .GearFractions.Count)
            .Append(" (")
            .Append(
                (settings
                    .GearFractions[
                        MathHelper.Clamp(
                            selectedGear,
                            0,
                            settings
                                .GearFractions
                                .Count - 1)] *
                 100)
                .ToString("0.##"))
            .AppendLine("%)");

        echoBuilder
            .Append("Nacelles: ")
            .AppendLine(
                vectorThrusters.Count
                    .ToString());

        echoBuilder
            .Append("Controlled thrust: ")
            .Append(
                (availableControlledThrust /
                 1000.0)
                .ToString("0.##"))
            .AppendLine(" kN");

        echoBuilder
            .Append("Residual: ")
            .Append(
                (residualForceWorld.Length() /
                 1000.0)
                .ToString("0.##"))
            .AppendLine(" kN");

        echoBuilder
            .Append("Gyros: ")
            .AppendLine(
                controlledGyros.Count
                    .ToString());

        if (mode == OperatingMode.Slave)
        {
            echoBuilder
                .Append("Heartbeat age: ")
                .Append(
                    slaveHeartbeatAgeUpdate10)
                .AppendLine("/2");
        }

        echoBuilder
            .Append("Runtime: ")
            .Append(
                Runtime.LastRunTimeMs
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
            .Append(Runtime
                .CurrentInstructionCount)
            .Append("/")
            .AppendLine(Runtime
                .MaxInstructionCount
                .ToString());

        Echo(echoBuilder.ToString());

        if (!force &&
            statusSurfaces.Count == 0)
        {
            return;
        }

        statusBuilder.Clear();

        statusBuilder
            .AppendLine("VECTOR THRUST REDUX")
            .Append("MODE  ")
            .AppendLine(
                mode.ToString()
                    .ToUpperInvariant())
            .Append("DAMP  ")
            .AppendLine(
                scriptDampeners
                    ? "ON"                    : "OFF")
            .Append("CRUISE ")
            .AppendLine(
                cruise
                    ? "ON"                    : "OFF")
            .Append("GEAR  ")
            .Append(selectedGear + 1)
            .Append("/")
            .AppendLine(
                settings
                    .GearFractions.Count
                    .ToString())
            .Append("VECTORS ")
            .AppendLine(
                vectorThrusters.Count
                    .ToString())
            .Append("THRUST ")
            .Append(
                (availableControlledThrust /
                 1000.0)
                .ToString("0.0"))
            .AppendLine(" kN")
            .Append("ERROR ")
            .Append(
                (residualForceWorld.Length() /
                 1000.0)
                .ToString("0.0"))
            .AppendLine(" kN");

        for (int i = 0;
            i < statusSurfaces.Count;
            i++)
        {
            statusSurfaces[i]
                .Write(
                    statusBuilder.ToString());
        }
    }

    const string ScriptName = "Vector Thrust Redux",ScriptVersion = "0.1.0",ConfigSection = "Vector Thrust Redux",HeartbeatSection = "Vector Thrust Redux Heartbeat",SurfaceSelector = "VT-Redux:";


    const double VectorEpsilon = 1e-8,ForceEpsilon = 1e-3,AngleEpsilon = 1e-4,EqualLimitEpsilon = 1e-4,DirectionBucketCosine = 1.0 - 1e-6,ParallelAxisCosine = 1.0 - 1e-4,DirectParkAlignmentCosine = 1.0 - 1e-4,JointResponseGain = 4.0,MaximumJointVelocityRad = Math.PI,JointWriteDeadbandRad = 1e-3,GyroLevelGain = 4.0,GyroAngularDampingGain = 1.5,GyroCommandAtFullTorque = 30.0,GyroWriteDeadband = 1e-3,SmallGridGyroCapacity = 448000.0,LargeGridGyroCapacity = 33600000.0,SmallGridPrototechGyroCapacity = 4480000.0,LargeGridPrototechGyroCapacity = 201600000.0,MinimumTimeStep = 1.0 / 120.0,MaximumTimeStep = 0.25;


    readonly Settings settings = new Settings();
    readonly MyIni storageIni = new MyIni();

    string knownProgrammableBlockCustomData = string.Empty;

    bool cruise,automaticParkRequested,forceStatusRefresh;
    bool scriptDampeners = true,manualParkRequested,controllerMissing = true,potentialMaster,slaveHeartbeatFresh,slaveFallbackPark,wasParkedBeforeSlaving,slaveHeartbeatChangedThisWindow,rescanRequested = true;
    int selectedGear,slaveHeartbeatAgeUpdate10,update10SkipCounter,update100SkipCounter;


    OperatingMode mode = OperatingMode.Initializing;

    IMyShipController referenceController;

    long heartbeatSequence;
    long lastSlaveHeartbeatSequence = long.MinValue,slaveMasterProgrammableBlockId;

    MasterCommand activeSlaveCommand = new MasterCommand();

    Vector3D requestedForceWorld;
    Vector3D residualForceWorld,normalizedMasterDemand,inducedTorqueWorld;

    double availableControlledThrust;
    double lastControlTimeStep,accumulatedControlTime;

    int update1SkipCounter;


    readonly List<IMyShipController> localControllers = new List<IMyShipController>(),remotelyReachableControllers = new List<IMyShipController>();

    readonly List<Thruster> thrusters = new List<Thruster>(),controlledThrusters = new List<Thruster>(),fixedControlledThrusters = new List<Thruster>(),observedReadOnlyThrusters = new List<Thruster>();

    readonly List<Rotor> controlledRotors = new List<Rotor>();
    readonly List<VectorThrust> vectorThrusters = new List<VectorThrust>();
    readonly List<VectorThrustGroup> vectorThrustGroups = new List<VectorThrustGroup>();

    readonly List<Gyro> controlledGyros = new List<Gyro>();

    readonly List<ParkConnector> parkConnectors = new List<ParkConnector>();
    readonly List<ParkLandingGear> parkLandingGears = new List<ParkLandingGear>();

    readonly List<IMyTimerBlock> parkTimers = new List<IMyTimerBlock>(),unparkTimers = new List<IMyTimerBlock>();

    readonly List<StatusSurface> statusSurfaces = new List<StatusSurface>();

    readonly Dictionary<long, bool> parkThrusterEnabledState = new Dictionary<long, bool>();
    readonly Dictionary<long, GridNode> gridNodes = new Dictionary<long, GridNode>();

    readonly StringBuilder echoBuilder = new StringBuilder(),statusBuilder = new StringBuilder();

    IEnumerator<int> scanStateMachine;

    RuntimeTracker runtimeTracker;

    public Program()
    {
        runtimeTracker = new RuntimeTracker(this);

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

        storageIni.Set("State", "Cruise", cruise);
        storageIni.Set("State", "Dampeners", scriptDampeners);
        storageIni.Set("State", "ManualPark", manualParkRequested);
        storageIni.Set("State", "Gear", selectedGear);

        Storage = storageIni.ToString();
    }

    public void Main(string argument, UpdateType updateSource)
    {
        runtimeTracker.BeginRun();

        double elapsedSeconds = Runtime.TimeSinceLastRun.TotalSeconds;

        if (elapsedSeconds < MinimumTimeStep)
        {
            elapsedSeconds = MinimumTimeStep;
        }
        else if (elapsedSeconds > MaximumTimeStep)
        {
            elapsedSeconds = MaximumTimeStep;
        }

        accumulatedControlTime += elapsedSeconds;

        bool explicitRun =
            (updateSource & (UpdateType.Terminal | UpdateType.Trigger | UpdateType.Script)) != 0 ||
            !string.IsNullOrWhiteSpace(argument);

        if (explicitRun)
        {
            HandleArgument(argument);
        }

        ContinueRescan();

        if ((updateSource & UpdateType.Update100) != 0 &&
            IsScheduled(ref update100SkipCounter, settings.Update100Skip))
        {
            RunUpdate100();
        }

        if ((updateSource & UpdateType.Update10) != 0 &&
            IsScheduled(ref update10SkipCounter, settings.Update10Skip))
        {
            RunUpdate10();
        }

        if ((updateSource & UpdateType.Update1) != 0)
        {
            heartbeatSequence++;

            if (mode == OperatingMode.Slave)
            {
                ReadActiveSlaveHeartbeat();
            }

            EvaluateOperatingMode();

            if (IsScheduled(ref update1SkipCounter, settings.Update1Skip))
            {
                lastControlTimeStep = MathHelper.Clamp(
                    accumulatedControlTime,
                    MinimumTimeStep,
                    MaximumTimeStep);

                accumulatedControlTime = 0;

                RunFlightControl(lastControlTimeStep);
            }

            PublishOrClearMasterHeartbeat();
        }

        if (explicitRun)
        {
            EvaluateOperatingMode();
            PublishOrClearMasterHeartbeat();
            forceStatusRefresh = true;
        }

        if (forceStatusRefresh)
        {
            WriteStatus(true);
            forceStatusRefresh = false;
        }

        runtimeTracker.EndRun();
    }

    static bool IsScheduled(ref int counter, int skippedIntervals)
    {
        if (counter < skippedIntervals)
        {
            counter++;
            return false;
        }

        counter = 0;
        return true;
    }

    void LoadStorage()
    {
        if (string.IsNullOrWhiteSpace(Storage))
        {
            return;
        }

        MyIniParseResult parseResult;

        if (!storageIni.TryParse(Storage, out parseResult))
        {
            return;
        }

        cruise = storageIni.Get("State", "Cruise").ToBoolean(false);
        scriptDampeners = storageIni.Get("State", "Dampeners").ToBoolean(true);
        manualParkRequested = storageIni.Get("State", "ManualPark").ToBoolean(false);
        selectedGear = Math.Max(0, storageIni.Get("State", "Gear").ToInt32(0));
    }
    sealed class ScanSnapshot
    {
        public readonly List<IMyTerminalBlock> Blocks =
            new List<IMyTerminalBlock>();

        public readonly List<IMyShipController> Controllers =
            new List<IMyShipController>(),LocalControllers =
            new List<IMyShipController>(),RemoteControllers =
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
            new List<IMyShipConnector>(),TopologyConnectors =
            new List<IMyShipConnector>();

        public readonly List<IMyLandingGear> RawLandingGears =
            new List<IMyLandingGear>();

        public readonly List<IMyTimerBlock> RawTimers =
            new List<IMyTimerBlock>(),ParkTimers =
            new List<IMyTimerBlock>(),UnparkTimers =
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

        public readonly List<Thruster> Thrusters =
            new List<Thruster>(),ControlledThrusters =
            new List<Thruster>(),FixedControlledThrusters =
            new List<Thruster>(),ObservedReadOnlyThrusters =
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


        for (int i = 0;
            i < snapshot.RawTimers.Count;
            i++)
        {
            IMyTimerBlock timer =
                snapshot.RawTimers[i];

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
