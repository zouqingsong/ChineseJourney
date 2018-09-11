using System.Threading.Tasks;
using ChineseJourney.Common.Controller;
using ChineseJourney.Common.Helpers;
using Urho;
using Urho.Actions;
using Urho.Gui;
using Urho.Shapes;

namespace ChineseJourney.Common.UrhoPackage.Core
{
	public class StartMenu : Component
	{
		TaskCompletionSource<bool> _menuTaskSource;
		Node _bigAircraft;
		Node _rotor;
		Text _textBlock;
		Node _menuLight;
		bool _finished = true;

		public StartMenu()
		{
			ReceiveSceneUpdates = true;
		}

		public async Task ShowStartMenu(bool gameOver)
		{
			var cache = Application.ResourceCache;
			_bigAircraft = Node.CreateChild();
			var model = _bigAircraft.CreateComponent<StaticModel>();

			if (gameOver)
			{
				model.Model = cache.GetModel(Assets.Models.Enemy1);
				model.SetMaterial(cache.GetMaterial(Assets.Materials.Enemy1).Clone());
				_bigAircraft.SetScale(0.3f);
				_bigAircraft.Rotate(new Quaternion(180, 90, 20));
			}
			else
			{
				model.Model = cache.GetModel(Assets.Models.Player);
				model.SetMaterial(cache.GetMaterial(Assets.Materials.Player).Clone());
				_bigAircraft.SetScale(1f);
				_bigAircraft.Rotate(new Quaternion(0, 40, -50));
			}

			_bigAircraft.Position = new Vector3(10, 2, 10);
			_bigAircraft.RunActions(new RepeatForever(new Sequence(new RotateBy(1f, 0f, 0f, 5f), new RotateBy(1f, 0f, 0f, -5f))));

			//TODO: rotor should be defined in the model + animation
			_rotor = _bigAircraft.CreateChild();
			var rotorModel = _rotor.CreateComponent<Box>();
			rotorModel.Color = Color.White;
			_rotor.Scale = new Vector3(0.1f, 1.5f, 0.1f);
			_rotor.Position = new Vector3(0, 0, -1.3f);
			var rotorAction = new RepeatForever(new RotateBy(1f, 0, 0, 360f*6)); //RPM
			_rotor.RunActions(rotorAction);
			
			_menuLight = _bigAircraft.CreateChild();
			_menuLight.Position = new Vector3(-3, 6, 2);
			_menuLight.AddComponent(new Light { LightType = LightType.Point, Brightness = 0.3f });

			await _bigAircraft.RunActionsAsync(new EaseIn(new MoveBy(1f, new Vector3(-10, -2, -10)), 2));

            _textBlock = new Text();
			_textBlock.HorizontalAlignment = HorizontalAlignment.Center;
			_textBlock.VerticalAlignment = VerticalAlignment.Bottom;
		    var str = gameOver ? "游戏结束" : "点击开始";
		    var font = cache.GetFont(Assets.Fonts.ChineseFont);
            _textBlock.SetFont(font, 36);
		    _textBlock.SetTextFix(str);
            Application.UI.Root.AddChild(_textBlock);

			_menuTaskSource = new TaskCompletionSource<bool>();
			_finished = false;

            await _menuTaskSource.Task;
		}

        protected override async void OnUpdate(float timeStep)
		{
		    if (_finished)
		    {
		        return;
		    }

			var input = Application.Input;
			if (input.GetMouseButtonDown(MouseButton.Left) || input.NumTouches > 0)
			{
				_finished = true;
				Application.UI.Root.RemoveChild(_textBlock);
				await _bigAircraft.RunActionsAsync(new EaseIn(new MoveBy(1f, new Vector3(-10, -2, -10)), 3));
				_rotor.RemoveAllActions();
				_menuTaskSource.TrySetResult(true);
			}
		}
	}
}
