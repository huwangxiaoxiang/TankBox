﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace TankFlow
{
    [RunInstaller(true)]
    public partial class Installers : System.Configuration.Install.Installer
    {
        public Installers()
        {
            InitializeComponent();
        }
    }
}