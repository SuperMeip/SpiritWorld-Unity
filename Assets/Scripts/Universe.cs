using SpiritWorld.Game.Controllers;
using SpiritWorld.Events;
using SpiritWorld.Managers;
using SpiritWorld.World;
using SpiritWorld.World.Entities.Creatures;

public class Universe {

	// TODO: buildable shrines and structures to attract more spirits or even trap them.
	// gives you more things to do with scape resources~

	/// <summary>
	/// The current worldscape
	/// </summary>
  public static WorldScape CurrentScape;

	/// <summary>
	/// The active board manager
	/// </summary>
  public static BoardManager ActiveBoardManager;

	/// <summary>
	/// The current local player
	/// </summary>
	public static LocalPlayerManager LocalPlayerManager;

	/// <summary>
	/// The current board id we're on for the given scape
	/// </summary>
  public static int CurrentBoardId = 0;

	/// <summary>
	/// The radius of a hexagon
	/// </summary>
  public const float HexRadius = 1;

	/// <summary>
	/// The height of a hexagon step
	/// </summary>
  public const float StepHeight = 1.0f / 6.0f;

	/// <summary>
	/// The system to send notification between controllers and managers across different channels
	/// </summary>
	public static WorldScapeEventSystem EventSystem {
		get;
	} = new WorldScapeEventSystem();

	/// <summary>
	/// The local player
	/// </summary>
	public static Player LocalPlayer 
		=> LocalPlayerManager.getPlayer();
}
