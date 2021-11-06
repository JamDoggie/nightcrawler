using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace nightcrawler.player
{
	// Most of this is based on https://github.com/nerd-two/sbox-vrshooter/blob/master/code/Player/VRHand.Base.cs
	public partial class VRHand : AnimEntity
	{


		// Inputs
		protected bool GripPressed => Grip > 0.5f;
		protected bool TriggerPressed => Trigger > 0.5f;
		public virtual float Trigger => 0f;
		public virtual float Grip => 0f;
		public virtual Vector3 ControllerVelocity => default;
		public virtual Vector3 ControllerAngularVelocity => default;


		// Transform of the hand
		public Vector3 ControllerPos { get; set; }

		public Rotation ControllerRot { get; set; }


		// Offsets so that the controllers are in the right place
		protected Vector3 PosOffset => ControllerRot.Backward * 2f + ControllerRot.Down * 4f;
		protected Rotation RotOffset => Rotation.FromPitch( 65 );

		protected virtual bool UsePhysics { get; set; } = true;

		public override void Spawn()
		{
			SetModel( "models/hands/alyx_hand_left.vmdl" );

			SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
			EnableSelfCollisions = false;

			Position = ControllerPos;
			Rotation = Rotation.From( 0, 0, 0 );

			UsePhysics = false;

			//PhysicsBody.GravityEnabled = false;
			//PhysicsBody.EnableAutoSleeping = false;
			//PhysicsBody.SpeculativeContactEnabled = true;

			//PhysicsBody.Mass = 250.0f;

			EnableDrawing = Local.Client == this.Client;
			EnableShadowCasting = false;

			
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( IsServer & PhysicsBody != null)
			{
				PhysicsBody.Enabled = false;

				UsePhysics = false;
				PhysicsEnabled = false;

				Transmit = TransmitType.Owner;

				
			}

			Position = ControllerPos;
			Rotation = ControllerRot;
		}

		public override void FrameSimulate( Client cl )
		{
			Position = ControllerPos;
			Rotation = ControllerRot;

			base.FrameSimulate( cl );
		}

		protected virtual void Animate()
		{
			if ( !IsServer ) return;

			
		}

	}

	public partial class VRLeftHand : VRHand
	{
		public override void Spawn()
		{
			base.Spawn();
			SetModel( "models/hands/alyx_hand_left.vmdl" );
			SetInteractsAs( CollisionLayer.Debris );
		}

		public override void Simulate( Client cl )
		{
			ControllerPos = Input.VR.LeftHand.Transform.Position + PosOffset;
			ControllerRot = Input.VR.LeftHand.Transform.Rotation * RotOffset;

			base.Simulate( cl );
		}

		public override void FrameSimulate( Client cl )
		{
			ControllerPos = Input.VR.LeftHand.Transform.Position + PosOffset;
			ControllerRot = Input.VR.LeftHand.Transform.Rotation * RotOffset;

			base.FrameSimulate( cl );
		}
	}

	public partial class VRRightHand : VRHand
	{
		public override void Spawn()
		{
			base.Spawn();
			SetModel( "models/hands/alyx_hand_right.vmdl" );
			SetInteractsAs( CollisionLayer.Debris );
		}

		public override void Simulate( Client cl )
		{
			ControllerPos = Input.VR.RightHand.Transform.Position + PosOffset;
			ControllerRot = Input.VR.RightHand.Transform.Rotation * RotOffset;

			base.Simulate( cl );
		}

		public override void FrameSimulate( Client cl )
		{
			ControllerPos = Input.VR.RightHand.Transform.Position + PosOffset;
			ControllerRot = Input.VR.RightHand.Transform.Rotation * RotOffset;

			base.FrameSimulate( cl );
		}
	}
}
