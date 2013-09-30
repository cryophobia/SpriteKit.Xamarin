using System;
using MonoTouch.UIKit;
using MonoTouch.SpriteKit;

namespace Tut002
{
	public partial class Tut002ViewController : UIViewController
	{
		public Tut002ViewController (IntPtr handle) : base (handle)
		{
		}

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

		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}
	}
}

