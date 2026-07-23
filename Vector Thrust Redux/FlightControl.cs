// <mdk sortorder="50" />   // FlightControl.cs
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRageMath;

namespace IngameScript
{
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
                    GetLocalProjectedCapacity(
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
                        GetLocalProjectedCapacity(
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
                    GetLocalProjectedCapacity(
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
                GetLocalProjectedCapacity(
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
                i <
                    localUnmanagedThrusters.Count;
                i++)
            {
                force +=
                    localUnmanagedThrusters[i]
                        .CurrentForceWorld;
            }

            if (mode ==
                OperatingMode.Master)
            {
                for (int i = 0;
                    i <
                        remoteUnmanagedThrusters
                            .Count;
                    i++)
                {
                    force +=
                        remoteUnmanagedThrusters[i]
                            .CurrentForceWorld;
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

                    if (bestSolution == null ||
                        solution
                            .ReachableProjectedCapacity >
                        bestSolution
                            .ReachableProjectedCapacity)
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
}
