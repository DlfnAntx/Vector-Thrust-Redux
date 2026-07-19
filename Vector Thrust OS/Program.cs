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
        const string ScriptVersion = "0.1.0";
        const string ConfigSection = "Vector Thrust Redux";
        const string HeartbeatSection = "Vector Thrust Redux Heartbeat";
        const string SurfaceSelector = "VT-Redux:";

        // ===== Numerical tolerances =====

        const double VectorEpsilon = 1e-8;
        const double ForceEpsilon = 1e-3;
        const double AngleEpsilon = 1e-4;
        const double EqualLimitEpsilon = 1e-4;
        const double DirectionBucketCosine = 1.0 - 1e-6;
        const double ParallelAxisCosine = 1.0 - 1e-4;
        const double DirectParkAlignmentCosine = 1.0 - 1e-4;

        // A proportional joint response is retained instead of pretending
        // that TargetVelocityRad is a position command.
        const double JointResponseGain = 4.0;
        const double MaximumJointVelocityRad = Math.PI;
        const double JointWriteDeadbandRad = 1e-3;

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

        // ===== Cached construct model =====

        readonly List<IMyShipController> localControllers = new List<IMyShipController>();
        readonly List<IMyShipController> remotelyReachableControllers = new List<IMyShipController>();

        readonly List<Thruster> thrusters = new List<Thruster>();
        readonly List<Thruster> controlledThrusters = new List<Thruster>();
        readonly List<Thruster> fixedControlledThrusters = new List<Thruster>();
        readonly List<Thruster> observedReadOnlyThrusters = new List<Thruster>();

        readonly List<Rotor> controlledRotors = new List<Rotor>();
        readonly List<VectorThrust> vectorThrusters = new List<VectorThrust>();
        readonly List<VectorThrustGroup> vectorThrustGroups = new List<VectorThrustGroup>();

        readonly List<Gyro> controlledGyros = new List<Gyro>();

        readonly List<ParkConnector> parkConnectors = new List<ParkConnector>();
        readonly List<ParkLandingGear> parkLandingGears = new List<ParkLandingGear>();

        readonly List<IMyTimerBlock> parkTimers = new List<IMyTimerBlock>();
        readonly List<IMyTimerBlock> unparkTimers = new List<IMyTimerBlock>();

        readonly List<StatusSurface> statusSurfaces = new List<StatusSurface>();

        readonly Dictionary<long, bool> parkThrusterEnabledState = new Dictionary<long, bool>();
        readonly Dictionary<long, GridNode> gridNodes = new Dictionary<long, GridNode>();

        readonly StringBuilder echoBuilder = new StringBuilder();
        readonly StringBuilder statusBuilder = new StringBuilder();

        IEnumerator<int> scanStateMachine;
        bool rescanRequested = true;

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

                // The heartbeat itself is not skipped. A skipped control frame
                // republishes the most recent command so slaves do not mistake
                // a configured performance setting for a dead master.
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
    }
}
