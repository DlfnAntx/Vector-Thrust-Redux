// <mdk sortorder="65" />   // Heartbeat.cs
using Sandbox.ModAPI.Ingame;
using System;
using System.Text;
using VRageMath;

namespace IngameScript
{
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
                "[" +
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
                   ";" +
                   vector.Y
                       .ToString("R") +
                   ";" +
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
}
