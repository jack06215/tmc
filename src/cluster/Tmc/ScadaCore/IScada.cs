﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tmc.Scada.Core
{
    public interface IScada
    {
        void Initialise();
        void Start();
        void Stop();
        void Resume();
        void EmergencyStop();
    }
}
