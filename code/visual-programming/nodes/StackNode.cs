// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using System.Collections.Generic;

namespace TTTReborn.VisualProgramming
{
    public partial class StackNode
    {
        public List<StackNode> NextNodes = new();

        public StackNode()
        {

        }

        public virtual void Reset()
        {
            NextNodes.Clear();
        }

        public virtual object[] Build(params object[] input)
        {
            return input;
        }

        public virtual void Evaluate(params object[] input)
        {
            // TODO handle this in/by the stack at a later time
            // NodeStack.Instance.AddNode(this);

            // for (int i = 0; i < NextNodes.Count; i++)
            // {
            //     NextNodes[i].Evaluate(input.Length > i ? input[i] : null);
            // }
        }
    }
}
