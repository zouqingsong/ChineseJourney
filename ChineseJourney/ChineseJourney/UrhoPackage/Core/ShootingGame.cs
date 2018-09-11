using System.Diagnostics;
using System.Threading.Tasks;
using ChineseJourney.Common.Controller;
using ChineseJourney.Common.Helpers;
using ChineseJourney.Common.UrhoPackage.Core.Aircrafts;
using ChineseJourney.Common.UrhoPackage.Core.Aircrafts.Enemies;
using ChineseJourney.Common.UrhoPackage.Core.Weapons;
using Urho;
using Urho.Actions;
using Urho.Gui;
using Urho.Physics;
using Urho.Shapes;
using ZibaobaoLib;
using ZibaobaoLib.Helpers;

namespace ChineseJourney.Common.UrhoPackage.Core
{
	public class ShootingGame : Application
	{
		const string CoinstFormat = "{0} coins";

		int _coins;
		Scene _scene;
		Text _coinsText;

		public Player Player { get; private set; }

		public Viewport Viewport { get; private set; }

	    public ShootingGame() : base(new ApplicationOptions(assetsFolder: "Data")
	    {
	        Height = 1024,
	        Width = 576,
	        Orientation = ApplicationOptions.OrientationType.LandscapeAndPortrait
	    })
	    {
	    }

	    [Preserve]
	    public ShootingGame(ApplicationOptions opts) 
	        : base(opts)
	    {
            X1LogHelper.Info($"{ZibaobaoLibContext.Instance.AppName} starting {opts.Width}, {opts.Height} from {ZibaobaoLibContext.Instance.PersistentStorage.DocumentPath}");
        }

        static ShootingGame()
		{
			UnhandledException += (s, e) =>
			{
                X1LogHelper.Exception(e.Exception);
				if (Debugger.IsAttached)
					Debugger.Break();
				e.Handled = true;
			};
		}

		protected override void Start()
		{
		    BaobaoGameContextFactory.Instance.Game = this;
            base.Start();
			CreateScene();
            Input.KeyDown += e =>
			{
			    if (e.Key == Key.Esc)
			    {
			        Exit().Forget();
			    }

			    if (e.Key == Key.C)
			    {
			        AddCollisionDebugBox(_scene, true);
			    }

			    if (e.Key == Key.V)
			    {
			        AddCollisionDebugBox(_scene, false);
			    }
			};
		}

        static void AddCollisionDebugBox(Node rootNode, bool add)
		{
			var nodes = rootNode.GetChildrenWithComponent<CollisionShape>(true);
			foreach (var node in nodes)
			{
				node.GetChild("CollisionDebugBox")?.Remove();
				if (!add)
					continue;
				var subNode = node.CreateChild("CollisionDebugBox");
				var box = subNode.CreateComponent<Box>();
				subNode.Scale = node.GetComponent<CollisionShape>().WorldBoundingBox.Size;
				box.Color = new Color(Color.Red, 0.4f);
			}
		}

		async void CreateScene()
		{
			_scene = new Scene();
			_scene.CreateComponent<Octree>();

			var physics = _scene.CreateComponent<PhysicsWorld>();
			physics.SetGravity(new Vector3(0, 0, 0));

			// Camera
			var cameraNode = _scene.CreateChild();
			cameraNode.Position = (new Vector3(0.0f, 0.0f, -10.0f));
			cameraNode.CreateComponent<Camera>();
			Viewport = new Viewport(Context, _scene, cameraNode.GetComponent<Camera>());

			if (Platform != Platforms.Android && Platform != Platforms.iOS && Platform != Platforms.UWP)
            {
				RenderPath effectRenderPath = Viewport.RenderPath.Clone();
				var fxaaRp = ResourceCache.GetXmlFile(Assets.PostProcess.FXAA3);
				effectRenderPath.Append(fxaaRp);
				Viewport.RenderPath = effectRenderPath;
			}

			Renderer.SetViewport(0, Viewport);

			var zoneNode = _scene.CreateChild();
			var zone = zoneNode.CreateComponent<Zone>();
			zone.SetBoundingBox(new BoundingBox(-300.0f, 300.0f));
			zone.AmbientColor = new Color(1f);
			
			// UI
			_coinsText = new Text();
			_coinsText.HorizontalAlignment = HorizontalAlignment.Right;
			_coinsText.SetFont(ResourceCache.GetFont(Assets.Fonts.ChineseFont), (int)(Graphics.Width / 20.0));
			UI.Root.AddChild(_coinsText);
			Input.SetMouseVisible(true);

			// Background
            
            /*
			var background = new Background();
			_scene.AddComponent(background);
			background.Start();
            */

			// Lights:
			var lightNode = _scene.CreateChild();
			lightNode.Position = new Vector3(0, -5, -40);
			lightNode.AddComponent(new Light { Range = 120, Brightness = 0.8f });

			// Game logic cycle
			bool firstCycle = true;

 			while (true)
			{
				var startMenu = _scene.CreateComponent<StartMenu>();
				await startMenu.ShowStartMenu(!firstCycle); //wait for "start"
				startMenu.Remove();
				await StartGame();
				firstCycle = false;
			}
		}

	    public void SetQuestionTitle(string text)
	    {
	        Player?.SetText(text);
        }

		async Task StartGame()
		{
		    InvokeOnMain(() => UpdateCoins(0));
			Player = new Player();
			var aircraftNode = _scene.CreateChild(nameof(Aircraft));
			aircraftNode.AddComponent(Player);
			var playersLife = Player.Play();
		    SpawnCoins();
			Enemies enemies = new Enemies(Player);
			_scene.AddComponent(enemies);
			enemies.StartSpawning();
			await playersLife;
			enemies.KillAll();
			aircraftNode.Remove();
		}
		
		async void SpawnCoins()
		{
			var player = Player;
			while (Player.IsAlive && player == Player)
			{
				var coinNode = _scene.CreateChild();
				coinNode.Position = new Vector3(RandomHelper.NextRandom(-2.5f, 2.5f), 5f, 0);
				var coin = new Apple();
				coinNode.AddComponent(coin);
				await coin.FireAsync(false);
				await _scene.RunActionsAsync(new DelayTime(3f));
				coinNode.Remove();
			}
		}

		public void OnCoinCollected() => UpdateCoins(_coins + 1);

		void UpdateCoins(int amount)
		{
			if (amount % 5 == 0 && amount > 0)
			{
				// give player a MassMachineGun each time he earns 5 coins
				Player.Node.AddComponent(new MassMachineGun());
			}
			_coins = amount;
			_coinsText.Value = string.Format(CoinstFormat, _coins);
		}
	}
}
