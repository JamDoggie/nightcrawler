
using nightcrawler.player;
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace nightcrawler
{

	public partial class NightCrawlerGame : Game
	{
		public NightCrawlerGame()
		{
			if ( IsServer )
			{
				// This HUD is for the desktop window of s&box, as the in-game HUD will be rendered world space.
				new HudEntity();
			}
		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new VRPlayer();
			client.Pawn = player;

			player.Respawn();
		}
	}

}
