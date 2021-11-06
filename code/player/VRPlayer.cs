using nightcrawler.camera;
using Sandbox;
using System;
using System.Linq;

namespace nightcrawler.player
{
	partial class VRPlayer : Player
	{
		[Net, Predicted]
		public VRHand LeftHand { get; set; }

		[Net, Predicted]
		public VRHand RightHand { get; set; }

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			LeftHand = new VRLeftHand();
			RightHand = new VRRightHand();

			LeftHand.Owner = this;

			RightHand.Owner = this;


			Controller = new VRLocomotion();
			Animator = new VRAnimator();
			Camera = new VRCamera();

			EnableAllCollisions = false;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			LeftHand?.Simulate( cl );
			RightHand?.Simulate( cl );

			Animate();

			Rotate();

			SimulateActiveChild( cl, ActiveChild );
		}

		public override void FrameSimulate( Client cl )
		{
			LeftHand?.FrameSimulate( cl );
			RightHand?.FrameSimulate( cl );
		}

		public override void OnKilled()
		{
			base.OnKilled();

			EnableDrawing = false;
		}

		private void Animate()
		{
			SetAnimBool( "b_vr", true );
			var leftHand = Transform.ToLocal( LeftHand.Transform );
			var rightHand = Transform.ToLocal( RightHand.Transform );
			SetAnimVector( "left_hand_ik.position", leftHand.Position );
			SetAnimVector( "right_hand_ik.position", rightHand.Position );

			SetAnimRotation( "left_hand_ik.rotation", leftHand.Rotation * Rotation.From( 65, 0, 90 ) );
			SetAnimRotation( "right_hand_ik.rotation", rightHand.Rotation * Rotation.From( 65, 0, 90 ) );

			float height = Input.VR.Head.Position.z - Position.z;
			SetAnimFloat( "duck", 1.0f - ((height - 32f) / 32f) );
		}

		[ServerCmd()]
		public static void SetHealth( float val )
		{
			var player = ConsoleSystem.Caller.Pawn as VRPlayer;
			player.Health = val;
		}

		private TimeSince timeSinceLastRotation;

		private void Rotate()
		{
			if ( timeSinceLastRotation > 0.25f )
			{
				var rotate = Input.VR.RightHand.Joystick.Value.x;

				if ( rotate > 0.2f )
				{
					Rotation = Rotation.RotateAroundAxis( Vector3.Up, -45f );
					timeSinceLastRotation = 0;
				}
				else if ( rotate < -0.2f )
				{
					Rotation = Rotation.RotateAroundAxis( Vector3.Up, 45f );
					timeSinceLastRotation = 0;
				}
			}
		}
	}
}
