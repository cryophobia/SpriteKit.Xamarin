using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SpriteKit;
using System.Collections.Generic;

namespace Tut002
{
	public class PlayerScene : SKScene
	{
		readonly Random _random = new Random();
		readonly SizeF _size;
		SKSpriteNode _backGround;
		PlayerSprite _PlayerSprite;
		SKAction _PlayerAnimation;
		SKAction _starCollisionSound;
		SKAction _enemyCollisionSound;
		const float PlayerMovePointsPerSec = 120f;
		const float PlayerRotateRadiansPerSec = 4 * (float)Math.PI;
		double _lastUpdateTime;
		double _dt;
		PointF _velocity;
		PointF _lastTouchedLocation;

		public PlayerScene (SizeF size): base (size)
		{
			_size = size;
			InitSound();
			Start();
		}

		void InitSound(){
			_starCollisionSound = SKAction.PlaySoundFileNamed("pop.wav", false);
			_enemyCollisionSound = SKAction.PlaySoundFileNamed("explosion.wav", false);
		}

		void Start(){
			_backGround = new SKSpriteNode ("background");
			_backGround.Position = new PointF (_size.Width / 2, _size.Height / 2);
			AddChild (_backGround);	

			_PlayerSprite = new PlayerSprite ("player1");
			_PlayerSprite.Position = new PointF(100f, 100f);
			AddChild (_PlayerSprite);

			_PlayerAnimation = CreatePlayerAnimation();

//			SpawnEnemy();
//			SpawnEnemy2();
//			SpawnEnemy3();

//			RunAction(
//				SKAction.RepeatActionForever(
//					SKAction.Sequence(
//						new SKAction[] {SKAction.RunBlock (SpawnEnemy4),
//							SKAction.WaitForDuration(2.0f)})));

			RunAction(
				SKAction.RepeatActionForever(
					SKAction.Sequence(
						new SKAction[] {SKAction.RunBlock (SpawnEnemy5),
							SKAction.WaitForDuration(2.0f)})));

			RunAction(
				SKAction.RepeatActionForever(
					SKAction.Sequence(
						new SKAction[] {SKAction.RunBlock (SpawnStar),
							SKAction.WaitForDuration(1.0f)})));
		}

		public override void Update (double currentTime)
		{
			if (_lastUpdateTime > 0) {
				_dt = currentTime - _lastUpdateTime;
			} else {
				_dt = 0;
			}
			_lastUpdateTime = currentTime;

			var offset = MathHelpers.PointSubtract(_PlayerSprite.Position, _lastTouchedLocation);
			if (MathHelpers.PointLength (offset) < PlayerMovePointsPerSec * _dt) {
				_PlayerSprite.Position = _lastTouchedLocation;
				_velocity = new PointF(0,0);
				StopPlayerAction();
			} else {
				MoveSprite(_PlayerSprite, _velocity);
				BoundsCheckPlayer ();
				RotateSprite (_PlayerSprite, _velocity, PlayerRotateRadiansPerSec);
			}
		}

		public override void DidEvaluateActions ()
		{
			base.DidEvaluateActions ();
			CheckCollisions ();
		}

		void MoveSprite(SKSpriteNode sprite, PointF velocity){
			var amountToMove = MathHelpers.PointMultiplyScalar (velocity, _dt);

			sprite.Position = MathHelpers.PointAdd (sprite.Position, amountToMove);
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
			sprite.ZRotation += MathHelpers.ScalarSign(shortest) * (float)amountToRotate;
		}

		void BoundsCheckPlayer(){
			var newPosition = _PlayerSprite.Position;
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

			_PlayerSprite.Position = newPosition;
			_velocity = newVelocity;
		}

		void MovePlayerToward(PointF location){
			StartPlayerAction();

			var offset = MathHelpers.PointSubtract (location, _PlayerSprite.Position);

			var direction = MathHelpers.PointNormalize(offset);

			_velocity = MathHelpers.PointMultiplyScalar (direction, PlayerMovePointsPerSec);

		}

		SKAction CreatePlayerAnimation ()
		{
			var textures = new List<SKTexture>();

			for (int i = 1; i < 5; i++){
				textures.Add(SKTexture.FromImageNamed(string.Format("player{0}", i)));
			}

			return SKAction.AnimateWithTextures(textures.ToArray(), 0.1);
		}

		void StartPlayerAction(){
			if (_PlayerSprite.GetActionForKey("animation") == null){
				_PlayerSprite.RunAction(SKAction.RepeatActionForever(_PlayerAnimation), "animation");
			}
		}

		void StopPlayerAction(){
			_PlayerSprite.RemoveActionForKey("animation");
		}

		void SpawnEnemy() {
			var enemySprite = new EnemySprite("enemy");
			enemySprite.Position = new PointF(Size.Width + enemySprite.Size.Width / 2, Size.Height / 2);
			AddChild(enemySprite);

			var actionMove = SKAction.MoveTo(new PointF(-enemySprite.Size.Width / 2, enemySprite.Position.Y), 3.0f);
			enemySprite.RunAction(actionMove);
		}

		void SpawnEnemy2() {
			var enemySprite = new EnemySprite("enemy");
			enemySprite.Position = new PointF(Size.Width + enemySprite.Size.Width / 2, Size.Height / 2);
			AddChild(enemySprite);

			var actionMidMove = SKAction.MoveTo(new PointF(Size.Width / 2, enemySprite.Size.Height), 1.0f);
			var actionMove = SKAction.MoveTo(new PointF(-enemySprite.Size.Width / 2, enemySprite.Position.Y), 1.0f);
			var actionWait = SKAction.WaitForDuration(0.25f);
			var actionLog = SKAction.RunBlock(LogMessage);
			var moveSequence = SKAction.Sequence( new SKAction[] {actionMidMove, actionLog, actionWait, actionMove});

			enemySprite.RunAction(moveSequence);
		}

		static void LogMessage ()
		{
			Console.WriteLine ("Reached Bottom");
		}

		void SpawnEnemy3() {
			var enemySprite = new EnemySprite("enemy");
			enemySprite.Position = new PointF(Size.Width + enemySprite.Size.Width / 2, Size.Height / 2);
			AddChild(enemySprite);

			var actionMidMove = SKAction.MoveBy(-Size.Width / 2 - enemySprite.Size.Width / 2, -Size.Height / 2 + enemySprite.Size.Height / 2, 1.0f);
			var actionMove = SKAction.MoveBy(-Size.Width / 2 - enemySprite.Size.Width / 2, Size.Height / 2 + enemySprite.Size.Height / 2, 1.0f);
			var actionWait = SKAction.WaitForDuration(0.25f);
			var actionLog = SKAction.RunBlock (LogMessage);
			var actionReverseMove = actionMove.ReversedAction;
			var actionReverseMidMove = actionMidMove.ReversedAction;

			var moveSequence = SKAction.Sequence( new SKAction[] {actionMidMove, actionLog, actionWait, actionMove,
				actionReverseMove,actionLog, actionWait, actionReverseMidMove});

			var actionRepeat = SKAction.RepeatActionForever(moveSequence);

			enemySprite.RunAction(actionRepeat);
		}

		void SpawnEnemy4() {
			var enemySprite = new EnemySprite("enemy");

			var x = Size.Width + enemySprite.Size.Width / 2;
			var y = MathHelpers.ScalarRandomRange(_random, enemySprite.Size.Height / 2, Size.Height - enemySprite.Size.Height / 2);

			enemySprite.Position = new PointF(x, y);
			AddChild(enemySprite);

			var actionMove = SKAction.MoveToX(-enemySprite.Size.Width / 2, 3.0f);
			enemySprite.RunAction(actionMove);
		}

		void SpawnEnemy5() {
			var enemySprite = new EnemySprite("enemy");
			enemySprite.Name = "enemy";

			var x = Size.Width + enemySprite.Size.Width / 2;
			var y = MathHelpers.ScalarRandomRange(_random, enemySprite.Size.Height / 2, Size.Height - enemySprite.Size.Height / 2);

			enemySprite.Position = new PointF(x, y);
			AddChild(enemySprite);

			var actionMove = SKAction.MoveToX(-enemySprite.Size.Width / 2, 3.0f);
			var actionRemove = SKAction.RemoveFromParent();

			var moveSequence = SKAction.Sequence( new SKAction[] {actionMove, actionRemove});

			enemySprite.RunAction(moveSequence);
		}

		void SpawnStar(){
			var starSprite = new StarSprite("star");
			starSprite.Name = "star";

			starSprite.Position = new PointF(MathHelpers.ScalarRandomRange(_random, 0, _size.Width), MathHelpers.ScalarRandomRange(_random, 0, _size.Height));
			starSprite.XScale = 0;
			starSprite.YScale = 0;

			AddChild(starSprite);

			starSprite.ZRotation = (float)-Math.PI / 16;

			var appearAction = SKAction.ScaleTo(1.0f, 0.5f);

			var leftWiggleAction = SKAction.RotateByAngle ((float)(Math.PI / 8), 0.5f);
			var rightWiggleAction = leftWiggleAction.ReversedAction;
			var fullWiggleAction = SKAction.Sequence (new SKAction[] { leftWiggleAction, rightWiggleAction});

			var scaleUpAction = SKAction.ScaleBy (1.2f, 0.25f);
			var scaleDownAction = scaleUpAction.ReversedAction;
			var fullScaleAction = SKAction.Sequence (new SKAction[] { scaleUpAction, scaleDownAction, scaleUpAction, scaleDownAction});

			var groupAction = SKAction.Group (new SKAction[]{ fullScaleAction, fullWiggleAction});
			var groupWaitAction = SKAction.RepeatAction (groupAction, 10);

			var disappearAction = SKAction.ScaleTo(0.0f, 0.5f);
			var removeFromParentAction = SKAction.RemoveFromParent();

			starSprite.RunAction(SKAction.Sequence(new SKAction[] {appearAction, groupWaitAction, disappearAction, removeFromParentAction}));
		}

		void CheckCollisions(){
			EnumerateChildNodes ("star", StarEnumerationHandler);

			EnumerateChildNodes ("enemy", EnemyEnumerationHandler);
		}

		void StarEnumerationHandler (SKNode node, out bool stop)
		{
			var star = (SKSpriteNode)node;
			if (!RectangleF.Intersect(star.Frame, _PlayerSprite.Frame).Size.IsEmpty){
				star.RemoveFromParent();
				RunAction(_starCollisionSound);
				stop = true;
			}
			stop = false;
		}

		void EnemyEnumerationHandler (SKNode node, out bool stop)
		{
			var enemy = (SKSpriteNode)node;
			var smallerFrame = RectangleF.Inflate (enemy.Frame, -20, -20);
			if (!RectangleF.Intersect (smallerFrame, _PlayerSprite.Frame).Size.IsEmpty) {
				enemy.RemoveFromParent ();
				RunAction(_enemyCollisionSound);
				stop = true;
			}
			stop = false;
		}
	}
}

