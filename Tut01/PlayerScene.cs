using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SpriteKit;

namespace Tut001
{
	public class PlayerScene : SKScene
	{
		readonly SKSpriteNode _backGround;
		readonly PlayerSprite _playerSprite;
		const float PlayerMovePointsPerSec = 120f;
		const float PlayerRotateRadiansPerSec = 4 * (float)Math.PI;
		double _lastUpdateTime;
		double _dt;
		PointF _velocity;
		PointF _lastTouchedLocation;

		public PlayerScene (SizeF size): base (size)
		{
			_backGround = new SKSpriteNode ("background");
			_backGround.Position = new PointF (size.Width / 2, size.Height / 2);
			AddChild (_backGround);	

			_playerSprite = new PlayerSprite ("player1");
			_playerSprite.Position = new PointF(100f, 100f);
			AddChild (_playerSprite);
		}

		public override void Update (double currentTime)
		{
			if (_lastUpdateTime > 0) {
				_dt = currentTime - _lastUpdateTime;
			} else {
				_dt = 0;
			}
			_lastUpdateTime = currentTime;

			var distance = MathHelpers.PointSubtract(_playerSprite.Position, _lastTouchedLocation);
			if (MathHelpers.PointLength (distance) > PlayerMovePointsPerSec / 4) {
				MoveSprite(_playerSprite, _velocity);
				BoundsCheckForPlayer ();
				RotateSprite (_playerSprite, _velocity, PlayerRotateRadiansPerSec);
			}
		}

		void MoveSprite(SKSpriteNode sprite, PointF velocity){
			var amountToMove = MathHelpers.PointMultiplyScalar (velocity, _dt);

			sprite.Position = MathHelpers.PointAdd (sprite.Position, amountToMove);
		}

		void MovePlayerToward(PointF location){
			var offset = MathHelpers.PointSubtract (location, _playerSprite.Position);

			var direction = MathHelpers.PointNormalize(offset);

			_velocity = MathHelpers.PointMultiplyScalar (direction, PlayerMovePointsPerSec);
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			UITouch touch = (UITouch)touches.AnyObject;
			_lastTouchedLocation = touch.LocationInNode (this.Scene);
			MovePlayerToward (_lastTouchedLocation);
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			UITouch touch = (UITouch)touches.AnyObject;
			_lastTouchedLocation = touch.LocationInNode (this.Scene);
			MovePlayerToward (_lastTouchedLocation);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			UITouch touch = (UITouch)touches.AnyObject;
			_lastTouchedLocation = touch.LocationInNode (this.Scene);
			MovePlayerToward (_lastTouchedLocation);
		}

		void RotateSprite(SKSpriteNode sprite, PointF direction, float rotateRadiansPerSec){
			var targetAngle = MathHelpers.PointToAngle(_velocity);
			var shortest = MathHelpers.ScalarShortestAngleBetween(sprite.ZRotation, targetAngle);
			var amountToRotate = rotateRadiansPerSec * _dt;
			if(Math.Abs(shortest) < amountToRotate) {
				amountToRotate = Math.Abs(shortest);
			}
			sprite.ZRotation += (float)(MathHelpers.ScalarSign(shortest) * amountToRotate);
		}

		void BoundsCheckForPlayer(){
			var newPosition = _playerSprite.Position;
			var newVelocity = _velocity;

			var bottomLeft = new PointF (0, 0);
			var topRight = new PointF (Size.Width, Size.Height);

			if (newPosition.X <= bottomLeft.X) {
				newPosition.X = bottomLeft.X;
				newVelocity.X = -newVelocity.X;
			}

			if (newPosition.X >= topRight.X) {
				newPosition.X = topRight.X;
				newVelocity.X = -newVelocity.X;
			}

			if (newPosition.Y <= bottomLeft.Y) {
				newPosition.Y = bottomLeft.Y;
				newVelocity.Y = -newVelocity.Y;
			}

			if (newPosition.Y >= topRight.Y) {
				newPosition.Y = topRight.Y;
				newVelocity.Y = -newVelocity.Y;
			}

			_playerSprite.Position = newPosition;
			_velocity = newVelocity;
		}
	}
}

