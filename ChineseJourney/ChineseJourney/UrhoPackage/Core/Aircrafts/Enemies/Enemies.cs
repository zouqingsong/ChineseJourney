using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChineseJourney.Common.Helpers;
using Urho;
using Urho.Actions;
using ZibaobaoLib.Helpers;

namespace ChineseJourney.Common.UrhoPackage.Core.Aircrafts.Enemies
{
	/// <summary>
	/// A component to rule all enemies
	/// </summary>
	public class Enemies : Component
	{
	    private readonly Player _player;
		readonly List<Enemy> _enemies;

		public Enemies(Player player)
		{
			_enemies = new List<Enemy>();
			_player = player;
		}

		public void KillAll()
		{
			foreach (var enemy in _enemies.ToArray())
			{
				enemy.Explode().Forget();
			}
		}

		public async void StartSpawning()
		{
			int count = 3;
			while (_player.IsAlive)
			{
			    await SpawnBats(count: count, pause: 5f);
				await SpawnTwoMonitors();
			}
		}

		Task SpawnTwoMonitors()
		{
			return Task.WhenAll(
				SpawnEnemy(() => new EnemyEvilMonitor(true), 1), 
				SpawnEnemy(() => new EnemyEvilMonitor(false), 1));
		}

		async Task SpawnBats(int count, float pause)
		{
			var tasks = new List<Task>();
			for (int i = 1; i < count + 1 && _player.IsAlive; i++)
			{
			    if (i % 3 == 0)
			    {
			        tasks.Add(SpawnEnemy(() => new EnemySkull(), 4));
                }
                else
			    {
			        tasks.Add(SpawnEnemy(() => new EnemyXamonkey(), 1));
			    }
				await Node.RunActionsAsync(new DelayTime(pause));
			}
			await Task.WhenAll(tasks);
		}

		async Task SpawnEnemy(Func<Enemy> enemyFactory, int times)
		{
			for (int i = 0; i < times && _player.IsAlive; i++)
			{
				var enemyNode = Node.CreateChild(nameof(Aircraft));
				var enemy = enemyFactory();
				enemyNode.AddComponent(enemy);
				_enemies.Add(enemy);
				await enemy.Play();
				_enemies.Remove(enemy);
				enemyNode.RemoveAllActions();
				enemyNode.Remove();
			}
		}
	}
}
