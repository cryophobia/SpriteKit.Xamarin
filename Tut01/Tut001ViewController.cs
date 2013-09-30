using System;
using MonoTouch.UIKit;
using MonoTouch.SpriteKit;

namespace Tut001
{
	public partial class Tut001ViewController : UIViewController
	{
		public Tut001ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillLayoutSubviews ()
		{
			//Configure the View
			skView = View as SKView;
			if (skView.Scene == null) {

				//Set up rendering stats visibility
				skView.ShowsFPS = true;
				skView.ShowsDrawCount = true;
				skView.ShowsNodeCount = true;

				//Create and configure the scene
				var playerScene = new PlayerScene (skView.Bounds.Size);
				playerScene.ScaleMode = SKSceneScaleMode.AspectFill;
				skView.PresentScene (playerScene);
			}
		}

		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}
	}
}

