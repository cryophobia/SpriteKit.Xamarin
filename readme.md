## SpriteKit and Xamarin.iOS

With the addition of SpritKit in iOS 7 creating games should now be easier then ever. Here is how the public-facing portion of Apple’s developer portal describes [SpritKit](https://developer.apple.com/ios7/): 

*“Create new immersive experiences using the latest game technologies in iOS 7. Develop high-performance 2D games with the powerful new Sprite Kit framework, which combines everything you need to animate sprites, simulate physics and create beautiful particle systems all in one easy-to-use set of APIs.”*

Being a gamer at hart I have decided to dig into SpriteKit with a series of blog posts, looking at what it takes to develop games using Xamarin.iOS.  

Since SpriteKit is not the only option available to you for developing games on iOS using C#, it would be an oversight not to mention [cocos2d-xna](http://www.cocos2dxna.com/) and [Unity](http://unity3d.com/).

The last item on the list, before we continue with some code, a quick look at the pros and cons of SpriteKit.

### Sprite Kit Pros
-	SpriteKit is part of iOS 7, built into the OS. There is no need to download any additional libraries and as a result no external dependencies. It is written and maintained by Apple and therefor well supported and updated in the future.
-	SpriteKit has built-in tool support for both texture atlases as well as particles.
-	SpriteKit lets you do some pretty neat things like using videos as sprites and applying image effects and masks. Some of these are pretty difficult to do in other frameworks.

### Sprite Kit Cons
-	SpriteKit is a young framework. This means that many useful features you may find in Cocos2D are not available. For example, you are not able to write custom OpemGL code as yet.
-	Lock-in. Unlike Cocos2D-xna, which runs on 10 platforms, SpritKit runs on iOS only. You will be spending a little more time getting your hit game to Android and/or Windows.

### Getting started
The 1st thing you’ll need is an installation of [Xamarin Studio](http://xamarin.com/download) with Xamarin.iOS support.

Once downloaded and configured launch and select a new *C# -> iPhone Storyboard -> Single View Application* project template, you are now ready to start adding some code.

We are going to start off with replacing the code in our main View Controller, in this example it is **Tut001ViewController**.

Override the method **ViewWillLayoutSubViews** and add the following code:

	public override void ViewWillLayoutSubviews ()
	{
		skView = View as SKView;
		if (skView.Scene == null) {
			skView.ShowsFPS = true;
			skView.ShowsDrawCount = true;
			skView.ShowsNodeCount = true;

			var PlayerScene = new PlayerScene (skView.Bounds.Size);
			PlayerScene.ScaleMode = SKSceneScaleMode.AspectFill;
			skView.PresentScene (PlayerScene);
		}
	}

We could have used the same code in ViewDidLoad, however since the view’s size is not guaranteed to be correct at this point, **ViewWillLayoutSubviews** is a safer bet.

You may have noticed that instead of making use of an **UIView** in our view controller, we are using **SKView**. Make sure to set this in your Storyboard.

Note the PlayerScene we are creating in this method, this is where most of our game logic will reside for the time being.

Lastly we override the **PreferStatusBarHidden** method to return true.

	public override bool PrefersStatusBarHidden ()
	{
		return true;
	}

You should now be able to run the application and once the simulator is launched, have a black screen with a node count and fps total.


### Adding Sprites

To add sprites to our game, we need to look at our **PlayerScene** class. **PlayerScene** inherits from **SKScene**. **SKScene** give us the methods, which handles our game loop and where we manage our game logic.

Our 1st sprite will be the game background. The images for the background are located in the resources folder. We instantiate an SKNode variable using the default constructor, passing in the filename of our background. You do not need to specify the file extension, since SpriteKit automatically determines this for us.

We will set the background anchor point to the center point of screen.

Call **AddChild()** to add the Node to our scene.

Running the application now, should present you with a starfield based on our background image.

Next we will add our player sprite. We will load our player sprite using the same method we used to load the background. 

	public PlayerScene (SizeF size): base (size)
	{
		_backGround = new SKSpriteNode ("background");
		_backGround.Position = new PointF (size.Width / 2, size.Height / 2);
		AddChild (_backGround);	

		_playerSprite = new PlayerSprite ("player1");
		_playerSprite.Position = new PointF(100f, 100f);
		AddChild (_playerSprite);
	}
	
### Moving sprites, manually

The next step is to have our player sprite react on touch input from the user. In order to achieve this we are going to add a few helper methods to help us with some of the calculations required. These are:

	public static PointF PointAdd(PointF a, PointF b)

	public static PointF PointSubtract(PointF a, PointF b)

	public static PointF PointMultiplyScalar(PointF a, double b)

	public static float PointLength(PointF a)

	public static PointF PointNormalize(PointF a)

	public static float PointToAngle(PointF a)

	public static float ScalarSign(float a)

	public static float ScalarShortestAngleBetween(float a, float b)

Hopefully the method names and implementations should be self-explanatory. 

Behind the scenes SpriteKit runs an endless loop, which often gets referred to the game loop. To give us access to the game loop, **SKScene** implements a method call **Update()**. It is in this method where we will update our sprites positions and rotations.

We can illustrate this with a simple update of the player’s position inside the update method:

	_playerSprite.Position = new PointF (_playerSprite.Position.X + 2, _playerSprite.Position.Y);

Our player’s movement may however seem a bit jagged. To overcome this, we will need to change our movement calculation to take into consideration the amount of time elapsed since our previous update and there by adjust the amount of points we need to move to have a smooth animation.

By looking at the code in our project, you will now notice we have a calculated variable called velocity, which is used to indicate the next position the sprite is located following each update. Inside **MovePlayerToward()** we are calculating the difference or offset we need to move our player sprite, the direction we need to move and finally the velocity by which our player will move.

An important component to the above calculation is the speed by which we want our player sprite to move per second. By varying this, you will be able to have different sprites move at different speeds.

Next we will get the player object to respond to user input. We will record the position the user touched on the screen and move the player sprite in that direction.

We are going to start by overriding **TouchesBegan**, **TouchesMoved** and **TouchesEnded**. In here we are going to call **MovePlayerToward** and passing in the last touched location. Pretty straight forward.

To ensure that our player sprite stays within the bounds of our screen, we are going to add a method called **BoundsCheckForPlayer()**.

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

This method checks if our player sprite has reached the edges of our scene. If so, then we will X and Y positions as required and reverse the appropriate velocity component.

In our example we stop the player sprite once we have reached the position the user has touched. To see the player sprite bounce, uncomment the following if statement:

	if (MathHelpers.PointLength (distance) > PlayerMovePointsPerSec / 4) {

### SpriteKit Actions

The above is pretty manual and will become tedious really fast. What if we have animations that require combinations of animations or effects that we want to repeat or reverse over periods of time? Fortunately SpriteKit comes with Actions, which significantly simplifies coding.

To illustrate this you will now need to look at the 2nd project, **Tut002**. The **Tut002Viewcontroller** kicks off where the previous stopped. Our player sprite is animated as per the manual methods already explained. We are going to add an enemy and some stars using Actions.

We start by spawning some enemies. The 1st example is contained in the method **SpawnEnemy()**. In this method we create an enemy sprite, add it to our scene and the we create an **MoveTo SKAction**. 

	void SpawnEnemy() {
		var enemySprite = new EnemySprite("enemy");
		enemySprite.Position = new PointF(Size.Width + enemySprite.Size.Width / 2, Size.Height / 2);
		AddChild(enemySprite);

		var actionMove = SKAction.MoveTo(new PointF(-enemySprite.Size.Width / 2, enemySprite.Position.Y), 3.0f);
		enemySprite.RunAction(actionMove);
	}

To run the action, we call **RunAction** on our sprite passing in the newly created action.  Uncommenting the **SpawnEnemy()** call and commenting out the last 2 **RunActions** inside the **Start** method will allow you to see the effect of the action when run.

The power of actions becomes a little bit more apparent looking at **SpawnEnemy2** and **SpawnEnemy3**. In **SpawnEnemy2** we make use of chaining actions inside an **SKAction Sequence**.  This allows us to chain actions and have them execute in sequence.

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

In **SpawnEnemy3** we still use the **SKSequence**, but in addition we add an **SKAction RunBlock**. The **RunBlock** allows you to run custom code as part of your sequence. In this example we log a message to the console.

In the same method we introduce the **SKAction ReversedAction**. Here we are reversing an **SKAction MoveBy** action, i.e. go back the same way your came.  It is important to note that not all Acations are reversible, for example **MoveTo** is not.

In **SpawnEnemy4** we introduce multiple enemies, by spawning them randomly on the screen. Running this method over an extended period you will however see the node count on our stats counter incrementing with each spawned enemy. This is a problem, since each node takes up memory and given enough time, our app will run out of memory and will be shutdown by the OS.

In **SpawnEnemy5** we introduce a new **SKAction RemoveFromParent**. This action will remove the sprite from its parent and clean up memory as required. Pretty nifty.

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

The player in our game is able to collect stars, which appears random and includes a little animation to draw attention. The stars will disappear from the screen if not collected by the player. In **SpawnStar()** we introduce yet another new action, **SKAction Group**. 

This action allows for the grouping of actions, but instead of running sequentially, these actions will run in parallel. Also note, this is a reversible action.

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

### Collisions and Sound

The last bit to this tutorial is to add collision detection and sounds effects to our game.  We handle this in the method called **CheckCollisions()**. However, instead of calling **CheckCollisions()** in the scene’s **Update()** method, we are going to call the method rather in the scene’s **DidEvaluateActions()** method. The reason for this is that all actions are only evaluated after the Update method in the event loop.

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


### Conclusion
So far the SpriteKit experience has been pleasant and I look forward to some more code using this framework. We will have a look at scene transitions in the next installment.


