using Sandbox;
using System;
using System.Linq;

namespace TTTGamemode
{
	partial class Inventory : BaseInventory
	{
		public Inventory( Sandbox.Player player ) : base( player )
		{

		}

		public override bool Add( Entity entity, bool makeActive = false )
		{
			TTTGamemode.Player player = Owner as Player;

			if ( entity is Weapon weapon && IsCarryingType( entity.GetType() ) )
			{
				int ammo = weapon.AmmoClip;
				AmmoType ammoType = weapon.AmmoType;

				if ( ammo > 0 )
				{
					player.GiveAmmo( ammoType, ammo );
				}

				entity.Delete();

				return false;
			}

			return base.Add( entity, makeActive );
		}

		public bool IsCarryingType( Type t )
		{
			return List.Any( x => x.GetType() == t );
		}
	}
}
