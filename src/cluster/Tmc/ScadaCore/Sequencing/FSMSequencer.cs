﻿#region Header
/// FileName: FSMSequencer.cs
/// Author: Denis Kaynarca (denis@dkaynarca.com)
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using Appccelerate.StateMachine;
using Tmc.Common;

namespace Tmc.Scada.Core.Sequencing
{
    public enum OperationMode
    {
        SortOnly,
        AssembleOnly,
        Normal
    }

    public class FSMSequencer : ISequencer
    {
        public string Name { get; set; }
        public OperationMode Mode { get; set; }

        private PassiveStateMachine<State, Trigger> _fsm;
        private ScadaEngine _engine;
        private ConveyorController _conveyorController;
        private Assembler _assembler;
        private Loader _loader;
        private Sorter _sorter;
        private TrayVerifier _trayVerifier;
        private Palletiser _palletiser;
        private OrderConsumer _orderConsumer;

        public FSMSequencer(ScadaEngine engine)
        {
            this._engine = engine;

            Debug.Assert(this._engine != null);

            this._conveyorController = _engine.ClusterConfig.Controllers[typeof(ConveyorController)] as ConveyorController;
            this._assembler = _engine.ClusterConfig.Controllers[typeof(Assembler)] as Assembler;
            this._loader = _engine.ClusterConfig.Controllers[typeof(Loader)] as Loader;
            this._sorter = _engine.ClusterConfig.Controllers[typeof(Sorter)] as Sorter;
            this._trayVerifier = _engine.ClusterConfig.Controllers[typeof(TrayVerifier)] as TrayVerifier;
            this._palletiser = _engine.ClusterConfig.Controllers[typeof(Palletiser)] as Palletiser;

            this._orderConsumer = _engine.OrderConsumer;

            Debug.Assert(this._conveyorController != null);
            Debug.Assert(this._assembler != null);
            Debug.Assert(this._loader != null);
            Debug.Assert(this._sorter != null);
            Debug.Assert(this._trayVerifier != null);
            Debug.Assert(this._palletiser != null);

            _fsm = new PassiveStateMachine<State, Trigger>();
            Create();
        }

        #region Public Methods

        public void Start()
        {
            _fsm.Fire(Trigger.Start);
        }

        public void Stop()
        {
            _fsm.Fire(Trigger.Stop);
        }

        public void Shutdown()
        {
            _fsm.Fire(Trigger.Shutdown);
        }

        public void Resume()
        {
            _fsm.Fire(Trigger.Resume);
        }

        #endregion

        private void Create()
        {
            CreateSortingStates();
            CreateAssemblingStates();
            CreateGlobalStates();

            _fsm.Initialize(State.Shutdown);
        }

        #region State machine

        private void CreateGlobalStates()
        {
            _fsm.In(State.Startup)
                .On(Trigger.Completed)
                    .If(() => Mode == OperationMode.Normal)
                        .Goto(State.Sorting)
                    .If(() => Mode == OperationMode.AssembleOnly)
                        .Goto(State.Idle)
                    .If(() => Mode == OperationMode.SortOnly)
                        .Goto(State.Sorting);
                      

            _fsm.In(State.Shutdown)
                .On(Trigger.Start)
                    .Goto(State.Startup);

            _fsm.In(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown)
                .On(Trigger.Resume)
                    .Goto(State.Running);

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
                .On(Trigger.Completed)
                    .Goto(State.PlacingTabletMagazineOnSortingConveyorFromSorter)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.PlacingTabletMagazineOnSortingConveyorFromSorter)
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
                .On(Trigger.Completed)
                    .Goto(State.Idle)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.PlacingTabletMagazineOnSortingConveyorFromAssembler)
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
                .On(Trigger.Completed)
                    .Goto(State.Sorting)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);
        }

        private void CreateAssemblingStates()
        {
            _fsm.In(State.Idle)
                .On(Trigger.Completed)
                    .Goto(State.LoadingTray)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.LoadingTray)
                .ExecuteOnEntry(() =>
                    {
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
                    .Goto(State.Shutdown);

            _fsm.In(State.AssemblyConveyorMovingForward)
                .ExecuteOnEntry(() => 
                { 
                    _conveyorController.Begin(new ConveyorControllerParams
                        {
                            ConveyorType = ConveyorType.Assembly,
                            ConveyorAction = ConveyorAction.MoveForward,
                            Sender = this
                        }); })
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
                    _trayVerifier.Begin(new TrayVerifierParams()
                    {
                        TraySpecification = trayToVerify,
                        VerificationMode = VerificationMode.Product,
                        Sender = this
                    });
                })
                .On(Trigger.Valid).If<VerificationMode>((mode) => mode == VerificationMode.Tray)
                    .Goto(State.AssemblyConveyorMovingForward)
                .On(Trigger.Valid).If<VerificationMode>((mode) => mode == VerificationMode.Product)
                    .Goto(State.AssemblyConveyorMovingBackward)
                .On(Trigger.Invalid)
                    .Goto(State.AssemblyConveyorMovingBackward)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            _fsm.In(State.Assembling)
                .ExecuteOnEntry(() =>
                {
                    var order = _orderConsumer.PeekOrder();
                    _assembler.Begin(new AssemblerParams
                    {
                        OrderConfiguration = order.Configuration,
                        Sender = this
                    });
                })
                .On(Trigger.Completed)
                    .Goto(State.AssemblyConveyorMovingBackward)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

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
                        .Goto(State.VerifyingTray)
                    .Otherwise()
                        .Goto(State.PlacingTrayInBuffer)
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
                    //.Goto(State.Palletising)
                    .Goto(State.Idle)
                .On(Trigger.Stop)
                    .Goto(State.Stopped)
                .On(Trigger.Shutdown)
                    .Goto(State.Shutdown);

            // Skip this state because the palletiser robot is inactive.
            //_fsm.In(State.Palletising)
            //    .ExecuteOnEntry(() => _fsm.Fire(Trigger.Completed))
            //    .On(Trigger.Completed)
            //        .Goto(State.Idle)
            //    .On(Trigger.Stop)
            //        .Goto(State.Stopped)
            //    .On(Trigger.Shutdown)
            //        .Goto(State.Shutdown);
        }

        #endregion

        #region Event handlers
        private void BindEvents()
        {
            _assembler.Completed += Assembler_Completed;
            _conveyorController.Completed += ConveyorController_Completed;
            _loader.Completed += Loader_Completed;
            _palletiser.Completed += Palletiser_Completed;
            _sorter.Completed += Sorter_Completed;
            _trayVerifier.Completed += TrayVerifier_Completed;
            _orderConsumer.OrdersAvailable += Idle_Completed;
        }

        private void Assembler_Completed(object sender, ControllerEventArgs e)
        {
            var args = e as AssemblerEventArgs;
            _fsm.Fire(Trigger.Completed);
        }

        private void ConveyorController_Completed(object sender, ControllerEventArgs e)
        {
            _fsm.Fire(Trigger.Completed);
        }

        private void Loader_Completed(object sender, ControllerEventArgs e)
        {
            _fsm.Fire(Trigger.Completed);
        }

        private void Palletiser_Completed(object sender, ControllerEventArgs e)
        {
            _fsm.Fire(Trigger.Completed);
        }

        private void Sorter_Completed(object sender, ControllerEventArgs e)
        {
            _fsm.Fire(Trigger.Completed);
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
        }

        private void Idle_Completed(object sender, EventArgs e)
        {
            _fsm.Fire(Trigger.Completed);
        }

        #endregion
    }
}
