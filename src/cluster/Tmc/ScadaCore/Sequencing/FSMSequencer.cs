﻿#region Header

/// FileName: FSMSequencer.cs
/// Author: Denis Kaynarca (denis@dkaynarca.com)

#endregion Header

using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Extensions;
using Appccelerate.StateMachine.Machine;
using System;
using System.Diagnostics;
using Tmc.Common;
using System.Collections.Generic;
using TmcData;
using System.Threading.Tasks;

namespace Tmc.Scada.Core.Sequencing
{
    public enum OperationMode
    {
        SortOnly,
        AssembleOnly,
        Normal
    }

    public class StateLoggerExtension : ExtensionBase<State, Trigger>
    {
        public State CurrentState { get; private set; }
        public State PreviousState { get; private set; }

        public StateLoggerExtension() : base()
        {
            CurrentState = State.None;
            CurrentState = State.None;
        }

        public override void SwitchedState(IStateMachineInformation<State, Trigger> stateMachine, 
            Appccelerate.StateMachine.Machine.IState<State, Trigger> oldState, 
            Appccelerate.StateMachine.Machine.IState<State, Trigger> newState)
        {
            if (newState != null)
            {
                this.CurrentState = newState.Id;
            }

            if (oldState != null)
            {
                this.PreviousState = oldState.Id;
            }

            Logger.Instance.Write(String.Format("[Sequencer] Transitioned: {0} -> {1}", this.PreviousState, this.CurrentState), LogType.Message);

            base.SwitchedState(stateMachine, oldState, newState);
        }
    }

    public class FSMSequencer : ISequencer
    {
        public bool IsRunning { get; private set; }

        public OperationMode Mode { get; set; }

        public string Name { get; set; }
        private Assembler _assembler;
        private ConveyorController _conveyorController;
        private ScadaEngine _engine;
        private PassiveStateMachine<State, Trigger> _fsm;
        private Loader _loader;
        private OrderConsumer _orderConsumer;
        private Sorter _sorter;
        private TrayVerifier _trayVerifier;
        public StateLoggerExtension TransitionLogger { get; private set; }
        private List<IHardware> _hardware;

        public FSMSequencer(ScadaEngine engine)
        {
            this._engine = engine;

            Debug.Assert(this._engine != null);

            this._conveyorController = _engine.ClusterConfig.Controllers[typeof(ConveyorController)] as ConveyorController;
            this._assembler = _engine.ClusterConfig.Controllers[typeof(Assembler)] as Assembler;
            this._loader = _engine.ClusterConfig.Controllers[typeof(Loader)] as Loader;
            this._sorter = _engine.ClusterConfig.Controllers[typeof(Sorter)] as Sorter;
            this._trayVerifier = _engine.ClusterConfig.Controllers[typeof(TrayVerifier)] as TrayVerifier;
            this._orderConsumer = _engine.OrderConsumer;
            this._hardware = _engine.ClusterConfig.GetAllHardware();

            Debug.Assert(this._conveyorController != null);
            Debug.Assert(this._assembler != null);
            Debug.Assert(this._loader != null);
            Debug.Assert(this._sorter != null);
            Debug.Assert(this._trayVerifier != null);

            this.Mode = OperationMode.Normal;

            _fsm = new PassiveStateMachine<State, Trigger>();
            TransitionLogger = new StateLoggerExtension();
            _fsm.AddExtension(TransitionLogger);
            this.Create();
            this.BindEvents();
        }

        #region Public Methods

        public void FireResumeTrigger()
        {
            if (IsRunning)
            {
                _fsm.Fire(Trigger.Resume);
            }
        }

        public void FireShutdownTrigger()
        {
            if (IsRunning)
            {
                _fsm.Fire(Trigger.Shutdown);
            }
        }

        public void FireStartTrigger()
        {
            if (IsRunning)
            {
                _fsm.Fire(Trigger.Start);
            }
        }

        public void FireStopTrigger()
        {
            if (IsRunning)
            {
                _fsm.Fire(Trigger.Stop);
            }
        }

        public void StartSequencing()
        {
            _fsm.Start();
            IsRunning = true;
        }

        public void StopSequencing()
        {
            _fsm.Stop();
            IsRunning = false;
        }
        #endregion Public Methods

        #region State machine

        private void Create()
        {
            CreateSortingStates();
            CreateAssemblingStates();
            CreateGlobalStates();

            _fsm.Initialize(State.Startup);

            Logger.Instance.Write("[Sequencer] Initialization Completed", LogType.Message);
        }

        private void CreateAssemblingStates()
        {
            _fsm.In(State.Idle)
                .ExecuteOnEntry(() => _orderConsumer.OrdersAvailable += Idle_Completed)
                .ExecuteOnExit(() => _orderConsumer.OrdersAvailable -= Idle_Completed)
                .On(Trigger.Completed)
                .If(() => _assembler.canCompleteOrder(_engine.TabletMagazine, _orderConsumer.peekNextOrder().Configuration))
                        .Goto(State.LoadingTray)
                    .Otherwise()
                        .Goto(State.PlacingTabletMagazineOnSortingConveyorFromAssembler)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.LoadingTray)
                .ExecuteOnEntry(() =>
                {
                    Logger.Instance.Write("[Sequencer] Loading tray to conveyor");
                    _loader.Begin(new LoaderParams
                    {
                        Action = LoaderAction.LoadToConveyor,
                        Sender = this
                    });
                })
                .On(Trigger.Completed)
                    .Goto(State.AssemblyConveyorMovingForward)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown)
                .On(Trigger.Failed)
                    .Goto(State.Idle);

            _fsm.In(State.AssemblyConveyorMovingForward)
                .ExecuteOnEntry(() =>
                {
                    _conveyorController.Begin(new ConveyorControllerParams
                    {
                        ConveyorType = ConveyorType.Assembly,
                        ConveyorAction = ConveyorAction.MoveForward,
                        Sender = this
                    });
                })
                .On(Trigger.Completed)
                    .If(() => _conveyorController.CanMoveForward(ConveyorType.Assembly))
                        .Goto(State.VerifyingTray)
                    .Otherwise()
                        .Goto(State.Assembling)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.VerifyingTray)
                .ExecuteOnEntry(() =>
                {
                    Tray<Tablet> trayToVerify = _assembler.LastOrderTray;
                    Logger.Instance.Write("[Sequencer] Verifying tray");
                    _trayVerifier.Begin(new TrayVerifierParams()
                    {
                        TraySpecification = trayToVerify,
                        VerificationMode = VerificationMode.Tray,
                        Sender = this
                    });
                })
                .On(Trigger.Valid)
                    .Goto(State.AssemblyConveyorMovingForward)
                .On(Trigger.Invalid)
                    .Goto(State.AssemblyConveyorMovingBackward)
                .On(Trigger.NoTray)
                    .Goto(State.LoadingTray)
                    .Execute(() => 
                        {
                            Logger.Instance.Write("[Sequencer] No tray found detected. Trying to get another tray");
                            _conveyorController.SetPosition(ConveyorType.Assembly, Robotics.ConveyorPosition.Right);
                        })
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.Assembling)
                .ExecuteOnEntry(() =>
                {
                    var order = _orderConsumer.GetNextOrder();
                    Logger.Instance.Write("[Sequencer] Assembling tray", LogType.Message);
                    _assembler.Begin(new AssemblerParams
                    {
                        OrderConfiguration = order.Configuration,
                        Magazine = _engine.TabletMagazine,
                        Action = AssemblerAction.Assemble,
                        Sender = this
                    });
                })
                .On(Trigger.Completed)
                    .Goto(State.AssemblyConveyorMovingBackward)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown)
                .On(Trigger.TabletRefill)
                    .Goto(State.PlacingTabletMagazineOnSortingConveyorFromAssembler);
                

            _fsm.In(State.AssemblyConveyorMovingBackward)
                .ExecuteOnEntry(() =>
                {
                    _conveyorController.Begin(new ConveyorControllerParams
                    {
                        ConveyorType = ConveyorType.Assembly,
                        ConveyorAction = ConveyorAction.MoveBackward,
                        Sender = this
                    });
                })
                .On(Trigger.Completed)
                    .If(() => _conveyorController.CanMoveBackward(ConveyorType.Assembly))
                        .Goto(State.VerifyingProduct)
                    .Otherwise()
                        .Goto(State.PlacingTrayInBuffer)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.VerifyingProduct)
                .ExecuteOnEntry(() =>
                {
                    Tray<Tablet> trayToVerify = _assembler.LastOrderTray;
                    Logger.Instance.Write("[Sequencer] Verifying product");
                    _trayVerifier.Begin(new TrayVerifierParams()
                    {
                        TraySpecification = trayToVerify,
                        VerificationMode = VerificationMode.Product,
                        Sender = this
                    });
                })
                .On(Trigger.Valid)
                    .Goto(State.OrderComplete)
                .On(Trigger.Invalid)
                    .Goto(State.AssemblyConveyorMovingBackward)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.PlacingTrayInBuffer)
                .ExecuteOnEntry(() =>
                {
                    _loader.Begin(new LoaderParams()
                    {
                        Action = LoaderAction.LoadToPalletiser,
                        Sender = this
                    });

                })
                .On(Trigger.Completed)
                    .Goto(State.Idle)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown)
                .On(Trigger.Failed)
                    .Goto(State.PlacingTrayInBuffer);

            _fsm.In(State.OrderComplete)
                .ExecuteOnEntry(() =>
                {
                    _orderConsumer.CompleteOrder();
                    _fsm.Fire(Trigger.Completed);
                })
                .On(Trigger.Completed)
                    .Goto(State.AssemblyConveyorMovingBackward)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);
        }

        private void CreateGlobalStates()
        {
            _fsm.In(State.Startup)
                .On(Trigger.Start)
                    .If(() => Mode == OperationMode.Normal)
                        .Goto(State.PlacingTabletMagazineInSortingBuffer)
                    .If(() => Mode == OperationMode.AssembleOnly)
                        .Goto(State.Idle)
                    .If(() => Mode == OperationMode.SortOnly)
                        .Goto(State.PlacingTabletMagazineInSortingBuffer);

            _fsm.In(State.Shutdown)
                .ExecuteOnEntry(() =>
                {
                    foreach(var hardware in _hardware)
                    {
                        Logger.Instance.Write(String.Format("[Sequencer] Shutting down {0}", hardware.Name));
                        hardware.Shutdown();
                        Logger.Instance.Write(String.Format("[Sequencer] {0} Shutdown complete", hardware.Name));
                    }

                    //Removed because robots are using the same Named EventWaitHandle (cbf changing this) - Shale
                    //(var hardware in _hardware)
                    //Parallel.ForEach(_hardware, (hardware) =>
                    //    {
                    //        Logger.Instance.Write(String.Format("[Sequencer] Shutting down {0}", hardware.Name), LogType.Message);
                    //        hardware.Shutdown();
                    //    });
                })
                .On(Trigger.Start)
                    .Goto(State.Startup);

            _fsm.In(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown)
                .On(Trigger.Resume)
                    .Goto(State.Running);

            _fsm.In(State.None)
                .On(Trigger.Start)
                    .Goto(State.Startup);

            _fsm.DefineHierarchyOn(State.Running)
                .WithHistoryType(HistoryType.Deep)
                .WithInitialSubState(State.Startup)
                .WithSubState(State.Idle)
                .WithSubState(State.LoadingTray)
                .WithSubState(State.AssemblyConveyorMovingForward)
                .WithSubState(State.VerifyingTray)
                .WithSubState(State.Assembling)
                .WithSubState(State.AssemblyConveyorMovingBackward)
                .WithSubState(State.PlacingTrayInBuffer)
                .WithSubState(State.Palletising)
                .WithSubState(State.Sorting)
                .WithSubState(State.PlacingTabletMagazineInSortingBuffer)
                .WithSubState(State.PlacingTabletMagazineInAssemblyBuffer)
                .WithSubState(State.PlacingTabletMagazineOnSortingConveyorFromSorter)
                .WithSubState(State.PlacingTabletMagazineOnSortingConveyorFromAssembler)
                .WithSubState(State.SortingConveyorMovingForward)
                .WithSubState(State.SortingConveyorMovingBackward);
        }

        private void CreateSortingStates()
        {
            _fsm.In(State.Sorting)
                .ExecuteOnEntry(() =>
                {
                    _sorter.Begin(new SorterParams
                    {
                        Action = SorterAction.Sort,
                        Magazine = _engine.TabletMagazine
                    });
                })
                .On(Trigger.Completed)
                    .Goto(State.PlacingTabletMagazineOnSortingConveyorFromSorter)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                    .Execute(() => _sorter.Cancel())
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.PlacingTabletMagazineOnSortingConveyorFromSorter)
                .ExecuteOnEntry(() =>
                {
                    _sorter.Begin(new SorterParams
                    {
                        Action = SorterAction.LoadToConveyor
                    });
                })
                .On(Trigger.Completed)
                    .Goto(State.SortingConveyorMovingBackward)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.SortingConveyorMovingBackward)
                .ExecuteOnEntry(() =>
                {
                    _conveyorController.Begin(new ConveyorControllerParams
                    {
                        ConveyorType = ConveyorType.Sorting,
                        ConveyorAction = ConveyorAction.MoveBackward
                    });
                })
                .On(Trigger.Completed)
                    .Goto(State.PlacingTabletMagazineInAssemblyBuffer)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.PlacingTabletMagazineInAssemblyBuffer)
                .ExecuteOnEntry(() =>
                {
                    _assembler.Begin(new AssemblerParams
                    {
                        Action = AssemblerAction.GetTabletMagazine
                    });
                })
                .On(Trigger.Completed)
                    .Goto(State.Idle)
                    //.Goto(State.LoadingTray) //- for testing only
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.PlacingTabletMagazineOnSortingConveyorFromAssembler)
                .ExecuteOnEntry(() =>
                {
                    _assembler.Begin(new AssemblerParams
                    {
                        Action = AssemblerAction.ReturnTabletMagazine
                    });
                })
                .On(Trigger.Completed)
                    .Goto(State.SortingConveyorMovingForward)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.SortingConveyorMovingForward)
                .ExecuteOnEntry(() =>
                {
                    _conveyorController.Begin(new ConveyorControllerParams
                    {
                        ConveyorType = ConveyorType.Sorting,
                        ConveyorAction = ConveyorAction.MoveForward
                    });
                })
                .On(Trigger.Completed)
                    .Goto(State.PlacingTabletMagazineInSortingBuffer)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.PlacingTabletMagazineInSortingBuffer)
                .ExecuteOnEntry(() =>
                {
                    _sorter.Begin(new SorterParams
                    {
                        Action = SorterAction.LoadToBuffer
                    });
                })
                .On(Trigger.Completed)
                    .Goto(State.Sorting)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);
        }
        #endregion State machine

        #region Event handlers

        private void Assembler_Completed(object sender, ControllerEventArgs e)
        {
            var args = e as AssemblerEventArgs;

            if (args.AssemblerOperationStatus == AssemblerOperationStatus.Normal)
            {
                _fsm.Fire(Trigger.Completed);
            }
            else if(args.AssemblerOperationStatus == AssemblerOperationStatus.TabletRefill)
            {
                _fsm.Fire(Trigger.TabletRefill);
            }
            
        }

        private void BindEvents()
        {
            _assembler.Completed += Assembler_Completed;
            _conveyorController.Completed += ConveyorController_Completed;
            _loader.Completed += Loader_Completed;
            _sorter.Completed += Sorter_Completed;
            _trayVerifier.Completed += TrayVerifier_Completed;
            // The Idle_Completed handler is not registered here. It is registered/unregistered
            // in the Entry/Exit callbacks for the Idle event.
        }
        private void ConveyorController_Completed(object sender, ControllerEventArgs e)
        {
            _fsm.Fire(Trigger.Completed);
        }

        private void Idle_Completed(object sender, EventArgs e)
        {
            _fsm.Fire(Trigger.Completed);
        }

        private void Loader_Completed(object sender, ControllerEventArgs e)
        {
            if (e.OperationStatus == ControllerOperationStatus.Succeeded)
            {
                _fsm.Fire(Trigger.Completed);
            }
            else
            {
                _fsm.Fire(Trigger.Failed);
            }
        }

        private void Sorter_Completed(object sender, ControllerEventArgs e)
        {
            var args = e as SorterCompletedEventArgs;
            _fsm.Fire(Trigger.Completed, args.Action);
        }

        private void TrayVerifier_Completed(object sender, ControllerEventArgs e)
        {
            var args = e as OnVerificationCompleteEventArgs;

            if (args.VerificationResult == VerificationResult.Valid)
            {
                _fsm.Fire(Trigger.Valid, args.VerificationMode);
            }
            else if (args.VerificationResult == VerificationResult.Invalid)
            {
                _fsm.Fire(Trigger.Invalid, args.VerificationMode);
            }
            else if (args.VerificationResult == VerificationResult.NoTray)
            {
                _fsm.Fire(Trigger.NoTray, args.VerificationMode);
            }
        }
        #endregion Event handlers
    }
}