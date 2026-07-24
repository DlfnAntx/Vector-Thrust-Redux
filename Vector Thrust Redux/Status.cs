// <mdk sortorder="70" />   // Status.cs
using System;
using System.Text;
using VRageMath;

namespace IngameScript
{
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
                            ? "WARNING: "
                            : "Command: ")
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
                            ? "WARN "
                            : "CMD  ")
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
                            ? "ON"
                            : "OFF");

                builder
                    .Append("Cruise: ")
                    .Append(
                        cruise
                            ? "ON"
                            : "OFF");

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
                        ? "ON"
                        : "OFF")
                .Append(" | master ")
                .Append(
                    activeSlaveCommand
                        .Dampeners
                        ? "ON"
                        : "OFF")
                .Append(" | effective ")
                .AppendLine(
                    EffectiveDampeners
                        ? "ON"
                        : "OFF");

            builder
                .Append("Cruise: local ")
                .Append(
                    cruise
                        ? "ON"
                        : "OFF");

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
                        ? "ON"
                        : "OFF")
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
                            ? "ON"
                            : "OFF")
                    .Append("CRUISE ")
                    .Append(
                        cruise
                            ? "ON"
                            : "OFF");

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
                        ? "ON"
                        : "OFF")
                .Append(" M:")
                .AppendLine(
                    activeSlaveCommand
                            .Dampeners
                        ? "ON"
                        : "OFF")
                .Append("CRUISE L:")
                .Append(
                    cruise
                        ? "ON"
                        : "OFF")
                .Append(" M:")
                .Append(
                    activeSlaveCommand
                            .Cruise
                        ? "ON"
                        : "OFF")
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
}
