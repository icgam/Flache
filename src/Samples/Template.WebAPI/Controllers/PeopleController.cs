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
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Template.WebAPI.Services;

namespace Template.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class PeopleController : Controller
    {
        private readonly IPersonStore _store;

        public PeopleController(IPersonStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        [HttpGet("all")]
        public async Task<IActionResult> Get()
        {
            var result = await _store.GetAllPeopleAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get([Url] PersonSearchRequest request)
        {
            var result = await _store.GetPeopleAsync(request);
            return Ok(result);
        }
    }
}
