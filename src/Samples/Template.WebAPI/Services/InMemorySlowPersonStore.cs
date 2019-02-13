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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Template.WebAPI.Services
{
    public sealed class InMemorySlowPersonStore : IPersonStore
    {
        private readonly List<Person> _people;
        private readonly Random _rnd;

        public InMemorySlowPersonStore()
        {
            _rnd = new Random();
            _people = new List<Person>
            {
                new Person
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Age = 45
                },
                new Person
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Age = 25
                }
            };
        }

        public Task<List<Person>> GetPeopleAsync(PersonSearchRequest request)
        {
            return Task.Factory.StartNew(() => { Thread.Sleep(_rnd.Next(500, 1500)); }).ContinueWith(t =>
            {
                return _people.Where(p =>
                        p.FirstName.IndexOf(request.SearchCriteria, StringComparison.CurrentCultureIgnoreCase) > -1 ||
                        p.LastName.IndexOf(request.SearchCriteria, StringComparison.CurrentCultureIgnoreCase) > -1)
                    .ToList();
            });
        }

        public Task<List<Person>> GetAllPeopleAsync()
        {
            return Task.Factory.StartNew(() => { Thread.Sleep(_rnd.Next(500, 1500)); })
                .ContinueWith(t => _people.ToList());
        }
    }
}