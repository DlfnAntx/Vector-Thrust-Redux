// <mdk sortorder="0" />    // Program.cs
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    /*
     * Vector Thrust Redux
     *
     * VTOS is a fork by pyro000, with help from Digi, Whiplash, d1ag0n,
     * feoranis, and Malware, from VectorThrust2 by 1wsx10, with help from
     * ETHEREAL1, leiflang, and KenDaveRob.
     *
     * Redux retains applicable algorithms, explanations, and attribution
     * where its implementation follows the original work.
     *
     * Development and deployment use MDK², Malware's Development Kit for
     * Space Engineers.
     */

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
                        ? "1"
                        : "0") +
                    ";" +
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
}
