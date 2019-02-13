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

namespace Flache.Tests.Shared
{
    public sealed class InMemoryPeopleRepository : IPeopleRepository
    {
        private readonly IEnumerable<Person> _people;

        public InMemoryPeopleRepository(IEnumerable<Person> people)
        {
            _people = people ?? throw new ArgumentNullException(nameof(people));
        }

        public int GetPeopleUsingParametersCallCount { get; private set; }
        public int GetPeopleUsingRequestCallCount { get; private set; }
        public int GetAllPeopleCallCount { get; private set; }

        public IEnumerable<Person> GetAllPeople()
        {
            GetAllPeopleCallCount++;
            return _people.ToList();
        }

        public Task<IEnumerable<Person>> GetAllPeopleAsync()
        {
            var rnd = new Random();
            return Task.Factory.StartNew(GetAllPeople).ContinueWith(t =>
            {
                Thread.Sleep(rnd.Next(20, 300));
                return t.Result;
            });
        }

        public IEnumerable<Person> GetPeople(string nameContains, int minAge, int maxAge)
        {
            GetPeopleUsingParametersCallCount++;
            return _people.Where(p => p.Name.Contains(nameContains) && p.Age >= minAge && p.Age <= maxAge).ToList();
        }

        public IEnumerable<Person> GetPeople(GetPeopleRequest request)
        {
            GetPeopleUsingRequestCallCount++;

            return _people.Where(p =>
                p.Name.Contains(request.NameContains) && p.Age >= request.MinAge && p.Age <= request.MaxAge).ToList();
        }

        public Task<IEnumerable<Person>> GetPeopleAsync(string nameContains, int minAge, int maxAge)
        {
            var rnd = new Random();
            return Task.Factory.StartNew(() => GetPeople(nameContains, minAge, maxAge)).ContinueWith(t =>
            {
                Thread.Sleep(rnd.Next(20, 300));
                return t.Result;
            });
        }

        public Task<IEnumerable<Person>> GetPeopleAsync(GetPeopleRequest request)
        {
            var rnd = new Random();
            return Task.Factory.StartNew(() => GetPeople(request)).ContinueWith(t =>
            {
                Thread.Sleep(rnd.Next(20, 300));
                return t.Result;
            });
        }
    }
}