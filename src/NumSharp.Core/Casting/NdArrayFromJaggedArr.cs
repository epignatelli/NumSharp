﻿/*
 * NumSharp
 * Copyright (C) 2018 Haiping Chen
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the Apache License 2.0 as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the Apache License 2.0
 * along with this program.  If not, see <http://www.apache.org/licenses/LICENSE-2.0/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections;
using NumSharp.Core;

namespace NumSharp.Core
{
    public partial class NDArray
    {
        public void FromJaggedArray(Array dotNetArray)
        {
            if(!dotNetArray.GetType().GetElementType().IsArray)
                throw new Exception("Multi dim arrays are not allowed here!");

            List<int> dimList = new List<int>();

            dimList.Add(dotNetArray.Length);

            object currentArr = dotNetArray;

            while (currentArr.GetType().GetElementType().IsArray)
            {
                Array child = (Array) ((Array) currentArr).GetValue(0);
                dimList.Add(child.Length);
                currentArr = child;
            }

            Type elementType = currentArr.GetType().GetElementType();

            int[] dims = dimList.ToArray();

            Shape shape = new Shape(dims);
            shape.ChangeTensorLayout(1);

            NDArray nd = new NDArray(elementType,shape);

            Array ndStrg = nd.Storage.GetData();

            for (int idx = 0; idx < shape.Size;idx++)
            {
                int[] indexes = shape.GetDimIndexOutShape(idx);

                Array puffer = (Array) dotNetArray.GetValue(indexes[0]);

                for (int jdx = 1; jdx < indexes.Length-1;jdx++)
                {
                    puffer = (Array) puffer.GetValue(indexes[jdx]);
                }

                ndStrg.SetValue(puffer.GetValue(indexes[indexes.Length-1]),nd.Storage.Shape.GetIndexInShape(indexes));
            }

            this.Storage = nd.Storage;
        }
        
    }
}
