﻿using Application.Repositories;
using Domain.Model;
using Infrastructure.Repositories.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface ICropRequirementRepository : IGenericRepository<CropRequirement>
    {
    }
}
