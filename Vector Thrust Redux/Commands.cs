// <mdk sortorder="60" />   // Commands.cs
using System;
using System.Text;
using VRageMath;

namespace IngameScript
{
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
                        "Unknown command: \"" +
                        command +
                        "\"";

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
                    "unpark" ||
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
                    "rescan" ||
                words[0] ==
                    "scan" ||
                words[0] ==
                    "refresh")
            {
                if (words.Length != 1)
                {
                    result =
                        "Invalid rescan command: \"" +
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
                    "Expected dampeners " +
                    "on, off, or toggle.";

                warning =
                    true;

                return true;
            }

            SetLocalDampeners(
                enabled);

            result =
                "Local dampeners: " +
                (scriptDampeners
                    ? "ON"
                    : "OFF");

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
                        "Unexpected text after " +
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
                    "increase" ||
                operation ==
                    "increment" ||
                operation ==
                    "up" ||
                operation ==
                    "add" ||
                operation ==
                    "faster";

            bool decrease =
                operation ==
                    "decrease" ||
                operation ==
                    "decrement" ||
                operation ==
                    "down" ||
                operation ==
                    "subtract" ||
                operation ==
                    "slower";

            if (increase ||
                decrease)
            {
                if (argumentIndex + 2 !=
                    words.Length)
                {
                    result =
                        "Cruise adjustment requires " +
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
                        "Invalid Cruise speed: \"" +
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
                    "target" ||
                operation ==
                    "speed" ||
                operation ==
                    "set")
            {
                if (argumentIndex + 2 !=
                    words.Length)
                {
                    result =
                        "Cruise target requires " +
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
                        "Invalid Cruise target: \"" +
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
                "Expected Cruise on, off, toggle, " +
                "+value, -value, increase value, " +
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
                    "Expected parking " +
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
                "Parking: " +
                (manualParkRequested
                    ? "ON"
                    : "OFF");

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
                    "next" ||
                operation ==
                    "up" ||
                operation ==
                    "increase" ||
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
                    "previous" ||
                operation ==
                    "prev" ||
                operation ==
                    "down" ||
                operation ==
                    "decrease" ||
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
                    "Invalid gear: \"" +
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
                    "Gear must be between 1 and " +
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
                    "dampeners" ||
                words[0] ==
                    "dampener" ||
                words[0] ==
                    "dampers" ||
                words[0] ==
                    "damper" ||
                words[0] ==
                    "damping" ||
                words[0] ==
                    "dampening")
            {
                return true;
            }

            if (words.Length >= 2 &&
                words[0] ==
                    "inertia" &&
                (words[1] ==
                     "dampeners" ||
                 words[1] ==
                     "dampener" ||
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

            if (words[0] ==
                    "cruise" ||
                words[0] ==
                    "cruisecontrol")
            {
                return true;
            }

            if (words.Length >= 2 &&
                words[0] ==
                    "cruise" &&
                words[1] ==
                    "control")
            {
                argumentIndex = 2;
                return true;
            }

            return false;
        }

        static bool IsParkingCommand(
            string[] words,
            out int argumentIndex)
        {
            argumentIndex = 1;

            return words[0] ==
                       "park" ||
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
                    "on" ||
                operation ==
                    "enable" ||
                operation ==
                    "enabled" ||
                operation ==
                    "start")
            {
                result = true;
                return true;
            }

            if (operation ==
                    "off" ||
                operation ==
                    "disable" ||
                operation ==
                    "disabled" ||
                operation ==
                    "stop")
            {
                result = false;
                return true;
            }

            if (operation ==
                    "toggle" ||
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
                     "+" ||
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
                        "+"
                        ? Math.Abs(amount)
                        : -Math.Abs(amount);

                return true;
            }

            return false;
        }

        string GetLocalCruiseResult()
        {
            string result =
                "Local Cruise: " +
                (cruise
                    ? "ON"
                    : "OFF");

            if (cruiseTargetInitialized)
            {
                result +=
                    " @ " +
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
                "Local gear: " +
                (selectedGear + 1) +
                "/" +
                gearCount +
                " (" +
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
}
