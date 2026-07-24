// <mdk sortorder="55" />   // OperatingAndParking.cs
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
}
