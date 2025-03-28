﻿using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CurrentTime : ICurrentTime
    {
        public DateOnly GetCurrentTime()
        {
            DateOnly DateNow = DateOnly.FromDateTime(DateTime.Now);
            return DateNow;
        }
    }
}
