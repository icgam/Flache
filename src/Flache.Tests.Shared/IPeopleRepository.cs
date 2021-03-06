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


﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flache.Tests.Shared
{
    public interface IPeopleRepository
    {
        IEnumerable<Person> GetAllPeople();
        Task<IEnumerable<Person>> GetAllPeopleAsync();
        IEnumerable<Person> GetPeople(string nameContains, int minAge, int maxAge);
        IEnumerable<Person> GetPeople(GetPeopleRequest request);
        Task<IEnumerable<Person>> GetPeopleAsync(string nameContains, int minAge, int maxAge);
        Task<IEnumerable<Person>> GetPeopleAsync(GetPeopleRequest request);
    }
}
