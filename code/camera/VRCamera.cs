using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightcrawler.camera
{
	public class VRCamera : Camera
	{
		public override void Update()
		{
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			var eyePos = pawn.EyePos;
			Position = eyePos;

			Rotation = pawn.EyeRot;

			Viewer = pawn;
		}
	}
}
