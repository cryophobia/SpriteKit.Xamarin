// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Tut001
{
	[Register ("Tut001ViewController")]
	partial class Tut001ViewController
	{
		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		MonoTouch.SpriteKit.SKView skView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (skView != null) {
				skView.Dispose ();
				skView = null;
			}
		}
	}
}
