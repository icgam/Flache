// This software is part of the Flache Library
// Copyright (C) 2018 Intermediate Capital Group
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.


﻿using System;
using System.Threading.Tasks;
using Flache.Region;
using Microsoft.AspNetCore.Mvc;

namespace Template.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class CacheController : Controller
    {
        private readonly ICacheSupervisor _supervisor;

        public CacheController(ICacheSupervisor supervisor)
        {
            _supervisor = supervisor ?? throw new ArgumentNullException(nameof(supervisor));
        }

        [HttpPost]
        public async Task<IActionResult> InvalidateCache()
        {
            await _supervisor.ClearCacheAsync(Constants.DefaultRegion);
            return Ok();
        }
    }
}
