﻿// This script is only executed on the server
public class WorldGameMode : GameMode {
	// OnEnable
	protected override void OnEnable() {
		base.OnEnable();
		
		InvokeRepeating("GameModeUpdate", 1f, 1f);
	}
	
	// GameModeUpdate
	void GameModeUpdate() {
		CheckShutdown();
	}
}