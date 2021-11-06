using nightcrawler.player;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nightcrawler
{
	public static class PhysicsMoveExt
	{
		public static void PhysicsMove( this ModelEntity self, Vector3 targetPos, Rotation targetRot )
		{
			float followRate = 50f;
			float rotateRate = 25f;

			if ( !self.IsValid() || self.PhysicsBody == null )
				return;

			self.PhysicsBody.SpeculativeContactEnabled = true;

			// Rot
			{
				const float DEG_TO_RAD = 0.0174533f;

				var quat = targetRot * self.Rotation.Inverse;
				var angles = quat.Angles();

				var axis = new Vector3(
					angles.roll,
					angles.pitch,
					angles.yaw
				).Normal;
				var angle = quat.Angle();

				self.PhysicsBody.AngularVelocity = angle * axis * DEG_TO_RAD * rotateRate;
			}

			//
			// Trace to controller position (used for controller unstuck)
			//
			float deadzone = 5f;
			float maxVelocity = 125f;

			Entity playerOwner = self.Owner;
			if ( playerOwner is not VRPlayer )
				playerOwner = playerOwner.Owner;

			if ( playerOwner.Velocity.Length > deadzone )
			{
				float controllerTraceRadius = 6f;
				var controllerTrace = Trace.Ray( Input.VR.Head.Position, targetPos ).Ignore( self.Owner ).Radius( controllerTraceRadius ).Ignore( self ).Run();

				targetPos = controllerTrace.EndPos - targetRot.Forward * 2f;

				if ( playerOwner.IsValid )
				{
					targetPos += playerOwner.Velocity * Time.Delta * 2.5f;
				}
				self.Position = targetPos;
				return;
			}

			var distance = Vector3.DistanceBetween( targetPos, self.Position );
			var controllerDir = (targetPos - self.Position).Normal;
			self.Velocity = controllerDir * (followRate * distance);

			{
				var worldTrace = Trace.Ray( self.Position, targetPos ).WorldOnly().Ignore( self.Owner ).Ignore( self ).Run();
				var entTrace = Trace.Ray( self.Position, targetPos ).EntitiesOnly().Ignore( self.Owner ).Ignore( self ).Run();


				if ( worldTrace.Hit || entTrace.Hit )
				{
					float amt = 0f;
					if ( entTrace.Hit )
						amt = 125 / entTrace.Body.Mass;

					maxVelocity = amt;
				}
			}

			self.Velocity = controllerDir * (followRate * distance);
			self.Velocity = self.Velocity.ClampLength( maxVelocity );
		}
	}
}
