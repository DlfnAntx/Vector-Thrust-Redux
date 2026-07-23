// <mdk sortorder="35" />   // StateAndAuthority.cs
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRageMath;

namespace IngameScript
{
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
                "Cruise target: " +
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
                    SetCommandResult(
                        "WARNING: Cruise cannot control " +
                        "main-grid reverse thrusters; " +
                        "all are " +
                        settings.IgnoreTag +
                        ".",
                        true);
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
                SetCommandResult(
                    "WARNING: Cruise cannot control " +
                    "main-grid reverse thrusters; " +
                    "add " +
                    settings.UseTag +
                    ".",
                    true);
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
}
