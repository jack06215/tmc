﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiServer.Authentication
{
    class SCADADbContext : IdentityDbContext<SCADAUser>
    {

        public SCADADbContext()
            : base("SCADAConnectionString")
        {

        }


    }
}
