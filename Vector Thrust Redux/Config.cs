// <mdk sortorder="10" />   // Config.cs
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game.ModAPI.Ingame.Utilities;


namespace IngameScript
{
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
                    "\n\nCustom Data could not be parsed as INI:\n" +
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
                " Vector Thrust Redux ownership and coordination.\n" +
                " Greedy controls eligible mechanical-subgrid blocks unless ignored.\n" +
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
                " Number of matching update intervals skipped between executions.\n" +
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
}
