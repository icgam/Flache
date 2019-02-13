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


﻿using System.Linq;
using System.Reflection;
using System.Text;

namespace Flache.Options
{
    public sealed class MethodMetadataBasedCacheOptionsKeyGenerator : ICacheOptionsKeyGenerator
    {
        public string GetOptionsKey(MethodInfo methodInfo)
        {
            var sb = new StringBuilder();
            var typeName = methodInfo.DeclaringType.FullName;
            var methodName = methodInfo.Name;

            sb.Append(typeName);
            sb.Append(".");
            sb.Append(methodName);

            var arguments = methodInfo.GetParameters();
            if (arguments.Any())
                sb.Append(":");

            for (int i = 0; i < arguments.Length; i++)
            {
                sb.Append(arguments[i].ParameterType.FullName);
                if (i + 1 < arguments.Length)
                    sb.Append("|");
            }

            return sb.ToString();
        }
    }
}