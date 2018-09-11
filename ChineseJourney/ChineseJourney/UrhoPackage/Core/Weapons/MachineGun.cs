using System;
using System.Threading.Tasks;
using Urho;
using Urho.Actions;
using Urho.Audio;

namespace ChineseJourney.Common.UrhoPackage.Core.Weapons
{
	public class MachineGun : Weapon
	{
		const float GunOffsetSize = 0.2f; //accuracy (lower - better)
		float _currentGunOffset = -GunOffsetSize;
	    private SoundSource _soundSource;

		public override TimeSpan ReloadDuration => TimeSpan.FromSeconds(0.1f);

		public override int Damage => 3;
		
		protected override async Task OnFire(bool player)
		{
			var cache = Application.ResourceCache;
			_currentGunOffset += GunOffsetSize;
			if (_currentGunOffset > GunOffsetSize)
				_currentGunOffset = -GunOffsetSize;

			var bulletNode = CreateRigidBullet(player);
			bulletNode.Translate(new Vector3(_currentGunOffset, 0, 0));

			var model = bulletNode.CreateComponent<StaticModel>();
			model.Model = cache.GetModel(Assets.Models.Box);
			var mat = cache.GetMaterial(Assets.Materials.MachineGun);
			model.SetMaterial(mat);

			bulletNode.LookAt(new Vector3(bulletNode.WorldPosition.X, 10, -10), new Vector3(0, 1, -1));
			bulletNode.Rotate(new Quaternion(0, 45, 0));
			bulletNode.Scale = new Vector3(0.1f, 0.3f, 0.1f);

			_soundSource.Play(Application.ResourceCache.GetSound(Assets.Sounds.MachineGun));
			await bulletNode.RunActionsAsync(
				new MoveBy(0.5f, new Vector3(0, 10, 0) * (player ? 1 : -1)),
				new CallFunc(() => bulletNode.SetScale(0f))); // collapse

			//remove the bullet from the scene.
			bulletNode.Remove();
		}

		protected override void Init()
		{
			_soundSource = Node.CreateComponent<SoundSource>();
			_soundSource.Gain = 0.1f;
		}
	}
}
