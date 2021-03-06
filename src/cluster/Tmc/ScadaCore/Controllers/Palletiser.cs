﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmc.Robotics;
using TmcData;

namespace Tmc.Scada.Core
{
    public sealed class Palletiser : ControllerBase
    {
        private PalletiserRobot _palletiserRobot;
        public Palletiser(ClusterConfig config) : base(config)
        {
            _palletiserRobot = config.Robots[typeof(PalletiserRobot)] as PalletiserRobot;

            if (_palletiserRobot == null)
            {
                throw new ArgumentException("Could not retrieve a PalletiserRobot from ClusterConfig");
            }
        }

        public override void Begin(ControllerParams parameters)
        {
            if (!IsRunning)
            {
                IsRunning = true;
                PalletiseAsync();
            }
        }

        public override void Cancel()
        {
        }

        private ControllerOperationStatus Palletise()
        {
            var status = ControllerOperationStatus.Succeeded;
            try
            {
                _palletiserRobot.Palletise();
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(new LogEntry(ex));
                status = ControllerOperationStatus.Failed;
            }
            return status;
        }

        private void PalletiseAsync()
        {
            Task.Run(() =>
                {
                    var status = Palletise();
                    IsRunning = false;
                    OnCompleted(new ControllerEventArgs() { OperationStatus = status });
                });
        }
    }
}
