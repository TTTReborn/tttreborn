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

using TTTReborn.Player;

namespace TTTReborn.Map
{
    [Library("ttt_credit_adjust", Description = "Changes the amount of credits upon the activator.")]
    public partial class TTTChangeCredits : Entity
    {
        [Property("Credits", "Amount of credits to remove from activator. Negative numbers add credits. Removes 1 credit by default.")]
        public int Credits { get; set; } = 1;

        [Input]
        public void ExchangeCredits(Entity activator)
        {
            if (activator is TTTPlayer player)
            {
                if (player.Credits >= Credits)
                {
                    player.Credits -= Credits;

                    OnSuccess.Fire(activator);
                }
                else
                {
                    OnFailure.Fire(activator);
                }
            }
        }

        /// <summary>
        /// Fires when credits are successfully added or removed from activator.
        /// </summary>
        protected Output OnSuccess { get; set; }

        /// <summary>
        /// Fires if credits cannot be removed or added to activator. Such as not having enough credits for removal as a player cannot have 'negative' credits.
        /// </summary>
        protected Output OnFailure { get; set; }
    }
}
