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

using Sandbox;

namespace TTTReborn.Items
{
    /// <summary>
	/// A prop that physically simulates as a single rigid body. It can be constrained to other physics objects using hinges
	/// or other constraints. It can also be configured to break when it takes enough damage.
	/// Note that the health of the object will be overridden by the health inside the model, to ensure consistent health game-wide.
	/// If the model used by the prop is configured to be used as a prop_dynamic (i.e. it should not be physically simulated) then it CANNOT be
	/// used as a prop_physics. Upon level load it will display a warning in the console and remove itself. Use a prop_dynamic instead.
	/// </summary>
	[Library("ttt_prop_physics")]
    [Hammer.Skip]
    public abstract partial class Prop : Sandbox.Prop
    {
        public abstract string ModelPath { get; }

        public Prop() : base()
        {

        }

        public override void Spawn()
        {
            Tags.Add(IItem.ITEM_TAG);

            base.Spawn();
        }
    }
}

